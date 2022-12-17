using System;
using OpenLR.Codecs.Binary.Data;
using OpenLR.Model.Locations;

namespace OpenLR.Codecs.Binary.Codecs;

/// <summary>
/// A decoder that decodes binary data into a geo coordinate.
/// </summary>
public static class GeoCoordinateLocationCodec
{
    /// <summary>
    /// Decodes the given data into a location reference.
    /// </summary>
    public static GeoCoordinateLocation Decode(byte[]? data)
    {
        var geoCoordinate = new GeoCoordinateLocation { Coordinate = CoordinateConverter.Decode(data, 1) };
        return geoCoordinate;
    }

    /// <summary>
    /// Returns true if the given data can be decoded by this decoder.
    /// </summary>
    public static bool CanDecode(byte[] data)
    {
        // decode the header first.
        var header = HeaderConvertor.Decode(data, 0);

        // check header info.
        if (header.ArF1 ||
            !header.IsPoint ||
            header.ArF0 ||
            header.HasAttributes)
        { // header is incorrect.
            return false;
        }

        return data.Length == 7;
    }
}
