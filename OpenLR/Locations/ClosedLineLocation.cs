using OpenLR.Model;

namespace OpenLR.Locations
{
    /// <summary>
    /// Represents a closed line location or the area defined by a closed path (i.e. a circuit) in the road network.
    /// </summary>
    public class ClosedLineLocation : ILocation
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
    }
}