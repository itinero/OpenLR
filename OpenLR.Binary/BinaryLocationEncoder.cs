using OpenLR.Locations;
using System;

namespace OpenLR.Binary
{
    /// <summary>
    /// Abstract representation of a binary encoder.
    /// </summary>
    /// <typeparam name="TLocation">An OpenLR unreference location type.</typeparam>
    public abstract class BinaryLocationEncoder<TLocation> : OpenLR.Encoding.LocationEncoder<TLocation>
        where TLocation : ILocation
    {

        /// <summary>
        /// Encode the given location reference into a base64 string.
        /// </summary>
        /// <param name="location">The unreferenced location to encode.</param>
        /// <returns></returns>
        public override string Encode(TLocation location)
        {
            if (location == null) { throw new ArgumentNullException("location"); }

            // decode into a byte array and convert to a base64 string.
            return Convert.ToBase64String(this.EncodeByteArray(location));
        }

        /// <summary>
        /// Encode the given location reference into a byte array.
        /// </summary>
        /// <param name="location">The unreferenced location to encode.</param>
        /// <returns></returns>
        protected abstract byte[] EncodeByteArray(TLocation location);
    }
}