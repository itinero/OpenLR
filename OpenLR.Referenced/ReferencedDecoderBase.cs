using OpenLR.Decoding;
using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.Referenced.Decoding;
using OpenLR.Referenced.Decoding.Candidates;
using OpenLR.Referenced.Encoding;
using OpenLR.Referenced.Router;
using OpenLR.Referenced.Scoring;
using OpenLR.Referenced;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Units.Angle;
using OsmSharp.Units.Distance;
using System;
using System.Collections.Generic;
using System.Linq;
using OsmSharp.Routing.Osm.Graphs;

namespace OpenLR.Referenced
{
    /// <summary>
    /// A referenced decoder implementation.
    /// </summary>
    public abstract class ReferencedDecoderBase : OpenLR.Referenced.Decoding.ReferencedDecoder
    {
        /// <summary>
        /// Holds the maximum vertex distance.
        /// </summary>
        private Meter _maxVertexDistance = 40;

        /// <summary>
        /// Holds the basic router datasource.
        /// </summary>
        private readonly BasicRouterDataSource<LiveEdge> _graph;

        /// <summary>
        /// Holds the referenced circle decoder.
        /// </summary>
        private readonly ReferencedCircleDecoder _referencedCircleDecoder;

        /// <summary>
        /// Holds the referenced geo coordinate decoder.
        /// </summary>
        private readonly ReferencedGeoCoordinateDecoder _referencedGeoCoordinateDecoder;

        /// <summary>
        /// Holds the referenced grid decoder.
        /// </summary>
        private readonly ReferencedGridDecoder _referencedGridDecoder;

        /// <summary>
        /// Holds the referenced line decoder.
        /// </summary>
        private readonly ReferencedLineDecoder _referencedLineDecoder;

        /// <summary>
        /// Holds the referenced point along line decoder.
        /// </summary>
        private readonly ReferencedPointAlongLineDecoder _referencedPointAlongLineDecoder;

        /// <summary>
        /// Holds the referenced polygon decoder.
        /// </summary>
        private readonly ReferencedPolygonDecoder _referencedPolygonDecoder;

        /// <summary>
        /// Holds the referenced rectangle decoder.
        /// </summary>
        private readonly ReferencedRectangleDecoder _referencedRectangleDecoder;

        /// <summary>
        /// The vehicle profile to use for decoding.
        /// </summary>
        private readonly Vehicle _vehicle;

        /// <summary>
        /// Creates a new referenced decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="vehicle"></param>
        /// <param name="locationDecoder"></param>
        /// <param name="maxVertexDistance"></param>
        public ReferencedDecoderBase(BasicRouterDataSource<LiveEdge> graph, Vehicle vehicle, Decoder locationDecoder, Meter maxVertexDistance)
            :base(locationDecoder)
        {
            _graph = graph;
            _maxVertexDistance = maxVertexDistance;
            _vehicle = vehicle;

            _referencedCircleDecoder = this.GetReferencedCircleDecoder();
            _referencedGeoCoordinateDecoder = this.GetReferencedGeoCoordinateDecoder();
            _referencedGridDecoder = this.GetReferencedGridDecoder();
            _referencedLineDecoder = this.GetReferencedLineDecoder();
            _referencedPointAlongLineDecoder = this.GetReferencedPointAlongLineDecoder();
            _referencedPolygonDecoder = this.GetReferencedPolygonDecoder();
            _referencedRectangleDecoder = this.GetReferencedRectangleDecoder();
        }

        /// <summary>
        /// Creates a new referenced decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="vehicle"></param>
        /// <param name="locationDecoder"></param>
        public ReferencedDecoderBase(BasicRouterDataSource<LiveEdge> graph, Vehicle vehicle, Decoder locationDecoder)
            :base(locationDecoder)
        {
            _graph = graph;
            _vehicle = vehicle;

            _referencedCircleDecoder = this.GetReferencedCircleDecoder();
            _referencedGeoCoordinateDecoder = this.GetReferencedGeoCoordinateDecoder();
            _referencedGridDecoder = this.GetReferencedGridDecoder();
            _referencedLineDecoder = this.GetReferencedLineDecoder();
            _referencedPointAlongLineDecoder = this.GetReferencedPointAlongLineDecoder();
            _referencedPolygonDecoder = this.GetReferencedPolygonDecoder();
            _referencedRectangleDecoder = this.GetReferencedRectangleDecoder();
        }

