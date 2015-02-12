using OpenLR.Encoding;
using OpenLR.Referenced.Router;
using OsmSharp.Collections.PriorityQueues;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenLR.Referenced
{
    /// <summary>
    /// A reference encoder base class for live edges.
    /// </summary>
    public abstract class ReferencedEncoderBaseLiveEdge : ReferencedEncoderBase<LiveEdge>
    {
        /// <summary>
        /// Creates a new referenced live edge decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="locationEncoder"></param>
        public ReferencedEncoderBaseLiveEdge(BasicRouterDataSource<LiveEdge> graph, Encoder locationEncoder)
            : base(graph, locationEncoder)
        {
            
        }

        /// <summary>
        /// Finds a valid vertex for the given vertex but does not search in the direction of the target neighbour.
        /// </summary>
        /// <param name="vertex">The invalid vertex.</param>
        /// <param name="edge">The edge that leads to the target vertex.</param>
        /// <param name="targetVertex">The target vertex.</param>
        /// <param name="excludeSet">The set of vertices that should be excluded in the search.</param>
        /// <param name="searchForward">When true, the search is forward, otherwise backward.</param>
        public override PathSegment FindValidVertexFor(long vertex, LiveEdge edge, long targetVertex, HashSet<long> excludeSet, bool searchForward)
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
            if(pathTo == null)
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
        public override PathSegment FindShortestPath(long from, long to, bool searchForward)
        {
            var router = this.GetRouter();
            var result = router.Calculate(this.Graph, this.Vehicle,
                from, to, searchForward);
            if(result == null)
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
        public override PathSegment FindShortestPath(List<PathSegment> fromPaths, List<PathSegment> toPaths, bool searchForward)
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
}