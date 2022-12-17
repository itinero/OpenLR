namespace OpenLR;

/// <summary>
/// Defines general default OpenLR parameters.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Holds the bearing distance parameter.
    /// </summary>
    /// <remarks>This parameter can be changed but decoding/encoding locations might not work anymore when this parameters has been changed in the meantime.</remarks>
    public const int BearingDistance = 20;

    /// <summary>
    /// Holds the tolerance of the distance difference to care about.
    /// </summary>
    public const int DontCareDistance = 100;
}
