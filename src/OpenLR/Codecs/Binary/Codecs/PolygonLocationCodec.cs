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

using System;
using System.Collections.Generic;
using OpenLR.Codecs.Binary.Data;
using OpenLR.Model;
using OpenLR.Model.Locations;

namespace OpenLR.Codecs.Binary.Codecs;

/// <summary>
/// A decoder that decodes binary data into a polygon location.
/// </summary>
public static class PolygonLocationCodec
{
    /// <summary>
    /// Decodes the given data into a location reference.
    /// </summary>
    public static PolygonLocation Decode(byte[] data)
    {
        // just need to decode list of coordinate.
        var coordinates = new List<Coordinate> { CoordinateConverter.Decode(data, 1) };

        // calculate the number of points.
        var previous = coordinates[0];
        const int location = 7;
        int points = 1 + (data.Length - 6) / 4;
        for (int idx = 0; idx < points - 1; idx++)
        {
            coordinates.Add(CoordinateConverter.DecodeRelative(
                coordinates[^1], data, location + (idx * 4)));
        }

        var polygonLocation = new PolygonLocation { Coordinates = coordinates.ToArray() };
        return polygonLocation;
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
            !header.ArF0 ||
            header.HasAttributes)
        {
            // header is incorrect.
            return false;
        }

        int count = (data.Length - 15);
        return count % 4 == 0;
    }
}
