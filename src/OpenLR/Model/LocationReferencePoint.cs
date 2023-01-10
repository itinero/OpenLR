﻿namespace OpenLR.Model;

/// <summary>
/// Represents a point of the location which holds relevant information for a map-independent location reference.
/// </summary>
public class LocationReferencePoint
{
    /// <summary>
    /// Gets location.
    /// </summary>
    public Coordinate Coordinate { get; set; }

    /// <summary>
    /// Gets or sets the bearing.
    /// </summary>
    /// <remarks>Bearing is an angle in the range [0, 360[.</remarks>
    public int? Bearing { get; set; }

    /// <summary>
    /// Gets or sets the functional road class.
    /// </summary>
    public FunctionalRoadClass? FunctionalRoadClass { get; set; }

    /// <summary>
    /// Gets or sets the form of way.
    /// </summary>
    public FormOfWay? FormOfWay { get; set; }

    /// <summary>
    /// Gets or sets the lowest functional road class to the next point.
    /// </summary>
    public FunctionalRoadClass? LowestFunctionalRoadClassToNext { get;set; }

    /// <summary>
    /// Gets or sets the distance to the next point in meters.
    /// </summary>
    public int DistanceToNext { get; set; }
}
