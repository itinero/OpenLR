using OpenLR.Locations;
using OpenLR.Model;
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
    /// Represents a referenced polygon location decoder.
    /// </summary>
    /// <typeparam name="TEdge"></typeparam>
    public class ReferencedPolygonDecoder<TEdge> : ReferencedDecoder<ReferencedPolygon, PolygonLocation, TEdge>
        where TEdge : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Creates a polygon location graph decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="router"></param>
        public ReferencedPolygonDecoder(OpenLR.Decoding.Decoder<PolygonLocation> rawDecoder, IBasicRouterDataSource<TEdge> graph, IBasicRouter<TEdge> router)
            : base(rawDecoder, graph, router)
        {

        }

        /// <summary>
        /// Decodes an OpenLR-encoded unreferenced raw OpenLR location into a referenced Location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override ReferencedPolygon Decode(PolygonLocation location)
        {
            return new ReferencedPolygon()
            {
                Coordinates = location.Coordinates.Clone() as Coordinate[]
            };
        }
    }
}