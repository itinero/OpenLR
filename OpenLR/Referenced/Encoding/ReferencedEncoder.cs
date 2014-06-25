using OpenLR.Encoding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenLR.Referenced.Encoding
{
    /// <summary>
    /// Abstract class representing a referenced encoder.
    /// </summary>
    public abstract class ReferencedEncoder
    {
        /// <summary>
        /// Holds the OpenLR unreferenced encoder.
        /// </summary>
        private Encoder _encoder;

        /// <summary>
        /// Creates a new referenced encoder.
        /// </summary>
        /// <param name="encoder"></param>
        public ReferencedEncoder(Encoder encoder)
        {
            _encoder = encoder;
        }

        /// <summary>
        /// Gets the location encoder.
        /// </summary>
        public Encoder LocationEncoder
        {
            get
            {
                return _encoder;
            }
        }

        /// <summary>
        /// Encodes the given referenced location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public abstract string Encode(ReferencedLocation location);
    }
}