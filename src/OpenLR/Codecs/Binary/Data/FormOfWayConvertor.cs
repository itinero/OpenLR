﻿using OpenLR.Model;
using System;

namespace OpenLR.Codecs.Binary.Data;

/// <summary>
/// Represents a form of way convertor that encodes/decodes into the binary OpenLR format.
/// </summary>
public static class FormOfWayConvertor
{
    /// <summary>
    /// Decodes a form of way type from binary data.
    /// </summary>
    /// <param name="data">The binary data.</param>
    /// <param name="byteIndex">The index of the data in the first byte.</param>
    /// <returns></returns>
    public static FormOfWay Decode(byte[] data, int byteIndex)
    {
        return FormOfWayConvertor.Decode(data, 0, byteIndex);
    }

    /// <summary>
    /// Decodes a form of way type from binary data.
    /// </summary>
    /// <param name="data">The binary data.</param>
    /// <param name="startIndex">The index of the byte in data.</param>
    /// <param name="byteIndex">The index of the data in the given byte.</param>
    public static FormOfWay Decode(byte[] data, int startIndex, int byteIndex)
    {
        if (byteIndex > 5) { throw new ArgumentOutOfRangeException(nameof(byteIndex), "byteIndex has to be a value in the range of [0-5]."); }

        byte classData = data[startIndex];

        // create mask.
        int mask = 7 << (5 - byteIndex);
        int value = (classData & mask) >> (5 - byteIndex);

        return value switch
        {
            0 => FormOfWay.Undefined,
            1 => FormOfWay.Motorway,
            2 => FormOfWay.MultipleCarriageWay,
            3 => FormOfWay.SingleCarriageWay,
            4 => FormOfWay.Roundabout,
            5 => FormOfWay.TrafficSquare,
            6 => FormOfWay.SlipRoad,
            7 => FormOfWay.Other,
            _ => throw new InvalidOperationException("Decoded a value from three bits not in the range of [0-7]?!")
        };
    }

    /// <summary>
    /// Encodes a form of way into binary data.
    /// </summary>
    /// <param name="formOfWay"></param>
    /// <param name="data"></param>
    /// <param name="startIndex"></param>
    /// <param name="byteIndex"></param>
    public static void Encode(FormOfWay formOfWay, byte[] data, int startIndex, int byteIndex)
    {
        if (byteIndex > 5) { throw new ArgumentOutOfRangeException(nameof(byteIndex), "byteIndex has to be a value in the range of [0-5]."); }

        int value = formOfWay switch
        {
            FormOfWay.Undefined => 0,
            FormOfWay.Motorway => 1,
            FormOfWay.MultipleCarriageWay => 2,
            FormOfWay.SingleCarriageWay => 3,
            FormOfWay.Roundabout => 4,
            FormOfWay.TrafficSquare => 5,
            FormOfWay.SlipRoad => 6,
            FormOfWay.Other => 7,
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