        /// <summary>
        /// Gets a new router.
        /// </summary>
        /// <returns></returns>
        protected virtual BasicRouter GetRouter()
        {
            return new BasicRouter();
        }
        
        /// <summary>
        /// Returns the graph.
        /// </summary>
        protected BasicRouterDataSource<LiveEdge> Graph
        {
            get
            {
                return _graph;
            }
        }

        /// <summary>
        /// Returns the vehicle.
        /// </summary>
        protected Vehicle Vehicle
        {
            get
            {
                return _vehicle;
            }
        }

        /// <summary>
        /// Returns the max vertex distance.
        /// </summary>
        public Meter MaxVertexDistance
        {
            get
            {
                return _maxVertexDistance;
            }
        }

        /// <summary>
        /// Holds the referenced circle decoder.
        /// </summary>
        protected virtual ReferencedCircleDecoder GetReferencedCircleDecoder()
        {
            return new ReferencedCircleDecoder(this, this.LocationDecoder.CreateCircleLocationDecoder());
        }

        /// <summary>
        /// Holds the referenced geo coordinate decoder.
        /// </summary>
        protected virtual ReferencedGeoCoordinateDecoder GetReferencedGeoCoordinateDecoder()
        {
            return new ReferencedGeoCoordinateDecoder(this, this.LocationDecoder.CreateGeoCoordinateLocationDecoder());
        }

        /// <summary>
        /// Holds the referenced grid decoder.
        /// </summary>
        protected virtual ReferencedGridDecoder GetReferencedGridDecoder()
        {
            return new ReferencedGridDecoder(this, this.LocationDecoder.CreateGridLocationDecoder());
        }

        /// <summary>
        /// Holds the referenced line decoder.
        /// </summary>
        protected virtual ReferencedLineDecoder GetReferencedLineDecoder()
        {
            return new ReferencedLineDecoder(this, this.LocationDecoder.CreateLineLocationDecoder());
        }

        /// <summary>
        /// Holds the referenced point along line decoder.
        /// </summary>
        protected virtual ReferencedPointAlongLineDecoder GetReferencedPointAlongLineDecoder()
        {
            return new ReferencedPointAlongLineDecoder(this, this.LocationDecoder.CreatePointAlongLineLocationDecoder());
        }


        /// <summary>
        /// Holds the referenced polygon decoder.
        /// </summary>
        protected virtual ReferencedPolygonDecoder GetReferencedPolygonDecoder()
        {
            return new ReferencedPolygonDecoder(this, this.LocationDecoder.CreatePolygonLocationDecoder());
        }


        /// <summary>
        /// Holds the referenced rectangle decoder.
        /// </summary>
        protected virtual ReferencedRectangleDecoder GetReferencedRectangleDecoder()
        {
            return new ReferencedRectangleDecoder(this, this.LocationDecoder.CreateRectangleLocationDecoder());
        }

        /// <summary>
        /// Decodes the given data into a raw location.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual ILocation DecodeRaw(string data)
        {
            if (_referencedCircleDecoder.CanDecode(data))
            {
                return _referencedCircleDecoder.DecodeRaw(data);
            }
            if (_referencedGeoCoordinateDecoder.CanDecode(data))
            {
                return _referencedGeoCoordinateDecoder.DecodeRaw(data);
            }
            if (_referencedGridDecoder.CanDecode(data))
            {
                return _referencedGridDecoder.DecodeRaw(data);
            }
            if (_referencedLineDecoder.CanDecode(data))
            {
                return _referencedLineDecoder.DecodeRaw(data);
            }
            if (_referencedPointAlongLineDecoder.CanDecode(data))
            {
                return _referencedPointAlongLineDecoder.DecodeRaw(data);
            }
            if (_referencedPolygonDecoder.CanDecode(data))
            {
                return _referencedPolygonDecoder.DecodeRaw(data);
            }
            if (_referencedRectangleDecoder.CanDecode(data))
            {
                return _referencedRectangleDecoder.DecodeRaw(data);
            }
            throw new ArgumentOutOfRangeException("data",
                string.Format("Data cannot be decode by any of the registered decoders: {0}", data));
        }

