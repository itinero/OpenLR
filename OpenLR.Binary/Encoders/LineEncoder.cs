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
    /// An encoder that decodes binary data into a line.
    /// </summary>
    public class LineEncoder : BinaryLocationEncoder<LineLocation>
    {
        /// <summary>
        /// Encodes a point along line location.
        /// </summary>
        protected override byte[] EncodeByteArray(LineLocation location)
        {
            int size = 18;
            if (location.Intermediate != null)
            { // each intermediate adds 7 bytes.
                size = size + (location.Intermediate.Length * 7);
            }
            byte[] data = new byte[size];

            var header = new Header();
            header.Version = 3;
            header.HasAttributes = true;
            header.ArF0 = false;
            header.IsPoint = false;
            header.ArF1 = false;
            HeaderConvertor.Encode(data, 0, header);
            CoordinateConverter.Encode(location.First.Coordinate, data, 1);
            FunctionalRoadClassConvertor.Encode(location.First.FuntionalRoadClass.Value, data, 7, 2);
            FormOfWayConvertor.Encode(location.First.FormOfWay.Value, data, 7, 5);
            FunctionalRoadClassConvertor.Encode(location.First.LowestFunctionalRoadClassToNext.Value, data, 8, 0);
            BearingConvertor.Encode(BearingConvertor.EncodeAngleToBearing(location.First.Bearing.Value), data, 8, 3);
            data[9] = DistanceToNextConvertor.Encode(location.First.DistanceToNext);

            // calculate the intermediate points count.
            var position = 10;
            var reference = location.First.Coordinate;
            if (location.Intermediate != null)
            {
                for (int idx = 0; idx < location.Intermediate.Length; idx++)
                { // create an intermediate point.
                    var intermediate = location.Intermediate[idx];
                    CoordinateConverter.EncodeRelative(location.First.Coordinate, intermediate.Coordinate, data, position);
                    reference = intermediate.Coordinate;
                    position = position + 4;
                    FunctionalRoadClassConvertor.Encode(intermediate.FuntionalRoadClass.Value, data, position, 2);
                    FormOfWayConvertor.Encode(intermediate.FormOfWay.Value, data, position, 5);
                    position = position + 1;
                    BearingConvertor.Encode(BearingConvertor.EncodeAngleToBearing(intermediate.Bearing.Value), data, position, 3);
                    FunctionalRoadClassConvertor.Encode(intermediate.LowestFunctionalRoadClassToNext.Value, data, position, 0);
                    position = position + 1;
                    data[position] = DistanceToNextConvertor.Encode(intermediate.DistanceToNext);
                    position = position + 1;
                }
            }

            CoordinateConverter.EncodeRelative(reference, location.Last.Coordinate, data, position);
            FunctionalRoadClassConvertor.Encode(location.Last.FuntionalRoadClass.Value, data, position + 4, 2);
            FormOfWayConvertor.Encode(location.Last.FormOfWay.Value, data, position + 4, 5);
            BearingConvertor.Encode(BearingConvertor.EncodeAngleToBearing(location.Last.Bearing.Value), data, position + 5, 3);

            if (location.PositiveOffsetPercentage.HasValue)
            { // positive offset percentage is present.
                OffsetConvertor.EncodeFlag(true, data, position + 5, 1);
                OffsetConvertor.Encode(location.PositiveOffsetPercentage.Value, data, position + 6);
            }
            if (location.NegativeOffsetPercentage.HasValue)
            { // positive offset percentage is present.
                OffsetConvertor.EncodeFlag(true, data, position + 5, 2);
                OffsetConvertor.Encode(location.NegativeOffsetPercentage.Value, data, position + 7);
            }

            return data;
        }
    }
}