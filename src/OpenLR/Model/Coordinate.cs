namespace OpenLR.Model;

/// <summary>
/// Represents a geographical coordinate.
/// </summary>
public class Coordinate
{
    /// <summary>
    /// Gets or sets the latitude.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Gets or sets the longitude.
    /// </summary>
    public double Longitude { get; set; }

    internal (double longitude, double latitude, float? e) ToLocation()
    {
        return (this.Longitude, this.Latitude, null);
    }
}
