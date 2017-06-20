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

namespace OpenLR.Codecs.Binary.Data
{
    /// <summary>
    /// Represents a distance to next convertor that encodes/decodes coordinates into the binary OpenLR format.
    /// </summary>
    public static class DistanceToNextConvertor
    {
        /// <summary>
        /// Holds the distance per interval for 256 intervals in 15000m 
        /// </summary>
        private const double DISTANCE_PER_INTERVAL = 58.6;

        /// <summary>
        /// Encodes the distance into a byte.
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static byte Encode(int distance)
        {
            if (distance < 0 || distance >= 15000) { throw new ArgumentOutOfRangeException("distance", "Distance cannot be encoded if not in the range [0-15000["); }

            return (byte)System.Math.Floor(distance / DISTANCE_PER_INTERVAL);
        }

        /// <summary>
        /// Decodes the distance from a byte.
        /// </summary>
        /// <param name="distanceByte"></param>
        /// <returns></returns>
        public static int Decode(byte distanceByte)
        {
            return (int)System.Math.Ceiling(distanceByte * DISTANCE_PER_INTERVAL);
        }
    }
}