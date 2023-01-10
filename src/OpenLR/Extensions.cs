using System;
using System.Collections.Generic;
using System.Globalization;
using Itinero;
using OpenLR.Referenced.Locations;

namespace OpenLR;

/// <summary>
/// Contains extension methods for some OpenLR core stuff.
/// </summary>
public static class Extensions
{
    // /// <summary>
    // /// Copies elements from the list and the range into the given array starting at the given index.
    // /// </summary>
    // /// <typeparam name="T"></typeparam>
    // /// <param name="source">The array to copy from.</param>
    // /// <param name="index">The start of the elements </param>
    // /// <param name="count">The # of elements.</param>
    // /// <param name="array">The array to copy to.</param>
    // /// <param name="arrayIndex">The index to start copying to in the array.</param>
    // public static void CopyTo<T>(this T[] source, int index, T[] array, int arrayIndex, int count)
    // {
    //     for (int idx = index; idx < index + count; idx++)
    //     {
    //         array[arrayIndex] = source[idx];
    //         arrayIndex++;
    //     }
    // }
    //
    // /// <summary>
    // /// Gets a specific range from the array and copies it to a new one.
    // /// </summary>
    // /// <param name="source">The array to copy from.</param>
    // /// <param name="index">The start of the elements </param>
    // /// <param name="count">The # of elements.</param>
    // /// <returns></returns>
    // public static T[] Range<T>(this T[] source, int index, int count)
    // {
    //     var result = new T[count];
    //     source.CopyTo(index, result, 0, count);
    //     return result;
    // }
    //
    // /// <summary>
    // /// Converts the referenced point along the line location to features.
    // /// </summary>
    // public static float Length(this ReferencedPointAlongLine referencedPointALongLineLocation, RouterDb routerDb)
    // {
    //     return referencedPointALongLineLocation.Route.Length(routerDb);
    // }

    // /// <summary>
    // /// Converts the referenced point along the line location to features.
    // /// </summary>
    // public static float Length(this ReferencedLine referencedLine, RouterDb routerDb)
    // {
    //     var length = 0.0f;
    //     for (int idx = 0; idx < referencedLine.Edges.Length; idx++)
    //     {
    //         length = length + routerDb.Network.GetShape(
    //             routerDb.Network.GetEdge(referencedLine.Edges[idx])).Length();
    //     }
    //     return length;
    // }
    //
    // /// <summary>
    // /// Calculates the length of the shape formed by the given coordinates.
    // /// </summary>
    // public static float Length(this IEnumerable<Itinero.LocalGeo.Coordinate> enumerable)
    // {
    //     var length = 0.0f;
    //     Itinero.LocalGeo.Coordinate? previous = null;
    //     foreach (var c in enumerable)
    //     {
    //         if (previous != null)
    //         {
    //             length += Itinero.LocalGeo.Coordinate.DistanceEstimateInMeter(previous.Value, c);
    //         }
    //         previous = c;
    //     }
    //     return length;
    // }

    /// <summary>
    /// Subtracts the two angles returning an angle in the range -180, +180 
    /// </summary>
    public static double AngleSmallestDifference(double angle1, double angle2)
    {
        var diff = angle1 - angle2;

        if (diff > 180)
        {
            return diff - 360;
        }
        return diff;
    }

    //
    // /// <summary>
    // /// Converts the given coordinate to and OpenLR coordinate.
    // /// </summary>
    // public static OpenLR.Model.Coordinate ToCoordinate(this Itinero.LocalGeo.Coordinate coordinate)
    // {
    //     return new OpenLR.Model.Coordinate()
    //     {
    //         Latitude  = coordinate.Latitude,
    //         Longitude = coordinate.Longitude
    //     };
    // }

    /// <summary>
    /// Returns a string representing the object in a culture invariant way.
    /// </summary>
    public static string ToInvariantString(this object obj)
    {
        return obj switch
        {
            IConvertible convertible => convertible.ToString(CultureInfo.InvariantCulture),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => obj.ToString()
        };
    }
}
