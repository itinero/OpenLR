using OpenLR.Model.Locations;
using OpenLR.Referenced.Locations;

namespace OpenLR.Referenced.Codecs;

/// <summary>
/// The referenced grid codec.
/// </summary>
public static class ReferencedGridCodec
{
    /// <summary>
    /// Decodes the given location.
    /// </summary>
    public static GridLocation Encode(ReferencedGrid location)
    {
        return new GridLocation()
        {
            Columns = location.Columns,
            Rows = location.Rows,
            LowerLeft = new Model.Coordinate()
            {
                Latitude = location.LowerLeftLatitude,
                Longitude = location.LowerLeftLongitude
            },
            UpperRight = new Model.Coordinate()
            {
                Latitude = location.UpperRightLatitude,
                Longitude = location.UpperRightLongitude
            }
        };
    }

    /// <summary>
    /// Encodes the given location.
    /// </summary>
    public static ReferencedGrid Decode(GridLocation location)
    {
        return new ReferencedGrid()
        {
            Columns = location.Columns,
            Rows = location.Rows,
            LowerLeftLatitude = location.LowerLeft.Latitude,
            LowerLeftLongitude = location.LowerLeft.Longitude,
            UpperRightLatitude = location.UpperRight.Latitude,
            UpperRightLongitude = location.UpperRight.Longitude
        };
    }
}
