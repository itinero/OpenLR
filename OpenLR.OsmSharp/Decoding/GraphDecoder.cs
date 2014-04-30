using OpenLR.Locations;
using OpenLR.Referenced;
using OpenLR.Referenced.Decoding;
using OsmSharp.Routing.Graph;

namespace OpenLR.OsmSharp.Decoding
{
    /// <summary>
    /// Represents a dynamic graph decoder: Decodes a raw OpenLR location into a location referenced to a dynamic graph.
    /// </summary>
    public abstract class GraphDecoder<TReferencedLocation, TLocation, TEdge> : ReferencedDecoder<TReferencedLocation, TLocation>
        where TEdge : IDynamicGraphEdgeData
        where TReferencedLocation : ReferencedLocation
        where TLocation : ILocation
    {
        /// <summary>
        /// Holds a dynamic graph.
        /// </summary>
        private DynamicGraphRouterDataSource<TEdge> _graph;

        /// <summary>
        /// Creates a new dynamic graph decoder.
        /// </summary>
        /// <param name="graph"></param>
        public GraphDecoder(OpenLR.Decoding.Decoder rawDecoder, DynamicGraphRouterDataSource<TEdge> graph)
            : base(rawDecoder)
        {
            _graph = graph;
        }

        /// <summary>
        /// Gets the graph.
        /// </summary>
        public DynamicGraphRouterDataSource<TEdge> Graph
        {
            get
            {
                return _graph;
            }
        }

        /// <summary>
        /// Decodes an OpenLR-encoded unreferenced raw OpenLR location into a referenced Location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override abstract TReferencedLocation Decode(TLocation location);
    }
}