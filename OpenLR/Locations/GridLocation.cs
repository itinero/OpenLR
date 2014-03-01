using OsmSharp.Math.Geo;

namespace OpenLR.Locations
{
    /// <summary>
    /// Represents a grid location.
    /// </summary>
    public class GridLocation : ILocation
    {
        /// <summary>
        /// Gets or sets the box.
        /// </summary>
        public GeoCoordinateBox Box { get; set; }

        /// <summary>
        /// Gets or sets the number of rows.
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// Gets or sets the number of columns.
        /// </summary>
        public int Columns { get; set; }
    }
}