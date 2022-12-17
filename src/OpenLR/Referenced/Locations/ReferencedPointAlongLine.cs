using OpenLR.Model;
using OpenLR.Referenced.Scoring;

namespace OpenLR.Referenced.Locations;

/// <summary>
/// Represents a referenced point along line location with a graph as a reference.
/// </summary>
public class ReferencedPointAlongLine : IReferencedLocation
{
    /// <summary>
    /// Gets or sets the latitude.
    /// </summary>
    public float Latitude { get; set; }

    /// <summary>
    /// Gets or sets the longitude.
    /// </summary>
    public float Longitude { get; set; }

    /// <summary>
    /// Gets or sets the route (vertex->edge->vertex->edge->vertex) associated with this location.
    /// </summary>
    public ReferencedLine Route { get; set; }

    /// <summary>
    /// Gets or sets the orientation.
    /// </summary>
    public Orientation Orientation { get; set; }
}
