namespace OpenLR.Model.Locations;

/// <summary>
/// Represents a grid location.
/// </summary>
public class GridLocation : ILocation
{
    /// <summary>
    /// Gets or sets the box.
    /// </summary>
    public Coordinate LowerLeft { get; set; }

    /// <summary>
    /// Gets or sets the box.
    /// </summary>
    public Coordinate UpperRight { get; set; }

    /// <summary>
    /// Gets or sets the number of rows.
    /// </summary>
    public int Rows { get; set; }

    /// <summary>
    /// Gets or sets the number of columns.
    /// </summary>
    public int Columns { get; set; }
}
