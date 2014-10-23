using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.OsmSharp.Decoding.Candidates;
using OpenLR.OsmSharp.Decoding.Scoring;
using OpenLR.OsmSharp.Locations;
using OpenLR.OsmSharp.Router;
using OpenLR.OsmSharp.Scoring;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Units.Angle;
using OsmSharp.Units.Distance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.OsmSharp.Decoding
{
    /// <summary>
    /// Represents a referenced point along line location decoder.
    /// </summary>
    /// <typeparam name="TEdge"></typeparam>
    public class ReferencedPointAlongLineDecoder<TEdge> : ReferencedDecoder<ReferencedPointAlongLine<TEdge>, PointAlongLineLocation, TEdge>
        where TEdge : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Creates a point along line location graph decoder.
        /// </summary>
        /// <param name="mainDecoder"></param>
        /// <param name="rawDecoder"></param>
        public ReferencedPointAlongLineDecoder(ReferencedDecoderBase<TEdge> mainDecoder, OpenLR.Decoding.LocationDecoder<PointAlongLineLocation> rawDecoder)
            : base(mainDecoder, rawDecoder)
        {

        }

        /// <summary>
        /// Decodes an OpenLR-encoded unreferenced raw OpenLR location into a referenced Location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override ReferencedPointAlongLine<TEdge> Decode(PointAlongLineLocation location)
        {
            CandidateRoute<TEdge> best = null;
            CombinedScore<TEdge> bestCombinedEdge = null;
            var vertexDistance = this.MaxVertexDistance.Value / 8;
            while ((best == null || best.Route == null) && vertexDistance <= this.MaxVertexDistance.Value)
            {
                // reset the graph.
                this.ResetCreatedCandidates();

                // get candidate vertices and edges.
                var candidates = new List<SortedSet<CandidateVertexEdge<TEdge>>>();
                var lrps = new List<LocationReferencePoint>();
                var fromBearingReference = (Degree)location.First.Bearing;
                var toBearingReference = (Degree)location.Last.Bearing;

                // loop over all lrps.
                lrps.Add(location.First);
                candidates.Add(this.FindCandidatesFor(location.First, true, vertexDistance));
                lrps.Add(location.Last);
                candidates.Add(this.FindCandidatesFor(location.Last, false, vertexDistance));

                // resolve points if one of the locations still hasn't been found.
                if ((vertexDistance * 2) > this.MaxVertexDistance.Value)
                { // this is the maximum distance that will be tested.
                    if(candidates[0].Count == 0)
                    { // explicitly resolve from.
                        candidates[0] = this.CreateCandidatesFor(location.First, true, vertexDistance);
                    }
                    else if (candidates[1].Count == 1)
                    { // explicitly remove to.
                        candidates[1] = this.CreateCandidatesFor(location.First, true, vertexDistance);
                    }
                }

                // build a list of combined scores.
                var combinedScoresSet = new SortedSet<CombinedScore<TEdge>>(new CombinedScoreComparer<TEdge>());
                foreach (var previousCandidate in candidates[0])
                {
                    foreach (var currentCandidate in candidates[1])
                    {
                        if (previousCandidate.Vertex != currentCandidate.Vertex)
                        { // make sure vertices are different.
                            combinedScoresSet.Add(new CombinedScore<TEdge>()
                            {
                                Source = previousCandidate,
                                Target = currentCandidate
                            });
                        }
                    }
                }

                // find the best candidate route.
                var combinedScores = new List<CombinedScore<TEdge>>(combinedScoresSet);
                while (combinedScores.Count > 0)
                {
                    // get the first pair.
                    var combinedScore = combinedScores.First();
                    combinedScores.Remove(combinedScore);

                    // find a route.
                    var candidate = this.FindCandidateRoute(combinedScore.Source, combinedScore.Target,
                        lrps[0].LowestFunctionalRoadClassToNext.Value);

                    // verify bearing by adding it to the score.
                    if (candidate != null && candidate.Route != null)
                    { // calculate bearing and compare with reference bearing.
                        // calculate distance and compare with distancetonext.
                        var distance = this.GetDistance(candidate.Route).Value;
                        var expectedDistance = location.First.DistanceToNext;
                        var distanceDiff = System.Math.Abs(distance - expectedDistance);
                        var deviation = Score.New(Score.DISTANCE_COMPARISON, "Compares expected location distance with decoded location distance (1=prefect, 0=difference bigger than total distance)",
                            1 - System.Math.Min(System.Math.Max(distanceDiff / expectedDistance, 0), 1), 1);

                        // add deviation-score.
                        candidate.Score = candidate.Score * deviation;
                    }

                    // check candidate.
                    if (best == null)
                    { // there was no previous candidate.
                        best = candidate;
                        bestCombinedEdge = combinedScore;
                    }
                    else if (best.Score.Value < candidate.Score.Value)
                    { // the new candidate is better.
                        best = candidate;
                        bestCombinedEdge = combinedScore;
                    }
                    else if (best.Score.Value > candidate.Score.Value)
                    { // the current candidate is better.
                        break;
                    }
                }

                // increase distance.
                vertexDistance = vertexDistance * 2;
            }

            // check if a location was found or not.
            if(best == null || best.Route == null)
            { // no location could be found.
                throw new ReferencedDecodingException(location, "No valid location was found.");
            }

            // calculate total score.
            var totalScore = bestCombinedEdge.Score + best.Score;

            // calculate the percentage value.
            var offsetRatio = 0.0;
            if(location.PositiveOffsetPercentage.HasValue)
            { // there is a percentage set.
                offsetRatio = location.PositiveOffsetPercentage.Value / 100.0;
            }

            // calculate the actual location and take into account the shape.
            int offsetEdgeIdx;
            GeoCoordinate offsetLocation;
            Meter offsetLength;
            Meter offsetEdgeLength;
            Meter edgeLength;
            var coordinates = this.GetCoordinates(best.Route, offsetRatio, out offsetEdgeIdx, out offsetLocation, out offsetLength, out offsetEdgeLength, out edgeLength);

            var longitudeReference = offsetLocation.Longitude;
            var latitudeReference = offsetLocation.Latitude;

            // create the referenced location.
            var pointAlongLineLocation = new ReferencedPointAlongLine<TEdge>();
            pointAlongLineLocation.Score = totalScore;
            pointAlongLineLocation.Route = best.Route;
            pointAlongLineLocation.Latitude = latitudeReference;
            pointAlongLineLocation.Longitude = longitudeReference;
            if(location.Orientation.HasValue)
            {
                pointAlongLineLocation.Orientation = location.Orientation.Value;
            }
            else
            {
                pointAlongLineLocation.Orientation = Orientation.NoOrientation;
            }

            // add the edge meta data.
            pointAlongLineLocation.EdgeMeta = new EdgeMeta()
            {
                Idx = offsetEdgeIdx,
                Length = edgeLength,
                Offset = offsetEdgeLength
            };

            return pointAlongLineLocation;
        }
    }
}