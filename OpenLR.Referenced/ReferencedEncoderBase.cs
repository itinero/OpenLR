using OpenLR.Encoding;
using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.Referenced.Encoding;
using OpenLR.Referenced.Exceptions;
using OpenLR.Referenced.Locations;
using OpenLR.Referenced.Router;
using OpenLR.Referenced;
using OsmSharp;
using OsmSharp.Collections.PriorityQueues;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Units.Angle;
using OsmSharp.Units.Distance;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenLR.Referenced
{
    /// <summary>
    /// A referenced encoder implementation.
    /// </summary>
    public abstract class ReferencedEncoderBase : OpenLR.Referenced.Encoding.ReferencedEncoder
    {
        /// <summary>
        /// Holds the basic router datasource.
        /// </summary>
        private readonly BasicRouterDataSource<LiveEdge> _graph;

        /// <summary>
        /// Creates a new referenced encoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="locationEncoder"></param>
        public ReferencedEncoderBase(BasicRouterDataSource<LiveEdge> graph, Encoder locationEncoder)
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
        public BasicRouterDataSource<LiveEdge> Graph
        {
            get
            {
                return _graph;
            }
        }

        /// <summary>
        /// Gets the referenced point along line encoder.
        /// </summary>
        protected virtual ReferencedPointAlongLineEncoder GetReferencedPointAlongLineEncoder()
        {
            return new ReferencedPointAlongLineEncoder(this, this.LocationEncoder.CreatePointAlongLineLocationEncoder());
        }

        /// <summary>
        /// Encodes a referenced point along line location into an unreferenced location.
        /// </summary>
        /// <param name="pointAlongLineLocation"></param>
        /// <returns></returns>
        public virtual PointAlongLineLocation EncodeReferenced(ReferencedPointAlongLine pointAlongLineLocation)
        {
            return this.GetReferencedPointAlongLineEncoder().EncodeReferenced(pointAlongLineLocation);
        }

        /// <summary>
        /// Encodes a point along line location.
        /// </summary>
        /// <param name="pointAlongLineLocation"></param>
        /// <returns></returns>
        public virtual string Encode(ReferencedPointAlongLine pointAlongLineLocation)
        {
            return this.GetReferencedPointAlongLineEncoder().Encode(pointAlongLineLocation);
        }

        /// <summary>
        /// Gets the referenced line encoder.
        /// </summary>
        protected virtual ReferencedLineEncoder GetReferencedLineEncoder()
        {
            return new ReferencedLineEncoder(this, this.LocationEncoder.CreateLineLocationEncoder());
        }

        /// <summary>
        /// Encodes a referenced line location into an unreferenced location.
        /// </summary>
        /// <param name="lineLocation"></param>
        /// <returns></returns>
        public virtual LineLocation EncodeReferenced(ReferencedLine lineLocation)
        {
            return this.GetReferencedLineEncoder().EncodeReferenced(lineLocation);
        }

        /// <summary>
        /// Encodes a line location.
        /// </summary>
        /// <param name="lineLocation"></param>
        /// <returns></returns>
        public virtual string Encode(ReferencedLine lineLocation)
        {
            return this.GetReferencedLineEncoder().Encode(lineLocation);
        }

        /// <summary>
        /// Encodes the given location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override string Encode(ReferencedLocation location)
        {
            if (location == null) { throw new ArgumentNullException("location"); }

            if (location is ReferencedLine)
            {
                return this.Encode(location as ReferencedLine);
            }
            else if (location is ReferencedPointAlongLine)
            {
                return this.Encode(location as ReferencedPointAlongLine);
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
                    coordinates.AddRange(edgeShape.Select<GeoCoordinateSimple, GeoCoordinate>(x => { 
                        return new GeoCoordinate(x.Latitude, x.Longitude); 
                    }));
                }
                else
                {
                    coordinates.AddRange(edgeShape.Reverse().Select<GeoCoordinateSimple, GeoCoordinate>(x => { 
                        return new GeoCoordinate(x.Latitude, x.Longitude); 
                    }));
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
        public virtual bool IsVertexValid(long vertex)
        {
            var arcs = this.Graph.GetEdges(vertex);

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
        /// <param name="edge">The edge that leads to the target vertex.</param>
        /// <param name="targetVertex">The target vertex.</param>
        /// <param name="excludeSet">The set of vertices that should be excluded in the search.</param>
        /// <param name="searchForward">When true, the search is forward, otherwise backward.</param>
        public virtual PathSegment FindValidVertexFor(long vertex, LiveEdge edge, long targetVertex, HashSet<long> excludeSet, bool searchForward)
        {
            // GIST: execute a dykstra search to find a vertex that is valid.
            // this will return a vertex that is on the shortest path:
            // foundVertex -> vertex -> targetNeighbour.

            // initialize settled set.
            var settled = new HashSet<long>();
            settled.Add(targetVertex);

            // initialize heap.
            var heap = new BinaryHeap<PathSegment>(10);
            heap.Push(new PathSegment(vertex), 0);

            // find the path to the closest valid vertex.
            PathSegment pathTo = null;
            while (heap.Count > 0)
            {
                // get next.
                var current = heap.Pop();
                settled.Add(current.Vertex);

                // check if valid.
                if (current.Vertex != vertex &&
                    this.IsVertexValid(current.Vertex))
                { // ok! vertex is valid.
                    pathTo = current;
                }
                else
                { // continue search.
                    // add unsettled neighbours.
                    var arcs = this.Graph.GetEdges(current.Vertex);
                    foreach (var arc in arcs)
                    {
                        if (!excludeSet.Contains(arc.Key) &&
                            !settled.Contains(arc.Key) &&
                           !(current.Vertex == vertex && arc.Key == targetVertex && edge.Distance == arc.Value.Distance))
                        { // ok, new neighbour, and ok, not the edge and neighbour to ignore.
                            var tags = this.Graph.TagsIndex.Get(arc.Value.Tags);
                            if (this.Vehicle.CanTraverse(tags))
                            { // ok, we can traverse this edge.
                                var onway = this.Vehicle.IsOneWay(tags);
                                if (onway == null ||
                                  !(onway.Value == arc.Value.Forward ^ searchForward))
                                { // ok, no oneway or oneway reversed.
                                    var weight = this.Vehicle.Weight(this.Graph.TagsIndex.Get(arc.Value.Tags), arc.Value.Distance);
                                    var path = new PathSegment(arc.Key, current.Weight + weight, arc.Value, current);
                                    heap.Push(path, (float)path.Weight);
                                }
                            }
                        }
                    }
                }
            }

            // ok, is there a path found.
            if (pathTo == null)
            { // oeps, probably something wrong with network-topology.
                // just take the default option.
                //throw new Exception(
                //    string.Format("Could not find a valid vertex for invalid vertex [{0}].", vertex));
                return null;
            }

            // add the path to the given location.
            return pathTo;
        }

        /// <summary>
        /// Finds the shortest path between the given from->to.
        /// </summary>
        /// <param name="from">The source vertex.</param>
        /// <param name="to">The target vertex.</param>
        /// <param name="searchForward">Flag for search direction.</param>
        /// <returns></returns>
        public virtual PathSegment FindShortestPath(long from, long to, bool searchForward)
        {
            var router = this.GetRouter();
            var result = router.Calculate(this.Graph, this.Vehicle,
                from, to, searchForward);
            if (result == null)
            {
                result = router.Calculate(this.Graph, this.Vehicle,
                    from, to, searchForward, BasicRouter.MAX_SETTLES * 8);
            }
            return result;
        }

        /// <summary>
        /// Finds the shortest path between the given from->to.
        /// </summary>
        /// <param name="fromPaths">The paths to the source.</param>
        /// <param name="toPaths">The paths to the target.</param>
        /// <param name="searchForward">Flag for search direction.</param>
        /// <returns></returns>
        public virtual PathSegment FindShortestPath(List<PathSegment> fromPaths, List<PathSegment> toPaths, bool searchForward)
        {
            var router = this.GetRouter();
            var result = router.Calculate(this.Graph, this.Vehicle,
                fromPaths, toPaths, searchForward);
            if (result == null)
            {
                result = router.Calculate(this.Graph, this.Vehicle,
                    fromPaths, toPaths, searchForward, BasicRouter.MAX_SETTLES * 8);
            }
            return result;
        }
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
        public static ReferencedPointAlongLine BuildPointAlongLine(this ReferencedEncoderBase encoder, GeoCoordinate location)
        {
            if (location == null) { throw new ArgumentNullException("location"); }

            // get the closest edge that can be traversed to the given location.
            var closest = encoder.Graph.GetClosestEdge(location);
            if (closest == null)
            { // no location could be found. 
                throw new Exception("No network features found near the given location. Make sure the network covers the given location.");
            }

            // check oneway.
            var oneway = encoder.Vehicle.IsOneWay(encoder.Graph.TagsIndex.Get(closest.Item3.Tags));
            var useForward = (oneway == null) || (oneway.Value == closest.Item3.Forward);

            // build location and make sure the vehicle can travel from from location to to location.
            LiveEdge edge;
            long from, to;
            if (useForward)
            { // encode first point to last point.
                edge = closest.Item3;
                from = closest.Item1;
                to = closest.Item2;
            }
            else
            { // encode last point to first point.
                var reverseEdge = new LiveEdge();
                reverseEdge.Tags = closest.Item3.Tags;
                reverseEdge.Forward = !closest.Item3.Forward;
                reverseEdge.Distance = closest.Item3.Distance;

                edge = reverseEdge;
                from = closest.Item2;
                to = closest.Item1;
            }

            // OK, now we have a naive location, we need to check if it's valid.
            // see: §C section 6 @ http://www.tomtom.com/lib/OpenLR/OpenLR-whitepaper.pdf
            var referencedPointAlongLine = new OpenLR.Referenced.Locations.ReferencedPointAlongLine()
                {
                    Route = new ReferencedLine(encoder.Graph)
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
        /// Builds a line location along the shortest path between start and end location.
        /// </summary>
        /// <param name="encoder">The encoder.</param>
        /// <param name="startLocation">The start location.</param>
        /// <param name="endLocation">The end location.</param>
        /// <returns></returns>
        /// <remarks>This should only be used when sure the start and endlocation or very close to the network use for encoding.</remarks>
        public static ReferencedLine BuildLineLocation(this ReferencedEncoderBase encoder, GeoCoordinate startLocation, GeoCoordinate endLocation)
        {
            return encoder.BuildLineLocation(startLocation, endLocation, 0.1);
        }

        /// <summary>
        /// Builds a line location along the shortest path between start and end location.
        /// </summary>
        /// <param name="encoder">The encoder.</param>
        /// <param name="startLocation">The start location.</param>
        /// <param name="endLocation">The end location.</param>
        /// <param name="tolerance">The tolerance value, the minimum distance between a given start or endlocation and the network used for encoding.</param>
        /// <returns></returns>
        /// <remarks>This should only be used when sure the start and endlocation or very close to the network use for encoding.</remarks>
        public static ReferencedLine BuildLineLocation(this ReferencedEncoderBase encoder, GeoCoordinate startLocation, GeoCoordinate endLocation, Meter tolerance)
        {
            PointF2D bestProjected;
            LinePointPosition bestPosition;
            Meter bestStartOffset;
            Meter bestEndOffset;
            double epsilon = 0.1;

            if (startLocation == null) { throw new ArgumentNullException("startLocation"); }
            if (endLocation == null) { throw new ArgumentNullException("endLocation"); }

            // search start and end location hooks.
            var startEdge = encoder.Graph.GetClosestEdge(startLocation, tolerance);
            if(startEdge == null)
            { // no closest edge found within tolerance, encoding has failed!
                throw new BuildLocationFailedException("Location {0} is too far from the network used for encoding with used tolerance {1}",
                    startLocation, tolerance);
            }
            // project the startlocation on the edge.
            var coordinates = encoder.Graph.GetCoordinates(startEdge);
            var startEdgeLength = coordinates.Length();
            if (!coordinates.ProjectOn(startLocation, out bestProjected, out bestPosition, out bestStartOffset))
            { // projection failed,.
                throw new BuildLocationFailedException("Projection of location {0} on the closest edge failed.",
                    startLocation);
            }
            // construct from pathsegments.
            var startPaths = new List<PathSegment>();
            if(bestStartOffset.Value < epsilon)
            { // use the first vertex as start location.
                startPaths.Add(new PathSegment(startEdge.Item1));
            }
            else if ((startEdgeLength.Value - bestStartOffset.Value) < epsilon)
            { // use the last vertex as end start location.
                startPaths.Add(new PathSegment(startEdge.Item2));
            }
            else
            { // point is somewhere in between.
                var tags = encoder.Graph.TagsIndex.Get(startEdge.Item3.Tags);
                var oneway = encoder.Vehicle.IsOneWay(tags);

                // weightBefore: vertex1->{x}
                var weightBefore = encoder.Vehicle.Weight(tags, (float)bestStartOffset.Value);
                // weightAfter: {x}->vertex2.
                var weightAfter = encoder.Vehicle.Weight(encoder.Graph.TagsIndex.Get(startEdge.Item3.Tags), (float)(startEdgeLength.Value - bestStartOffset.Value));

                if (startEdge.Item3.Forward)
                { // edge is forward.
                    // vertex1->{x}->vertex2

                    // consider the part {x}->vertex1 x being the source.
                    if (oneway == null || !oneway.Value)
                    {  // edge cannot be oneway forward.
                        startPaths.Add(new PathSegment(startEdge.Item1, weightBefore, startEdge.Item3.ToReverse(),
                            new PathSegment(-1)));
                    }

                    // consider the part {x}->vertex2 x being the source.
                    if (oneway == null || oneway.Value)
                    { // edge cannot be oneway backward.
                        startPaths.Add(new PathSegment(startEdge.Item2, weightAfter, startEdge.Item3,
                            new PathSegment(-1)));
                    }
                }
                else
                { // edge is backward.
                    // vertex1->{x}->vertex2

                    // consider the part {x}->vertex1 x being the source.
                    if (oneway == null || oneway.Value)
                    {  // edge cannot be oneway forward but edge is backward.
                        startPaths.Add(new PathSegment(startEdge.Item1, weightBefore, startEdge.Item3.ToReverse(),
                            new PathSegment(-1)));
                    }

                    // consider the part {x}->vertex2 x being the source.
                    if (oneway == null || !oneway.Value)
                    { // edge cannot be oneway backward but edge is backward.
                        startPaths.Add(new PathSegment(startEdge.Item2, weightAfter, startEdge.Item3,
                            new PathSegment(-1)));
                    }
                }
            }

            var endEdge = encoder.Graph.GetClosestEdge(endLocation, tolerance);
            if (endEdge == null)
            { // no closest edge found within tolerance, encoding has failed!
                throw new BuildLocationFailedException("Location {0} is too far from the network used for encoding with used tolerance {1}",
                    endLocation, tolerance);
            }
            // project the endlocation on the edge.
            coordinates = encoder.Graph.GetCoordinates(endEdge);
            var endEdgeLength = coordinates.Length();
            if (!coordinates.ProjectOn(endLocation, out bestProjected, out bestPosition, out bestEndOffset))
            { // projection failed.
                throw new BuildLocationFailedException("Projection of location {0} on the closest edge failed.",
                    endLocation);
            }
            // construct from pathsegments.
            var endPaths = new List<PathSegment>();
            if (bestEndOffset.Value < epsilon)
            { // use the first vertex as end location.
                endPaths.Add(new PathSegment(endEdge.Item1));
            }
            else if ((endEdgeLength.Value - bestEndOffset.Value) < epsilon)
            { // use the last vertex as end end location.
                endPaths.Add(new PathSegment(endEdge.Item2));
            }
            else
            { // point is somewhere in between.
                var tags = encoder.Graph.TagsIndex.Get(endEdge.Item3.Tags);
                var oneway = encoder.Vehicle.IsOneWay(tags);
                // weightBefore: vertex1->{x}
                var weightBefore = encoder.Vehicle.Weight(tags, (float)bestEndOffset.Value);
                // weightAfter: {x}->vertex2.
                var weightAfter = encoder.Vehicle.Weight(encoder.Graph.TagsIndex.Get(endEdge.Item3.Tags), (float)(endEdgeLength.Value - bestEndOffset.Value));

                if (endEdge.Item3.Forward)
                { // edge is forward.
                    // vertex1->{x}->vertex2

                    // consider vertex1->{x} x being the target.
                    if (oneway == null || oneway.Value)
                    {  // edge cannot be oneway backward.
                        endPaths.Add(new PathSegment(-1, weightBefore, endEdge.Item3,
                            new PathSegment(endEdge.Item1)));
                    }

                    // consider vertex2->{x} x being the target.
                    if(oneway ==null || !oneway.Value)
                    { // edge cannot be onway forward.
                        endPaths.Add(new PathSegment(-1, weightAfter, endEdge.Item3.ToReverse(),
                            new PathSegment(endEdge.Item2)));
                    }
                }
                else
                { // edge is backward.
                    // vertex1->{x}->vertex2

                    // consider vertex1->{x} x being the target.
                    if (oneway == null || !oneway.Value)
                    {  // edge cannot be oneway backward.
                        endPaths.Add(new PathSegment(-1, weightBefore, endEdge.Item3,
                            new PathSegment(endEdge.Item1)));
                    }

                    // consider vertex2->{x} x being the target.
                    if (oneway == null || oneway.Value)
                    { // edge cannot be onway forward.
                        endPaths.Add(new PathSegment(-1, weightAfter, endEdge.Item3.ToReverse(),
                            new PathSegment(endEdge.Item2)));
                    }
                }
            }

            // calculate shortest path.
            var shortestPath = encoder.FindShortestPath(startPaths, endPaths, true);
            if(shortestPath == null)
            { // routing failed,.
                throw new BuildLocationFailedException("A route between start {0} and end point {1} was not found.",
                    startLocation, endLocation);
            }

            // convert to edge and vertex-array.
            var vertices = new List<long>();
            var edges = new List<LiveEdge>();
            vertices.Add(shortestPath.Vertex);
            edges.Add(shortestPath.Edge);
            while (shortestPath.From != null)
            {
                shortestPath = shortestPath.From;
                vertices.Add(shortestPath.Vertex);
                if (shortestPath.From != null)
                {
                    edges.Add(shortestPath.Edge);
                }
            }
            vertices.Reverse();
            edges.Reverse();

            // extract vertices, edges and offsets.
            if(vertices[0] < 0)
            { // replace the first virtual vertex with the real vertex.
                if (vertices[1] == startEdge.Item1)
                { // the virtual vertex should be item2.
                    vertices[0] = startEdge.Item2;
                }
                else
                { // the virtual vertex should be item1.
                    vertices[0] = startEdge.Item1;
                }
            }
            if(vertices[vertices.Count - 1] < 0)
            { // replace the last virtual vertex with the real vertex.
                if (vertices[vertices.Count - 2] == endEdge.Item1)
                { // the virtual vertex should be item2.
                    vertices[vertices.Count - 1] = endEdge.Item2;
                }
                else
                { // the virtual vertex should be item1.
                    vertices[vertices.Count - 1] = endEdge.Item1;
                }
            }

            // calculate offset.
            var referencedLine = new OpenLR.Referenced.Locations.ReferencedLine(encoder.Graph)
            {
                Edges = edges.ToArray(),
                Vertices = vertices.ToArray()
            };
            var length = referencedLine.Length(encoder).Value;

            // project again on the first edge.
            startEdge = new Tuple<long,long,LiveEdge>(referencedLine.Vertices[0], referencedLine.Vertices[1], referencedLine.Edges[0]);
            coordinates = encoder.Graph.GetCoordinates(startEdge);
            if(!coordinates.ProjectOn(startLocation, out bestProjected, out bestPosition, out bestStartOffset))
            { // projection did not succeed.
                throw new BuildLocationFailedException("Projection of location {0} on the first edge of shortest path failed.",
                    endLocation);
            }
            var positivePercentageOffset = (float)System.Math.Max(System.Math.Min((bestStartOffset.Value / length) * 100.0, 100), 0);

            // project again on the last edge.
            endEdge = new Tuple<long, long, LiveEdge>(referencedLine.Vertices[referencedLine.Vertices.Length - 2], 
                referencedLine.Vertices[referencedLine.Vertices.Length - 1],  referencedLine.Edges[referencedLine.Edges.Length - 1]);
            coordinates = encoder.Graph.GetCoordinates(endEdge);
            endEdgeLength = coordinates.Length();
            if (!coordinates.ProjectOn(endLocation, out bestProjected, out bestPosition, out bestEndOffset))
            { // projection did not succeed.
                throw new BuildLocationFailedException("Projection of location {0} on the first edge of shortest path failed.",
                    endLocation);
            }
            var negativePercentageOffset = (float)System.Math.Max((System.Math.Min(((endEdgeLength.Value - bestEndOffset.Value) / length) * 100.0, 100)), 0);

            return encoder.BuildLineLocation(vertices.ToArray(), edges.ToArray(), positivePercentageOffset, negativePercentageOffset);
        }

        /// <summary>
        /// Builds a line location given a sequence of vertex->edge->vertex...edge->vertex.
        /// </summary>
        /// <param name="encoder">The encoder.</param>
        /// <param name="vertices">The vertices along the path to create the location for. Contains at least two vertices (#vertices = #edges + 1).</param>
        /// <param name="edges">The edge along the path to create the location for. Contains at least one edge (#vertices = #edges + 1). Edges need to be traversible by the vehicle profile used by the encoder in the direction of the path</param>
        /// <param name="positivePercentageOffset">The offset in percentage relative to the distance of the total path and it's start. [0-100[</param>
        /// <param name="negativePercentageOffset">The offset in percentage relative to the distance of the total path and it's end. [0-100[</param>
        /// <returns></returns>
        public static ReferencedLine BuildLineLocation(this ReferencedEncoderBase encoder, long[] vertices, LiveEdge[] edges, 
            float positivePercentageOffset, float negativePercentageOffset)
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
            var referencedLine = new OpenLR.Referenced.Locations.ReferencedLine(encoder.Graph)
            {
                Edges = edges.Clone() as LiveEdge[],
                Vertices = vertices.Clone() as long[]
            };
            var length = (float)referencedLine.Length(encoder).Value;
            var positiveOffsetLength = length * (positivePercentageOffset / 100.0f);
            var negativeOffsetLength = length * (negativePercentageOffset / 100.0f);
            referencedLine.NegativeOffsetPercentage = negativeOffsetLength;
            referencedLine.PositiveOffsetPercentage = positiveOffsetLength;

            // fill shapes.
            referencedLine.EdgeShapes = new GeoCoordinateSimple[referencedLine.Edges.Length][];
            for (int i = 0; i < referencedLine.Edges.Length; i++)
            {
                referencedLine.EdgeShapes[i] = encoder.Graph.GetEdgeShape(
                    referencedLine.Vertices[i], referencedLine.Vertices[i + 1]);
            }

            return referencedLine;
        }
    }
}