        /// <summary>
        /// Decodes the given data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override ReferencedLocation Decode(string data)
        {
            if(_referencedCircleDecoder.CanDecode(data))
            {
                return _referencedCircleDecoder.Decode(data);
            }
            if(_referencedGeoCoordinateDecoder.CanDecode(data))
            {
                return _referencedGeoCoordinateDecoder.Decode(data);
            }
            if(_referencedGridDecoder.CanDecode(data))
            {
                return _referencedGridDecoder.Decode(data);
            }
            if(_referencedLineDecoder.CanDecode(data))
            {
                return _referencedLineDecoder.Decode(data);
            }
            if(_referencedPointAlongLineDecoder.CanDecode(data))
            {
                return _referencedPointAlongLineDecoder.Decode(data);
            }
            if(_referencedPolygonDecoder.CanDecode(data))
            {
                return _referencedPolygonDecoder.Decode(data);
            }
            if(_referencedRectangleDecoder.CanDecode(data))
            {
                return _referencedRectangleDecoder.Decode(data);
            }
            throw new ArgumentOutOfRangeException("data",
                string.Format("Data cannot be decode by any of the registered decoders: {0}", data));
        }

        /// <summary>
        /// Finds candidate vertices for a location reference point.
        /// </summary>
        /// <param name="lrp"></param>
        /// <returns></returns>
        public virtual IEnumerable<CandidateVertex> FindCandidateVerticesFor(LocationReferencePoint lrp)
        {
            return this.FindCandidateVerticesFor(lrp, _maxVertexDistance);
        }

        /// <summary>
        /// Finds all candidate vertex/edge pairs for a given location reference point.
        /// </summary>
        /// <param name="lrp"></param>
        /// <param name="forward"></param>
        /// <returns></returns>
        public virtual SortedSet<CandidateVertexEdge> FindCandidatesFor(LocationReferencePoint lrp, bool forward)
        {
            return this.FindCandidatesFor(lrp, forward, _maxVertexDistance);
        }

        /// <summary>
        /// Finds all candidate vertex/edge pairs for a given location reference point.
        /// </summary>
        /// <param name="lrp"></param>
        /// <param name="forward"></param>
        /// <param name="maxVertexDistance"></param>
        /// <returns></returns>
        public virtual SortedSet<CandidateVertexEdge> FindCandidatesFor(LocationReferencePoint lrp, bool forward, Meter maxVertexDistance)
        {
            var vertexEdgeCandidates = new SortedSet<CandidateVertexEdge>(new CandidateVertexEdgeComparer());
            var vertexCandidates = this.FindCandidateVerticesFor(lrp, maxVertexDistance);
            foreach (var vertexCandidate in vertexCandidates)
            {
                var edgeCandidates = this.FindCandidateEdgesFor(vertexCandidate.Vertex, forward, lrp.FormOfWay.Value, lrp.FuntionalRoadClass.Value, (Degree)lrp.Bearing.Value);
                foreach (var edgeCandidate in edgeCandidates)
                {
                    vertexEdgeCandidates.Add(new CandidateVertexEdge()
                    {
                        Edge = edgeCandidate.Edge,
                        Vertex = vertexCandidate.Vertex,
                        TargetVertex = edgeCandidate.TargetVertex,
                        Score = vertexCandidate.Score * edgeCandidate.Score
                    });
                }
            }
            return vertexEdgeCandidates;
        }

        /// <summary>
        /// Resets all created candidates.
        /// </summary>
        public virtual void ResetCreatedCandidates()
        {
            this.Graph.ClearModifications();
        }

        /// <summary>
        /// Creates candidate vertex/edge pairs for a given location reference point.
        /// </summary>
        /// <param name="lrp"></param>
        /// <param name="forward"></param>
        /// <returns></returns>
        public virtual SortedSet<CandidateVertexEdge> CreateCandidatesFor(LocationReferencePoint lrp, bool forward)
        {
            return this.CreateCandidatesFor(lrp, forward, _maxVertexDistance);
        }

        /// <summary>
        /// Finds all candidate vertex/edge pairs for a given location reference point.
        /// </summary>
        /// <param name="lrp"></param>
        /// <param name="forward"></param>
        /// <param name="maxVertexDistance"></param>
        /// <returns></returns>
        public abstract SortedSet<CandidateVertexEdge> CreateCandidatesFor(LocationReferencePoint lrp, bool forward, Meter maxVertexDistance);
        
