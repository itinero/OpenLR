using OsmSharp.Math.Geo;

namespace OpenLR.Locations
{
    /// <summary>
    /// Represents a position in a map defined by its longitude and latitude coordinate values.
    /// </summary>
    public class GeoCoordinateLocation
    {
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public GeoCoordinate Coordinate { get; set; }
    }
}