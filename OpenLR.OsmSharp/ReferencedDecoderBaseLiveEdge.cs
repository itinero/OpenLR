using OpenLR.Decoding;
using OpenLR.Model;
using OpenLR.OsmSharp.Decoding.Candidates;
using OpenLR.OsmSharp.Router;
using OpenLR.OsmSharp.Scoring;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Units.Angle;
using OsmSharp.Units.Distance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenLR.OsmSharp
{
    /// <summary>
    /// A reference decoder base class for live edges.
    /// </summary>
    public abstract class ReferencedDecoderBaseLiveEdge : ReferencedDecoderBase<LiveEdge>
    {
        /// <summary>
        /// Creates a new referenced live edge decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="vehicle"></param>
        /// <param name="locationDecoder"></param>
        public ReferencedDecoderBaseLiveEdge(BasicRouterDataSource<LiveEdge> graph, Vehicle vehicle, Decoder locationDecoder)
            : base(graph, vehicle, locationDecoder)
        {

        }

        /// <summary>
        /// Creates a new referenced live edge decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="vehicle"></param>
        /// <param name="locationDecoder"></param>
        /// <param name="maxVertexDistance"></param>
        public ReferencedDecoderBaseLiveEdge(BasicRouterDataSource<LiveEdge> graph, Vehicle vehicle, Decoder locationDecoder, Meter maxVertexDistance)
            : base(graph, vehicle, locationDecoder, maxVertexDistance)
        {

        }

        /// <summary>
        /// Finds all candidate vertex/edge pairs for a given location reference point.
        /// </summary>
        /// <param name="lrp"></param>
        /// <param name="forward"></param>
        /// <param name="maxVertexDistance"></param>
        /// <returns></returns>
        public override SortedSet<CandidateVertexEdge<LiveEdge>> CreateCandidatesFor(LocationReferencePoint lrp, bool forward, Meter maxVertexDistance)
        {
            // convert to geo coordinate.
            var lrpLocation = new GeoCoordinate(lrp.Coordinate.Latitude, lrp.Coordinate.Longitude);
            var fow = lrp.FormOfWay.Value;
            var frc = lrp.FuntionalRoadClass.Value;

            // build candidates list.
            var candidates = new HashSet<long>();
            var scoredCandidates = new SortedSet<CandidateVertexEdge<LiveEdge>>(new CandidateVertexEdgeComparer<LiveEdge>());

            float latitude, longitude;

            // create a search box.
            var box = new GeoCoordinateBox(lrpLocation, lrpLocation);
            box = box.Resize(0.1);

            // get arcs.
            var arcs = this.Graph.GetEdges(box);
            foreach (var arc in arcs)
            {
                // check if arc was removed already.
                if (!this.Graph.HasArc(arc.Key, arc.Value.Key))
                { // this arc was already removed, probably the reverse one.
                    continue;
                }

                // get from/to coordinates.
                this.Graph.GetVertex(arc.Key, out latitude, out longitude);
                var fromCoordinates = new GeoCoordinate(latitude, longitude);
                this.Graph.GetVertex(arc.Value.Key, out latitude, out longitude);
                var toCoordinates = new GeoCoordinate(latitude, longitude);
                var arcCoordinates = new List<GeoCoordinateSimple>();
                if (arc.Value.Value.Coordinates != null)
                { // there are coordinates.
                    arcCoordinates.AddRange(arc.Value.Value.Coordinates);
                }

                // search along the line.
                var closestDistance = double.MaxValue;
                var closestPosition = -1;
                var closestLocation = fromCoordinates;
                var closestRatio = -1.0;

                var distanceTotal = 0.0;
                var distance = 0.0;
                var previous = fromCoordinates;
                for (int idx = 0; idx < arcCoordinates.Count; idx++)
                {
                    var current = new GeoCoordinate(arcCoordinates[idx].Latitude, arcCoordinates[idx].Longitude);
                    distanceTotal = distanceTotal + current.DistanceReal(previous).Value;
                    previous = current;
                }
                distanceTotal = distanceTotal + toCoordinates.DistanceReal(previous).Value;
                if (distanceTotal > 0)
                { // the from/to are not the same location.
                    // loop over all edges that are represented by this arc (counting intermediate coordinates).
                    previous = fromCoordinates;
                    GeoCoordinateLine line;
                    double distanceToSegment = 0;
                    for (int idx = 0; idx < arcCoordinates.Count; idx++)
                    {
                        var current = new GeoCoordinate(
                            arcCoordinates[idx].Latitude, arcCoordinates[idx].Longitude);
                        line = new GeoCoordinateLine(previous, current, true, true);

                        distance = line.DistanceReal(lrpLocation).Value;

                        if (distance < closestDistance)
                        { // the distance is smaller.
                            var projectedPoint = line.ProjectOn(lrpLocation);

                            // calculate the position.
                            if (projectedPoint != null)
                            { // calculate the distance
                                var distancePoint = previous.DistanceReal(new GeoCoordinate(projectedPoint)).Value + distanceToSegment;
                                var position = distancePoint / distanceTotal;

                                closestDistance = distance;
                                closestRatio = position;
                                closestPosition = idx;
                                closestLocation = new GeoCoordinate(projectedPoint);
                            }
                        }

                        // add current segment distance to distanceToSegment for the next segment.
                        distanceToSegment = distanceToSegment + line.LengthReal.Value;

                        // set previous.
                        previous = current;
                    }

                    // check the last segment.
                    line = new GeoCoordinateLine(previous, toCoordinates, true, true);
                    distance = line.DistanceReal(lrpLocation).Value;
                    if (distance < closestDistance)
                    { // the distance is smaller.
                        var projectedPoint = line.ProjectOn(lrpLocation);

                        // calculate the position.
                        if (projectedPoint != null)
                        { // calculate the distance
                            var distancePoint = previous.DistanceReal(new GeoCoordinate(projectedPoint)).Value + distanceToSegment;
                            var position = distancePoint / distanceTotal;

                            closestDistance = distance;
                            closestRatio = position;
                            closestPosition = arcCoordinates.Count;
                            closestLocation = new GeoCoordinate(projectedPoint);
                        }
                    }
                }

                // check if the distance is closer.
                if (distance < maxVertexDistance.Value)
                { // ok, this arc is closer.
                    // calculate score for projected new vertex.
                    var newVertexScore = Score.New(Score.VERTEX_DISTANCE, string.Format("Metric of vertex quality relative to distance {0}.", this.MaxVertexDistance),
                        (float)System.Math.Max(0, (1.0 - (distance / this.MaxVertexDistance.Value))), 1);

                    // add intermediate vertex.
                    this.Graph.RemoveEdge(arc.Key, arc.Value.Key);
                    this.Graph.RemoveEdge(arc.Value.Key, arc.Key);

                    // add a new vertex.
                    long newId = this.Graph.AddVertex((float)closestLocation.Latitude, (float)closestLocation.Longitude);

                    // build distance before/after.
                    var distanceBefore = arc.Value.Value.Distance * closestRatio;
                    var distanceAfter = arc.Value.Value.Distance - distanceBefore;

                    // build coordinates before/after.
                    var coordinatesBefore = new List<GeoCoordinateSimple>(arcCoordinates.TakeWhile((x, idx) =>
                    {
                        return idx <= closestPosition;
                    }));
                    var coordinatesAfter = new List<GeoCoordinateSimple>(arcCoordinates.TakeWhile((x, idx) =>
                    {
                        return idx > closestPosition;
                    }));

                    // add new edges forward/backward.
                    this.Graph.AddEdge(arc.Key, newId, new LiveEdge()
                    {
                        Coordinates = coordinatesBefore.Count > 0 ? coordinatesBefore.ToArray() : null,
                        Distance = (float)distanceBefore,
                        Forward = arc.Value.Value.Forward,
                        Tags = arc.Value.Value.Tags
                    });
                    coordinatesBefore.Reverse();
                    this.Graph.AddEdge(newId, arc.Key, new LiveEdge()
                    {
                        Coordinates = coordinatesBefore.Count > 0 ? coordinatesBefore.ToArray() : null,
                        Distance = (float)distanceBefore,
                        Forward = !arc.Value.Value.Forward,
                        Tags = arc.Value.Value.Tags
                    });
                    this.Graph.AddEdge(newId, arc.Value.Key, new LiveEdge()
                    {
                        Coordinates = coordinatesAfter.Count > 0 ? coordinatesAfter.ToArray() : null,
                        Distance = (float)distanceAfter,
                        Forward = arc.Value.Value.Forward,
                        Tags = arc.Value.Value.Tags
                    });
                    coordinatesAfter.Reverse();
                    this.Graph.AddEdge(arc.Value.Key, newId, new LiveEdge()
                    {
                        Coordinates = coordinatesAfter.Count > 0 ? coordinatesAfter.ToArray() : null,
                        Distance = (float)distanceAfter,
                        Forward = !arc.Value.Value.Forward,
                        Tags = arc.Value.Value.Tags
                    });

                    // add candidates for the new vertex.
                    var edgeCandidates = this.FindCandidateEdgesFor(newId, forward, lrp.FormOfWay.Value, lrp.FuntionalRoadClass.Value, (Degree)lrp.Bearing.Value);
                    foreach (var edgeCandidate in edgeCandidates)
                    {
                        scoredCandidates.Add(new CandidateVertexEdge<LiveEdge>()
                        {
                            Edge = edgeCandidate.Edge,
                            Vertex = newId,
                            TargetVertex = edgeCandidate.TargetVertex,
                            Score = newVertexScore * edgeCandidate.Score
                        });
                    }
                }
            }
            return scoredCandidates;
        }
    }
}