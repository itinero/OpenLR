using OpenLR.Decoding;
using OpenLR.Model;
using OpenLR.OsmSharp.Decoding;
using OpenLR.OsmSharp.Decoding.Candidates;
using OpenLR.Referenced;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Units.Distance;
using System;
using System.Collections.Generic;

namespace OpenLR.OsmSharp
{
    /// <summary>
    /// A referenced decoder implementation.
    /// </summary>
    public abstract class ReferencedDecoderBase<TEdge> : OpenLR.Referenced.Decoding.ReferencedDecoder
        where TEdge : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Holds the basic router datasource.
        /// </summary>
        private readonly IBasicRouterDataSource<TEdge> _graph;

        /// <summary>
        /// Holds the referenced circle decoder.
        /// </summary>
        private readonly ReferencedCircleDecoder<TEdge> _referencedCircleDecoder;

        /// <summary>
        /// Holds the referenced geo coordinate decoder.
        /// </summary>
        private readonly ReferencedGeoCoordinateDecoder<TEdge> _referencedGeoCoordinateDecoder;

        /// <summary>
        /// Holds the referenced grid decoder.
        /// </summary>
        private readonly ReferencedGridDecoder<TEdge> _referencedGridDecoder;

        /// <summary>
        /// Holds the referenced line decoder.
        /// </summary>
        private readonly ReferencedLineDecoder<TEdge> _referencedLineDecoder;

        /// <summary>
        /// Holds the referenced point along line decoder.
        /// </summary>
        private readonly ReferencedPointAlongLineDecoder<TEdge> _referencedPointAlongLineDecoder;

        /// <summary>
        /// Holds the referenced polygon decoder.
        /// </summary>
        private readonly ReferencedPolygonDecoder<TEdge> _referencedPolygonDecoder;

        /// <summary>
        /// Holds the referenced rectangle decoder.
        /// </summary>
        private readonly ReferencedRectangleDecoder<TEdge> _referencedRectangleDecoder;

        /// <summary>
        /// Creates a new referenced decoder.
        /// </summary>
        /// <param name="locationDecoder"></param>
        public ReferencedDecoderBase(IBasicRouterDataSource<TEdge> graph, Decoder locationDecoder)
            :base(locationDecoder)
        {
            _graph = graph;

            _referencedCircleDecoder = this.GetReferencedCircleDecoder(_graph);
            _referencedGeoCoordinateDecoder = this.GetReferencedGeoCoordinateDecoder(_graph);
            _referencedGridDecoder = this.GetReferencedGridDecoder(_graph);
            _referencedLineDecoder = this.GetReferencedLineDecoder(_graph);
            _referencedPointAlongLineDecoder = this.GetReferencedPointAlongLineDecoder(_graph);
            _referencedPolygonDecoder = this.GetReferencedPolygonDecoder(_graph);
            _referencedRectangleDecoder = this.GetReferencedRectangleDecoder(_graph);
        }

        /// <summary>
        /// Returns the router.
        /// </summary>
        /// <returns></returns>
        protected abstract IBasicRouter<TEdge> GetRouter();
        
        /// <summary>
        /// Returns the graph.
        /// </summary>
        protected IBasicRouterDataSource<TEdge> Graph
        {
            get
            {
                return _graph;
            }
        }

        /// <summary>
        /// Holds the referenced circle decoder.
        /// </summary>
        protected virtual ReferencedCircleDecoder<TEdge> GetReferencedCircleDecoder(IBasicRouterDataSource<TEdge> graph)
        {
            return new ReferencedCircleDecoder<TEdge>(this, this.LocationDecoder.CreateCircleLocationDecoder(), graph, this.GetRouter());
        }

        /// <summary>
        /// Holds the referenced geo coordinate decoder.
        /// </summary>
        protected virtual ReferencedGeoCoordinateDecoder<TEdge> GetReferencedGeoCoordinateDecoder(IBasicRouterDataSource<TEdge> graph)
        {
            return new ReferencedGeoCoordinateDecoder<TEdge>(this, this.LocationDecoder.CreateGeoCoordinateLocationDecoder(), graph, this.GetRouter());
        }

