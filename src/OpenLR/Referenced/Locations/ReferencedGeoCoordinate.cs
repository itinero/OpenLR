namespace OpenLR.Referenced.Locations;

/// <summary>
/// Represents a referenced geo coordinate location with a graph as a reference.
/// </summary>
/// <remarks>The reference graph play no part here, a geo coordinate is just a geo coordinate.</remarks>
public class ReferencedGeoCoordinate : IReferencedLocation
{
    /// <summary>
    /// Gets or sets the latitude.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Gets or sets the longitude.
    /// </summary>
    public double Longitude { get; set; }
}
