using System.Collections.Generic;
using System.Linq;
using Itinero.Geo;

namespace OpenLR.Referenced.Codecs;

/// <summary>
/// Holds bearing encoder code.
/// </summary>
internal static class BearingExtensions
{
    /// <summary>
    /// Encodes a bearing based on the list of coordinates and the BEARDIST parameter.
    /// </summary>
    /// <param name="coordinates">The coordinates representing the shape to calculate the bearing for.</param>
    /// <param name="invert">Invert the angle, use the first location as the arrival point. This is used for calculating bearings for LRPs that use incoming edges as bearing.</param>
    /// <returns>The bearing as the angle between the north and in range of [0, 360[ clockwise.</returns>
    public static float Bearing(this IEnumerable<(double longitude, double latitude, float? e)> coordinates, bool invert = false)
    {
        // get the location along the shape at BEARDIST.
        // ReSharper disable once PossibleMultipleEnumeration
        var bearingPosition = coordinates.PositionAlongLineInMeters(Constants.BearingDistance);

        if (invert)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            var angle = bearingPosition.AngleWithMeridian(coordinates.First());
            if (angle < 0) angle += (System.Math.Ceiling(System.Math.Abs(angle / 360.0))) * 360;
            return (float)angle;
        }
        else
        {
            // ReSharper disable once PossibleMultipleEnumeration
            var angle = coordinates.First().AngleWithMeridian(bearingPosition);
            if (angle < 0) angle += (System.Math.Ceiling(System.Math.Abs(angle / 360.0))) * 360;
            return (float)angle;
        }
    }

    // /// <summary>
    // /// Encodes a bearing based on the list of coordinates and the BEARDIST parameter.
    // /// </summary>
    // public static float Bearing(IEnumerable<(double longitude, double latitude, float? e)> coordinates, bool startAtEnd)
    // {
    //     if(startAtEnd)
    //     {
    //         return BearingExtensions.Bearing(new List<(double longitude, double latitude, float? e)>(
    //             coordinates.Reverse()));
    //     }
    //     return BearingExtensions.Bearing(coordinates);
    // }
    //
    // /// <summary>
    // /// Encodes a bearing based on the list of coordinates and the BEARDIST parameter.
    // /// </summary>
    // public static float Bearing(IEnumerable<Coordinate> coordinates)
    // {
    //     return Bearing(coordinates.Select<Coordinate, (double longitude, double latitude, float? e)>(x =>
    //         (x.Longitude, x.Latitude, null)));
    // }
    //
    // /// <summary>
    // /// Encodes a bearing based on the list of coordinates and the BEARDIST parameter.
    // /// </summary>
    // public static float Bearing(IEnumerable<OpenLR.Model.Coordinate> coordinates, bool startAtEnd)
    // {
    //     if (startAtEnd)
    //     {
    //         return BearingExtensions.Bearing(new List<OpenLR.Model.Coordinate>(coordinates.Reverse<OpenLR.Model.Coordinate>()));
    //     }
    //     return BearingExtensions.Bearing(coordinates);
    // }
}
