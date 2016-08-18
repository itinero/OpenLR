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

using OpenLR.Model;
using System;

namespace OpenLR.Codecs.Binary.Data
{
    /// <summary>
    /// Represents a orientation convertor that encodes/decodes orientation info into the binary OpenLR format.
    /// </summary>
    public static class OrientationConverter
    {
        /// <summary>
        /// Decodes binary OpenLR orientation data into an orientation.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="byteIndex"></param>
        /// <returns></returns>
        public static Orientation Decode(byte[] data, int byteIndex)
        {
            return OrientationConverter.Decode(data, 0, byteIndex);
        }

        /// <summary>
        /// Decodes binary OpenLR orientation data into an orientation.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="byteIndex"></param>
        /// <returns></returns>
        public static Orientation Decode(byte[] data, int startIndex, int byteIndex)
        {
            if (byteIndex > 6) { throw new ArgumentOutOfRangeException("byteIndex", "byteIndex has to be a value in the range of [0-6]."); }

            byte classData = data[startIndex];

            // create mask.
            int mask = 7 << (6 - byteIndex);
            int value = (classData & mask) >> (6 - byteIndex);

            switch(value)
            {
                case 0:
                    return Orientation.NoOrientation;
                case 1:
                    return Orientation.FirstToSecond;
                case 2:
                    return Orientation.SecondToFirst;
                case 3:
                    return Orientation.BothDirections;
            }
            throw new InvalidOperationException("Decoded a value from three bits not in the range of [0-3]?!");
        }

        /// <summary>
        /// Encodes an OpenLR orientation into a binary representation.
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="byteIndex"></param>
        public static void Encode(Orientation orientation, byte[] data, int startIndex, int byteIndex)
        {
            if (byteIndex > 6) { throw new ArgumentOutOfRangeException("byteIndex", "byteIndex has to be a value in the range of [0-6]."); }

            int value = 0;
            switch (orientation)
            {
                case Orientation.NoOrientation:
                    value = 0;
                    break;
                case Orientation.FirstToSecond:
                    value = 1;
                    break;
                case Orientation.SecondToFirst:
                    value = 2;
                    break;
                case Orientation.BothDirections:
                    value = 3;
                    break;
            }

            byte target = data[startIndex];

            byte mask = (byte)(3 << (6 - byteIndex));
            target = (byte)(target & ~mask); // set to zero.
            value = (byte)(value << (6 - byteIndex)); // move value to correct position.
            target = (byte)(target | value); // add to byte.

            data[startIndex] = target;
        }
    }
}