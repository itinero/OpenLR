﻿namespace OpenLR.Referenced.Locations;

/// <summary>
/// Represents a referenced grid location with a graph as a reference.
/// </summary>
/// <remarks>The reference graph play no part here, a grid is just a grid.</remarks>
public class ReferencedGrid : IReferencedLocation
{
    /// <summary>
    /// Gets or sets the lower left latitude.
    /// </summary>
    public double LowerLeftLatitude { get; set; }

    /// <summary>
    /// Gets or sets the lower left longitude.
    /// </summary>
    public double LowerLeftLongitude { get; set; }

    /// <summary>
    /// Gets or sets the upper right latitude.
    /// </summary>
    public double UpperRightLatitude { get; set; }

    /// <summary>
    /// Gets or sets the upper right longitude.
    /// </summary>
    public double UpperRightLongitude { get; set; }

    /// <summary>
    /// Gets or sets the number of rows.
    /// </summary>
    public int Rows { get; set; }

    /// <summary>
    /// Gets or sets the number of columns.
    /// </summary>
    public int Columns { get; set; }
}
