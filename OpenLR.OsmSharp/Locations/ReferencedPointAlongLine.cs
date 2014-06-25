using OpenLR.Referenced;
using OsmSharp.Routing.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Gets or sets the from vertex.
        /// </summary>
        public long VertexFrom { get; set; }

        /// <summary>
        /// Gets or sets the to vertex.
        /// </summary>
        public long VertexTo { get; set; }

        /// <summary>
        /// Gets or sets the edge.
        /// </summary>
        public TEdge Edge { get; set; }
    }
}
