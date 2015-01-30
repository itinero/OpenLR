using OpenLR.Referenced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.Referenced.Locations
{
    /// <summary>
    /// Represents a referenced rectangle location with a graph as a reference.
    /// </summary>
    /// <remarks>The reference graph play no part here, a rectangle is just a rectangle.</remarks>
    public class ReferencedRectangle : ReferencedLocation
    {
        /// <summary>
        /// Gets or sets the lower left latitude.
        /// </summary>
        public double LowerLeftLatitude { get; set; }

        /// <summary>
        /// Gets or sets the lower left longitude.
        /// </summary>
        public double LowerLeftLongitude { get; set; }

        /// <summary>
        /// Gets or sets the upper right latitude.
        /// </summary>
        public double UpperRightLatitude { get; set; }

        /// <summary>
        /// Gets or sets the upper right longitude.
        /// </summary>
        public double UpperRightLongitude { get; set; }
    }
}