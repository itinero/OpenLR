namespace OpenLR.Model.Locations;

/// <summary>
/// Represents a circle location.
/// </summary>
public class CircleLocation : ILocation
{
    /// <summary>
    /// Gets or sets the location (COORD).
    /// </summary>
    public Coordinate Coordinate { get; set;  }

    /// <summary>
    /// Gets or sets the radius in meter (RAD).
    /// </summary>
    public int Radius { get; set; }
}
