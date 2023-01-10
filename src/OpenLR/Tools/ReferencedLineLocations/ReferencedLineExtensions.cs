using System;
using System.Collections;
using System.Collections.Generic;
using Itinero.Geo;
using Itinero.Network;
using Itinero.Network.Enumerators.Edges;
using OpenLR.Referenced.Locations;
using OpenLR.Tools.LineLocations;

namespace OpenLR.Tools.ReferencedLineLocations;

/// <summary>
/// Contains extension methods for referenced line.
/// </summary>
public static class ReferencedLineExtensions
{
    /// <summary>
    /// Calculates the covered edges.
    /// </summary>
    /// <param name="referencedLine">The referenced line.</param>
    /// <returns>A set of edges, direction and offsets.</returns>
    public static IEnumerable<(EdgeId edge, bool forward, ushort tailOffset, ushort headOffset)> GetCoveredEdges(this ReferencedLine referencedLine)
    {
        var length = referencedLine.GetCoordinates().DistanceEstimateInMeter();
        var tailOffsetInMeters = length * (referencedLine.PositiveOffsetPercentage / 100.0);
        var headOffsetInMeters = length - (length * (referencedLine.NegativeOffsetPercentage / 100.0));

        var edgeEnumerator = referencedLine.RoutingNetwork.GetEdgeEnumerator();
        var currentEdgeOffset = 0.0;
        foreach (var (edge, forward) in referencedLine)
        {
            if (!edgeEnumerator.MoveTo(edge, forward))
                throw new Exception("Edge not found");

            var edgeLength = edgeEnumerator.GetCompleteShape().DistanceEstimateInMeter();
            var edgeStart = currentEdgeOffset;
            var edgeEnd = currentEdgeOffset + edgeLength;

            // edge ends before tail begins.
            if (edgeEnd < tailOffsetInMeters)
            {
                currentEdgeOffset = edgeEnd;
                continue;
            }

            // edge starts after head ends.
            if (edgeStart > headOffsetInMeters)
            {
                currentEdgeOffset = edgeEnd;
                continue;
            }

            // for sure edge, or part of it, is included now.
            ushort tailOffset = 0;
            if (tailOffsetInMeters > edgeStart)
            {
                tailOffset = (ushort)(((tailOffsetInMeters - edgeStart) / edgeLength) * ushort.MaxValue);
            }

            ushort headOffset = ushort.MaxValue;
            if (headOffsetInMeters < edgeEnd)
            {
                headOffset = (ushort)(((headOffsetInMeters - edgeEnd) / edgeLength) * ushort.MaxValue);
            }

            yield return (edge, forward, tailOffset, headOffset);

            currentEdgeOffset = edgeEnd;
        }
    }

    /// <summary>
    /// Calculates the covered edges.
    /// </summary>
    /// <param name="referencedLines">The referenced lines.</param>
    /// <returns>A set of edges, direction and offsets.</returns>
    public static IEnumerable<(EdgeId edge, bool forward, ushort tailOffset, ushort headOffset)> GetCoveredEdges(this IEnumerable<ReferencedLine> referencedLines)
    {
        var cover = new LineLocationCover();

        foreach (var referencedLine in referencedLines)
        {
            foreach (var (edge, forward, tailOffset, headOffset) in referencedLine.GetCoveredEdges())
            {
                cover.Add(edge, forward, tailOffset, headOffset);
            }
        }

        return cover;
    }
}
