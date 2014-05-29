using OpenLR.Locations;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.OsmSharp.Decoding
{
    /// <summary>
    /// Represents a referenced circle location decoder.
    /// </summary>
    /// <typeparam name="TEdge"></typeparam>
    public class ReferencedCircleDecoder<TEdge> : ReferencedDecoder<ReferencedCircle, CircleLocation, TEdge>
        where TEdge : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Creates a circle location graph decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="router"></param>
        public ReferencedCircleDecoder(OpenLR.Decoding.Decoder<CircleLocation> rawDecoder, DynamicGraphRouterDataSource<TEdge> graph, IBasicRouter<TEdge> router)
            : base(rawDecoder, graph, router)
        {

        }

        /// <summary>
        /// Decodes an OpenLR-encoded unreferenced raw OpenLR location into a referenced Location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override ReferencedCircle Decode(CircleLocation location)
        {
            return new ReferencedCircle()
            {
                Latitude = location.Coordinate.Latitude,
                Longitude = location.Coordinate.Longitude,
                Radius = location.Radius
            };
        }
    }
}