        /// <summary>
        /// Holds the referenced grid decoder.
        /// </summary>
        protected virtual ReferencedGridDecoder<TEdge> GetReferencedGridDecoder(IBasicRouterDataSource<TEdge> graph)
        {
            return new ReferencedGridDecoder<TEdge>(this, this.LocationDecoder.CreateGridLocationDecoder(), graph, this.GetRouter());
        }

        /// <summary>
        /// Holds the referenced line decoder.
        /// </summary>
        protected virtual ReferencedLineDecoder<TEdge> GetReferencedLineDecoder(IBasicRouterDataSource<TEdge> graph)
        {
            return new ReferencedLineDecoder<TEdge>(this, this.LocationDecoder.CreateLineLocationDecoder(), graph, this.GetRouter());
        }

        /// <summary>
        /// Holds the referenced point along line decoder.
        /// </summary>
        protected virtual ReferencedPointAlongLineDecoder<TEdge> GetReferencedPointAlongLineDecoder(IBasicRouterDataSource<TEdge> graph)
        {
            return new ReferencedPointAlongLineDecoder<TEdge>(this, this.LocationDecoder.CreatePointAlongLineLocationDecoder(), graph, this.GetRouter());
        }


        /// <summary>
        /// Holds the referenced polygon decoder.
        /// </summary>
        protected virtual ReferencedPolygonDecoder<TEdge> GetReferencedPolygonDecoder(IBasicRouterDataSource<TEdge> graph)
        {
            return new ReferencedPolygonDecoder<TEdge>(this, this.LocationDecoder.CreatePolygonLocationDecoder(), graph, this.GetRouter());
        }


