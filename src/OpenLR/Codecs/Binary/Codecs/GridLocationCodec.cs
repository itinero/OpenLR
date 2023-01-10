using System;
using OpenLR.Codecs.Binary.Data;
using OpenLR.Model.Locations;

namespace OpenLR.Codecs.Binary.Codecs;

/// <summary>
/// A decoder that decodes binary data into a grid location.
/// </summary>
public static class GridLocationCodec
{
    /// <summary>
    /// Decodes the given data into a location reference.
    /// </summary>
    public static GridLocation Decode(byte[] data)
    {
        // decode box.
        var lowerLeft = CoordinateConverter.Decode(data, 1);
        var upperRight = CoordinateConverter.DecodeRelative(lowerLeft, data, 7);

        // decode column/row info.
        var columns = data[11] * 256 + data[12];
        var rows = data[13] * 256 + data[14];

        // create grid location.
        var grid = new GridLocation
        {
            LowerLeft = lowerLeft,
            UpperRight = upperRight,
            Columns = columns,
            Rows = rows
        };
        return grid;
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

        return data.Length is 15 or 17;
    }
}
