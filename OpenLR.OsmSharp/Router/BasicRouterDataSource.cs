using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using System;
using System.Collections.Generic;

namespace OpenLR.OsmSharp.Router
{
    /// <summary>
    /// Represents an editable data source or graph of routing data.
    /// </summary>
    public class BasicRouterDataSource<TEdge>
        where TEdge : IGraphEdgeData
    {
        /// <summary>
        /// Holds the datasource behing this one.
        /// </summary>
        private IBasicRouterDataSource<TEdge> _datasource;

        /// <summary>
        /// Holds the next vertex id.
        /// </summary>
        private long _nextVertexId = -1;

        /// <summary>
        /// Holds the new vertices.
        /// </summary>
        private Dictionary<long, GeoCoordinate> _newVertices;

        /// <summary>
        /// Holds the new arcs.
        /// </summary>
        private List<KeyValuePair<long, KeyValuePair<long, Tuple<TEdge, ICoordinateCollection>>>> _newEdges;

        /// <summary>
        /// Holds the removed arcs.
        /// </summary>
        private HashSet<Arc> _removedArcs;

        /// <summary>
        /// Creates a basic router datasource.
        /// </summary>
        /// <param name="datasource"></param>
        public BasicRouterDataSource(IBasicRouterDataSource<TEdge> datasource)
        {
            _datasource = datasource;

            _newEdges = new List<KeyValuePair<long, KeyValuePair<long, Tuple<TEdge, ICoordinateCollection>>>>();
            _removedArcs = new HashSet<Arc>();
            _newVertices = new Dictionary<long, GeoCoordinate>();
        }

        /// <summary>
        /// Clears all modifications.
        /// </summary>
        public void ClearModifications()
        {
            _nextVertexId = -1;

            _newEdges.Clear();
            _removedArcs.Clear();
            _newVertices.Clear();
        }

        /// <summary>
        /// Adds a new vertex.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public long AddVertex(float latitude, float longitude)
        {
            var newId = _nextVertexId;
            _newVertices[newId] = new GeoCoordinate(latitude, longitude);
            _nextVertexId--;
            return newId;
        }

        /// <summary>
        /// Mark the given arc as removed.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        public void RemoveEdge(long vertex1, long vertex2)
        {
            _removedArcs.Add(new Arc()
            {
                Vertex1 = vertex1,
                Vertex2 = vertex2
            });
        }

        /// <summary>
        /// Adds a new arc.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="edge"></param>
        public void AddEdge(long vertex1, long vertex2, TEdge edge)
        {
            _newEdges.Add(new KeyValuePair<long, KeyValuePair<long, Tuple<TEdge, ICoordinateCollection>>>(
                vertex1, new KeyValuePair<long, Tuple<TEdge, ICoordinateCollection>>(vertex2, new Tuple<TEdge, ICoordinateCollection>(edge, null))));
        }

        /// <summary>
        /// Adds a new arc.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="edge"></param>
        public void AddEdge(long vertex1, long vertex2, TEdge edge, GeoCoordinateSimple[] shape)
        {
            _newEdges.Add(new KeyValuePair<long, KeyValuePair<long, Tuple<TEdge, ICoordinateCollection>>>(
                vertex1, new KeyValuePair<long, Tuple<TEdge, ICoordinateCollection>>(vertex2, new Tuple<TEdge, ICoordinateCollection>(edge, new CoordinateArrayCollection<GeoCoordinateSimple>(shape)))));
        }

        /// <summary>
        /// Adds a new arc.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="edge"></param>
        public void AddEdge(long vertex1, long vertex2, TEdge edge, ICoordinateCollection shape)
        {
            _newEdges.Add(new KeyValuePair<long, KeyValuePair<long, Tuple<TEdge, ICoordinateCollection>>>(
                vertex1, new KeyValuePair<long, Tuple<TEdge,ICoordinateCollection>>(vertex2, new Tuple<TEdge, ICoordinateCollection>(edge, shape))));
        }

        /// <summary>
        /// Returns all the arcs in the given bbox.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public KeyValuePair<long, KeyValuePair<long, TEdge>>[] GetEdges(GeoCoordinateBox box)
        {
            var baseArcs = _datasource.GetEdges(box);
            var arcs = new List<KeyValuePair<long, KeyValuePair<long, TEdge>>>();
            foreach(var baseArc in baseArcs)
            {
                // check if the arc was removed.
                var arc = new Arc() {
                    Vertex1 = baseArc.Key, 
                    Vertex2 = baseArc.Value.Key
                };
                if (!_removedArcs.Contains(arc))
                { // the arc was not removed, also return it.
                    arcs.Add(new KeyValuePair<long, KeyValuePair<long, TEdge>>(baseArc.Key,
                        new KeyValuePair<long, TEdge>(baseArc.Value.Key, baseArc.Value.Value)));
                }
            }

            // also include new arcs.
            var inBoxNewVertices = new HashSet<long>();
            foreach(var newVertex in _newVertices)
            {
                if(box.Contains(newVertex.Value))
                {
                    inBoxNewVertices.Add(newVertex.Key);
                }
            }
            // check arcs to include.
            foreach(var newArc in _newEdges)
            {
                var arc = new Arc()
                {
                    Vertex1 = newArc.Key,
                    Vertex2 = newArc.Value.Key
                };
                if (inBoxNewVertices.Contains(arc.Vertex1) ||
                    inBoxNewVertices.Contains(arc.Vertex2))
                { // ok, vertices are contained.
                    arcs.Add(new KeyValuePair<long, KeyValuePair<long, TEdge>>(newArc.Key,
                        new KeyValuePair<long, TEdge>(newArc.Value.Key, newArc.Value.Value.Item1)));
                }
            }
            return arcs.ToArray();
        }

        /// <summary>
        /// Returns true if the given vehicle profile is supported.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public bool SupportsProfile(Vehicle vehicle)
        {
            return true;
        }

        /// <summary>
        /// Returns the tags index.
        /// </summary>
        public ITagsCollectionIndexReadonly TagsIndex
        {
            get { return _datasource.TagsIndex; }
        }

        /// <summary>
        /// Returns the restrictions at the given vertex.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="vertex"></param>
        /// <param name="routes"></param>
        /// <returns></returns>
        public bool TryGetRestrictionAsEnd(Vehicle vehicle, long vertex, out List<long[]> routes)
        {
            List<uint[]> baseRoutes;
            if (_datasource.TryGetRestrictionAsEnd(vehicle, Convert.ToUInt32(vertex), out baseRoutes))
            { // ok, there are restrictions, convert them.
                routes = null;
                if(baseRoutes != null)
                {
                    foreach(var restriction in baseRoutes)
                    {
                        var convertedRestriction = new long[restriction.Length];
                        for(int idx = 0; idx < restriction.Length; idx++)
                        {
                            convertedRestriction[idx] = restriction[idx];
                        }
                        routes.Add(convertedRestriction);
                    }
                }
                return true;
            }
            routes = null;
            return false;
        }

        /// <summary>
        /// Returns the restrictions at the given vertex.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="vertex"></param>
        /// <param name="routes"></param>
        /// <returns></returns>
        public bool TryGetRestrictionAsStart(Vehicle vehicle, long vertex, out List<long[]> routes)
        {
            List<uint[]> baseRoutes;
            if (_datasource.TryGetRestrictionAsStart(vehicle, Convert.ToUInt32(vertex), out baseRoutes))
            { // ok, there are restrictions, convert them.
                routes = null;
                if (baseRoutes != null)
                {
                    foreach (var restriction in baseRoutes)
                    {
                        var convertedRestriction = new long[restriction.Length];
                        for (int idx = 0; idx < restriction.Length; idx++)
                        {
                            convertedRestriction[idx] = restriction[idx];
                        }
                        routes.Add(convertedRestriction);
                    }
                }
                return true;
            }
            routes = null;
            return false;
        }

        /// <summary>
        /// Returns the arcs for the given vertex.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        public KeyValuePair<long, TEdge>[] GetEdges(long vertexId)
        {
            var baseArcs = new List<KeyValuePair<long, TEdge>>();
            if(vertexId > 0)
            { // vertex exists in base-graph.
                var baseArcsUint = _datasource.GetEdges(Convert.ToUInt32(vertexId));
                while(baseArcsUint.MoveNext())
                {
                    baseArcs.Add(new KeyValuePair<long, TEdge>(
                        baseArcsUint.Neighbour, baseArcsUint.EdgeData));
                }
            }
            var arcs = new List<KeyValuePair<long, TEdge>>();
            foreach (var baseArc in baseArcs)
            {
                // check if the arc was removed.
                var arc = new Arc()
                {
                    Vertex1 = vertexId,
                    Vertex2 = baseArc.Key
                };
                if (!_removedArcs.Contains(arc))
                { // the arc was not removed, also return it.
                    arcs.Add(new KeyValuePair<long, TEdge>(baseArc.Key, baseArc.Value));
                }
            }

            // also include new arcs.
            foreach (var newArc in _newEdges)
            {
                var arc = new Arc()
                {
                    Vertex1 = newArc.Key,
                    Vertex2 = newArc.Value.Key
                };
                if (arc.Vertex1 == vertexId)
                { // ok, vertices are contained.
                    arcs.Add(new KeyValuePair<long, TEdge>(newArc.Value.Key, newArc.Value.Value.Item1));
                }
            }
            return arcs.ToArray();
        }
        
        /// <summary>
        /// Returns the edge shape between the two given vertices.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="shape"></param>
        public bool GetEdgeShape(long vertex1, long vertex2, out ICoordinateCollection shape)
        {
            if (vertex1 > 0 && vertex2 > 0)
            {
                if (_datasource.ContainsEdge((uint)vertex1, (uint)vertex2))
                { // has the arc.
                    if(!_removedArcs.Contains(new Arc()
                    {
                        Vertex1 = vertex1,
                        Vertex2 = vertex2
                    }))
                    { // edge was not removed, only now return it's shape from the source.
                        return _datasource.GetEdgeShape((uint)vertex1, (uint)vertex2, out shape);
                    }
                }
            }

            // also check new arcs.
            foreach (var newArc in _newEdges)
            {
                if (newArc.Key == vertex1 &&
                    newArc.Value.Key == vertex2)
                {
                    shape = newArc.Value.Value.Item2;
                    return true;
                }
            }
            shape = null;
            return false;
        }

        /// <summary>
        /// Returns the edge shape or an empty array if no shape was found.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <returns></returns>
        public GeoCoordinateSimple[] GetEdgeShape(long vertex1, long vertex2)
        {
            ICoordinateCollection shape;
            if(this.GetEdgeShape(vertex1, vertex2, out shape) &&
                shape != null)
            {
                return shape.ToSimpleArray();
            }
            return new GeoCoordinateSimple[0];
        }

        /// <summary>
        /// Returns the vertex with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public bool GetVertex(long id, out float latitude, out float longitude)
        {
            latitude = 0;
            longitude = 0;
            if (id > 0)
            {
                if (_datasource.GetVertex((uint)id, out latitude, out longitude))
                {
                    return true;
                }
            }
            GeoCoordinate coordinates;
            if (_newVertices.TryGetValue(id, out coordinates))
            {
                latitude = (float)coordinates.Latitude;
                longitude = (float)coordinates.Longitude;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if this datasource contains the given arc.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <param name="neighbour"></param>
        /// <returns></returns>
        public bool HasEdge(long vertexId, long neighbour)
        {
            if (vertexId > 0 && neighbour > 0)
            {
                if (_datasource.ContainsEdge((uint)vertexId, (uint)neighbour))
                { // has the arc.
                    return !_removedArcs.Contains(new Arc()
                    {
                        Vertex1 = vertexId,
                        Vertex2 = neighbour
                    });
                }
            }

            // also check new arcs.
            foreach (var newArc in _newEdges)
            {
                if(newArc.Key == vertexId &&
                    newArc.Value.Key == neighbour)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the vertex count.
        /// </summary>
        public uint VertexCount
        {
            get { return (uint)(_datasource.VertexCount + _newVertices.Count); }
        }

        /// <summary>
        /// Represents an arc.
        /// </summary>
        private class Arc
        {
            /// <summary>
            /// Gets or sets the first vertex.
            /// </summary>
            public long Vertex1 { get; set; }

            /// <summary>
            /// Gets or sets the second vertex.
            /// </summary>
            public long Vertex2 { get; set; }

            /// <summary>
            /// Returns the hashcode.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return this.Vertex1.GetHashCode() ^
                    this.Vertex2.GetHashCode();
            }

            /// <summary>
            /// Returns true the given object represents the same arc as this one.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                var other = (obj as Arc);
                if(other != null)
                {
                    return other.Vertex1 == this.Vertex1 &&
                        other.Vertex2 == this.Vertex2;
                }
                return false;
            }
        }
    }
}