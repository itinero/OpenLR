using OpenLR.Model;
using System;

namespace OpenLR.Codecs.Binary.Data;

/// <summary>
/// Represents a orientation convertor that encodes/decodes orientation info into the binary OpenLR format.
/// </summary>
public static class OrientationConverter
{
    /// <summary>
    /// Decodes binary OpenLR orientation data into an orientation.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="byteIndex"></param>
    /// <returns></returns>
    public static Orientation Decode(byte[]? data, int byteIndex)
    {
        return OrientationConverter.Decode(data, 0, byteIndex);
    }

    /// <summary>
    /// Decodes binary OpenLR orientation data into an orientation.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="startIndex"></param>
    /// <param name="byteIndex"></param>
    /// <returns></returns>
    public static Orientation Decode(byte[] data, int startIndex, int byteIndex)
    {
        if (byteIndex > 6) { throw new ArgumentOutOfRangeException(nameof(byteIndex), "byteIndex has to be a value in the range of [0-6]."); }

        byte classData = data[startIndex];

        // create mask.
        int mask = 7 << (6 - byteIndex);
        int value = (classData & mask) >> (6 - byteIndex);

        return value switch
        {
            0 => Orientation.NoOrientation,
            1 => Orientation.FirstToSecond,
            2 => Orientation.SecondToFirst,
            3 => Orientation.BothDirections,
            _ => throw new InvalidOperationException("Decoded a value from three bits not in the range of [0-3]?!")
        };
    }

    /// <summary>
    /// Encodes an OpenLR orientation into a binary representation.
    /// </summary>
    /// <param name="orientation"></param>
    /// <param name="data"></param>
    /// <param name="startIndex"></param>
    /// <param name="byteIndex"></param>
    public static void Encode(Orientation orientation, byte[] data, int startIndex, int byteIndex)
    {
        if (byteIndex > 6) { throw new ArgumentOutOfRangeException(nameof(byteIndex), "byteIndex has to be a value in the range of [0-6]."); }

        int value = orientation switch
        {
            Orientation.NoOrientation => 0,
            Orientation.FirstToSecond => 1,
            Orientation.SecondToFirst => 2,
            Orientation.BothDirections => 3,
            _ => 0
        };

        byte target = data[startIndex];

        byte mask = (byte)(3 << (6 - byteIndex));
        target = (byte)(target & ~mask); // set to zero.
        value = (byte)(value << (6 - byteIndex)); // move value to correct position.
        target = (byte)(target | value); // add to byte.

        data[startIndex] = target;
    }
}
