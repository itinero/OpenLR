using OpenLR.Model.Locations;

namespace OpenLR.Codecs;

/// <summary>
/// A base class for a code (XML/Binary/...).
/// </summary>
public abstract class CodecBase
{
    /// <summary>
    /// Encodes the given location.
    /// </summary>
    public abstract string Encode(ILocation location);

    /// <summary>
    /// Decodes the given string.
    /// </summary>
    public abstract ILocation Decode(string encoded);
}
