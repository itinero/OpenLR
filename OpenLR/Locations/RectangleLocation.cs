using OsmSharp.Math.Geo;

namespace OpenLR.Locations
{
    /// <summary>
    /// Represents a rectangle location.
    /// </summary>
    public class RectangleLocation : ILocation
    {
        /// <summary>
        /// Gets or sets the box.
        /// </summary>
        public GeoCoordinateBox Box { get; set; }
    }
}