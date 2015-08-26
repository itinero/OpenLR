using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OpenLR.Referenced.Router;
using OpenLR.Referenced.Scoring;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Graphs;
using System.Collections.Generic;

namespace OpenLR.Referenced.Decoding.Candidates
{
    /// <summary>
    /// Represents a candidate vertex/edge pair and associated score.
    /// </summary>
    public class CandidateVertexEdge
    {
        /// <summary>
        /// The combined score of vertex and edge.
        /// </summary>
        public Score Score { get; set; }

        /// <summary>
        /// Gets or sets the candidate vertex.
        /// </summary>
        public long Vertex { get; set; }

        /// <summary>
        /// Gets or sets the candidate edge.
        /// </summary>
        public LiveEdge Edge { get; set; }

        /// <summary>
        /// Gets or sets the vertex this edge leads to.
        /// </summary>
        public long TargetVertex { get; set; }

        /// <summary>
        /// Converts this referenced location to a geometry.
        /// </summary>
        /// <returns></returns>
        public FeatureCollection ToFeatures(BasicRouterDataSource<LiveEdge> graph)
        {
            var featureCollection = new FeatureCollection();
            var geometryFactory = new GeometryFactory();

            // build coordinates list.
            var coordinates = new List<Coordinate>();
            float latitude, longitude;
            graph.GetVertex(this.Vertex, out latitude, out longitude);
            coordinates.Add(new Coordinate(longitude, latitude));

            var edgeShape = graph.GetEdgeShape(this.Vertex, this.TargetVertex);
            if (edgeShape != null)
            {
                for (int coordIdx = 0; coordIdx < edgeShape.Length; coordIdx++)
                {
                    coordinates.Add(new Coordinate()
                    {
                        X = edgeShape[coordIdx].Longitude,
                        Y = edgeShape[coordIdx].Latitude
                    });
                }
            }

            var tags = graph.TagsIndex.Get(this.Edge.Tags);
            var table = tags.ToAttributes();

            graph.GetVertex(this.TargetVertex, out latitude, out longitude);
            coordinates.Add(new Coordinate(longitude, latitude));

            featureCollection.Add(new Feature(geometryFactory.CreateLineString(coordinates.ToArray()), table));

            return featureCollection;
        }


        /// <summary>
        /// Determines whether this object is equal to the given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = (obj as CandidateVertexEdge);
            return other != null && other.Vertex == this.Vertex && other.TargetVertex == this.TargetVertex && other.Edge.Equals(this.Edge) && other.Score == this.Score;
        }

        /// <summary>
        /// Serves as a hashfunction.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Score.GetHashCode() ^
                this.Edge.GetHashCode() ^
                this.Vertex.GetHashCode() ^
                this.TargetVertex.GetHashCode();
        }

        /// <summary>
        /// Returns a description of this candidate.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}->{1}->{2}: {3}",
                this.Vertex.ToString(), this.Edge.ToString(), this.TargetVertex.ToString(), 
                this.Score.ToString());
        }
    }
}
