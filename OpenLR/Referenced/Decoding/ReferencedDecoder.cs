using OpenLR.Decoding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenLR.Referenced.Decoding
{
    /// <summary>
    /// Abstract class representing a decoder.
    /// </summary>
    public abstract class ReferencedDecoder
    {
        /// <summary>
        /// Holds the OpenLR unreferenced location decoder.
        /// </summary>
        private Decoder _locationDecoder;

        /// <summary>
        /// Creates a new referenced decoder.
        /// </summary>
        /// <param name="locationDecoder"></param>
        public ReferencedDecoder(Decoder locationDecoder)
        {
            _locationDecoder = locationDecoder;
        }

        /// <summary>
        /// Gets the location decoder.
        /// </summary>
        public Decoder LocationDecoder
        {
            get
            {
                return _locationDecoder;
            }
        }

        /// <summary>
        /// Decodes a the given data into a referenced location.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract ReferencedLocation Decode(string data);
    }
}