using OpenLR.Encoding;
using OpenLR.OsmSharp.Router;
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
        public ReferencedEncoderBaseLiveEdge(BasicRouterDataSource<LiveEdge> graph, Encoder locationEncoder)
            : base(graph, locationEncoder)
        {

        }

        /// <summary>
        /// Finds a valid vertex for the given vertex but does not search in the direction of the target neighbour.
        /// </summary>
        /// <param name="vertex">The invalid vertex.</param>
        /// <param name="edge">The edge that leads to the target vertex.</param>
        /// <param name="searchForward">When true, the search is forward, otherwise backward.</param>
        public override PathSegment FindValidVertexFor(long vertex, LiveEdge edge, bool searchForward)
        {
            // GIST: execute a dykstra search to find a vertex that is valid.
            // this will return a vertex that is on the shortest path:
            // foundVertex -> vertex -> targetNeighbour.

            // initialize settled set.
            var settled = new HashSet<long>();

            // initialize heap.
            var heap = new BinairyHeap<PathSegment>(10);
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
                    var arcs = this.Graph.GetArcs(current.Vertex);
                    foreach (var arc in arcs)
                    {
                        if (!settled.Contains(arc.Key) &&
                            !edge.Equals(arc.Value))
                        { // ok, new neighbour!
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
                throw new Exception(
                    string.Format("Could not find a valid vertex for invalid vertex [{0}].", vertex));
            }

            // add the path to the given location.
            return pathTo;
        }
    }
}
