using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.Referenced;
using OpenLR.Referenced.Decoding;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Graph;
using OsmSharp.Units.Distance;
using System.Collections.Generic;
using System.Linq;

namespace OpenLR.OsmSharp.Decoding
{
    /// <summary>
    /// Represents a dynamic graph decoder: Decodes a raw OpenLR location into a location referenced to a dynamic graph.
    /// </summary>
    public abstract class GraphDecoder<TReferencedLocation, TLocation, TEdge> : ReferencedDecoder<TReferencedLocation, TLocation>
        where TEdge : IDynamicGraphEdgeData
        where TReferencedLocation : ReferencedLocation
        where TLocation : ILocation
    {
        /// <summary>
        /// Holds the maximum vertex distance.
        /// </summary>
        private Meter _maxVertexDistance = 20;

        /// <summary>
        /// Holds a dynamic graph.
        /// </summary>
        private DynamicGraphRouterDataSource<TEdge> _graph;

        /// <summary>
        /// Creates a new dynamic graph decoder.
        /// </summary>
        /// <param name="graph"></param>
        public GraphDecoder(OpenLR.Decoding.Decoder rawDecoder, DynamicGraphRouterDataSource<TEdge> graph)
            : base(rawDecoder)
        {
            _graph = graph;
        }

        /// <summary>
        /// Decodes an OpenLR-encoded unreferenced raw OpenLR location into a referenced Location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override abstract TReferencedLocation Decode(TLocation location);

        /// <summary>
        /// Finds all candidate vertex/edge pairs for a given location reference point.
        /// </summary>
        /// <param name="lrp"></param>
        /// <param name="forward"></param>
        /// <returns></returns>
        protected virtual SortedSet<CandidateVertexEdge> FindCandidatesFor(LocationReferencePoint lrp, bool forward)
        {
            var vertexEdgeCandidates = new SortedSet<CandidateVertexEdge>();
            var vertexCandidates = this.FindCandidateVerticesFor(lrp);
            foreach(var vertexCandidate in vertexCandidates)
            {
                var edgeCandidates = this.FindCandidateEdgesFor(vertexCandidate.Vertex, forward, lrp.FormOfWay.Value, lrp.FuntionalRoadClass.Value);
                foreach(var edgeCandidate in edgeCandidates)
                {
                    vertexEdgeCandidates.Add(new CandidateVertexEdge()
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
        protected virtual IEnumerable<CandidateVertex> FindCandidateVerticesFor(LocationReferencePoint lrp)
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
            var arcs = _graph.GetArcs(box);
            foreach (var arc in arcs)
            {
                uint vertex = arc.Key;
                if (!candidates.Contains(vertex))
                {
                    _graph.GetVertex(vertex, out latitude, out longitude);
                    var distance = geoCoordinate.DistanceEstimate(new GeoCoordinate(latitude, longitude));
                    if (distance.Value < _maxVertexDistance.Value)
                    {
                        candidates.Add(vertex);
                        scoredCandidates.Add(new CandidateVertex()
                            {
                                Score = (float)(1.0 - (distance.Value / _maxVertexDistance.Value)),
                                Vertex = vertex
                            });
                    }
                }
                vertex = arc.Value.Key;
                if (!candidates.Contains(vertex))
                {
                    _graph.GetVertex(vertex, out latitude, out longitude);
                    var distance = geoCoordinate.DistanceEstimate(new GeoCoordinate(latitude, longitude));
                    if (distance.Value < _maxVertexDistance.Value)
                    {
                        candidates.Add(vertex);
                        scoredCandidates.Add(new CandidateVertex()
                        {
                            Score = (float)(1.0 - (distance.Value / _maxVertexDistance.Value)),
                            Vertex = vertex
                        });
                    }
                }
            }
            return scoredCandidates;
        }

        /// <summary>
        /// Finds candidate edges for a vertex matching a given fow and frc.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="forward"></param>
        /// <param name="fow"></param>
        /// <param name="frc"></param>
        /// <returns></returns>
        protected virtual IEnumerable<CandidateEdge> FindCandidateEdgesFor(uint vertex, bool forward, FormOfWay fow, FunctionalRoadClass frc)
        {
            var relevantEdges = new List<CandidateEdge>();
            foreach (var arc in _graph.GetArcs(vertex))
            {
                if (arc.Value.Forward == forward)
                {
                    var tags = _graph.TagsIndex.Get(arc.Value.Tags);
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
        protected virtual float MatchArc(TagsCollectionBase tags, FormOfWay fow, FunctionalRoadClass frc)
        {
            string highway;
            if(!tags.TryGetValue("highway", out highway))
            { // not even a highway tag!
                return 0;
            }

            // TODO: take into account form of way? Maybe not for OSM-data?

            switch(frc)
            { // check there reference values against OSM: http://wiki.openstreetmap.org/wiki/Highway
                case FunctionalRoadClass.Frc0: // main road.
                    if(highway == "motorway" || highway == "trunk")
                    {
                        return 1;
                    }
                    return 0; // no match.
                case FunctionalRoadClass.Frc1: // first class road.
                    if (highway == "primary" || highway == "primary_link")
                    {
                        return 1;
                    }
                    return 0; // no match.
                case FunctionalRoadClass.Frc2: // second class road.
                    if (highway == "secondary" || highway == "secondary_link")
                    {
                        return 1;
                    }
                    return 0; // no match.
                case FunctionalRoadClass.Frc3: // third class road.
                    if (highway == "tertiary" || highway == "tertiary_link")
                    {
                        return 1;
                    }
                    return 0; // no match.
                case FunctionalRoadClass.Frc4:
                    if (highway == "road" || highway == "road_link" ||
                        highway == "unclassified" || highway == "residential")
                    {
                        return 1;
                    }
                    return 0; // no match.
                case FunctionalRoadClass.Frc5:
                    if (highway == "road" || highway == "road_link" ||
                        highway == "unclassified" || highway == "residential" ||
                        highway == "living_street")
                    {
                        return 1;
                    }
                    return 0; // no match.
                case FunctionalRoadClass.Frc6:
                    if (highway == "road" || highway == "track" ||
                        highway == "unclassified" || highway == "residential" ||
                        highway == "living_street")
                    {
                        return 1;
                    }
                    return 0; // no match.
                case FunctionalRoadClass.Frc7: // other class road.
                    if (highway == "footway" || highway == "bridleway" ||
                        highway == "steps" || highway == "path" ||
                        highway == "living_street")
                    {
                        return 1;
                    }
                    return 0; // no match.
            }
            return 0;
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

        /// <summary>
        /// Represents a candidate vertex/edge pair and associated score.
        /// </summary>
        public class CandidateVertexEdge
        {
            /// <summary>
            /// The combined score of vertex and edge.
            /// </summary>
            public float Score { get; set; }

            /// <summary>
            /// Gets or sets the candidate vertex.
            /// </summary>
            public uint Vertex { get; set; }

            /// <summary>
            /// Gets or sets the candidate edge.
            /// </summary>
            public TEdge Edge { get; set; }

            /// <summary>
            /// Determines whether this object is equal to the given object.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                var other = (obj as CandidateVertexEdge);
                return other != null && other.Vertex == this.Vertex && other.Edge.Equals(this.Edge) && other.Score == this.Score;
            }

            /// <summary>
            /// Serves as a hashfunction.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return this.Score.GetHashCode() ^
                    this.Edge.GetHashCode() ^
                    this.Vertex.GetHashCode();
            }
        }
        
        /// <summary>
        /// A comparer for vertex edge candidates.
        /// </summary>
        public class CandidateVertexEdgeComparer : IComparer<CandidateVertexEdge>
        {
            /// <summary>
            /// Compares the two given vertex-edge candidates.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public int Compare(CandidateVertexEdge x, CandidateVertexEdge y)
            {
                return x.Score.CompareTo(y.Score);
            }
        }
    }
}