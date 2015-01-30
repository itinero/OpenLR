using OpenLR.Locations;
using OpenLR.Referenced;
using System;

namespace OpenLR.Referenced
{
    /// <summary>
    /// Represents a referenced decoding exception.
    /// </summary>
    public class ReferencedDecodingException : Exception
    {
        /// <summary>
        /// Holds the location.
        /// </summary>
        private ILocation _location;

        /// <summary>
        /// Creates a new referenced decoding exception.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ReferencedDecodingException(ILocation location, string message, Exception innerException)
            : base(message, innerException)
        {
            _location = location;
        }
        /// <summary>
        /// Creates a new referenced decoding exception.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        public ReferencedDecodingException(ILocation location, string message)
            : base(message)
        {
            _location = location;
        }

        /// <summary>
        /// Returns the location that was being encoded.
        /// </summary>
        public ILocation Location
        {
            get
            {
                return _location;
            }
        }
    }
}