        /// <summary>
        /// Holds the referenced rectangle decoder.
        /// </summary>
        protected virtual ReferencedRectangleDecoder<TEdge> GetReferencedRectangleDecoder(IBasicRouterDataSource<TEdge> graph)
        {
            return new ReferencedRectangleDecoder<TEdge>(this, this.LocationDecoder.CreateRectangleLocationDecoder(), graph, this.GetRouter());
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
        /// Finds all candidate vertex/edge pairs for a given location reference point.
        /// </summary>
        /// <param name="lrp"></param>
        /// <param name="forward"></param>
        /// <returns></returns>
        public virtual SortedSet<CandidateVertexEdge<TEdge>> FindCandidatesFor(LocationReferencePoint lrp, bool forward)
        {
            var vertexEdgeCandidates = new SortedSet<CandidateVertexEdge<TEdge>>(new CandidateVertexEdgeComparer<TEdge>());
            var vertexCandidates = this.FindCandidateVerticesFor(lrp);
            foreach (var vertexCandidate in vertexCandidates)
            {
                var edgeCandidates = this.FindCandidateEdgesFor(vertexCandidate.Vertex, forward, lrp.FormOfWay.Value, lrp.FuntionalRoadClass.Value);
                foreach (var edgeCandidate in edgeCandidates)
                {
                    vertexEdgeCandidates.Add(new CandidateVertexEdge<TEdge>()
                    {
                        Edge = edgeCandidate.Edge,
                        Vertex = vertexCandidate.Vertex,
                        Score = vertexCandidate.Score = edgeCandidate.Score
                    });
                }
            }
            return vertexEdgeCandidates;
        }

        /// <summary>
        /// Finds candidate vertices for a location reference point.
        /// </summary>
        /// <param name="lrp"></param>
        /// <returns></returns>
        public virtual IEnumerable<CandidateVertex> FindCandidateVerticesFor(LocationReferencePoint lrp, Meter maxVertexDistance)
        {
            // convert to geo coordinate.
            var geoCoordinate = new GeoCoordinate(lrp.Coordinate.Latitude, lrp.Coordinate.Longitude);

            // build candidates list.
            var candidates = new HashSet<uint>();
            var scoredCandidates = new List<CandidateVertex>();

            float latitude, longitude;

            // create a search box.
            var box = new GeoCoordinateBox(geoCoordinate, geoCoordinate);
            box = box.Resize(0.1);

            // get arcs.
            var arcs = this.Graph.GetArcs(box);
            foreach (var arc in arcs)
            {
                uint vertex = arc.Key;
                if (!candidates.Contains(vertex))
                {
                    this.Graph.GetVertex(vertex, out latitude, out longitude);
                    var distance = geoCoordinate.DistanceEstimate(new GeoCoordinate(latitude, longitude));
                    if (distance.Value < maxVertexDistance.Value)
                    {
                        candidates.Add(vertex);
                        scoredCandidates.Add(new CandidateVertex()
                        {
                            Score = (float)(1.0 - (distance.Value / maxVertexDistance.Value)),
                            Vertex = vertex
                        });
                    }
                }
                vertex = arc.Value.Key;
                if (!candidates.Contains(vertex))
                {
                    this.Graph.GetVertex(vertex, out latitude, out longitude);
                    var distance = geoCoordinate.DistanceEstimate(new GeoCoordinate(latitude, longitude));
                    if (distance.Value < maxVertexDistance.Value)
                    {
                        candidates.Add(vertex);
                        scoredCandidates.Add(new CandidateVertex()
                        {
                            Score = (float)(1.0 - (distance.Value / maxVertexDistance.Value)),
                            Vertex = vertex
                        });
                    }
                }
            }
            return scoredCandidates;
        }

        /// <summary>
        /// Finds candidate vertices for a location reference point.
        /// </summary>
        /// <param name="lrp"></param>
        /// <returns></returns>
        public abstract IEnumerable<CandidateVertex> FindCandidateVerticesFor(LocationReferencePoint lrp);

        /// <summary>
        /// Finds candidate edges for a vertex matching a given fow and frc.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="forward"></param>
        /// <param name="fow"></param>
        /// <param name="frc"></param>
        /// <returns></returns>
        public virtual IEnumerable<CandidateEdge> FindCandidateEdgesFor(uint vertex, bool forward, FormOfWay fow, FunctionalRoadClass frc)
        {
            var relevantEdges = new List<CandidateEdge>();
            foreach (var arc in this.Graph.GetArcs(vertex))
            {
                if (arc.Value.Forward == forward)
                {
                    var tags = this.Graph.TagsIndex.Get(arc.Value.Tags);
                    var score = this.MatchArc(tags, fow, frc);
                    if (score > 0)
                    { // ok, there is a match.
                        relevantEdges.Add(new CandidateEdge()
                        {
                            Score = score,
                            Edge = arc.Value
                        });
                    }
                }
            }
            return relevantEdges;
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
        /// Calculates a route between the two given vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="minimum">The minimum FRC.</param>
        /// <returns></returns>
        public abstract CandidateRoute<TEdge> FindCandiateRoute(uint from, uint to, FunctionalRoadClass minimum);

        /// <summary>
        /// Returns the coordinate of the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public virtual Coordinate GetVertexLocation(long vertex)
        {
            float latitude, longitude;
            if (!this.Graph.GetVertex((uint)vertex, out latitude, out longitude))
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
        /// Represents a candidate vertex and associated score.
        /// </summary>
        public class CandidateVertex
        {
            /// <summary>
            /// Gets or sets the score.
            /// </summary>
            public float Score { get; set; }

            /// <summary>
            /// Gets or sets the vertex.
            /// </summary>
            public uint Vertex { get; set; }

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
            public float Score { get; set; }

            /// <summary>
            /// Gets or sets the vertex.
            /// </summary>
            public TEdge Edge { get; set; }

            /// <summary>
            /// Determines whether this object is equal to the given object.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                var other = (obj as CandidateEdge);
                return other != null && other.Edge.Equals(this.Edge) && other.Score == this.Score;
            }

            /// <summary>
            /// Serves as a hashfunction.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return this.Score.GetHashCode() ^
                    this.Edge.GetHashCode();
            }
        }
    }
}