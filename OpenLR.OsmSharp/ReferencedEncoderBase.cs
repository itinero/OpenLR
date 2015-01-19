using OpenLR.Encoding;
using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.OsmSharp.Encoding;
using OpenLR.OsmSharp.Locations;
using OpenLR.OsmSharp.Router;
using OpenLR.Referenced;
using OsmSharp;
using OsmSharp.Collections.PriorityQueues;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Units.Angle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenLR.OsmSharp
{
    /// <summary>
    /// A referenced encoder implementation.
    /// </summary>
    public abstract class ReferencedEncoderBase<TEdge> : OpenLR.Referenced.Encoding.ReferencedEncoder
        where TEdge : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Holds the basic router datasource.
        /// </summary>
        private readonly BasicRouterDataSource<TEdge> _graph;

        /// <summary>
        /// Creates a new referenced encoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="locationEncoder"></param>
        public ReferencedEncoderBase(BasicRouterDataSource<TEdge> graph, Encoder locationEncoder)
            : base(locationEncoder)
        {
            _graph = graph;
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
        /// Returns the reference graph.
        /// </summary>
        public BasicRouterDataSource<TEdge> Graph
        {
            get
            {
                return _graph;
            }
        }

        /// <summary>
        /// Gets the referenced point along line encoder.
        /// </summary>
        protected virtual ReferencedPointAlongLineEncoder<TEdge> GetReferencedPointAlongLineEncoder()
        {
            return new ReferencedPointAlongLineEncoder<TEdge>(this, this.LocationEncoder.CreatePointAlongLineLocationEncoder());
        }

        /// <summary>
        /// Encodes a referenced point along line location into an unreferenced location.
        /// </summary>
        /// <param name="pointAlongLineLocation"></param>
        /// <returns></returns>
        public virtual PointAlongLineLocation EncodeReferenced(ReferencedPointAlongLine<TEdge> pointAlongLineLocation)
        {
            return this.GetReferencedPointAlongLineEncoder().EncodeReferenced(pointAlongLineLocation);
        }

        /// <summary>
        /// Encodes a point along line location.
        /// </summary>
        /// <param name="pointAlongLineLocation"></param>
        /// <returns></returns>
        public virtual string Encode(ReferencedPointAlongLine<TEdge> pointAlongLineLocation)
        {
            return this.GetReferencedPointAlongLineEncoder().Encode(pointAlongLineLocation);
        }

        /// <summary>
        /// Encodes the given location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override string Encode(ReferencedLocation location)
        {
            if (location == null) { throw new ArgumentNullException("location"); }

            if (location is ReferencedPointAlongLine<TEdge>)
            {
                return this.Encode(location as ReferencedPointAlongLine<TEdge>);
            }

            throw new ArgumentOutOfRangeException("location",
                string.Format("Location cannot be encoded by any of the encoders: {0}", location.ToString()));
        }

        /// <summary>
        /// Returns the encoder vehicle profile.
        /// </summary>
        public abstract Vehicle Vehicle
        {
            get;
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
        /// Returns the tags associated with the given tags id.
        /// </summary>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        public abstract TagsCollectionBase GetTags(uint tagsId);

        /// <summary>
        /// Tries to match the given tags and figure out a corresponding frc and fow.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="frc"></param>
        /// <param name="fow"></param>
        /// <returns>False if no matching was found.</returns>
        public abstract bool TryMatching(TagsCollectionBase tags, out FunctionalRoadClass frc, out FormOfWay fow);

        /// <summary>
        /// Returns a value if a oneway restriction is found.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns>null: no restrictions, true: forward restriction, false: backward restriction.</returns>
        /// <returns></returns>
        public abstract bool? IsOneway(TagsCollectionBase tags);

        /// <summary>
        /// Returns the bearing calculate between two given vertices along the given edge.
        /// </summary>
        /// <param name="vertexFrom"></param>
        /// <param name="edge"></param>
        /// <param name="vertexTo"></param>
        /// <param name="forward">When true the edge is forward relative to the vertices, false the edge is backward.</param>
        /// <returns></returns>
        public virtual Degree GetBearing(long vertexFrom, TEdge edge, long vertexTo, bool forward)
        {
            var coordinates = new List<GeoCoordinate>();
            float latitude, longitude;
            this.Graph.GetVertex(vertexFrom, out latitude, out longitude);
            coordinates.Add(new GeoCoordinate(latitude, longitude));

            if (edge.Coordinates != null)
            { // there are intermediates, add them in the correct order.
                if (forward)
                {
                    coordinates.AddRange(edge.Coordinates.Select<GeoCoordinateSimple, GeoCoordinate>(x => { return new GeoCoordinate(x.Latitude, x.Longitude); }));
                }
                else
                {
                    coordinates.AddRange(edge.Coordinates.Reverse().Select<GeoCoordinateSimple, GeoCoordinate>(x => { return new GeoCoordinate(x.Latitude, x.Longitude); }));
                }
            }

            this.Graph.GetVertex(vertexTo, out latitude, out longitude);
            coordinates.Add(new GeoCoordinate(latitude, longitude));

            return BearingEncoder.EncodeBearing(coordinates);
        }

        /// <summary>
        /// Returns true if the given vertex is a valid candidate to use as a location reference point.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool IsVertexValid(long vertex)
        {
            var arcs = this.Graph.GetArcs(vertex);

            // filter out non-traversable arcs.
            var traversableArcs = arcs.Where((arc) =>
            {
                return this.Vehicle.CanTraverse(this.Graph.TagsIndex.Get(arc.Value.Tags));
            }).ToList();

            if (traversableArcs.Count > 1)
            { // if there is one incoming edge where there is only one way out then this vertex is invalid.
                foreach (var incoming in traversableArcs)
                {
                    // check if this neighbour is incoming.
                    var incomingTags = this.Graph.TagsIndex.Get(incoming.Value.Tags);
                    var incomingOneway = this.Vehicle.IsOneWay(incomingTags);
                    if (incomingOneway == null ||
                        incomingOneway.Value != incoming.Value.Forward)
                    { // ok, is not oneway or oneway is in incoming direction.
                        var outgoingCount = 0;
                        foreach (var outgoing in traversableArcs)
                        {
                            if (outgoing.Key != incoming.Key ||
                                !outgoing.Value.Equals(incoming.Value))
                            { // don't take the same edge.
                                var outgoingTags = this.Graph.TagsIndex.Get(outgoing.Value.Tags);
                                var oneway = this.Vehicle.IsOneWay(outgoingTags);
                                if (oneway == null ||
                                    oneway.Value == outgoing.Value.Forward)
                                { // ok, is not oneway or oneway is outgoing direction.
                                    outgoingCount++;
                                    if (outgoingCount > 1)
                                    { // it's not going down again, stop the search.
                                        break;
                                    }
                                }
                            }
                        }
                        if (outgoingCount == 1)
                        { // there is only one option, so for some situations to vertex is invalid, mark it invalid for all.
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            { // one arc, vertex is at the end.
                return true;
            }
        }

        /// <summary>
        /// Finds a valid vertex for the given vertex but does not search in the direction of the target neighbour.
        /// </summary>
        /// <param name="vertex">The invalid vertex.</param>
        /// <param name="targetVertex">The target vertex.</param>
        /// <param name="edge">The edge that leads to the target vertex.</param>
        /// <param name="excludeSet">The set of vertices that should be excluded in the search.</param>
        /// <param name="searchForward">When true, the search is forward, otherwise backward.</param>
        public abstract PathSegment FindValidVertexFor(long vertex, LiveEdge edge, long targetVertex, HashSet<long> excludeSet, bool searchForward);

        /// <summary>
        /// Finds the shortest path between the two given vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="searchForward"></param>
        public abstract PathSegment FindShortestPath(long from, long to, bool searchForward);
    }

    /// <summary>
    /// Contains encoder extensions.
    /// </summary>
    public static class ReferencedEncoderBaseExtensions
    {
        /// <summary>
        /// Builds a point along line location.
        /// </summary>
        /// <param name="encoder">The encoder.</param>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        public static ReferencedPointAlongLine<LiveEdge> BuildPointAlongLine(this ReferencedEncoderBase<LiveEdge> encoder, GeoCoordinate location)
        {
            // get the closest edge that can be traversed to the given location.
            var closestNullable = encoder.Graph.GetClosestEdge<LiveEdge>(location);
            if(!closestNullable.HasValue)
            { // no location could be found. 
                throw new Exception("No network features found near the given location. Make sure the network covers the given location.");
            }
            var closest = closestNullable.Value;

            // check oneway.
            var oneway = encoder.Vehicle.IsOneWay(encoder.Graph.TagsIndex.Get(closest.Value.Value.Tags));
            var useForward = (oneway == null) || (oneway.Value == closest.Value.Value.Forward);

            // build location and make sure the vehicle can travel from from location to to location.
            LiveEdge edge;
            long from, to;
            if (useForward)
            { // encode first point to last point.
                edge = closest.Value.Value;
                //if (!closest.Value.Value.Forward)
                //{ // use reverse edge.
                //    var reverseEdge = new LiveEdge();
                //    reverseEdge.Tags = closest.Value.Value.Tags;
                //    reverseEdge.Forward = !closest.Value.Value.Forward;
                //    reverseEdge.Distance = closest.Value.Value.Distance;
                //    reverseEdge.Coordinates = null;
                //    if (closest.Value.Value.Coordinates != null)
                //    {
                //        var reverse = new GeoCoordinateSimple[closest.Value.Value.Coordinates.Length];
                //        closest.Value.Value.Coordinates.CopyToReverse(reverse, 0);
                //        reverseEdge.Coordinates = reverse;
                //    }
                //    edge = reverseEdge;
                //}
                from = closest.Key;
                to = closest.Value.Key;
            }
            else
            { // encode last point to first point.
                edge = closest.Value.Value.ToReverse();

                // Next OsmSharp version: use closest.Value.Value.Reverse()?
                var reverseEdge = new LiveEdge();
                reverseEdge.Tags = closest.Value.Value.Tags;
                reverseEdge.Forward = !closest.Value.Value.Forward;
                reverseEdge.Distance = closest.Value.Value.Distance;
                reverseEdge.Coordinates = closest.Value.Value.Coordinates;

                edge = reverseEdge;
                from = closest.Value.Key;
                to = closest.Key;
            }

            // OK, now we have a naive location, we need to check if it's valid.
            // see: §C section 6 @ http://www.tomtom.com/lib/OpenLR/OpenLR-whitepaper.pdf
            var referencedPointAlongLine = new OpenLR.OsmSharp.Locations.ReferencedPointAlongLine<LiveEdge>()
                {
                    Route = new ReferencedLine<LiveEdge>(encoder.Graph)
                    {
                        Edges = new LiveEdge[] { edge },
                        Vertices = new long[] { from, to }
                    },
                    Latitude = location.Latitude,
                    Longitude = location.Longitude
                };

            // RULE1: distance should not exceed 15km.
            var length = referencedPointAlongLine.Length(encoder);
            if (length.Value >= 15000)
            { // no implented this.
                throw new NotImplementedException("Distance between the two closest valid points is too big, should insert intermediate point.");
            }

            // RULE2: no need to check, will be rounded.

            // RULE3: ok, there are two points.

            // RULE4: choosen points should be valid network points.
            var excludeSet = new HashSet<long>();
            if (!encoder.IsVertexValid(referencedPointAlongLine.Route.Vertices[0]))
            { // from is not valid, try to find a valid point.
                var pathToValid = encoder.FindValidVertexFor(referencedPointAlongLine.Route.Vertices[0], referencedPointAlongLine.Route.Edges[0], referencedPointAlongLine.Route.Vertices[1],
                    excludeSet, false);

                // build edges list.
                if (pathToValid != null)
                { // no path found, just leave things as is.
                    var shortestRoute = encoder.FindShortestPath(referencedPointAlongLine.Route.Vertices[1], pathToValid.Vertex, false);
                    while (shortestRoute != null && !shortestRoute.Contains(referencedPointAlongLine.Route.Vertices[0]))
                    { // the vertex that should be on this shortest route, isn't anymore.
                        // exclude the current target vertex, 
                        excludeSet.Add(pathToValid.Vertex);
                        // calulate a new path-to-valid.
                        pathToValid = encoder.FindValidVertexFor(referencedPointAlongLine.Route.Vertices[0], referencedPointAlongLine.Route.Edges[0], referencedPointAlongLine.Route.Vertices[1],
                            excludeSet, false);
                        if (pathToValid == null)
                        { // a new path was not found.
                            break;
                        }
                        shortestRoute = encoder.FindShortestPath(referencedPointAlongLine.Route.Vertices[1], pathToValid.Vertex, false);
                    }
                    if (pathToValid != null)
                    { // no path found, just leave things as is.
                        var vertices = pathToValid.ToArray().Reverse().ToList();
                        var edges = new List<LiveEdge>();
                        for (int idx = 0; idx < vertices.Count - 1; idx++)
                        { // loop over edges.
                            edge = vertices[idx].Edge;
                            // Next OsmSharp version: use closest.Value.Value.Reverse()?
                            var reverseEdge = new LiveEdge();
                            reverseEdge.Tags = edge.Tags;
                            reverseEdge.Forward = !edge.Forward;
                            reverseEdge.Distance = edge.Distance;
                            reverseEdge.Coordinates = edge.Coordinates;

                            edge = reverseEdge;
                            edges.Add(edge);
                        }

                        // create new location.
                        var edgesArray = new LiveEdge[edges.Count + referencedPointAlongLine.Route.Edges.Length];
                        edges.CopyTo(0, edgesArray, 0, edges.Count);
                        referencedPointAlongLine.Route.Edges.CopyTo(0, edgesArray, edges.Count, referencedPointAlongLine.Route.Edges.Length);
                        var vertexArray = new long[vertices.Count - 1 + referencedPointAlongLine.Route.Vertices.Length];
                        vertices.ConvertAll(x => (long)x.Vertex).CopyTo(0, vertexArray, 0, vertices.Count - 1);
                        referencedPointAlongLine.Route.Vertices.CopyTo(0, vertexArray, vertices.Count - 1, referencedPointAlongLine.Route.Vertices.Length);

                        referencedPointAlongLine.Route.Edges = edgesArray;
                        referencedPointAlongLine.Route.Vertices = vertexArray;
                    }
                }
            }
            excludeSet.Clear();
            if (!encoder.IsVertexValid(referencedPointAlongLine.Route.Vertices[referencedPointAlongLine.Route.Vertices.Length - 1]))
            { // from is not valid, try to find a valid point.
                var vertexCount = referencedPointAlongLine.Route.Vertices.Length;
                var pathToValid = encoder.FindValidVertexFor(referencedPointAlongLine.Route.Vertices[vertexCount - 1], referencedPointAlongLine.Route.Edges[
                    referencedPointAlongLine.Route.Edges.Length - 1].ToReverse(), referencedPointAlongLine.Route.Vertices[vertexCount - 2], excludeSet, true);

                // build edges list.
                if (pathToValid != null)
                { // no path found, just leave things as is.
                    var shortestRoute = encoder.FindShortestPath(referencedPointAlongLine.Route.Vertices[vertexCount - 2], pathToValid.Vertex, true);
                    while (shortestRoute != null && !shortestRoute.Contains(referencedPointAlongLine.Route.Vertices[vertexCount - 1]))
                    { // the vertex that should be on this shortest route, isn't anymore.
                        // exclude the current target vertex, 
                        excludeSet.Add(pathToValid.Vertex);
                        // calulate a new path-to-valid.
                        pathToValid = encoder.FindValidVertexFor(referencedPointAlongLine.Route.Vertices[vertexCount - 1], referencedPointAlongLine.Route.Edges[
                            referencedPointAlongLine.Route.Edges.Length - 1].ToReverse(), referencedPointAlongLine.Route.Vertices[vertexCount - 2], excludeSet, true);
                        if (pathToValid == null)
                        { // a new path was not found.
                            break;
                        }
                        shortestRoute = encoder.FindShortestPath(referencedPointAlongLine.Route.Vertices[vertexCount - 2], pathToValid.Vertex, true);
                    }
                    if (pathToValid != null)
                    { // no path found, just leave things as is.
                        var vertices = pathToValid.ToArray().ToList();
                        var edges = new List<LiveEdge>();
                        for (int idx = 1; idx < vertices.Count; idx++)
                        { // loop over edges.
                            edge = vertices[idx].Edge;
                            //if (!edge.Forward)
                            //{ // use reverse edge.
                            //    edge = edge.ToReverse();
                            //}
                            edges.Add(edge);
                        }

                        // create new location.
                        var edgesArray = new LiveEdge[edges.Count + referencedPointAlongLine.Route.Edges.Length];
                        referencedPointAlongLine.Route.Edges.CopyTo(0, edgesArray, 0, referencedPointAlongLine.Route.Edges.Length);
                        edges.CopyTo(0, edgesArray, referencedPointAlongLine.Route.Edges.Length, edges.Count);
                        var vertexArray = new long[vertices.Count - 1 + referencedPointAlongLine.Route.Vertices.Length];
                        referencedPointAlongLine.Route.Vertices.CopyTo(0, vertexArray, 0, referencedPointAlongLine.Route.Vertices.Length);
                        vertices.ConvertAll(x => (long)x.Vertex).CopyTo(1, vertexArray, referencedPointAlongLine.Route.Vertices.Length, vertices.Count - 1);

                        referencedPointAlongLine.Route.Edges = edgesArray;
                        referencedPointAlongLine.Route.Vertices = vertexArray;
                    }
                }
            }

            // RULE1: check again, distance should not exceed 15km.
            length = referencedPointAlongLine.Length(encoder);
            if (length.Value >= 15000)
            { // not implented this just yet.
                throw new NotImplementedException("Distance between the two closest valid points is too big, should insert intermediate point.");
            }

            return referencedPointAlongLine;
        }

        /// <summary>
        /// Builds a line location given a sequence of vertex->edge->vertex...edge->vertex.
        /// </summary>
        /// <param name="encoder">The encoder.</param>
        /// <param name="vertices">The vertices along the path to create the location for. Contains at least two vertices (#vertices = #edges + 1).</param>
        /// <param name="edges">The edge along the path to create the location for. Contains at least one edge (#vertices = #edges + 1). Adges need to be taversible by the vehicle profile used by the encoder in the direction of the path</param>
        /// <param name="positivePercentageOffset">The offset in percentage relative to the distance of the total path and it's start. [0-100[</param>
        /// <param name="negativePercentageOffset">The offset in percentage relative to the distance of the total path and it's end. [0-100[</param>
        /// <returns></returns>
        public static ReferencedLine<LiveEdge> BuildLineLocation(this ReferencedEncoderBase<LiveEdge> encoder, long[] vertices, LiveEdge[] edges, float positivePercentageOffset, float negativePercentageOffset)
        {
            // validate parameters.
            if (encoder == null) { throw new ArgumentNullException("encoder"); }
            if (vertices == null) { throw new ArgumentNullException("vertices"); }
            if (edges == null) { throw new ArgumentNullException("edges"); }
            if (vertices.Length < 2) { throw new ArgumentOutOfRangeException("vertices", "A referenced line location can only be created with a valid path consisting of at least two vertices and one edge."); }
            if (edges.Length < 1) { throw new ArgumentOutOfRangeException("edges", "A referenced line location can only be created with a valid path consisting of at least two vertices and one edge."); }
            if (edges.Length + 1 != vertices.Length) { throw new ArgumentException("The #vertices need to equal #edges + 1 to have a valid path."); }

            if (positivePercentageOffset < 0 || positivePercentageOffset >= 100) { throw new ArgumentOutOfRangeException("positivePercentageOffset", "The positive percentage offset should be in the range [0-100[."); }
            if (negativePercentageOffset < 0 || negativePercentageOffset >= 100) { throw new ArgumentOutOfRangeException("negativePercentageOffset", "The negative percentage offset should be in the range [0-100[."); }
            if ((negativePercentageOffset + positivePercentageOffset) > 100) { throw new ArgumentException("The negative and positive percentage offsets together should be in the range [0-100[."); }

            // OK, now we have a naive location, we need to check if it's valid.
            // see: §F section 11.1 @ http://www.tomtom.com/lib/OpenLR/OpenLR-whitepaper.pdf
            var referencedLine = new OpenLR.OsmSharp.Locations.ReferencedLine<LiveEdge>(encoder.Graph)
            {
                Edges = edges.Clone() as LiveEdge[],
                Vertices = vertices.Clone() as long[]
            };
            var length = referencedLine.Length(encoder).Value;
            var positiveOffsetLength = length * (positivePercentageOffset / 100.0);
            var negativeOffsetLength = length * (negativePercentageOffset / 100.0);

            // 1: Is the path connected?
            // 2: Is the path traversable?
            for (int edgeIdx = 0; edgeIdx < edges.Length; edgeIdx++)
            {
                var from = vertices[edgeIdx];
                var to = vertices[edgeIdx + 1];

                bool found = false;
                foreach (var edge in encoder.Graph.GetArcs(from))
                {
                    if (edge.Key == to &&
                        edge.Value.Equals(edges[edgeIdx]))
                    { // edge was found, is valid.
                        found = true;
                        break;
                    }
                }
                if (!found)
                { // edge is not found, path not connected.
                    throw new ArgumentOutOfRangeException(string.Format("Edge {0} cannot be found between vertex {1} and {2}. The given path is not connected.",
                        edges[edgeIdx].ToInvariantString(), from, to));
                }
                // check whether the edge can traversed.
                var tags = encoder.Graph.TagsIndex.Get(edges[edgeIdx].Tags);
                if (!encoder.Vehicle.CanTraverse(tags))
                { // oeps, cannot be traversed.
                    throw new ArgumentOutOfRangeException(string.Format("Edge at index {0} cannot be traversed by vehicle {1}.", edgeIdx, encoder.Vehicle.UniqueName));
                }
                // check whether the edge can be traversed in the correct direction.
                var oneway = encoder.Vehicle.IsOneWay(tags);
                var canMoveForward = (oneway == null) || (oneway.Value == edges[edgeIdx].Forward);
                if (!canMoveForward)
                { // path cannot be traversed in this direction.
                    throw new ArgumentOutOfRangeException(string.Format("Edge at index {0} cannot be traversed by vehicle {1} in the direction given.", edgeIdx, encoder.Vehicle.UniqueName));
                }
            }

            // 3: The sum of the positive and negative offset cannot be greater than the total length of the location lines (see arguments check).
            // 4: Positive offset value shall be less than the length of the first line.
            //  o Otherwise the first line can be removed from the list of location lines and the offset value must be reduced in the same way.
            //  o This procedure shall be repeated until this constraint is fulfilled
            // 5: Negative offset value shall be less than the length of the last line
            //  o Otherwise the last line can be removed from the list of location lines and the offset value must be reduced in the same way
            //  o This procedure shall be repeated until this constraint is fulfilled
            // TODO: check 4 and 5, an implementation will work without this part but may be a bit inefficient.

            // RULE4: choosen points should be valid network points.
            var excludeSet = new HashSet<long>();
            if (!encoder.IsVertexValid(referencedLine.Vertices[0]))
            { // from is not valid, try to find a valid point.
                var pathToValid = encoder.FindValidVertexFor(referencedLine.Vertices[0], referencedLine.Edges[0], referencedLine.Vertices[1],
                    excludeSet, false);

                // build edges list.
                if (pathToValid != null)
                { // no path found, just leave things as is.
                    var shortestRoute = encoder.FindShortestPath(referencedLine.Vertices[1], pathToValid.Vertex, false);
                    while (shortestRoute != null && !shortestRoute.Contains(referencedLine.Vertices[0]))
                    { // the vertex that should be on this shortest route, isn't anymore.
                        // exclude the current target vertex, 
                        excludeSet.Add(pathToValid.Vertex);
                        // calulate a new path-to-valid.
                        pathToValid = encoder.FindValidVertexFor(referencedLine.Vertices[0], referencedLine.Edges[0], referencedLine.Vertices[1],
                            excludeSet, false);
                        if (pathToValid == null)
                        { // a new path was not found.
                            break;
                        }
                        shortestRoute = encoder.FindShortestPath(referencedLine.Vertices[1], pathToValid.Vertex, false);
                    }
                    if (pathToValid != null)
                    { // no path found, just leave things as is.
                        var newVertices = pathToValid.ToArray().Reverse().ToList();
                        var newEdges = new List<LiveEdge>();
                        for (int idx = 0; idx < newVertices.Count - 1; idx++)
                        { // loop over edges.
                            var edge = newVertices[idx].Edge;
                            // Next OsmSharp version: use closest.Value.Value.Reverse()?
                            var reverseEdge = new LiveEdge();
                            reverseEdge.Tags = edge.Tags;
                            reverseEdge.Forward = !edge.Forward;
                            reverseEdge.Distance = edge.Distance;
                            reverseEdge.Coordinates = edge.Coordinates;

                            edge = reverseEdge;
                            newEdges.Add(edge);
                        }

                        // create new location.
                        var edgesArray = new LiveEdge[newEdges.Count + referencedLine.Edges.Length];
                        newEdges.CopyTo(0, edgesArray, 0, newEdges.Count);
                        referencedLine.Edges.CopyTo(0, edgesArray, newEdges.Count, referencedLine.Edges.Length);
                        var vertexArray = new long[newVertices.Count - 1 + referencedLine.Vertices.Length];
                        newVertices.ConvertAll(x => (long)x.Vertex).CopyTo(0, vertexArray, 0, newVertices.Count - 1);
                        referencedLine.Vertices.CopyTo(0, vertexArray, newVertices.Count - 1, referencedLine.Vertices.Length);

                        referencedLine.Edges = edgesArray;
                        referencedLine.Vertices = vertexArray;

                        // adjust offset length.
                        var newLength = referencedLine.Length(encoder).Value;
                        positiveOffsetLength = positiveOffsetLength + (newLength - length);
                        length = newLength;
                    }
                }
            }
            excludeSet.Clear();
            if (!encoder.IsVertexValid(referencedLine.Vertices[referencedLine.Vertices.Length - 1]))
            { // from is not valid, try to find a valid point.
                var vertexCount = referencedLine.Vertices.Length;
                var pathToValid = encoder.FindValidVertexFor(referencedLine.Vertices[vertexCount - 1], referencedLine.Edges[
                    referencedLine.Edges.Length - 1].ToReverse(), referencedLine.Vertices[vertexCount - 2], excludeSet, true);

                // build edges list.
                if (pathToValid != null)
                { // no path found, just leave things as is.
                    var shortestRoute = encoder.FindShortestPath(referencedLine.Vertices[vertexCount - 2], pathToValid.Vertex, true);
                    while (shortestRoute != null && !shortestRoute.Contains(referencedLine.Vertices[vertexCount - 1]))
                    { // the vertex that should be on this shortest route, isn't anymore.
                        // exclude the current target vertex, 
                        excludeSet.Add(pathToValid.Vertex);
                        // calulate a new path-to-valid.
                        pathToValid = encoder.FindValidVertexFor(referencedLine.Vertices[vertexCount - 1], referencedLine.Edges[
                            referencedLine.Edges.Length - 1].ToReverse(), referencedLine.Vertices[vertexCount - 2], excludeSet, true);
                        if (pathToValid == null)
                        { // a new path was not found.
                            break;
                        }
                        shortestRoute = encoder.FindShortestPath(referencedLine.Vertices[vertexCount - 2], pathToValid.Vertex, true);
                    }
                    if (pathToValid != null)
                    { // no path found, just leave things as is.
                        var newVertices = pathToValid.ToArray().ToList();
                        var newEdges = new List<LiveEdge>();
                        for (int idx = 1; idx < newVertices.Count; idx++)
                        { // loop over edges.
                            var edge = newVertices[idx].Edge;
                            //if (!edge.Forward)
                            //{ // use reverse edge.
                            //    edge = edge.ToReverse();
                            //}
                            newEdges.Add(edge);
                        }

                        // create new location.
                        var edgesArray = new LiveEdge[newEdges.Count + referencedLine.Edges.Length];
                        referencedLine.Edges.CopyTo(0, edgesArray, 0, referencedLine.Edges.Length);
                        newEdges.CopyTo(0, edgesArray, referencedLine.Edges.Length, newEdges.Count);
                        var vertexArray = new long[newVertices.Count - 1 + referencedLine.Vertices.Length];
                        referencedLine.Vertices.CopyTo(0, vertexArray, 0, referencedLine.Vertices.Length);
                        newVertices.ConvertAll(x => (long)x.Vertex).CopyTo(1, vertexArray, referencedLine.Vertices.Length, newVertices.Count - 1);

                        referencedLine.Edges = edgesArray;
                        referencedLine.Vertices = vertexArray;

                        // adjust offset length.
                        var newLength = referencedLine.Length(encoder).Value;
                        negativeOffsetLength = negativeOffsetLength + (newLength - length);
                        length = newLength;
                    }
                }
            }

            // update offset percentags.
            referencedLine.PositiveOffsetPercentage = (float)((positiveOffsetLength / length) * 100.0);
            referencedLine.NegativeOffsetPercentage = (float)((negativeOffsetLength / length) * 100.0);

            return referencedLine;
        }
    }
}