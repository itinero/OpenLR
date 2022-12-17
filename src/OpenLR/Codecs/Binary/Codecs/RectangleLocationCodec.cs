using System;
using OpenLR.Codecs.Binary.Data;
using OpenLR.Model.Locations;

namespace OpenLR.Codecs.Binary.Codecs;

/// <summary>
/// A decoder that decodes binary data into a rectangle location.
/// </summary>
public static class RectangleLocationCodec
{
    /// <summary>
    /// Decodes the given data into a location reference.
    /// </summary>
    public static RectangleLocation Decode(byte[]? data)
    {
        var rectangleLocation = new RectangleLocation { LowerLeft = CoordinateConverter.Decode(data, 1) };
        rectangleLocation.UpperRight = CoordinateConverter.DecodeRelative(rectangleLocation.LowerLeft, data, 7);
        return rectangleLocation;
    }

    /// <summary>
    /// Returns true if the given data can be decoded by this decoder.
    /// </summary>
    public static bool CanDecode(byte[] data)
    {
        // decode the header first.
        var header = HeaderConvertor.Decode(data, 0);

        // check header info.
        if (!header.ArF1 ||
            header.IsPoint ||
            header.ArF0 ||
            header.HasAttributes)
        { // header is incorrect.
            return false;
        }

        return data.Length is 11 or 13;
    }
}
