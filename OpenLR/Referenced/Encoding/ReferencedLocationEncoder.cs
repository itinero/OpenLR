using OpenLR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenLR.Referenced.Encoding
{
    /// <summary>
    /// Encodes an OpenLR location into a string: Encodes an OpenLR with references to a string.
    /// </summary>
    public abstract class ReferencedLocationEncoder<TReferencedLocation, TLocation>
        where TReferencedLocation : ReferencedLocation
        where TLocation : ILocation
    {
        /// <summary>
        /// Holds the encoder to encode OpenLR-data.
        /// </summary>
        private OpenLR.Encoding.LocationEncoder<TLocation> _rawEncoder;

        /// <summary>
        /// Creates a new referenced location encoder.
        /// </summary>
        /// <param name="rawEncoder"></param>
        public ReferencedLocationEncoder(OpenLR.Encoding.LocationEncoder<TLocation> rawEncoder)
        {
            _rawEncoder = rawEncoder;
        }

        /// <summary>
        /// Encodes an OpenLR location.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string Decode(TReferencedLocation location)
        {
            return _rawEncoder.Encode(this.Encode(location));
        }

        /// <summary>
        /// Encodes a referenced raw OpenLR location into an unreferenced Location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public abstract TLocation Encode(TReferencedLocation location);
    }
}