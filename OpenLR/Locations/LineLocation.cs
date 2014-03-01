using OpenLR.Model;

namespace OpenLR.Locations
{
    /// <summary>
    /// Represents a line location.
    /// </summary>
    public class LineLocation : ILocation
    {
        /// <summary>
        /// Gets or sets the first point.
        /// </summary>
        public LocationReferencePoint First { get; set; }

        /// <summary>
        /// Gets or sets the intermediate points.
        /// </summary>
        public LocationReferencePoint[] Intermediate { get; set; }

        /// <summary>
        /// Gets or sets the last point.
        /// </summary>
        public LocationReferencePoint Last { get; set; }
        
        /// <summary>
        /// Gets or sets the positive offset (POFF) is the difference of the start point of the location and the start point of the desired location along the location reference path.
        /// </summary>
        public int? PositiveOffset { get; set; }

        /// <summary>
        /// The negative offset (NOFF) is the difference of the end point of the desired location and the end point of the location reference path.
        /// </summary>
        public int? NegativeOffset { get; set; }
    }
}