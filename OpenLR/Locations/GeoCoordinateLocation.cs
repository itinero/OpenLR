using OpenLR.Model;

namespace OpenLR.Locations
{
    /// <summary>
    /// Represents a position in a map defined by its longitude and latitude coordinate values.
    /// </summary>
    public class GeoCoordinateLocation : ILocation
    {
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public Coordinate Coordinate { get; set; }
    }
}