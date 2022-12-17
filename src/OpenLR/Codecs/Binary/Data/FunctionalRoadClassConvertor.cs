using OpenLR.Model;
using System;

namespace OpenLR.Codecs.Binary.Data;

/// <summary>
/// Represents a functional road class convertor that encodes/decodes into the binary OpenLR format.
/// </summary>
public static class FunctionalRoadClassConvertor
{
    /// <summary>
    /// Decodes a functional road class from binary data.
    /// </summary>
    /// <param name="data">The binary data.</param>
    /// <param name="byteIndex">The index of the data in the first byte.</param>
    /// <returns></returns>
    public static FunctionalRoadClass Decode(byte[] data, int byteIndex)
    {
        return FunctionalRoadClassConvertor.Decode(data, 0, byteIndex);
    }

    /// <summary>
    /// Decodes a functional road class from binary data.
    /// </summary>
    /// <param name="data">The binary data.</param>
    /// <param name="startIndex">The index of the byte in data.</param>
    /// <param name="byteIndex">The index of the data in the given byte.</param>
    public static FunctionalRoadClass Decode(byte[] data, int startIndex, int byteIndex)
    {
        if (byteIndex > 5) { throw new ArgumentOutOfRangeException(nameof(byteIndex), "byteIndex has to be a value in the range of [0-5]."); }

        byte classData = data[startIndex];

        // create mask.
        int mask = 7 << (5 - byteIndex);
        int value = (classData & mask) >> (5 - byteIndex);

        return value switch
        {
            0 => FunctionalRoadClass.Frc0,
            1 => FunctionalRoadClass.Frc1,
            2 => FunctionalRoadClass.Frc2,
            3 => FunctionalRoadClass.Frc3,
            4 => FunctionalRoadClass.Frc4,
            5 => FunctionalRoadClass.Frc5,
            6 => FunctionalRoadClass.Frc6,
            7 => FunctionalRoadClass.Frc7,
            _ => throw new InvalidOperationException("Decoded a value from three bits not in the range of [0-7]?!")
        };
    }

    /// <summary>
    /// Encodes a functional road class into binary data.
    /// </summary>
    /// <param name="functionalRoadClass"></param>
    /// <param name="data"></param>
    /// <param name="startIndex"></param>
    /// <param name="byteIndex"></param>
    public static void Encode(FunctionalRoadClass functionalRoadClass, byte[] data, int startIndex, int byteIndex)
    {
        if (byteIndex > 5) { throw new ArgumentOutOfRangeException(nameof(byteIndex), "byteIndex has to be a value in the range of [0-5]."); }

        byte value = functionalRoadClass switch
        {
            FunctionalRoadClass.Frc0 => 0,
            FunctionalRoadClass.Frc1 => 1,
            FunctionalRoadClass.Frc2 => 2,
            FunctionalRoadClass.Frc3 => 3,
            FunctionalRoadClass.Frc4 => 4,
            FunctionalRoadClass.Frc5 => 5,
            FunctionalRoadClass.Frc6 => 6,
            FunctionalRoadClass.Frc7 => 7,
            _ => 0
        };

        byte target = data[startIndex];

        byte mask = (byte)(7 << (5 - byteIndex));
        target = (byte)(target & ~mask); // set to zero.
        value = (byte)(value << (5 - byteIndex)); // move value to correct position.
        target = (byte)(target | value); // add to byte.

        data[startIndex] = target;
    }
}
