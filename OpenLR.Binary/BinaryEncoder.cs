using OpenLR.Binary.Encoders;
using OpenLR.Encoding;
using OpenLR.Locations;

namespace OpenLR.Binary
{
    /// <summary>
    /// An encoder implementation for the OpenLR binary format.
    /// </summary>
    public class BinaryEncoder : Encoder
    {
        /// <summary>
        /// Returns a circle location encoder.
        /// </summary>
        /// <returns></returns>
        public override LocationEncoder<CircleLocation> CreateCircleLocationEncoder()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns a circle location encoder.
        /// </summary>
        /// <returns></returns>
        public override LocationEncoder<ClosedLineLocation> CreateClosedLineLocationEncoder()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns a circle location encoder.
        /// </summary>
        /// <returns></returns>
        public override LocationEncoder<GeoCoordinateLocation> CreateGeoCoordinateLocationEncoder()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns a circle location encoder.
        /// </summary>
        /// <returns></returns>
        public override LocationEncoder<GridLocation> CreateGridLocationEncoder()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns a circle location encoder.
        /// </summary>
        /// <returns></returns>
        public override LocationEncoder<LineLocation> CreateLineLocationEncoder()
        {
            return new LineEncoder();
        }

        /// <summary>
        /// Returns a circle location encoder.
        /// </summary>
        /// <returns></returns>
        public override LocationEncoder<PointAlongLineLocation> CreatePointAlongLineLocationEncoder()
        {
            return new PointAlongLineEncoder();
        }

        /// <summary>
        /// Returns a circle location encoder.
        /// </summary>
        /// <returns></returns>
        public override LocationEncoder<PoiWithAccessPointLocation> CreatePoiWithAccessPointLocationEncoder()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns a circle location encoder.
        /// </summary>
        /// <returns></returns>
        public override LocationEncoder<PolygonLocation> CreatePolygonLocationEncoder()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns a circle location encoder.
        /// </summary>
        /// <returns></returns>
        public override LocationEncoder<RectangleLocation> CreateRectangleLocationEncoder()
        {
            throw new System.NotImplementedException();
        }
    }
}