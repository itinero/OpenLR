using System;

namespace OpenLR.Codecs.Binary.Data;

/// <summary>
/// Represents a distance to next convertor that encodes/decodes coordinates into the binary OpenLR format.
/// </summary>
public static class DistanceToNextConvertor
{
    /// <summary>
    /// Holds the distance per interval for 256 intervals in 15000m 
    /// </summary>
    private const double DistancePerInterval = 58.6;

    /// <summary>
    /// Encodes the distance into a byte.
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static byte Encode(int distance)
    {
        if (distance is < 0 or >= 15000) { throw new ArgumentOutOfRangeException(nameof(distance), "Distance cannot be encoded if not in the range [0-15000["); }

        return (byte)System.Math.Floor(distance / DistancePerInterval);
    }

    /// <summary>
    /// Decodes the distance from a byte.
    /// </summary>
    /// <param name="distanceByte"></param>
    /// <returns></returns>
    public static int Decode(byte distanceByte)
    {
        return (int)System.Math.Ceiling(distanceByte * DistancePerInterval);
    }
}