        /// <summary>
        /// Finds candidate vertices for a location reference point.
        /// </summary>
        /// <param name="lrp"></param>
        /// <param name="maxVertexDistance"></param>
        /// <returns></returns>
        public virtual IEnumerable<CandidateVertex> FindCandidateVerticesFor(LocationReferencePoint lrp, Meter maxVertexDistance)
        {
            // convert to geo coordinate.
            var geoCoordinate = new GeoCoordinate(lrp.Coordinate.Latitude, lrp.Coordinate.Longitude);

            // build candidates list.
            var candidates = new HashSet<long>();
            var scoredCandidates = new List<CandidateVertex>();

            float latitude, longitude;

            // create a search box.
            var box = new GeoCoordinateBox(geoCoordinate, geoCoordinate);
            box = box.Resize(0.1);

            // get arcs.
            var arcs = this.Graph.GetEdges(box);
            foreach (var arc in arcs)
            {
                long vertex = arc.Item1;
                if (!candidates.Contains(vertex))
                {
                    this.Graph.GetVertex(vertex, out latitude, out longitude);
                    var distance = geoCoordinate.DistanceEstimate(new GeoCoordinate(latitude, longitude));
                    if (distance.Value < maxVertexDistance.Value)
                    {
                        candidates.Add(vertex);
                        scoredCandidates.Add(new CandidateVertex()
                        {
                            Score = Score.New(Score.VERTEX_DISTANCE, string.Format("The vertex score compare to max distance {0}", _maxVertexDistance), (float)System.Math.Max(0, (1.0 - (distance.Value / _maxVertexDistance.Value))), 1), // calculate scoring compared to the fixed max distance.
                            Vertex = vertex
                        });
                    }
                }
                vertex = arc.Item2;
                if (!candidates.Contains(vertex))
                {
                    this.Graph.GetVertex(vertex, out latitude, out longitude);
                    var distance = geoCoordinate.DistanceEstimate(new GeoCoordinate(latitude, longitude));
                    if (distance.Value < maxVertexDistance.Value)
                    {
                        candidates.Add(vertex);
                        scoredCandidates.Add(new CandidateVertex()
                        {
                            Score = Score.New(Score.VERTEX_DISTANCE, string.Format("The vertex score compare to max distance {0}", _maxVertexDistance), (float)System.Math.Max(0, (1.0 - (distance.Value / _maxVertexDistance.Value))), 1), // calculate scoring compared to the fixed max distance.
                            Vertex = vertex
                        });
                    }
                }
            }
            return scoredCandidates;
        }

        /// <summary>
        /// Finds candidate edges starting at a given vertex matching a given fow and frc.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="forward"></param>
        /// <param name="fow"></param>
        /// <param name="frc"></param>
        /// <param name="bearing"></param>
        /// <returns></returns>
        public virtual IEnumerable<CandidateEdge> FindCandidateEdgesFor(long vertex, bool forward, FormOfWay fow, FunctionalRoadClass frc, Degree bearing)
        {
            var relevantEdges = new List<CandidateEdge>();
            foreach (var arc in this.Graph.GetEdges(vertex))
            {
                var tags = this.Graph.TagsIndex.Get(arc.Value.Tags);

                // check one-way.
                if (_vehicle.CanTraverse(tags))
                { // yay! can traverse.
                    var oneway = _vehicle.IsOneWay(tags);
                    if (oneway == null ||
                        (forward && oneway.Value == arc.Value.Forward) ||
                        (!forward && oneway.Value != arc.Value.Forward))
                    {
                        var score = Score.New(Score.MATCH_ARC, "Metric indicating a match with fow, frc etc...", this.MatchArc(tags, fow, frc), 1);
                        if (score.Value > 0)
                        { // ok, there is a match.
                            // check bearing.
                            var shape = this.Graph.GetEdgeShape(vertex, arc.Key);
                            var localBearing = this.GetBearing(vertex, arc.Value, shape, arc.Key, true);
                            var localBearingDiff = (float)System.Math.Abs(localBearing.SmallestDifference(bearing));

                            relevantEdges.Add(new CandidateEdge()
                            {
                                TargetVertex = arc.Key,
                                Score = score + 
                                    Score.New(Score.BEARING_DIFF, "Bearing difference (0=0 & 180=1)", ((180f - localBearingDiff) / 180f), 1),
                                Edge = arc.Value
                            });
                        }
                    }
                }
            }
            return relevantEdges;
        }

