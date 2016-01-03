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
using OpenLR.Locations;
using OpenLR.Model;
using System.Collections.Generic;

namespace OpenLR.Binary.Decoders
{
    /// <summary>
    /// A decoder that decodes binary data into a circle location.
    /// </summary>
    public class ClosedLineLocationDecoder : BinaryLocationDecoder<ClosedLineLocation>
    {
        /// <summary>
        /// Decodes the given data into a location reference.
        /// </summary>
        protected override ClosedLineLocation Decode(byte[] data)
        {
            // decode first location reference point.
            var first = new LocationReferencePoint();
            first.Coordinate = CoordinateConverter.Decode(data, 1);
            first.FuntionalRoadClass = FunctionalRoadClassConvertor.Decode(data, 7, 2);
            first.FormOfWay = FormOfWayConvertor.Decode(data, 7, 5);
            first.LowestFunctionalRoadClassToNext = FunctionalRoadClassConvertor.Decode(data, 8, 0);
            first.Bearing = BearingConvertor.DecodeAngleFromBearing(BearingConvertor.Decode(data, 8, 3));
            first.DistanceToNext = DistanceToNextConvertor.Decode(data[9]);

            // calculate the intermediate points count.
            var intermediateList = new List<LocationReferencePoint>();
            int intermediates = (data.Length - 12) / 7;
            int location = 10;
            var reference = first.Coordinate; // the reference for the relative coordinates.
            for (int idx = 0; idx < intermediates; idx++)
            {
                // create an intermediate point.
                var intermediate = new LocationReferencePoint();
                intermediate.Coordinate = CoordinateConverter.DecodeRelative(reference, data, location);
                reference = intermediate.Coordinate;
                location = location + 4;
                intermediate.FuntionalRoadClass = FunctionalRoadClassConvertor.Decode(data, location, 2);
                intermediate.FormOfWay = FormOfWayConvertor.Decode(data, location, 5);
                location = location + 1;
                intermediate.Bearing = BearingConvertor.DecodeAngleFromBearing(BearingConvertor.Decode(data, location, 3));
                intermediate.LowestFunctionalRoadClassToNext = FunctionalRoadClassConvertor.Decode(data, location, 0);
                location = location + 1;
                intermediate.DistanceToNext = DistanceToNextConvertor.Decode(data[location]);
                location = location + 1;

                intermediateList.Add(intermediate);
            }

            // decode last location reference point.
            var last = new LocationReferencePoint();
            // no last coordinates, identical to the first.
            last.Coordinate = first.Coordinate;
            last.FuntionalRoadClass = FunctionalRoadClassConvertor.Decode(data, location, 2);
            last.FormOfWay = FormOfWayConvertor.Decode(data, location, 5);
            location = location + 1;
            last.LowestFunctionalRoadClassToNext = FunctionalRoadClassConvertor.Decode(data, location, 0);
            last.Bearing = BearingConvertor.DecodeAngleFromBearing(BearingConvertor.Decode(data, location, 3));
            location = location + 1;

            // create line location.
            var lineLocation = new ClosedLineLocation();
            lineLocation.First = first;
            lineLocation.Intermediate = intermediateList.ToArray();
            lineLocation.Last = last;
            return lineLocation;
        }

        /// <summary>
        /// Returns true if the given data can be decoded by this decoder.
        /// </summary>
        protected override bool CanDecode(byte[] data)
        {
            // decode the header first.
            var header = HeaderConvertor.Decode(data, 0);

            // check header info.
            if (!header.ArF1 ||
                header.IsPoint ||
                !header.ArF0 ||
                !header.HasAttributes)
            { // header is incorrect.
                return false;
            }
            return true;
        }
    }
}