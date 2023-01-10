namespace OpenLR.Model.Locations;

/// <summary>
/// Represents a polygon location.
/// </summary>
public class PolygonLocation : ILocation
{
    /// <summary>
    /// Gets or sets the list of coordinates.
    /// </summary>
    public Coordinate[] Coordinates { get; set; }
}
