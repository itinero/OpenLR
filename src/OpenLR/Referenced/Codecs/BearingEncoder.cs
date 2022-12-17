using System.Collections.Generic;
using System.Linq;
using System;
using Itinero.Geo;
using OpenLR.Model;

namespace OpenLR.Referenced.Codecs;

/// <summary>
/// Holds bearing encoder code.
/// </summary>
public static class BearingEncoder
{
    /// <summary>
    /// Encodes a bearing based on the list of coordinates and the BEARDIST parameter.
    /// </summary>
    public static float EncodeBearing(IEnumerable<(double longitude, double latitude, float? e)> coordinates)
    {
        using var enumerator = coordinates.GetEnumerator();
        if (!enumerator.MoveNext()) return 0;
        var first = enumerator.Current;
        var previous = first;
        var bearingPosition = previous;
        var distance = 0.0;
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            var currentSegmentDistance = current.DistanceEstimateInMeter(previous);
            var currentDistance = currentSegmentDistance + distance;
            if (currentDistance > Constants.BearingDistance)
            { // the coordinate to calculate the beardist is in this segment!
                // calculate where.
                var relativeDistance = Constants.BearingDistance - distance;
                var relativeOffset = relativeDistance / currentSegmentDistance;

                bearingPosition = (previous.longitude + ((current.longitude - previous.longitude) * relativeOffset),
                    previous.latitude + ((current.latitude - previous.latitude) * relativeOffset), null);
                break;
            }
            distance = currentDistance;
            previous = current;
        }
            
        var angle = bearingPosition.AngleWithMeridian(first);
        if (Math.Abs(Math.Round(angle) - 360) < float.Epsilon)
        { // make sure any 360 degree angle is converted to 0, range allowed is [0, 360[
            angle = 0;
        }
        return (float)angle;
    }

    /// <summary>
    /// Encodes a bearing based on the list of coordinates and the BEARDIST parameter.
    /// </summary>
    public static float EncodeBearing(IEnumerable<(double longitude, double latitude, float? e)> coordinates, bool startAtEnd)
    {
        if(startAtEnd)
        {
            return BearingEncoder.EncodeBearing(new List<(double longitude, double latitude, float? e)>(
                coordinates.Reverse()));
        }
        return BearingEncoder.EncodeBearing(coordinates);
    }

    /// <summary>
    /// Encodes a bearing based on the list of coordinates and the BEARDIST parameter.
    /// </summary>
    public static float EncodeBearing(IEnumerable<Coordinate> coordinates)
    {
        return EncodeBearing(coordinates.Select<Coordinate, (double longitude, double latitude, float? e)>(x =>
            (x.Longitude, x.Latitude, null)));
    }

    /// <summary>
    /// Encodes a bearing based on the list of coordinates and the BEARDIST parameter.
    /// </summary>
    public static float EncodeBearing(IEnumerable<OpenLR.Model.Coordinate> coordinates, bool startAtEnd)
    {
        if (startAtEnd)
        {
            return BearingEncoder.EncodeBearing(new List<OpenLR.Model.Coordinate>(coordinates.Reverse<OpenLR.Model.Coordinate>()));
        }
        return BearingEncoder.EncodeBearing(coordinates);
    }
}
