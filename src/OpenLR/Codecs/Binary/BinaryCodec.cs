using System;
using OpenLR.Codecs.Binary.Codecs;
using OpenLR.Model.Locations;

namespace OpenLR.Codecs.Binary;

/// <summary>
/// A binary codec to encode/decode OpenLR binary format.
/// </summary>
public class BinaryCodec : CodecBase
{
    /// <summary>
    /// Decodes the given string.
    /// </summary>
    public override ILocation Decode(string encoded)
    {
        if (encoded == null) { throw new ArgumentNullException(nameof(encoded)); }

        // the data in a binary decoder should be a base64 string.
        var binaryData = Convert.FromBase64String(encoded);

        if (CircleLocationCodec.CanDecode(binaryData))
        {
            return CircleLocationCodec.Decode(binaryData);
        }
        if (ClosedLineLocationCodec.CanDecode(binaryData))
        {
            return ClosedLineLocationCodec.Decode(binaryData);
        }
        if (GeoCoordinateLocationCodec.CanDecode(binaryData))
        {
            return GeoCoordinateLocationCodec.Decode(binaryData);
        }
        if (GridLocationCodec.CanDecode(binaryData))
        {
            return GridLocationCodec.Decode(binaryData);
        }
        if (LineLocationCodec.CanDecode(binaryData))
        {
            return LineLocationCodec.Decode(binaryData);
        }
        if (PointAlongLineLocationCodec.CanDecode(binaryData))
        {
            return PointAlongLineLocationCodec.Decode(binaryData);
        }
        if (PoiWithAccessPointLocationCodec.CanDecode(binaryData))
        {
            return PoiWithAccessPointLocationCodec.Decode(binaryData);
        }
        if (PolygonLocationCodec.CanDecode(binaryData))
        {
            return PolygonLocationCodec.Decode(binaryData);
        }
        if (RectangleLocationCodec.CanDecode(binaryData))
        {
            return RectangleLocationCodec.Decode(binaryData);
        }
        throw new ArgumentException($"Cannot decode string, no codec found: {encoded}");
    }

    /// <summary>
    /// Encodes the given location.
    /// </summary>
    public override string Encode(ILocation location)
    {
        if (location == null) { throw new ArgumentNullException(nameof(location)); }

        var encoded = location switch
        {
            LineLocation lineLocation => LineLocationCodec.Encode(lineLocation),
            PointAlongLineLocation alongLineLocation => PointAlongLineLocationCodec.Encode(alongLineLocation),
            _ => throw new ArgumentException("Encoding failed, this type of location is not supported.")
        };

        // decode into a byte array and convert to a base64 string.
        return Convert.ToBase64String(encoded);
    }
}
