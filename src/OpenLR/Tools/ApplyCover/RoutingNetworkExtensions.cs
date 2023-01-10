using System;
using System.Collections.Generic;
using System.Linq;
using Itinero.Network;
using Itinero.Network.Enumerators.Edges;
using Itinero.Network.Mutation;

namespace OpenLR.Tools.ApplyCover;

/// <summary>
/// Extensions for a routing network for splitting edges and rewriting parts.
/// </summary>
public static class RoutingNetworkExtensions
{
    /// <summary>
    /// A function to rewrite edge attributes that are covered.
    /// </summary>
    public delegate IEnumerable<(string key, string value)> RewriteEdgeAttributesFunc(IEnumerable<(string key, string value)> attributes, bool coverIsForward);
    
    /// <summary>
    /// Applies the given edge cover, splits edges accordingly and rewrites tags on covered sections.
    /// </summary>
    /// <param name="routingNetwork">The routing network.</param>
    /// <param name="edges">The edges.</param>
    /// <param name="rewrite">The callback to rewrite attributes.</param>
    public static RoutingNetwork ApplyCover(this RoutingNetwork routingNetwork, IEnumerable<(EdgeId edge, bool forward, ushort tailOffset, ushort headOffset)> edges,
        RewriteEdgeAttributesFunc rewrite)
    {
        using (var mutator = routingNetwork.RouterDb.GetMutableNetwork())
        {
            // apply per edge.
            var edgeEnumerator = mutator.GetEdgeEnumerator();
            foreach (var covered in edges)
            {
                if (!edgeEnumerator.MoveTo(covered.edge, covered.forward)) throw new Exception("Edge not found");
                if (covered.tailOffset > covered.headOffset)
                    throw new Exception("Tail offset is larger than head offset");
                if (covered.tailOffset == covered.headOffset) continue;

                var tailShape =
                    ArraySegment<(double longitude, double latitude, float? e)>.Empty as
                        IEnumerable<(double longitude, double latitude, float? e)>;
                var tailVertex = edgeEnumerator.Tail;
                if (covered.tailOffset > 0)
                {
                    tailShape = edgeEnumerator.GetShapeBetween(0, covered.tailOffset).ToList();
                    var tailLocation = tailShape.Last();
                    tailVertex = mutator.AddVertex(tailLocation);
                }

                var headShape =
                    ArraySegment<(double longitude, double latitude, float? e)>.Empty as
                        IEnumerable<(double longitude, double latitude, float? e)>;
                var headVertex = edgeEnumerator.Head;
                if (covered.headOffset < ushort.MaxValue)
                {
                    headShape = edgeEnumerator.GetShapeBetween(covered.headOffset, ushort.MaxValue).ToList();
                    var headLocation = headShape.First();
                    headVertex = mutator.AddVertex(headLocation);
                }

                if (edgeEnumerator.Tail != tailVertex)
                {
                    // add tail segment.
                    mutator.AddEdge(edgeEnumerator.Tail, tailVertex, tailShape, edgeEnumerator.Attributes);
                }

                if (edgeEnumerator.Tail != tailVertex ||
                    edgeEnumerator.Head != headVertex)
                {
                    // add between segment.
                    var attributes = rewrite(edgeEnumerator.Attributes, edgeEnumerator.Forward);
                    var betweenShape = edgeEnumerator.GetShapeBetween(covered.tailOffset, covered.tailOffset);
                    mutator.AddEdge(tailVertex, headVertex, betweenShape, attributes);
                }

                if (edgeEnumerator.Head != headVertex)
                {
                    // add head segment.
                    mutator.AddEdge(headVertex, edgeEnumerator.Head, headShape, edgeEnumerator.Attributes);
                }

                mutator.DeleteEdge(covered.edge);
            }
        }

        return routingNetwork.RouterDb.Latest;
    }
}
