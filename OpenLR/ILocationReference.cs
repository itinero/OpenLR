using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenLR
{
    /// <summary>
    /// Abstract definition of a location reference in the OpenLR domain.
    /// </summary>
    public interface ILocationReference
    {
        /// <summary>
        /// Gets the version.
        /// </summary>
        int Version { get; }

        /// <summary>
        /// Gets the id.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Checks if this location reference is valid.
        /// </summary>
        /// <returns></returns>
        bool IsValid();
    }
}
