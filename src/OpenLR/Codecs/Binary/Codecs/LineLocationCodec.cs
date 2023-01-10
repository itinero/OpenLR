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

using OpenLR.Codecs.Binary.Data;
using OpenLR.Model;
using OpenLR.Model.Locations;
using System.Collections.Generic;

namespace OpenLR.Codecs.Binary.Codecs;

/// <summary>
/// A decoder that decodes binary data into a line location.
/// </summary>
public static class LineLocationCodec
{
    /// <summary>
    /// Decodes the given data into a location reference.
    /// </summary>
    public static LineLocation Decode(byte[]? data)
    {
        // decode first location reference point.
        var first = new LocationReferencePoint();
        first.Coordinate = CoordinateConverter.Decode(data, 1);
        first.FunctionalRoadClass = FunctionalRoadClassConvertor.Decode(data, 7, 2);
        first.FormOfWay = FormOfWayConvertor.Decode(data, 7, 5);
        first.LowestFunctionalRoadClassToNext = FunctionalRoadClassConvertor.Decode(data, 8, 0);
        first.Bearing = BearingConvertor.DecodeAngleFromBearing(BearingConvertor.Decode(data, 8, 3));
        first.DistanceToNext = DistanceToNextConvertor.Decode(data[9]);

        // calculate the intermediate points count.
        var intermediateList = new List<LocationReferencePoint>();
        int intermediates = (data.Length - 16) / 7;
        int location = 10;
        var reference = first.Coordinate; // the reference for the relative coordinates.
        for(int idx = 0; idx < intermediates; idx++)
        {
            // create an intermediate point.
            var intermediate = new LocationReferencePoint();
            intermediate.Coordinate = CoordinateConverter.DecodeRelative(reference, data, location);
            reference = intermediate.Coordinate;
            location = location + 4;
            intermediate.FunctionalRoadClass = FunctionalRoadClassConvertor.Decode(data, location, 2);
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
        last.Coordinate = CoordinateConverter.DecodeRelative(reference, data, location);
        location = location + 4;
        last.FunctionalRoadClass = FunctionalRoadClassConvertor.Decode(data, location, 2);
        last.FormOfWay = FormOfWayConvertor.Decode(data, location, 5);
        location = location + 1;
        last.Bearing = BearingConvertor.DecodeAngleFromBearing(BearingConvertor.Decode(data, location, 3));
        location = location + 1;                

        // create line location.
        var lineLocation = new LineLocation();
        if (location < data.Length)
        { // if present.
            lineLocation.PositiveOffsetPercentage = OffsetConvertor.Decode(data, location);
            location = location + 1;
        }
        if(location < data.Length)
        { // if present.
            lineLocation.NegativeOffsetPercentage = OffsetConvertor.Decode(data, location);
            location = location + 1;
        }
        lineLocation.First = first;
        lineLocation.Intermediate = intermediateList.ToArray();
        lineLocation.Last = last;
        return lineLocation;
    }

    /// <summary>
    /// Returns true if the given data can be decoded but this decoder.
    /// </summary>
    public static bool CanDecode(byte[] data)
    {
        // decode the header first.
        var header = HeaderConvertor.Decode(data, 0);

        // check header info.
        if (header.ArF1 ||
            header.IsPoint ||
            header.ArF0 ||
            !header.HasAttributes)
        {
            // header is incorrect.
            return false;
        }

        return true;
    }

    /// <summary>
    /// Encodes a point along line location.
    /// </summary>
    public static byte[] Encode(LineLocation location)
    {
        int size = 18;
        // each intermediate adds 7 bytes.
        size = size + (location.Intermediate.Length * 7);
        var data = new byte[size];

        var header = new Header { Version = 3, HasAttributes = true, ArF0 = false, IsPoint = false,
            ArF1 = false
        };
        HeaderConvertor.Encode(data, 0, header);
        CoordinateConverter.Encode(location.First.Coordinate, data, 1);
        FunctionalRoadClassConvertor.Encode(location.First.FunctionalRoadClass.Value, data, 7, 2);
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
                CoordinateConverter.EncodeRelative(reference, intermediate.Coordinate, data, position);
                reference = intermediate.Coordinate;
                position = position + 4;
                FunctionalRoadClassConvertor.Encode(intermediate.FunctionalRoadClass.Value, data, position, 2);
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
        FunctionalRoadClassConvertor.Encode(location.Last.FunctionalRoadClass.Value, data, position + 4, 2);
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
