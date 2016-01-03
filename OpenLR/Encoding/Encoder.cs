// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

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
        public abstract LocationEncoder<CircleLocation> CreateCircleLocationEncoder();

        /// <summary>
        /// Returns a closed line location encoder.
        /// </summary>
        public abstract LocationEncoder<ClosedLineLocation> CreateClosedLineLocationEncoder();

        /// <summary>
        /// Returns a geo coordinate location encoder.
        /// </summary>
        public abstract LocationEncoder<GeoCoordinateLocation> CreateGeoCoordinateLocationEncoder();

        /// <summary>
        /// Returns a grid location encoder.
        /// </summary>
        public abstract LocationEncoder<GridLocation> CreateGridLocationEncoder();

        /// <summary>
        /// Returns a line location encoder.
        /// </summary>
        public abstract LocationEncoder<LineLocation> CreateLineLocationEncoder();

        /// <summary>
        /// Returns a point along line location encoder.
        /// </summary>
        public abstract LocationEncoder<PointAlongLineLocation> CreatePointAlongLineLocationEncoder();

        /// <summary>
        /// Returns a poi with access point location encoder.
        /// </summary>
        public abstract LocationEncoder<PoiWithAccessPointLocation> CreatePoiWithAccessPointLocationEncoder();

        /// <summary>
        /// Returns a polygon location encoder.
        /// </summary>
        public abstract LocationEncoder<PolygonLocation> CreatePolygonLocationEncoder();

        /// <summary>
        /// Returns a rectangle location encoder.
        /// </summary>
        public abstract LocationEncoder<RectangleLocation> CreateRectangleLocationEncoder();
    }
}