namespace OpenLR.Model;

/// <summary>
/// The form of way (FOW) describes the physical road type of a line.
/// </summary>
public enum FormOfWay
{
    /// <summary>
    /// The physical road type is unknown.
    /// </summary>
    Undefined = 0,
    /// <summary>
    /// A Motorway is defined as a road permitted for motorized vehicles only in combination with a prescribed minimum speed. It has two or more physically separated carriageways and no single level-crossings.
    /// </summary>
    Motorway = 1,
    /// <summary>
    /// A multiple carriageway is defined as a road with physically separated carriageways regardless of the number of lanes. If a road is also a motorway, it should be coded as such and not as a multiple carriageway.
    /// </summary>
    MultipleCarriageWay = 2,
    /// <summary>
    /// All roads without separate carriageways are considered as roads with a single carriageway.
    /// </summary>
    SingleCarriageWay = 3,
    /// <summary>
    /// A Roundabout is a road which forms a ring on which traffic traveling in only one direction is allowed.
    /// </summary>
    Roundabout = 4,
    /// <summary>
    /// A Traffic Square is an open area (partly) enclosed by roads which is used for non-traffic purposes and which is not a Roundabout.
    /// </summary>
    TrafficSquare = 5,
    /// <summary>
    /// A Slip Road is a road especially designed to enter or leave a line.
    /// </summary>
    SlipRoad = 6,
    /// <summary>
    /// Other.
    /// </summary>
    Other = 7
}
