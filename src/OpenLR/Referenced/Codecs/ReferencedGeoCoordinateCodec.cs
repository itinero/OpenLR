using OpenLR.Referenced.Locations;
using OpenLR.Model.Locations;

namespace OpenLR.Referenced.Codecs;

/// <summary>
/// The geo coordinate codec.
/// </summary>
public static class ReferencedGeoCoordinateCodec
{
    /// <summary>
    /// Decodes the given location.
    /// </summary>
    public static GeoCoordinateLocation Encode(ReferencedGeoCoordinate location)
    {
        return new GeoCoordinateLocation()
        {
            Coordinate = new Model.Coordinate()
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude
            }
        };
    }

    /// <summary>
    /// Encodes the given location.
    /// </summary>
    public static ReferencedGeoCoordinate Decode(GeoCoordinateLocation location)
    {
        return new ReferencedGeoCoordinate()
        {
            Latitude = location.Coordinate.Latitude,
            Longitude = location.Coordinate.Longitude
        };
    }
}
