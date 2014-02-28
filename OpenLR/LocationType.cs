using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenLR
{
    /// <summary>
    /// Enumerates the different possible location types in the OpenLR domain.
    /// </summary>
    public enum LocationType
    {
        /// <summary>
        /// A circle location.
        /// </summary>
        Circle,
        /// <summary>
        /// A grid location.
        /// </summary>
        Grid,
        /// <summary>
        /// A line location.
        /// </summary>
        LineLocation,
        /// <summary>
        /// A POI with access point location.
        /// </summary>
        PoiWithAccessPoint,
        /// <summary>
        /// A point along line location.
        /// </summary>
        PointAlongLine,
        /// <summary>
        /// A rectangle location.
        /// </summary>
        Rectangle,
        /// <summary>
        /// A polygon location.
        /// </summary>
        Polygon
    }
}
