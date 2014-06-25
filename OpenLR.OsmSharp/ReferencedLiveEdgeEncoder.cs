using OpenLR.Encoding;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;

namespace OpenLR.OsmSharp
{
    /// <summary>
    /// Represents an implementation of a referenced live edge encoder. 
    /// </summary>
    public class ReferencedLiveEdgeEncoder : ReferencedEncoderBase<LiveEdge>
    {
        /// <summary>
        /// Creates a new referenced live edge decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="locationEncoder"></param>
        public ReferencedLiveEdgeEncoder(IBasicRouterDataSource<LiveEdge> graph, Encoder locationEncoder)
            : base(graph, locationEncoder)
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