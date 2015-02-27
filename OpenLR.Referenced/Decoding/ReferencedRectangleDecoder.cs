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
    /// Represents a referenced rectangle location decoder.
    /// </summary>
    public class ReferencedRectangleDecoder : ReferencedDecoder<ReferencedRectangle, RectangleLocation>
    {
        /// <summary>
        /// Creates a rectangle location graph decoder.
        /// </summary>
        /// <param name="mainDecoder"></param>
        /// <param name="rawDecoder"></param>
        public ReferencedRectangleDecoder(ReferencedDecoderBase mainDecoder, OpenLR.Decoding.LocationDecoder<RectangleLocation> rawDecoder)
            : base(mainDecoder, rawDecoder)
        {

        }

        /// <summary>
        /// Decodes an OpenLR-encoded unreferenced raw OpenLR location into a referenced Location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override ReferencedRectangle Decode(RectangleLocation location)
        {
            return new ReferencedRectangle()
            {
                LowerLeftLatitude = location.LowerLeft.Latitude,
                LowerLeftLongitude = location.LowerLeft.Longitude,
                UpperRightLatitude = location.UpperRight.Latitude,
                UpperRightLongitude = location.UpperRight.Longitude
            };
        }
    }
}