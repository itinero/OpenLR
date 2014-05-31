using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using OpenLR.Referenced;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using System;
using System.Collections.Generic;

namespace OpenLR.OsmSharp.Decoding
{
    /// <summary>
    /// Represents a referenced line location with a graph as a reference.
    /// </summary>
    public class ReferencedLine<TEdge> : ReferencedLocation
        where TEdge : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Holds the graph.
        /// </summary>
        private IBasicRouterDataSource<TEdge> _graph;

        /// <summary>
        /// Creates a new referenced line.
        /// </summary>
        /// <param name="graph"></param>
        public ReferencedLine(IBasicRouterDataSource<TEdge> graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Gets or sets the vertices.
        /// </summary>
        public long[] Vertices { get; set; }

        /// <summary>
        /// Gets or sets the edges.
        /// </summary>
        public TEdge[] Edges { get; set; }

        /// <summary>
        /// Adds another line location to this one.
        /// </summary>
        /// <param name="location"></param>
        public void Add(ReferencedLine<TEdge> location)
        {
            if(this.Vertices[this.Vertices.Length - 1] == location.Vertices[0])
            { // there is a match.
                // merge vertices.
                var vertices = new long[this.Vertices.Length + location.Vertices.Length - 1];
                this.Vertices.CopyTo(vertices, 0);
                for(int idx = 1; idx < location.Vertices.Length; idx++)
                {
                    vertices[this.Vertices.Length + idx - 1] = location.Vertices[idx];
                }
                this.Vertices = vertices;

                // merge edges.
                var edges = new TEdge[this.Edges.Length + location.Edges.Length];
                this.Edges.CopyTo(edges, 0);
                location.Edges.CopyTo(edges, this.Edges.Length);
                this.Edges = edges;
                return;
            }
            throw new Exception("Cannot add a location without them having one vertex incommon.");
        }

        /// <summary>
        /// Converts this referenced location to a geometry.
        /// </summary>
        /// <returns></returns>
        public IGeometry ToGeometry()
        {
            var geometryFactory = new GeometryFactory();
            
            // build coordinates list.
            var coordinates = new List<Coordinate>();
            for(int idx = 0; idx < this.Vertices.Length; idx++)
            {
                float latitude, longitude;
                _graph.GetVertex((uint)this.Vertices[idx], out latitude, out longitude);
                coordinates.Add(new Coordinate(longitude, latitude));

                if(idx < this.Edges.Length)
                {
                    var edge = this.Edges[idx];
                    if (edge.Coordinates != null)
                    {
                        foreach(var coordinate in edge.Coordinates)
                        {
                            coordinates.Add(new Coordinate()
                            {
                                X = coordinate.Longitude,
                                Y = coordinate.Latitude
                            });
                        }
                    }
                }
            }
            return geometryFactory.CreateLineString(coordinates.ToArray());
        }
    }
}