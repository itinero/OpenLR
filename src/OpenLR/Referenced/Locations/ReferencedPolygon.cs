using OpenLR.Model;

namespace OpenLR.Referenced.Locations;

/// <summary>
/// Represents a referenced polygon location with a graph as a reference.
/// </summary>
/// <remarks>The reference graph plays no part here, a polygon is just a polygon.</remarks>
public class ReferencedPolygon : IReferencedLocation
{
    /// <summary>
    /// Gets or sets the list of coordinates.
    /// </summary>
    public Coordinate[] Coordinates { get; set; }
}
