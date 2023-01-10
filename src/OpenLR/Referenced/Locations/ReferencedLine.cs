using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Itinero.Network;
using Itinero.Network.Enumerators.Edges;
using Itinero.Routes.Paths;
using Path = Itinero.Routes.Paths.Path;

namespace OpenLR.Referenced.Locations;

/// <summary>
/// Represents a referenced line location with a graph as a reference.
/// </summary>
public class ReferencedLine : IEnumerable<(EdgeId edge, bool forward)>, IReferencedLocation
{
    private readonly List<(EdgeId edge, bool forward)> _edges;

    /// <summary>
    /// Creates a new referenced line.
    /// </summary>
    /// <param name="network">The network.</param>
    /// <param name="edges">The edges.</param>
    /// <param name="positiveOffsetPercentage">The offset at the beginning of the path representing this location</param>
    /// <param name="negativeOffsetPercentage">The offset at the end of the path representing this location.</param>
    public ReferencedLine(RoutingNetwork network, IEnumerable<(EdgeId edge, bool forward)> edges, double positiveOffsetPercentage, double negativeOffsetPercentage)
    {
        this.RoutingNetwork = network;
        this.PositiveOffsetPercentage = positiveOffsetPercentage;
        this.NegativeOffsetPercentage = negativeOffsetPercentage;
        _edges = new List<(EdgeId edge, bool forward)>(edges);
    }

    /// <summary>
    /// Gets the network.
    /// </summary>
    public RoutingNetwork RoutingNetwork { get; }

    /// <summary>
    /// Gets or sets the offset at the beginning of the path representing this location.
    /// </summary>
    public double PositiveOffsetPercentage { get; }

    /// <summary>
    /// Gets or sets the offset at the end of the path representing this location.
    /// </summary>
    public double NegativeOffsetPercentage { get; }

    /// <summary>
    /// Returns the number of edges.
    /// </summary>
    public int Count => _edges.Count;

    /// <inheritdoc/>
    public IEnumerator<(EdgeId edge, bool forward)> GetEnumerator()
    {
        for (var i = 0; i < this.Count; i++)
        {
            var edge = _edges[i];

            yield return (edge.edge, edge.forward);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    /// <summary>
    /// Creates a referenced line from a path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="positiveOffsetPercentage"></param>§
    /// <param name="negativeOffsetPercentage"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public static ReferencedLine FromPath(Path path, double? positiveOffsetPercentage = null, double? negativeOffsetPercentage = null)
    {
        path.Trim();

        // if ((path.Offset1 is 0 or ushort.MaxValue) &&
        //     (path.Offset2 is 0 or ushort.MaxValue))
        // {
        //     return new ReferencedLine(path.RoutingNetwork, path.Select(x => (x.edge, x.forward)), 0, 0);
        // }

        var length = 0.0;
        var offsetStart = 0.0;
        var offsetEnd = 0.0;

        var edgeEnumerator = path.RoutingNetwork.GetEdgeEnumerator();
        using var pathEnumerator = path.GetEnumerator();
        if (pathEnumerator.MoveNext())
        {
            var (edge, direction, offset1, offset2) = pathEnumerator.Current;
            if (!edgeEnumerator.MoveTo(edge, direction)) throw new InvalidDataException("An edge in the path is not found!");

            var edgeLength = edgeEnumerator.EdgeLength();
            if (offset1 > 0) offsetStart = (offset1 / (double)ushort.MaxValue) * edgeLength;
            if (offset2 < ushort.MaxValue) offsetEnd = (1.0 - (offset2 / (double)ushort.MaxValue)) * edgeLength;
            length += edgeLength;
        }

        while (pathEnumerator.MoveNext())
        {
            var (edge, direction, _, offset2) = pathEnumerator.Current;
            if (!edgeEnumerator.MoveTo(edge, direction)) throw new InvalidDataException("An edge in the path is not found!");

            var edgeLength = edgeEnumerator.EdgeLength();
            if (offset2 < ushort.MaxValue) offsetEnd = (1.0 - (offset2 / (double)ushort.MaxValue)) * edgeLength;
            length += edgeLength;
        }

        if (positiveOffsetPercentage != null) offsetStart += (positiveOffsetPercentage.Value * length / 100.0);
        if (negativeOffsetPercentage != null) offsetEnd += (negativeOffsetPercentage.Value * length / 100.0);

        return new ReferencedLine(path.RoutingNetwork, path.Select(x => (x.edge, x.forward)),
            offsetStart / length * 100, offsetEnd / length * 100);
    }

    /// <summary>
    /// Returns the same referenced line but with new offsets.
    /// </summary>
    /// <param name="positiveOffsetPercentage">The offset at the beginning of the path representing this location</param>
    /// <param name="negativeOffsetPercentage">The offset at the end of the path representing this location.</param>
    /// <returns>The same referenced line but with new offsets</returns>
    public ReferencedLine WithOffsets(double positiveOffsetPercentage, double negativeOffsetPercentage)
    {
        return new ReferencedLine(this.RoutingNetwork, this, positiveOffsetPercentage, negativeOffsetPercentage);
    }
}
