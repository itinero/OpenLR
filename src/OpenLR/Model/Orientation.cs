namespace OpenLR.Model;

/// <summary>
/// Enumerates type of orientation.
/// </summary>
public enum Orientation
{
    /// <summary>
    /// 0 - Point has no sense of orientation, or determination of orientation is not applicable (default).
    /// </summary>
    NoOrientation = 0,
    /// <summary>
    /// 1 - Point has orientation from first LRP towards second LRP.
    /// </summary>
    FirstToSecond = 1,
    /// <summary>
    /// 2 - Point has orientation from second LRP towards first LRP.
    /// </summary>
    SecondToFirst = 2,
    /// <summary>
    /// 3  - Point has orientation in both directions.
    /// </summary>
    BothDirections = 3
}

/// <summary>
/// Extension methods to the orientation enumeration.
/// </summary>
public static class OrientationExtensions
{
    /// <summary>
    /// Decodes the orientation into a oneway boolean.
    /// </summary>
    public static bool? DecodeOneway(this Orientation orientation)
    {
        return orientation switch
        {
            Orientation.FirstToSecond => true,
            Orientation.SecondToFirst => false,
            _ => null
        };
    }

    /// <summary>
    /// Encodes a oneway flag into an orientation.
    /// </summary>
    public static Orientation EncodeOneway(this bool? oneway)
    {
        if (!oneway.HasValue)
        {
            return Orientation.NoOrientation;
        }

        if (oneway.Value)
        {
            return Orientation.FirstToSecond;
        }
        return Orientation.SecondToFirst;
    }
}
