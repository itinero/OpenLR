using OpenLR.Model;
using OpenLR.Referenced.Decoding.Candidates;
using OsmSharp.Collections.PriorityQueues;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Osm.Graphs;
using System;
using System.Collections.Generic;

namespace OpenLR.Referenced.Router
{
    /// <summary>
    /// A simple dykstra-based router.
    /// </summary>
    /// <remarks>Specific OpenLR-implementation that does only the bare necessities.</remarks>
    public class BasicRouter
    {
        /// <summary>
        /// Holds the maximum settles.
        /// </summary>
        public static uint MaxSettles = 100;

        /// <summary>
        /// Calculates a path between the two candidates using the information in the candidates.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="minimum"></param>
        public PathSegment<long> Calculate(BasicRouterDataSource<LiveEdge> graph, IRoutingInterpreter interpreter,
            Vehicle vehicle, CandidateVertexEdge from, CandidateVertexEdge to, FunctionalRoadClass minimum)
        {
            return this.Calculate(graph, interpreter, vehicle, from, to, minimum, MaxSettles);
        }

        /// <summary>
        /// Calculates a path between the two candidates using the information in the candidates.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="minimum"></param>
        /// <param name="maxSettles"></param>
        /// <returns></returns>
        public PathSegment<long> Calculate(BasicRouterDataSource<LiveEdge> graph, IRoutingInterpreter interpreter,
            Vehicle vehicle, CandidateVertexEdge from, CandidateVertexEdge to, FunctionalRoadClass minimum, uint maxSettles)
        {
            // first check for the simple stuff.
            if (from.Vertex == to.Vertex)
            { // route consists of one vertex.
                return new PathSegment<long>(from.Vertex);
            }

            // check paths.
            var fromPathWeight = vehicle.Weight(graph.TagsIndex.Get(from.Edge.Tags), from.Edge.Distance);
            var fromPath = new PathSegment<long>(from.TargetVertex, fromPathWeight, new PathSegment<long>(from.Vertex));
            if(from.Vertex == to.TargetVertex &&
                to.Vertex == from.TargetVertex)
            { // edges are the same, 
                return fromPath;
            }

            // initialize the heap/visit list.
            var heap = new BinaryHeap<PathSegment<long>>(maxSettles / 4);
            var visited = new HashSet<long>();
            visited.Add(from.Vertex);

            // also add the target to the visit list and actually search for the target candidate edge ending.
            visited.Add(to.Vertex);
            var target = to.TargetVertex;
            var targetWeight = double.MaxValue;

            // create a path segment from the from-candidate.
            heap.Push(fromPath, (float)fromPath.Weight);

            // keep searching for the target.
            while (true)
            {
                // get the next vertex.
                var current = heap.Pop();
                if (current == null)
                { // there is nothing more in the queue, target will not be found.
                    break;
                }
                if (visited.Contains(current.VertexId))
                { // move to the next neighbour.
                    continue;
                }
                visited.Add(current.VertexId);

                // check for the target.
                if(current.VertexId == target)
                { // target was found.
                    return new PathSegment<long>(to.Vertex, current.Weight + fromPathWeight, current);
                }

                // check if the maximum settled vertex count has been reached.
                if(visited.Count >= maxSettles)
                { // stop search, target will not be found.
                    break;
                }

                // add the neighbours to queue.
                var neighbours = graph.GetEdges(current.VertexId);
                if (neighbours != null)
                { // neighbours exist.
                    foreach (var neighbour in neighbours)
                    {
                        // check if the neighbour was settled before.
                        if (visited.Contains(neighbour.Key))
                        { // move to the next neighbour.
                            continue;
                        }

                        // get tags and check traversability and oneway.
                        var tags = graph.TagsIndex.Get(neighbour.Value.Tags);
                        if (vehicle.CanTraverse(tags))
                        { // yay! can traverse.
                            var oneway = vehicle.IsOneWay(tags);
                            if (oneway == null ||
                                oneway.Value == neighbour.Value.Forward)
                            {
                                // create path to neighbour and queue it.
                                var weight = vehicle.Weight(graph.TagsIndex.Get(neighbour.Value.Tags), neighbour.Value.Distance);
                                var path = new PathSegment<long>(neighbour.Key, current.Weight + weight, current);
                                if (path.Weight < targetWeight)
                                { // the weight of the neighbour is smaller than the first neighbour found.
                                    heap.Push(path, (float)path.Weight);

                                    // save distance.
                                    if (path.VertexId == target)
                                    { // the target is already found, no use of queuing neigbours that have a higher weight.
                                        if (targetWeight > path.Weight)
                                        { // set the weight.
                                            targetWeight = path.Weight;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Calculates the shortest path between the two given 'virtual' vertices described by pre-calculate path segments.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="fromPaths">The paths to the source.</param>
        /// <param name="toPaths">The paths to the target.</param>
        /// <param name="searchForward">Flag indicating search direction.</param>
        public PathSegment Calculate(BasicRouterDataSource<LiveEdge> graph, Vehicle vehicle, 
            List<PathSegment> fromPaths, List<PathSegment> toPaths, bool searchForward)
        {
            return this.Calculate(graph, vehicle, fromPaths, toPaths, searchForward, MaxSettles);
        }

        /// <summary>
        /// Calculates the shortest path between the two given 'virtual' vertices described by pre-calculate path segments.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="fromPaths">The paths to the source.</param>
        /// <param name="toPaths">The paths to the target.</param>
        /// <param name="searchForward">Flag indicating search direction.</param>
        /// <param name="maxSettles"></param>
        /// <returns></returns>
        public PathSegment Calculate(BasicRouterDataSource<LiveEdge> graph, Vehicle vehicle,
            List<PathSegment> fromPaths, List<PathSegment> toPaths, bool searchForward, uint maxSettles)
        {
            if (fromPaths.Count == 0) { return null; }
            if (toPaths.Count == 0) { return null; }

            // initialize the heap/visit list.
            var heap = new BinaryHeap<PathSegment>(maxSettles);
            var visited = new HashSet<long>();

            // queue the from-paths.
            heap.Push(fromPaths[0], (float)fromPaths[0].Weight);
            for (int i = 1; i < fromPaths.Count; i++)
            {
                if (fromPaths[0].From.Vertex != fromPaths[i].From.Vertex)
                {
                    throw new ArgumentException("When multiple from paths they should be one-hop away from a common vertex.");
                }
                heap.Push(fromPaths[i], (float)fromPaths[i].Weight);
            }

            // enumerate and store target-paths.
            var toPathDictonary = new Dictionary<long, PathSegment>();
            foreach (var toPath in toPaths)
            {
                if(toPath.From != null)
                {
                    toPathDictonary.Add(toPath.From.Vertex, toPath);
                }
                else
                {
                    toPathDictonary.Add(toPath.Vertex, toPath);
                }
            }

            // keep searching for the target.
            PathSegment bestToTarget = null;
                while (true)
                {
                    // get the next vertex.
                    var current = heap.Pop();

                    if (current == null)
                    { // there is nothing more in the queue, target will not be found.
                        break;
                    }
                    if (visited.Contains(current.Vertex))
                    { // move to the next neighbour.
                        continue;
                    }
                    visited.Add(current.Vertex);

                    // check for the target.
                    PathSegment foundToPath;
                    if (toPathDictonary.TryGetValue(current.Vertex, out foundToPath))
                    { // target was found.
                        toPathDictonary.Remove(current.Vertex);
                        if (bestToTarget == null ||
                            current.Weight + foundToPath.Weight < bestToTarget.Weight)
                        { // ok, this path is better!
                            if (foundToPath.From != null)
                            {
                                bestToTarget = new PathSegment(foundToPath.Vertex, foundToPath.Weight + current.Weight, foundToPath.Edge, current);
                            }
                            else
                            {
                                bestToTarget = current;
                            }
                        }
                        if (toPathDictonary.Count == 0)
                        { // no more targets let, this has to be it.
                            return bestToTarget;
                        }
                    }

                    // check if the maximum settled vertex count has been reached.
                    if (visited.Count >= maxSettles)
                    { // stop search, target will not be found.
                        break;
                    }

                    // add the neighbours to queue.
                    var neighbours = graph.GetEdges(current.Vertex);
                    if (neighbours != null)
                    { // neighbours exist.
                        foreach (var neighbour in neighbours)
                        {
                            // check if the neighbour was settled before.
                            if (visited.Contains(neighbour.Key))
                            { // move to the next neighbour.
                                continue;
                            }

                            // get tags and check traversability and oneway.
                            var tags = graph.TagsIndex.Get(neighbour.Value.Tags);
                            if (vehicle.CanTraverse(tags))
                            { // yay! can traverse.
                                var onway = vehicle.IsOneWay(tags);
                                if (onway == null ||
                                  !(onway.Value == neighbour.Value.Forward ^ searchForward))
                                {
                                    // create path to neighbour and queue it.
                                    var weight = vehicle.Weight(graph.TagsIndex.Get(neighbour.Value.Tags), neighbour.Value.Distance);
                                    var path = new PathSegment(neighbour.Key, current.Weight + weight, neighbour.Value, current);
                                    if (bestToTarget == null ||
                                        path.Weight < bestToTarget.Weight)
                                    { // the weight of the neighbour is smaller than the first neighbour found.
                                        heap.Push(path, (float)path.Weight);
                                    }
                                }
                            }
                        }
                    }
                }
            return bestToTarget;
        }

        /// <summary>
        /// Calculates the shortest path between the two given vertices.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="from">The source vertex.</param>
        /// <param name="to">The target vertex.</param>
        /// <param name="searchForward">Flag indicating search direction.</param>
        public PathSegment Calculate(BasicRouterDataSource<LiveEdge> graph, Vehicle vehicle, 
            long from, long to, bool searchForward)
        {
            return this.Calculate(graph, vehicle, from, to, searchForward, MaxSettles);
        }

        /// <summary>
        /// Calculates the shortest path between the two given vertices.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="vehicle">The vehicle profile.</param>
        /// <param name="from">The source vertex.</param>
        /// <param name="to">The target vertex.</param>
        /// <param name="searchForward">Flag indicating search direction.</param>
        /// <param name="maxSettles"></param>
        /// <returns></returns>
        public PathSegment Calculate(BasicRouterDataSource<LiveEdge> graph, Vehicle vehicle, 
            long from, long to, bool searchForward, uint maxSettles)
        {
            // first check for the simple stuff.
            if (from == to)
            { // route consists of one vertex.
                return new PathSegment(from);
            }

            // initialize the heap/visit list.
            var heap = new BinaryHeap<PathSegment>(100);
            var visited = new HashSet<long>();

            // set the target.
            var target = to;
            var targetWeight = double.MaxValue;

            // create a path segment from the from-candidate.
            heap.Push(new PathSegment(from), (float)0);

            // keep searching for the target.
            while (true)
            {
                // get the next vertex.
                var current = heap.Pop();
                if (current == null)
                { // there is nothing more in the queue, target will not be found.
                    break;
                }
                if (visited.Contains(current.Vertex))
                { // move to the next neighbour.
                    continue;
                }
                visited.Add(current.Vertex);

                // check for the target.
                if (current.Vertex == target)
                { // target was found.
                    return current;
                }

                // check if the maximum settled vertex count has been reached.
                if (visited.Count >= maxSettles)
                { // stop search, target will not be found.
                    break;
                }

                // add the neighbours to queue.
                var neighbours = graph.GetEdges(current.Vertex);
                if (neighbours != null)
                { // neighbours exist.
                    foreach (var neighbour in neighbours)
                    {
                        // check if the neighbour was settled before.
                        if (visited.Contains(neighbour.Key))
                        { // move to the next neighbour.
                            continue;
                        }

                        // get tags and check traversability and oneway.
                        var tags = graph.TagsIndex.Get(neighbour.Value.Tags);
                        if (vehicle.CanTraverse(tags))
                        { // yay! can traverse.
                            var onway = vehicle.IsOneWay(tags);
                            if (onway == null ||
                              !(onway.Value == neighbour.Value.Forward ^ searchForward))
                            {
                                // create path to neighbour and queue it.
                                var weight = vehicle.Weight(graph.TagsIndex.Get(neighbour.Value.Tags), neighbour.Value.Distance);
                                var path = new PathSegment(neighbour.Key, current.Weight + weight, neighbour.Value, current);
                                if (path.Weight < targetWeight)
                                { // the weight of the neighbour is smaller than the first neighbour found.
                                    heap.Push(path, (float)path.Weight);

                                    // save distance.
                                    if (path.Vertex == target)
                                    { // the target is already found, no use of queuing neigbours that have a higher weight.
                                        if (targetWeight > path.Weight)
                                        { // set the weight.
                                            targetWeight = path.Weight;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}