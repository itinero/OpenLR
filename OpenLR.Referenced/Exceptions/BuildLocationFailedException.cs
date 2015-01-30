using System;

namespace OpenLR.Referenced.Exceptions
{
    /// <summary>
    /// An exception type thrown when 
    /// </summary>
    public class BuildLocationFailedException : Exception
    {
        /// <summary>
        /// Creates a new exception.
        /// </summary>
        /// <param name="message">The message.</param>
        public BuildLocationFailedException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Creates a new exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public BuildLocationFailedException(string message, params object[] args)
            : base(string.Format(message, args))
        {

        }
    }
}
