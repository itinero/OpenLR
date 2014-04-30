using OpenLR.Referenced;
using OsmSharp.Routing.Graph;

namespace OpenLR.OsmSharp.Decoding
{
    /// <summary>
    /// Represents a referenced line location with a graph as a reference.
    /// </summary>
    public class LineLocationGraph<TEdge> : ReferencedLocation
        where TEdge : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Gets or sets the vertices.
        /// </summary>
        public long[] Vertices { get; set; }

        /// <summary>
        /// Gets or sets the edges.
        /// </summary>
        public TEdge[] Edges { get; set; }
    }
}