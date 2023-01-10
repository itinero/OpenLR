namespace OpenLR.Model.Locations;

/// <summary>
/// Represents a line location.
/// </summary>
public class LineLocation : ILocation
{
    /// <summary>
    /// Gets or sets the first point.
    /// </summary>
    public LocationReferencePoint First { get; set; }

    /// <summary>
    /// Gets or sets the intermediate points.
    /// </summary>
    public LocationReferencePoint[] Intermediate { get; set; } = System.Array.Empty<LocationReferencePoint>();

    /// <summary>
    /// Gets or sets the last point.
    /// </summary>
    public LocationReferencePoint Last { get; set; }

    /// <summary>
    /// Gets or sets the positive offset percentage value (POFF).
    /// </summary>
    public double? PositiveOffsetPercentage { get; set; }

    /// <summary>
    /// Gets or sets the negative offset percentage value (POFF).
    /// </summary>
    public double? NegativeOffsetPercentage { get;set; }
}