        /// <summary>
        /// Returns the location of the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public virtual Coordinate GetVertexLocation(long vertex)
        {
            float latitude, longitude;
            if (!this.Graph.GetVertex(vertex, out latitude, out longitude))
            { // oeps, vertex does not exist!
                throw new ArgumentOutOfRangeException("vertex", string.Format("Vertex {0} not found!", vertex));
            }
            return new Coordinate()
            {
                Latitude = latitude,
                Longitude = longitude
            };
        }

        /// <summary>
        /// Calculates a match between the tags collection and the properties of the OpenLR location reference.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="fow"></param>
        /// <param name="frc"></param>
        /// <returns></returns>
        public abstract float MatchArc(TagsCollectionBase tags, FormOfWay fow, FunctionalRoadClass frc);

        /// <summary>
        /// Returns a value if a oneway restriction is found.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns>null: no restrictions, true: forward restriction, false: backward restriction.</returns>
        public abstract bool? IsOneway(TagsCollectionBase tags);

        /// <summary>
        /// Calculates a route between the two given vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="minimum">The minimum FRC.</param>
        /// <returns></returns>
        public abstract CandidateRoute FindCandiateRoute(CandidateVertexEdge from, CandidateVertexEdge to, FunctionalRoadClass minimum);

        /// <summary>
        /// Returns the coordinate of the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public virtual Coordinate GetCoordinate(long vertex)
        {
            float latitude, longitude;
            if (!this.Graph.GetVertex(vertex, out latitude, out longitude))
            { // oeps, vertex does not exist!
                throw new ArgumentOutOfRangeException("vertex", string.Format("Vertex {0} not found!", vertex));
            }
            return new Coordinate()
            {
                Latitude = latitude,
                Longitude = longitude
            };
        }

        /// <summary>
        /// Returns the list of coordinates for the given route.
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public virtual List<GeoCoordinate> GetCoordinates(Locations.ReferencedLine route)
        {
            if (route == null) { throw new ArgumentNullException("route"); }
            if (route.Edges == null || route.Edges.Length == 0) { throw new ArgumentOutOfRangeException("route", "Route has no edges."); }
            if (route.Vertices == null || route.Vertices.Length == 0) { throw new ArgumentOutOfRangeException("route", "Route has no vertices."); }
            if (route.Vertices.Length != route.Edges.Length + 1) { throw new ArgumentOutOfRangeException("route", "Route is invalid: there should be n vertices and n-1 edges."); }

            // loop over all coordinates and collect offsetLocation and offsetEdgeIdx.
            var coordinates = new List<GeoCoordinate>();
            coordinates.Add(this.GetCoordinate(route.Vertices[0]).ToGeoCoordinate());
            for (int edgeIdx = 0; edgeIdx < route.Edges.Length; edgeIdx++)
            {
                var edge = route.Edges[edgeIdx];
                var edgeShape = route.EdgeShapes[edgeIdx];
                if (edgeShape != null)
                { // there are intermediate coordinates.
                    if (edge.Forward)
                    { // the edge is forward.
                        for (int idx = 0; idx < edgeShape.Length; idx++)
                        {
                            coordinates.Add(new GeoCoordinate(edgeShape[idx].Latitude, edgeShape[idx].Longitude));
                        }
                    }
                    else
                    { // the edge is backward.
                        for (int idx = edgeShape.Length - 1; idx >= 0; idx--)
                        {
                            coordinates.Add(new GeoCoordinate(edgeShape[idx].Latitude, edgeShape[idx].Longitude));
                        }
                    }
                }
                coordinates.Add(this.GetCoordinate(route.Vertices[edgeIdx + 1]).ToGeoCoordinate());
            }
            return coordinates;
        }


