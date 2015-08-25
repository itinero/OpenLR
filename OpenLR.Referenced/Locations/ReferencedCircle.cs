using OpenLR.Referenced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.Referenced.Locations
{
    /// <summary>
    /// Represents a referenced circle location with a graph as a reference.
    /// </summary>
    /// <remarks>The reference graph play no part here, a circle is just a circle.</remarks>
    public class ReferencedCircle : ReferencedLocation
    {
        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        public int Radius { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new ReferencedCircle()
            {
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                Radius = this.Radius
            };
        }
    }
}
