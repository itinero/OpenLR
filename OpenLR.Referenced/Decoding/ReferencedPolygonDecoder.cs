using OpenLR.Locations;
using OpenLR.Model;
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
    /// Represents a referenced polygon location decoder.
    /// </summary>
    public class ReferencedPolygonDecoder : ReferencedDecoder<ReferencedPolygon, PolygonLocation>
    {
        /// <summary>
        /// Creates a polygon location graph decoder.
        /// </summary>
        /// <param name="mainDecoder"></param>
        /// <param name="rawDecoder"></param>
        public ReferencedPolygonDecoder(ReferencedDecoderBase mainDecoder, OpenLR.Decoding.LocationDecoder<PolygonLocation> rawDecoder)
            : base(mainDecoder, rawDecoder)
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