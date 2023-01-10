using Itinero;
using Itinero.Network.Enumerators.Edges;
using Itinero.Snapping;

namespace OpenLR.Referenced.Codecs;

internal static class ISnapperExtensions
{
    /// <summary>
    /// Snaps to the vertex at the tail.
    /// </summary>
    /// <param name="snapper">The snapper.</param>
    /// <param name="edgeEnumerator">The edge enumerator.</param>
    /// <returns>The results if any. If the edge cannot be accessed by any configured profiles the result is in error.</returns>
    public static Result<SnapPoint> ToTail(this ISnapper snapper, IEdgeEnumerator edgeEnumerator)
    {
        return snapper.To(edgeEnumerator.EdgeId, (ushort)(edgeEnumerator.Forward ? 0 : ushort.MaxValue),
            edgeEnumerator.Forward);
    }
}
