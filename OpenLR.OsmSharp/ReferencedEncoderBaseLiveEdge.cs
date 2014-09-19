using OpenLR.Encoding;
using OsmSharp.Collections.PriorityQueues;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Osm.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenLR.OsmSharp
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
        public ReferencedEncoderBaseLiveEdge(IBasicRouterDataSource<LiveEdge> graph, Encoder locationEncoder)
            : base(graph, locationEncoder)
        {

        }

        /// <summary>
        /// Finds a valid vertex for the given vertex but does not search in the direction of the target neighbour.
        /// </summary>
        /// <param name="vertex">The invalid vertex.</param>
        /// <param name="targetNeighbour">The neighbour of this vertex that is part of the location.</param>
        /// <param name="searchForward">When true, the search is forward, otherwise backward.</param>
        public override PathSegment<uint> FindValidVertexFor(long vertex, long targetNeighbour, bool searchForward)
        {
            // GIST: execute a dykstra search to find a vertex that is valid.
            // this will return a vertex that is on the shortest path:
            // foundVertex -> vertex -> targetNeighbour.

            // initialize settled set.
            var settled = new HashSet<uint>();
            settled.Add((uint)targetNeighbour); // make sure not to select target neighbour.

            // initialize heap.
            var heap = new BinairyHeap<PathSegment<uint>>(10);
            heap.Push(new PathSegment<uint>((uint)vertex), 0);

            // find the path to the closest valid vertex.
            PathSegment<uint> pathTo = null;
            while (heap.Count > 0)
            {
                // get next.
                var current = heap.Pop();
                settled.Add(current.VertexId);

                // check if valid.
                if (current.VertexId != vertex && 
                    this.IsVertexValid(current.VertexId))
                { // ok! vertex is valid.
                    pathTo = current;
                }
                else
                { // continue search.
                    // add unsettled neighbours.
                    var arcs = this.Graph.GetArcs(current.VertexId);
                    foreach (var arc in arcs)
                    {
                        if (settled.Contains(arc.Key))
                        { // ok, new neighbour!
                            var tags = this.Graph.TagsIndex.Get(arc.Value.Tags);
                            if (this.Vehicle.CanTraverse(tags))
                            { // ok, we can traverse this edge.                    
                                var onway = this.Vehicle.IsOneWay(tags);
                                if (onway == null ||
                                  !(onway.Value == arc.Value.Forward ^ searchForward))
                                { // ok, no oneway or oneway reversed.
                                    var weight = this.Vehicle.Weight(this.Graph.TagsIndex.Get(arc.Value.Tags), arc.Value.Distance);
                                    var path = new PathSegment<uint>(arc.Key, current.Weight + weight, current);
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
                throw new Exception(
                    string.Format("Could not found a valid vertex for invalid vertex [{0}] and target neighbour [{1}].", vertex, targetNeighbour));
            }

            // add the path to the given location.
            return pathTo;
        }
    }
}
