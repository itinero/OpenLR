using OpenLR.Codecs.Binary.Data;
using OpenLR.Model.Locations;

namespace OpenLR.Codecs.Binary.Codecs;

/// <summary>
/// A decoder that decodes binary data into a circle location.
/// </summary>
public static class CircleLocationCodec
{
    /// <summary>
    /// Decodes the given data into a location reference.
    /// </summary>
    public static CircleLocation Decode(byte[] data)
    {
        return new CircleLocation() {
            Coordinate = CoordinateConverter.Decode(data, 1),
            Radius = data[7]
        };
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
            header.IsPoint ||
            header.ArF0 ||
            header.HasAttributes)
        { // header is incorrect.
            return false;
        }

        return data.Length is 8 or 9 or 10 or 11;
    }
}
