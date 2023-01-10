namespace OpenLR.Model;

/// <summary>
/// The side of road information (SOR) describes the relationship between the point of interest and a referenced line.
/// </summary>
public enum SideOfRoad
{
    /// <summary>
    /// 0 - Point is directly on (or above) the road, or determination of right/left side is not applicable (default).
    /// </summary>
    OnOrAbove = 0,
    /// <summary>
    /// 1 - Point is on right side of the road.
    /// </summary>
    Right = 1,
    /// <summary>
    /// 2 - Point is on the left side of the road.
    /// </summary>
    Left = 2,
    /// <summary>
    /// 3 - Point is on both sides of the road.
    /// </summary>
    Both = 3
}
