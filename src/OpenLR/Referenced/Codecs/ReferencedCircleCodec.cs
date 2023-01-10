using OpenLR.Model.Locations;
using OpenLR.Referenced.Locations;

namespace OpenLR.Referenced.Codecs;

/// <summary>
/// The circle codec.
/// </summary>
public static class ReferencedCircleCodec
{
    /// <summary>
    /// Decodes the given location.
    /// </summary>
    public static CircleLocation Encode(ReferencedCircle location)
    {
        return new CircleLocation()
        {
            Coordinate = new Model.Coordinate()
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude
            },
            Radius = location.Radius
        };
    }

    /// <summary>
    /// Encodes the given location.
    /// </summary>
    public static ReferencedCircle Decode(CircleLocation location)
    {
        return new ReferencedCircle()
        {
            Latitude = location.Coordinate.Latitude,
            Longitude = location.Coordinate.Longitude,
            Radius = location.Radius
        };
    }
}
