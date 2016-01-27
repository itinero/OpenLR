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

using OpenLR.Binary.Data;
using OpenLR.Encoding;
using OpenLR.Locations;

namespace OpenLR.Binary.Encoders
{
    /// <summary>
    /// An encoder that decodes binary data into a point along line.
    /// </summary>
    public class PointAlongLineEncoder : BinaryLocationEncoder<PointAlongLineLocation>
    {
        /// <summary>
        /// Encodes a point along line location.
        /// </summary>
        protected override byte[] EncodeByteArray(PointAlongLineLocation location)
        {
            var data = new byte[17];

            var header = new Header();
            header.Version = 3;
            header.HasAttributes = true;
            header.ArF0 = false;
            header.IsPoint = true;
            header.ArF1 = false;
            HeaderConvertor.Encode(data, 0, header);
            CoordinateConverter.Encode(location.First.Coordinate, data, 1);
            FunctionalRoadClassConvertor.Encode(location.First.FuntionalRoadClass.Value, data, 7, 2);
            FormOfWayConvertor.Encode(location.First.FormOfWay.Value, data, 7, 5);
            FunctionalRoadClassConvertor.Encode(location.First.LowestFunctionalRoadClassToNext.Value, data, 8, 0);
            BearingConvertor.Encode(BearingConvertor.EncodeAngleToBearing(location.First.Bearing.Value), data, 8, 3);
            data[9] = DistanceToNextConvertor.Encode(location.First.DistanceToNext);

            CoordinateConverter.EncodeRelative(location.First.Coordinate, location.Last.Coordinate, data, 10);
            FunctionalRoadClassConvertor.Encode(location.Last.FuntionalRoadClass.Value, data, 14, 2);
            FormOfWayConvertor.Encode(location.Last.FormOfWay.Value, data, 14, 5);
            BearingConvertor.Encode(BearingConvertor.EncodeAngleToBearing(location.Last.Bearing.Value), data, 15, 3);

            OrientationConverter.Encode(location.Orientation.Value, data, 7, 0);
            SideOfRoadConverter.Encode(location.SideOfRoad.Value, data, 14, 0);
            if (location.PositiveOffsetPercentage.HasValue)
            { // positive offset percentage is present.
                OffsetConvertor.Encode(location.PositiveOffsetPercentage.Value, data, 16);
            }

            return data;
        }
    }
}