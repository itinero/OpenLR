using OpenLR.Model;
using OpenLR.Model.Locations;
using OpenLR.Referenced.Locations;

namespace OpenLR.Referenced.Codecs;

/// <summary>
/// The polygon codec.
/// </summary>
public static class ReferencedPolygonCodec
{
    /// <summary>
    /// Decodes the given location.
    /// </summary>
    public static ReferencedPolygon Decode(PolygonLocation location)
    {
        return new ReferencedPolygon()
        {
            Coordinates = location.Coordinates.Clone() as Coordinate[]
        };
    }

    /// <summary>
    /// Encodes the given location.
    /// </summary>
    public static PolygonLocation Encode(ReferencedPolygon location)
    {
        return new PolygonLocation()
        {
            Coordinates = location.Coordinates.Clone() as Coordinate[]
        };
    }
}
