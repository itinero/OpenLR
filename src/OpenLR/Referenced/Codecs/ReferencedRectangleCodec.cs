using OpenLR.Model.Locations;
using OpenLR.Referenced.Locations;

namespace OpenLR.Referenced.Codecs;

/// <summary>
/// The rectangle codec.
/// </summary>
public static class ReferencedRectangleCodec
{
    /// <summary>
    /// Decodes the given location.
    /// </summary>
    public static ReferencedRectangle Decode(RectangleLocation location)
    {
        return new ReferencedRectangle()
        {
            LowerLeftLatitude = location.LowerLeft.Latitude,
            LowerLeftLongitude = location.LowerLeft.Longitude,
            UpperRightLatitude = location.UpperRight.Latitude,
            UpperRightLongitude = location.UpperRight.Longitude
        };
    }

    /// <summary>
    /// Encodes the given location.
    /// </summary>
    public static RectangleLocation Encode(ReferencedRectangle location)
    {
        return new RectangleLocation()
        {
            LowerLeft = new Model.Coordinate()
            {
                Latitude = location.LowerLeftLatitude,
                Longitude = location.LowerLeftLongitude
            },
            UpperRight = new Model.Coordinate()
            {
                Longitude = location.UpperRightLongitude,
                Latitude = location.UpperRightLatitude
            }
        };
    }
}
