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
    /// Represents a referenced circle location decoder.
    /// </summary>
    public class ReferencedCircleDecoder : ReferencedDecoder<ReferencedCircle, CircleLocation>
    {
        /// <summary>
        /// Creates a circle location graph decoder.
        /// </summary>
        /// <param name="mainDecoder"></param>
        /// <param name="rawDecoder"></param>
        public ReferencedCircleDecoder(ReferencedDecoderBase mainDecoder, OpenLR.Decoding.LocationDecoder<CircleLocation> rawDecoder)
            : base(mainDecoder, rawDecoder)
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
