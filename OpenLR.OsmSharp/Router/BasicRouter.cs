using OpenLR.Model;
using OpenLR.OsmSharp.Decoding.Candidates;
using OsmSharp.Collections.PriorityQueues;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Osm.Graphs;
using System.Collections.Generic;

namespace OpenLR.OsmSharp.Router
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
        private const uint MAX_SETTLES = 1000;

        /// <summary>
        /// Calculates a path between the two candidates using the information in the candidates.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="vehicle"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="minimum"></param>
        /// <returns></returns>
        public PathSegment<long> Calculate(BasicRouterDataSource<LiveEdge> graph, IRoutingInterpreter interpreter,
            Vehicle vehicle, CandidateVertexEdge<LiveEdge> from, CandidateVertexEdge<LiveEdge> to, FunctionalRoadClass minimum)
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
            var heap = new BinairyHeap<PathSegment<long>>(100);
            var visited = new HashSet<long>();
            visited.Add(from.Vertex);

            // also add the target to the visit list and actually search for the target candidate edge ending.
            visited.Add(to.Vertex);
            var target = to.TargetVertex;

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
                visited.Add(current.VertexId);

                // check for the target.
                if(current.VertexId == target)
                { // target was found.
                    return new PathSegment<long>(to.Vertex, current.Weight + fromPathWeight, current);
                }

                // check if the maximum settled vertex count has been reached.
                if(visited.Count >= MAX_SETTLES)
                { // stop search, target will not be found.
                    break;
                }

                // add the neighbours to queue.
                var neighbours = graph.GetArcs(current.VertexId);
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
                                heap.Push(path, (float)path.Weight);
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}