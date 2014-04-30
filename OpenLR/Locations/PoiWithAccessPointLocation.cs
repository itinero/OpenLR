using OpenLR.Model;

namespace OpenLR.Locations
{
    /// <summary>
    /// Represents a POI with access point location.
    /// </summary>
    public class PoiWithAccessPointLocation : ILocation
    {
        /// <summary>
        /// Gets or sets the first location reference point.
        /// </summary>
        public LocationReferencePoint First { get; set; }

        /// <summary>
        /// Gets or sets the last location reference point.
        /// </summary>
        public LocationReferencePoint Last { get; set; }

        /// <summary>
        /// Gets or sets the coordinate of the access point (COORD).
        /// </summary>
        public Coordinate Coordinate { get; set; }

        /// <summary>
        /// Gets or sets the positive offset (POFF).
        /// </summary>
        public int? PositiveOffset { get; set; }

        /// <summary>
        /// Gets or sets the side of road information (SOR).
        /// </summary>
        public SideOfRoad? SideOfRoad { get; set; }

        /// <summary>
        /// Gets or sets the orientation (ORI).
        /// </summary>
        public Orientation? Orientation { get; set; }
    }
}