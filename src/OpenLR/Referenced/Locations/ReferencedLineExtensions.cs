using System;
using System.Collections.Generic;
using Itinero.Geo;
using Itinero.Network.Enumerators.Edges;

namespace OpenLR.Referenced.Locations;

/// <summary>
/// Contains extension methods for the referenced line.
/// </summary>
public static class ReferencedLineExtensions
{
    /// <summary>
    /// Gets the coordinates representing the shape of the referenced line.
    /// </summary>
    /// <param name="referencedLine">The referenced line.</param>
    /// <returns>The coordinates.</returns>
    /// <exception cref="Exception"></exception>
    public static IEnumerable<(double longitude, double latitude, float? e)> GetCoordinates(this ReferencedLine referencedLine)
    {
        var firstDone = false;
        var edgeEnumerator = referencedLine.RoutingNetwork.GetEdgeEnumerator();
        foreach (var (edge, forward) in referencedLine)
        {
            if (!edgeEnumerator.MoveTo(edge, forward)) throw new Exception("Edge not found");

            using var shapeEnumerator = edgeEnumerator.GetCompleteShape().GetEnumerator();
            shapeEnumerator.MoveNext();
            if (!firstDone)
            {
                yield return shapeEnumerator.Current;
                firstDone = true;
            }

            while (shapeEnumerator.MoveNext())
            {
                yield return shapeEnumerator.Current;
            }
        }
    }

    /// <summary>
    /// Gets the locations of the offsets.
    /// </summary>
    /// <param name="referencedLine">The referenced line.</param>
    /// <returns></returns>
    public static ((double longitude, double latitude, float? e) positive, (double longitude, double latitude, float
        ? e) negative) GetOffsetLocations(this ReferencedLine referencedLine)
    {
        var coordinates = referencedLine.GetCoordinates();

        // ReSharper disable once PossibleMultipleEnumeration
        var positive = coordinates.PositionAlongLine(referencedLine.PositiveOffsetPercentage / 100.0);
        // ReSharper disable once PossibleMultipleEnumeration
        var negative = coordinates.PositionAlongLine(referencedLine.NegativeOffsetPercentage / 100.0);

        return (positive, negative);
    }
}
