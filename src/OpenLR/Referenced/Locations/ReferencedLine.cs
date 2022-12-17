using System;
using System.Collections;
using System.Collections.Generic;
using Itinero.Network;

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
    public ReferencedLine(RoutingNetwork network, IEnumerable<(EdgeId edge, bool forward)> edges, float positiveOffsetPercentage, float negativeOffsetPercentage)
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
    public float PositiveOffsetPercentage { get; }

    /// <summary>
    /// Gets or sets the offset at the end of the path representing this location.
    /// </summary>
    public float NegativeOffsetPercentage { get; }

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
    //
    // /// <summary>
    // /// Adds another line location to this one.
    // /// </summary>
    // public void Add(ReferencedLine location)
    // {
    //     if(this.Vertices[this.Vertices.Length - 1] == location.Vertices[0])
    //     { // there is a match.
    //         // merge vertices.
    //         var vertices = new uint[this.Vertices.Length + location.Vertices.Length - 1];
    //         this.Vertices.CopyTo(vertices, 0);
    //         for(int idx = 1; idx < location.Vertices.Length; idx++)
    //         {
    //             vertices[this.Vertices.Length + idx - 1] = location.Vertices[idx];
    //         }
    //         this.Vertices = vertices;
    //
    //         // merge edges.
    //         var edges = new long[this.Edges.Length + location.Edges.Length];
    //         this.Edges.CopyTo(edges, 0);
    //         location.Edges.CopyTo(edges, this.Edges.Length);
    //         this.Edges = edges;
    //         // Update EndLocation and NegativeOffset
    //         this.EndLocation = location.EndLocation;
    //         this.NegativeOffsetPercentage = location.NegativeOffsetPercentage;
    //         return;
    //     }
    //     throw new Exception("Cannot add a location without them having one vertex incommon.");
    // }

    // /// <summary>
    // /// Creates a new object that is a copy of the current instance.
    // /// </summary>
    // /// <returns></returns>
    // public override object Clone()
    // {
    //     return new ReferencedLine()
    //     {
    //         Edges = this.Edges == null ? null : this.Edges.Clone() as long[],
    //         Vertices = this.Vertices == null ? null : this.Vertices.Clone() as uint[],
    //         NegativeOffsetPercentage = this.NegativeOffsetPercentage,
    //         PositiveOffsetPercentage = this.PositiveOffsetPercentage,
    //         StartLocation = new RouterPoint(
    //             this.StartLocation.Latitude,
    //             this.StartLocation.Longitude,
    //             this.StartLocation.EdgeId,
    //             this.StartLocation.Offset),
    //         EndLocation = new RouterPoint(
    //             this.EndLocation.Latitude,
    //             this.EndLocation.Longitude,
    //             this.EndLocation.EdgeId,
    //             this.EndLocation.Offset)
    //     };
    // }
}
