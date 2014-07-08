using OpenLR.Model;
using OpenLR.Referenced;
using OsmSharp.Routing.Graph;

namespace OpenLR.OsmSharp.Locations
{
    /// <summary>
    /// Represents a referenced point along line location with a graph as a reference.
    /// </summary>
    public class ReferencedPointAlongLine<TEdge> : ReferencedLocation
        where TEdge : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the route (vertex->edge->vertex->edge->vertex) associated with this location.
        /// </summary>
        public ReferencedLine<TEdge> Route { get; set; }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        public Orientation Orientation { get; set; }
    }
}
