using OpenLR.Model;

namespace OpenLR.Locations
{
    /// <summary>
    /// Represents a point along line location.
    /// </summary>
    public class PointAlongLineLocation : ILocation
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
        /// Gets or sets the positive offset percentage value (POFF).
        /// </summary>
        public float? PositiveOffsetPercentage { get; set; }

        /// <summary>
        /// Gets or sets the negative offset percentage value (POFF).
        /// </summary>
        public float? NegativeOffsetPercentage { get; set; }

        /// <summary>
        /// Gets or sets the side of road information (SOR).
        /// </summary>
        public SideOfRoad? SideOfRoad { get; set; }

        /// <summary>
        /// Gets or sets the Orientation (ORI).
        /// </summary>
        public Orientation? Orientation { get; set; }
    }
}