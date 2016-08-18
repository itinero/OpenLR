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
    /// Represents an offset flag convertor that encodes/decodes into the binary OpenLR format.
    /// </summary>
    public static class OffsetConvertor
    {
        /// <summary>
        /// Decodes an offset flag from binary data.
        /// </summary>
        /// <param name="data">The binary data.</param>
        /// <param name="startIndex">The index of the byte in data.</param>
        /// <param name="byteIndex">The index of the data in the given byte.</param>
        public static bool DecodeFlag(byte[] data, int startIndex, int byteIndex)
        {
            if (byteIndex > 7) { throw new ArgumentOutOfRangeException("byteIndex", "byteIndex has to be a value in the range of [0-7]."); }

            byte classData = data[startIndex];

            return (classData & (1 << (7 - byteIndex))) != 0;
        }

        /// <summary>
        /// Encodes an offset flag into binary data.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="byteIndex"></param>
        /// <returns></returns>
        public static void EncodeFlag(bool offset, byte[] data, int startIndex, int byteIndex)
        {
            if (byteIndex > 7) { throw new ArgumentOutOfRangeException("byteIndex", "byteIndex has to be a value in the range of [0-7]."); }

            byte mask = (byte)(1 << (7 - byteIndex));
            
            if(offset)
            { // set to 1
                data[startIndex] |= mask;
            }
            else
            { // Set to zero
                data[startIndex] &= (byte)~mask;
            }
        }

        /// <summary>
        /// Encodes the offset in meter as a value relative to the length in meter.
        /// </summary>
        /// <param name="positiveOffsetPercentage"></param>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        public static void Encode(float positiveOffsetPercentage, byte[] data, int startIndex)
        {
            if (positiveOffsetPercentage < 0 || positiveOffsetPercentage >= 100) { throw new ArgumentOutOfRangeException("positiveOffsetPercentage", "The percentage has to be in the range [0-100["); }

            // calculate offset value.
            var offsetValue = (byte)(int)System.Math.Floor(256.0 * (positiveOffsetPercentage / 100));

            // set byte.
            data[startIndex] = offsetValue;
        }

        /// <summary>
        /// Decodes the offset in meter.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static float Decode(byte[] data, int startIndex)
        {
            // get offset value.
            var offsetValue = data[startIndex];

            return (float)(offsetValue / 255.0) * 100.0f;
        }
    }
}