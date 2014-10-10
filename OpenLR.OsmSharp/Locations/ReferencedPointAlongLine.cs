using OpenLR.Model;
using OpenLR.Referenced;
using OsmSharp.Routing.Graph;
using OsmSharp.Units.Distance;

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

        /// <summary>
        /// Gets or sets the edge meta.
        /// </summary>
        public EdgeMeta EdgeMeta { get; set; }
    }

    /// <summary>
    /// Represents edge meta data.
    /// </summary>
    public struct EdgeMeta
    {
        /// <summary>
        /// Gets or sets the edge idx.
        /// </summary>
        public int Idx { get; set; }

        /// <summary>
        /// Gets or sets the edge idx.
        /// </summary>
        public Meter Offset { get; set; }

        /// <summary>
        /// Gets or sets the edge length.
        /// </summary>
        public Meter Length { get; set; }
    }
}