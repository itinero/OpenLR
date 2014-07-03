using OpenLR.Referenced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.OsmSharp
{
    /// <summary>
    /// Represents a referenced encoding exception.
    /// </summary>
    public class ReferencedEncodingException : Exception
    {
        /// <summary>
        /// Holds the referenced location.
        /// </summary>
        private ReferencedLocation _referencedLocation;

        /// <summary>
        /// Creates a new referenced encoding exception.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ReferencedEncodingException(ReferencedLocation location, string message, Exception innerException)
            : base(message, innerException)
        {
            _referencedLocation = location;
        }
        /// <summary>
        /// Creates a new referenced encoding exception.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        public ReferencedEncodingException(ReferencedLocation location, string message)
            : base(message)
        {
            _referencedLocation = location;
        }

        /// <summary>
        /// Returns the location that was being encoded.
        /// </summary>
        public ReferencedLocation Location
        {
            get
            {
                return _referencedLocation;
            }
        }
    }
}