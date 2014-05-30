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
    /// Represents a referenced grid location decoder.
    /// </summary>
    /// <typeparam name="TEdge"></typeparam>
    public class ReferencedGridDecoder<TEdge> : ReferencedDecoder<ReferencedGrid, GridLocation, TEdge>
        where TEdge : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Creates a grid location graph decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="router"></param>
        public ReferencedGridDecoder(OpenLR.Decoding.Decoder<GridLocation> rawDecoder, IBasicRouterDataSource<TEdge> graph, IBasicRouter<TEdge> router)
            : base(rawDecoder, graph, router)
        {

        }

        /// <summary>
        /// Decodes an OpenLR-encoded unreferenced raw OpenLR location into a referenced Location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override ReferencedGrid Decode(GridLocation location)
        {
            return new ReferencedGrid()
            {
                LowerLeftLatitude = location.LowerLeft.Latitude,
                LowerLeftLongitude = location.LowerLeft.Longitude,
                UpperRightLatitude = location.UpperRight.Latitude,
                UpperRightLongitude = location.UpperRight.Longitude,
                Rows = location.Rows,
                Columns = location.Columns
            };
        }
    }
}
