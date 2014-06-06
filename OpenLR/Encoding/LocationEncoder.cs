using OpenLR.Locations;

namespace OpenLR.Encoding
{
    /// <summary>
    /// Abstract representation of an encoder.
    /// </summary>
    public abstract class LocationEncoder<TLocation>
        where TLocation : ILocation
    {
        /// <summary>
        /// Encodes a location reference into a string.
        /// </summary>
        /// <param name="location">The location to encode.</param>
        /// <returns></returns>
        public abstract string Encode(TLocation location);
    }
}