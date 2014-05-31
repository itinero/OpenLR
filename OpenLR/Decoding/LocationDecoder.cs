using OpenLR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenLR.Decoding
{
    /// <summary>
    /// Abstract representation of a decoder.
    /// </summary>
    public abstract class LocationDecoder<TLocation>
        where TLocation : ILocation
    {
        /// <summary>
        /// Returns true if the given data can be decoded using this decoder.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract bool CanDecode(string data);

        /// <summary>
        /// Decodes a byte array into a location reference.
        /// </summary>
        /// <returns></returns>
        public abstract TLocation Decode(string data);
    }
}