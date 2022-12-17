using OpenLR.Model.Locations;

namespace OpenLR.Codecs;

/// <summary>
/// Abstract representation of a codec for a specific location type.
/// </summary>
public abstract class LocationCodec<TLocation>
    where TLocation : ILocation
{
    /// <summary>
    /// Returns true if the given data can be decoded using this decoder.
    /// </summary>
    public abstract bool CanDecode(string data);

    /// <summary>
    /// Decodes a string into a location reference.
    /// </summary>
    public abstract TLocation Decode(string data);

    /// <summary>
    /// Encodes a location reference into a string.
    /// </summary>
    public abstract string Encode(TLocation location);
}
