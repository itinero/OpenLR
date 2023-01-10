using System;
using OpenLR.Model;

namespace OpenLR.Codecs.Binary.Data;

/// <summary>
/// Represents a coordinate convertor that encodes/decodes coordinates into the binary OpenLR format.
/// </summary>
public static class CoordinateConverter
{
    /// <summary>
    /// Decodes binary OpenLR coordinate data into a coordinate.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static Coordinate Decode(byte[]? data)
    {
        return CoordinateConverter.Decode(data, 0);
    }

    /// <summary>
    /// Decodes binary OpenLR coordinate data into a coordinate.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="startIndex"></param>
    /// <returns></returns>
    public static Coordinate Decode(byte[]? data, int startIndex)
    {
        return new Coordinate()
        {
            Latitude = CoordinateConverter.DecodeDegrees(CoordinateConverter.DecodeInt24(data, startIndex + 3)),
            Longitude = CoordinateConverter.DecodeDegrees(CoordinateConverter.DecodeInt24(data, startIndex + 0))
        };
    }

    /// <summary>
    /// Decodes binary OpenLR relative coordinate data into a coordinate.
    /// </summary>
    /// <param name="reference"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static Coordinate DecodeRelative(Coordinate reference, byte[]? data)
    {
        return CoordinateConverter.DecodeRelative(reference, data, 0);
    }

    /// <summary>
    /// Decodes binary OpenLR relative coordinate data into a coordinate.
    /// </summary>
    /// <param name="reference"></param>
    /// <param name="data"></param>
    /// <param name="startIndex"></param>
    /// <returns></returns>
    public static Coordinate DecodeRelative(Coordinate reference, byte[]? data, int startIndex)
    {
        return new Coordinate()
        {
            Latitude = reference.Latitude + (CoordinateConverter.DecodeInt16(data, startIndex + 2) / 100000.0),
            Longitude = reference.Longitude + (CoordinateConverter.DecodeInt16(data, startIndex + 0) / 100000.0)
        };
    }

    /// <summary>
    /// Decodes an integer-encoded coordinate.
    /// </summary>
    /// <param name="valueInt"></param>
    /// <returns></returns>
    private static double DecodeDegrees(int valueInt)
    {
        return (((valueInt - System.Math.Sign(valueInt) * 0.5) * 360) / 16777216);
    }

    /// <summary>
    /// Decodes the given coordinate into a binary OpenLR coordinate.
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="data"></param>
    /// <param name="startIndex"></param>
    public static void Encode(Coordinate coordinate, byte[]? data, int startIndex)
    {
        CoordinateConverter.EncodeInt24(CoordinateConverter.EncodeDegree(coordinate.Longitude), data, startIndex + 0);
        CoordinateConverter.EncodeInt24(CoordinateConverter.EncodeDegree(coordinate.Latitude), data, startIndex + 3);
    }

    /// <summary>
    /// Decodes the given coorrdinate into a binary OpenLR coordinate.
    /// </summary>
    /// <param name="reference"></param>
    /// <param name="coordinate"></param>
    /// <param name="data"></param>
    /// <param name="startIndex"></param>
    public static void EncodeRelative(Coordinate reference, Coordinate coordinate, byte[]? data, int startIndex)
    {
        CoordinateConverter.EncodeInt16((int)((coordinate.Latitude - reference.Latitude) * 100000.0), data, startIndex + 2);
        CoordinateConverter.EncodeInt16((int)((coordinate.Longitude - reference.Longitude) * 100000.0), data, startIndex + 0);
    }

    /// <summary>
    /// Encodes the given degrees into an integer.
    /// </summary>
    /// <param name="value"></param>
    public static int EncodeDegree(double value)
    {
        return (int)(((value * 16777216) / 360.0) + System.Math.Sign(value) * 0.5);
    }

    /// <summary>
    /// Encodes a 32-bit signed integer into a little-endian 24-bit signed integer into the given byte array.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="data"></param>
    /// <param name="startIndex"></param>
    public static void EncodeInt24(int value, byte[] data, int startIndex)
    {
        data[startIndex + 0] = (byte)(value >> 16);
        if (value < 0)
        { // set sign bit.
            data[startIndex + 0] = (byte)(data[startIndex + 0] | (1 << 7));
        }
        else
        { // turn off sign bit.
            data[startIndex + 0] = (byte)(data[startIndex + 0] & ((1 << 7) - 1));
        }
        value = value % (1 << 16);
        data[startIndex + 1] = (byte)(value >> 8);
        value = value % (1 << 8);
        data[startIndex + 2] = (byte)value;
    }

    /// <summary>
    /// Encodes a 32-bit signed integer into a little-endian 16-bit signed integer into the given byte array.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="data"></param>
    /// <param name="startIndex"></param>
    public static void EncodeInt16(int value, byte[] data, int startIndex)
    {
        data[startIndex + 0] = (byte)(value >> 8);
        if (value < 0)
        { // set sign bit.
            data[startIndex + 0] = (byte)(data[startIndex + 0] | (1 << 7));
        }
        else
        { // turn off sign bit.
            data[startIndex + 0] = (byte)(data[startIndex + 0] & ((1 << 7) - 1));
        }
        value = value % (1 << 8);
        data[startIndex + 1] = (byte)value;
    }

    /// <summary>
    /// Decodes a little-endian 24-bit signed integer from the given byte array into a 32-bit signed integer.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="startIndex"></param>
    /// <returns></returns>
    public static int DecodeInt24(byte[] data, int startIndex)
    {
        var bytes = new byte[4];
        bytes[0] = data[startIndex + 2];
        bytes[1] = data[startIndex + 1];
        bytes[2] = data[startIndex + 0];
        bytes[3] = 0;
        // take into account the sign-bit.
        if ((bytes[2] & (1 << 8 - 1)) != 0)
        { // negative, remove sign bit and make sure decodes properly.
            bytes[3] = 255;
        }
        return BitConverter.ToInt32(bytes, 0);
    }

    /// <summary>
    /// Decodes a little-endian 16-bit signed integer from the given byte array into a 32-bit signed integer.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="startIndex"></param>
    /// <returns></returns>
    public static int DecodeInt16(byte[] data, int startIndex)
    {
        var bytes = new byte[4];
        bytes[0] = data[startIndex + 1];
        bytes[1] = data[startIndex + 0];
        bytes[2] = 0;
        bytes[3] = 0;
        // take into account the sign-bit.
        if ((bytes[1] & (1 << 8 - 1)) != 0)
        { // make sure decodes properly.
            bytes[2] = 255;
            bytes[3] = 255;
        }
        return BitConverter.ToInt32(bytes, 0);
    }
}
