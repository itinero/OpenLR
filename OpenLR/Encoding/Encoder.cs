using OpenLR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenLR.Encoding
{
    /// <summary>
    /// An abstract base class for a binary/xml encoder.
    /// </summary>
    public abstract class Encoder
    {
        /// <summary>
        /// Returns a circle location encoder.
        /// </summary>
        /// <returns></returns>
        public abstract LocationEncoder<CircleLocation> CreateCircleLocationEncoder();

        /// <summary>
        /// Returns a closed line location encoder.
        /// </summary>
        /// <returns></returns>
        public abstract LocationEncoder<ClosedLineLocation> CreateClosedLineLocationEncoder();

        /// <summary>
        /// Returns a geo coordinate location encoder.
        /// </summary>
        /// <returns></returns>
        public abstract LocationEncoder<GeoCoordinateLocation> CreateGeoCoordinateLocationEncoder();

        /// <summary>
        /// Returns a grid location encoder.
        /// </summary>
        /// <returns></returns>
        public abstract LocationEncoder<GridLocation> CreateGridLocationEncoder();

        /// <summary>
        /// Returns a line location encoder.
        /// </summary>
        /// <returns></returns>
        public abstract LocationEncoder<LineLocation> CreateLineLocationEncoder();

        /// <summary>
        /// Returns a point along line location encoder.
        /// </summary>
        /// <returns></returns>
        public abstract LocationEncoder<PointAlongLineLocation> CreatePointAlongLineLocationEncoder();

        /// <summary>
        /// Returns a poi with access point location encoder.
        /// </summary>
        /// <returns></returns>
        public abstract LocationEncoder<PoiWithAccessPointLocation> CreatePoiWithAccessPointLocationEncoder();

        /// <summary>
        /// Returns a polygon location encoder.
        /// </summary>
        /// <returns></returns>
        public abstract LocationEncoder<PolygonLocation> CreatePolygonLocationEncoder();

        /// <summary>
        /// Returns a rectangle location encoder.
        /// </summary>
        /// <returns></returns>
        public abstract LocationEncoder<RectangleLocation> CreateRectangleLocationEncoder();
    }
}