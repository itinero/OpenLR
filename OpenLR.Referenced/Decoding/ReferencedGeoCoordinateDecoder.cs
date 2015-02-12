using OpenLR.Locations;
using OpenLR.Referenced.Locations;
using OpenLR.Referenced.Router;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.Referenced.Decoding
{
    /// <summary>
    /// Represents a referenced geo coordinate location decoder.
    /// </summary>
    /// <typeparam name="TEdge"></typeparam>
    public class ReferencedGeoCoordinateDecoder<TEdge> : ReferencedDecoder<ReferencedGeoCoordinate, GeoCoordinateLocation, TEdge>
        where TEdge : IGraphEdgeData
    {
        /// <summary>
        /// Creates a geo coordinate location referenced decoder.
        /// </summary>
        /// <param name="mainDecoder"></param>
        /// <param name="rawDecoder"></param>
        public ReferencedGeoCoordinateDecoder(ReferencedDecoderBase<TEdge> mainDecoder, OpenLR.Decoding.LocationDecoder<GeoCoordinateLocation> rawDecoder)
            : base(mainDecoder, rawDecoder)
        {

        }

        /// <summary>
        /// Decodes an OpenLR-encoded unreferenced raw OpenLR location into a referenced Location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override ReferencedGeoCoordinate Decode(GeoCoordinateLocation location)
        {
            return new ReferencedGeoCoordinate()
            {
                Latitude = location.Coordinate.Latitude,
                Longitude = location.Coordinate.Longitude
            };
        }
    }
}