        /// <summary>
        /// Returns the list of coordinates for the given route.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="offsetRatio"></param>
        /// <param name="offsetEdgeIdx"></param>
        /// <param name="offsetLocation"></param>
        /// <param name="offsetLength"></param>
        /// <param name="offsetEdgeLength"></param>
        /// <param name="edgeLength"></param>
        /// <returns></returns>
        public virtual List<GeoCoordinate> GetCoordinates(Locations.ReferencedLine route, double offsetRatio, 
            out int offsetEdgeIdx, out GeoCoordinate offsetLocation, out Meter offsetLength, out Meter offsetEdgeLength, out Meter edgeLength)
        {
            if (route == null) { throw new ArgumentNullException("route"); }
            if (route.Edges == null || route.Edges.Length == 0) { throw new ArgumentOutOfRangeException("route", "Route has no edges."); }
            if (route.Vertices == null || route.Vertices.Length == 0) { throw new ArgumentOutOfRangeException("route", "Route has no vertices."); }
            if (route.Vertices.Length != route.Edges.Length + 1) { throw new ArgumentOutOfRangeException("route", "Route is invalid: there should be n vertices and n-1 edges."); }

            // calculate the total length first.
            var totalLength = this.GetDistance(route);

            // calculate the lenght at the offst.
            offsetLength = (Meter)(totalLength.Value * offsetRatio);
            offsetEdgeLength = -1;
            offsetEdgeIdx = -1;
            edgeLength = -1;

            // loop over all coordinates and collect offsetLocation and offsetEdgeIdx.
            double currentOffsetLength = 0;
            double currentEdgeLength = 0;
            var coordinates = new List<GeoCoordinate>();
            coordinates.Add(this.GetCoordinate(route.Vertices[0]).ToGeoCoordinate());
            for (int edgeIdx = 0; edgeIdx < route.Edges.Length; edgeIdx++)
            {
                currentEdgeLength = 0;
                var edge = route.Edges[edgeIdx];
                var edgeShapes = route.EdgeShapes[edgeIdx];
                if (edgeShapes != null)
                { // there are intermediate coordinates.
                    if (edge.Forward)
                    { // the edge is forward.
                        for (int idx = 0; idx < edgeShapes.Length; idx++)
                        {
                            coordinates.Add(new GeoCoordinate(edgeShapes[idx].Latitude, edgeShapes[idx].Longitude));
                            currentEdgeLength = currentEdgeLength + coordinates[coordinates.Count - 2].DistanceEstimate(coordinates[coordinates.Count - 1]).Value;
                        }
                    }
                    else
                    { // the edge is backward.
                        for (int idx = edgeShapes.Length - 1; idx >= 0; idx--)
                        {
                            coordinates.Add(new GeoCoordinate(edgeShapes[idx].Latitude, edgeShapes[idx].Longitude));
                            currentEdgeLength = currentEdgeLength + coordinates[coordinates.Count - 2].DistanceEstimate(coordinates[coordinates.Count - 1]).Value;
                        }
                    }
                }
                coordinates.Add(this.GetCoordinate(route.Vertices[edgeIdx + 1]).ToGeoCoordinate());
                currentEdgeLength = currentEdgeLength + coordinates[coordinates.Count - 2].DistanceEstimate(coordinates[coordinates.Count - 1]).Value;

                // add current edge length to current offset.
                if ((currentOffsetLength + currentEdgeLength) >= offsetLength.Value && 
                    edgeLength.Value < 0)
                { // it's this edge that has the valuable info.
                    offsetEdgeIdx = edgeIdx;
                    offsetEdgeLength = offsetLength - currentOffsetLength;
                    edgeLength = currentEdgeLength;
                }
                currentOffsetLength = currentOffsetLength + currentEdgeLength;
            }

            // choose the last edge.
            if (edgeLength.Value < 0)
            { // it's this edge that has the valuable info.
                offsetEdgeIdx = route.Edges.Length - 1;
                offsetEdgeLength = offsetLength - currentOffsetLength;
                edgeLength = currentEdgeLength;
            }

            // calculate actual offset position.
            offsetLocation = coordinates.GetPositionLocation(offsetRatio);
            return coordinates;
        }

