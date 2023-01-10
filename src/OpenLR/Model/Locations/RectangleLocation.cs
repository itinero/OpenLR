namespace OpenLR.Model.Locations;

/// <summary>
/// Represents a rectangle location.
/// </summary>
public class RectangleLocation : ILocation
{
    /// <summary>
    /// Gets or sets the box.
    /// </summary>
    public Coordinate LowerLeft { get; set; }

    /// <summary>
    /// Gets or sets the box.
    /// </summary>
    public Coordinate UpperRight { get; set; }
}
