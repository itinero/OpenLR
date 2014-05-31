using OpenLR.Decoding;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;

namespace OpenLR.OsmSharp
{
    /// <summary>
    /// Represents an implementation of a referenced live edge decoder.
    /// </summary>
    public class ReferencedLiveEdgeDecoder : ReferencedDecoderBase<LiveEdge>
    {
        /// <summary>
        /// Creates a new referenced live edge decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="locationDecoder"></param>
        public ReferencedLiveEdgeDecoder(IBasicRouterDataSource<LiveEdge> graph, Decoder locationDecoder)
            : base(graph, locationDecoder)
        {

        }

        /// <summary>
        /// Gets a new router.
        /// </summary>
        /// <returns></returns>
        protected override global::OsmSharp.Routing.Graph.Router.IBasicRouter<LiveEdge> GetRouter()
        {
            return new DykstraRoutingLive();
        }
    }
}