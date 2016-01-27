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
        public override LocationDecoder<CircleLocation> CreateCircleLocationDecoder()
        {
            return new Decoders.CircleLocationDecoder();
        }

        /// <summary>
        /// Returns a closed line location decoder.
        /// </summary>
        public override LocationDecoder<ClosedLineLocation> CreateClosedLineLocationDecoder()
        {
            return new Decoders.ClosedLineLocationDecoder();
        }

        /// <summary>
        /// Returns a geo coordinate location decoder.
        /// </summary>
        public override LocationDecoder<GeoCoordinateLocation> CreateGeoCoordinateLocationDecoder()
        {
            return new Decoders.GeoCoordinateLocationDecoder();
        }

        /// <summary>
        /// Returns a grid location decoder.
        /// </summary>
        public override LocationDecoder<GridLocation> CreateGridLocationDecoder()
        {
            return new Decoders.GridLocationDecoder();
        }

        /// <summary>
        /// Returns a line location decoder.
        /// </summary>
        public override LocationDecoder<LineLocation> CreateLineLocationDecoder()
        {
            return new Decoders.LineLocationDecoder();
        }

        /// <summary>
        /// Returns a point along line location decoder.
        /// </summary>
        public override LocationDecoder<PointAlongLineLocation> CreatePointAlongLineLocationDecoder()
        {
            return new Decoders.PointAlongLineDecoder();
        }

        /// <summary>
        /// Returns a poi with access point location decoder.
        /// </summary>
        public override LocationDecoder<PoiWithAccessPointLocation> CreatePoiWithAccessPointLocationDecoder()
        {
            return new Decoders.PoiWithAccessPointLocationDecoder();
        }

        /// <summary>
        /// Returns a polygon location decoder.
        /// </summary>
        public override LocationDecoder<PolygonLocation> CreatePolygonLocationDecoder()
        {
            return new Decoders.PolygonLocationDecoder();
        }

        /// <summary>
        /// Returns a rectangle location decoder.
        /// </summary>
        public override LocationDecoder<RectangleLocation> CreateRectangleLocationDecoder()
        {
            return new Decoders.RectangleLocationDecoder();
        }
    }
}