        /// <summary>
        /// Returns the distance for the given route.
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public virtual Meter GetDistance(Locations.ReferencedLine route)
        {
            var coordinates = new List<GeoCoordinate>();
            coordinates.Add(this.GetCoordinate(route.Vertices[0]).ToGeoCoordinate());
            for (int edgeIdx = 0; edgeIdx < route.Edges.Length; edgeIdx++)
            {
                var edge = route.Edges[edgeIdx];
                var edgeShape = route.EdgeShapes[edgeIdx];
                if (edgeShape != null)
                { // there are intermediate coordinates.
                    if (edge.Forward)
                    { // the edge is forward.
                        for (int idx = 0; idx < edgeShape.Length; idx++)
                        {
                            coordinates.Add(new GeoCoordinate(edgeShape[idx].Latitude, edgeShape[idx].Longitude));
                        }
                    }
                    else
                    { // the edge is backward.
                        for (int idx = edgeShape.Length - 1; idx >= 0; idx--)
                        {
                            coordinates.Add(new GeoCoordinate(edgeShape[idx].Latitude, edgeShape[idx].Longitude));
                        }
                    }
                }
                coordinates.Add(this.GetCoordinate(route.Vertices[edgeIdx + 1]).ToGeoCoordinate());
            }

            Meter distance = 0;
            if(coordinates != null && coordinates.Count > 1)
            {
                for(int idx = 1; idx < coordinates.Count; idx++)
                {
                    distance = distance + coordinates[idx - 1].DistanceReal(coordinates[idx]);
                }
            }
            return distance;
        }

        /// <summary>
        /// Returns the bearing calculate between two given vertices along the given edge.
        /// </summary>
        /// <param name="vertexFrom"></param>
        /// <param name="edge"></param>
        /// <param name="edgeShape"></param>
        /// <param name="vertexTo"></param>
        /// <param name="forward">When true the edge is forward relative to the vertices, false the edge is backward.</param>
        /// <returns></returns>
        public virtual Degree GetBearing(long vertexFrom, LiveEdge edge, GeoCoordinateSimple[] edgeShape, long vertexTo, bool forward)
        {
            var coordinates = new List<GeoCoordinate>();
            float latitude, longitude;
            this.Graph.GetVertex(vertexFrom, out latitude, out longitude);
            coordinates.Add(new GeoCoordinate(latitude, longitude));

            if (edgeShape != null)
            { // there are intermediates, add them in the correct order.
                if (forward)
                {
                    coordinates.AddRange(edgeShape.Select<GeoCoordinateSimple, GeoCoordinate>(x => { return new GeoCoordinate(x.Latitude, x.Longitude); }));
                }
                else
                {
                    coordinates.AddRange(edgeShape.Reverse().Select<GeoCoordinateSimple, GeoCoordinate>(x => { return new GeoCoordinate(x.Latitude, x.Longitude); }));
                }
            }

            this.Graph.GetVertex(vertexTo, out latitude, out longitude);
            coordinates.Add(new GeoCoordinate(latitude, longitude));

            return BearingEncoder.EncodeBearing(coordinates);
        }

        /// <summary>
        /// Represents a candidate vertex and associated score.
        /// </summary>
        public class CandidateVertex
        {
            /// <summary>
            /// Gets or sets the score.
            /// </summary>
            public Score Score { get; set; }

            /// <summary>
            /// Gets or sets the vertex.
            /// </summary>
            public long Vertex { get; set; }

            /// <summary>
            /// Determines whether this object is equal to the given object.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                var other = (obj as CandidateVertex);
                return other != null && other.Vertex == this.Vertex && other.Score == this.Score;
            }

            /// <summary>
            /// Serves as a hashfunction.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return this.Score.GetHashCode() ^
                    this.Vertex.GetHashCode();
            }
        }

        /// <summary>
        /// Represents a candidate edge and associated score.
        /// </summary>
        public class CandidateEdge
        {
            /// <summary>
            /// Gets or sets the score.
            /// </summary>
            public Score Score { get; set; }

            /// <summary>
            /// Gets or sets the vertex.
            /// </summary>
            public LiveEdge Edge { get; set; }

            /// <summary>
            /// Gets or sets the target vertex.
            /// </summary>
            public long TargetVertex { get; set; }

            /// <summary>
            /// Determines whether this object is equal to the given object.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                var other = (obj as CandidateEdge);
                return other != null && other.Edge.Equals(this.Edge) && other.Score == this.Score && other.TargetVertex == this.TargetVertex;
            }

            /// <summary>
            /// Serves as a hashfunction.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return this.Score.GetHashCode() ^
                    this.Edge.GetHashCode() ^
                    this.TargetVertex.GetHashCode();
            }
        }
    }
}