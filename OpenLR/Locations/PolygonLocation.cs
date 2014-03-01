using OsmSharp.Math.Geo;

namespace OpenLR.Locations
{
    /// <summary>
    /// Represents a polygon location.
    /// </summary>
    public class PolygonLocation : ILocation
    {
        /// <summary>
        /// Gets or sets the list of coordinates.
        /// </summary>
        public GeoCoordinate[] GeoCoordinate { get; set; }
    }
}