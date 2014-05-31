using OpenLR.Locations;

namespace OpenLR.Decoding
{
    /// <summary>
    /// An abstract base class for a binary/xml decoder.
    /// </summary>
    public abstract class Decoder
    {
        /// <summary>
        /// Returns a circle location decoder.
        /// </summary>
        /// <returns></returns>
        public abstract LocationDecoder<CircleLocation> CreateCircleLocationDecoder();

        /// <summary>
        /// Returns a closed line location decoder.
        /// </summary>
        /// <returns></returns>
        public abstract LocationDecoder<ClosedLineLocation> CreateClosedLineLocationDecoder();

        /// <summary>
        /// Returns a geo coordinate location decoder.
        /// </summary>
        /// <returns></returns>
        public abstract LocationDecoder<GeoCoordinateLocation> CreateGeoCoordinateLocationDecoder();

        /// <summary>
        /// Returns a grid location decoder.
        /// </summary>
        /// <returns></returns>
        public abstract LocationDecoder<GridLocation> CreateGridLocationDecoder();

        /// <summary>
        /// Returns a line location decoder.
        /// </summary>
        /// <returns></returns>
        public abstract LocationDecoder<LineLocation> CreateLineLocationDecoder();

        /// <summary>
        /// Returns a point along line location decoder.
        /// </summary>
        /// <returns></returns>
        public abstract LocationDecoder<PointAlongLineLocation> CreatePointAlongLineLocationDecoder();

        /// <summary>
        /// Returns a poi with access point location decoder.
        /// </summary>
        /// <returns></returns>
        public abstract LocationDecoder<PoiWithAccessPointLocation> CreatePoiWithAccessPointLocationDecoder();

        /// <summary>
        /// Returns a polygon location decoder.
        /// </summary>
        /// <returns></returns>
        public abstract LocationDecoder<PolygonLocation> CreatePolygonLocationDecoder();

        /// <summary>
        /// Returns a rectangle location decoder.
        /// </summary>
        /// <returns></returns>
        public abstract LocationDecoder<RectangleLocation> CreateRectangleLocationDecoder();
    }
}