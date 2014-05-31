using OpenLR.Decoding;
using OpenLR.Locations;

namespace OpenLR.Binary
{
    /// <summary>
    /// A decoder implementation for the OpenLR binary format.
    /// </summary>
    public class BinaryDecoder : OpenLR.Decoding.Decoder
    {
        /// <summary>
        /// Returns a circle location decoder.
        /// </summary>
        /// <returns></returns>
        public override LocationDecoder<CircleLocation> CreateCircleLocationDecoder()
        {
            return new Decoders.CircleLocationDecoder();
        }

        /// <summary>
        /// Returns a closed line location decoder.
        /// </summary>
        /// <returns></returns>
        public override LocationDecoder<ClosedLineLocation> CreateClosedLineLocationDecoder()
        {
            return new Decoders.ClosedLineLocationDecoder();
        }

        /// <summary>
        /// Returns a geo coordinate location decoder.
        /// </summary>
        /// <returns></returns>
        public override LocationDecoder<GeoCoordinateLocation> CreateGeoCoordinateLocationDecoder()
        {
            return new Decoders.GeoCoordinateLocationDecoder();
        }

        /// <summary>
        /// Returns a grid location decoder.
        /// </summary>
        /// <returns></returns>
        public override LocationDecoder<GridLocation> CreateGridLocationDecoder()
        {
            return new Decoders.GridLocationDecoder();
        }

        /// <summary>
        /// Returns a line location decoder.
        /// </summary>
        /// <returns></returns>
        public override LocationDecoder<LineLocation> CreateLineLocationDecoder()
        {
            return new Decoders.LineLocationDecoder();
        }

        /// <summary>
        /// Returns a point along line location decoder.
        /// </summary>
        /// <returns></returns>
        public override LocationDecoder<PointAlongLineLocation> CreatePointAlongLineLocationDecoder()
        {
            return new Decoders.PointAlongLineDecoder();
        }

        /// <summary>
        /// Returns a poi with access point location decoder.
        /// </summary>
        /// <returns></returns>
        public override LocationDecoder<PoiWithAccessPointLocation> CreatePoiWithAccessPointLocationDecoder()
        {
            return new Decoders.PoiWithAccessPointLocationDecoder();
        }

        /// <summary>
        /// Returns a polygon location decoder.
        /// </summary>
        /// <returns></returns>
        public override LocationDecoder<PolygonLocation> CreatePolygonLocationDecoder()
        {
            return new Decoders.PolygonLocationDecoder();
        }

        /// <summary>
        /// Returns a rectangle location decoder.
        /// </summary>
        /// <returns></returns>
        public override LocationDecoder<RectangleLocation> CreateRectangleLocationDecoder()
        {
            return new Decoders.RectangleLocationDecoder();
        }
    }
}