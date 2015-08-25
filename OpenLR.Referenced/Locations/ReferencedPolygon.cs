using OpenLR.Model;
using OpenLR.Referenced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.Referenced.Locations
{
    /// <summary>
    /// Represents a referenced polygon location with a graph as a reference.
    /// </summary>
    /// <remarks>The reference graph plays no part here, a polygon is just a polygon.</remarks>
    public class ReferencedPolygon : ReferencedLocation
    {
        /// <summary>
        /// Gets or sets the list of coordinates.
        /// </summary>
        public Coordinate[] Coordinates { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            if(this.Coordinates == null)
            {
                return new ReferencedPolygon();
            }
            return new ReferencedPolygon()
            {
                Coordinates = this.Coordinates.Clone() as Coordinate[]
            };
        }
    }
}