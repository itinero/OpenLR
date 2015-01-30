using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OpenLR.Referenced.Router;
using OpenLR.Referenced;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenLR.Referenced.Locations
{
    /// <summary>
    /// Represents a referenced line location with a graph as a reference.
    /// </summary>
    public class ReferencedLine<TEdge> : ReferencedLocation
        where TEdge : IGraphEdgeData
    {
        /// <summary>
        /// Holds the graph.
        /// </summary>
        private BasicRouterDataSource<TEdge> _graph;

        /// <summary>
        /// Creates a new referenced line.
        /// </summary>
        /// <param name="graph"></param>
        public ReferencedLine(BasicRouterDataSource<TEdge> graph)
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
        /// Gets or sets the edge shapes.
        /// </summary>
        public GeoCoordinateSimple[][] EdgeShapes { get; set; }

        /// <summary>
        /// Gets or sets the offset at the beginning of the path representing this location.
        /// </summary>
        public float PositiveOffsetPercentage { get; set; }

        /// <summary>
        /// Gets or sets the offset at the end of the path representing this location.
        /// </summary>
        public float NegativeOffsetPercentage { get; set; }

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
            for (int idx = 0; idx < this.Vertices.Length; idx++)
            {
                float latitude, longitude;
                _graph.GetVertex(this.Vertices[idx], out latitude, out longitude);
                coordinates.Add(new Coordinate(longitude, latitude));

                if (idx < this.Edges.Length)
                {
                    var edge = this.Edges[idx];
                    var edgeShape = this.EdgeShapes[idx];
                    if (edgeShape != null)
                    {
                        if (edge.Forward)
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
                        else
                        {
                            for (int coordIdx = edgeShape.Length - 1; coordIdx >= 0; coordIdx--)
                            {
                                coordinates.Add(new Coordinate()
                                {
                                    X = edgeShape[coordIdx].Longitude,
                                    Y = edgeShape[coordIdx].Latitude
                                });
                            }
                        }
                    }
                }
            }
            return geometryFactory.CreateLineString(coordinates.ToArray());
        }

        /// <summary>
        /// Converts this referenced location to a geometry.
        /// </summary>
        /// <returns></returns>
        public FeatureCollection ToFeatures()
        {
            var featureCollection = new FeatureCollection();
            var geometryFactory = new GeometryFactory();
            
            // build coordinates list.
            for (int idx = 0; idx < this.Edges.Length; idx++)
            {
                var coordinates = new List<Coordinate>();
                float latitude, longitude;
                _graph.GetVertex(this.Vertices[idx], out latitude, out longitude);
                coordinates.Add(new Coordinate(longitude, latitude));

                var edge = this.Edges[idx];
                var edgeShape = this.EdgeShapes[idx];
                if (edgeShape != null)
                {
                    if (edge.Forward)
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
                    else
                    {
                        for (int coordIdx = edgeShape.Length - 1; coordIdx >= 0; coordIdx--)
                        {
                            coordinates.Add(new Coordinate()
                            {
                                X = edgeShape[coordIdx].Longitude,
                                Y = edgeShape[coordIdx].Latitude
                            });
                        }
                    }
                }

                var tags = _graph.TagsIndex.Get(edge.Tags);
                var table = tags.ToAttributes();

                _graph.GetVertex(this.Vertices[idx + 1], out latitude, out longitude);
                coordinates.Add(new Coordinate(longitude, latitude));

                featureCollection.Add(new Feature(geometryFactory.CreateLineString(coordinates.ToArray()), table));
            }
            return featureCollection;
        }
    }
}