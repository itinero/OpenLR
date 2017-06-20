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
    /// Represents a form of way convertor that encodes/decodes into the binary OpenLR format.
    /// </summary>
    public static class FormOfWayConvertor
    {
        /// <summary>
        /// Decodes a form of way type from binary data.
        /// </summary>
        /// <param name="data">The binary data.</param>
        /// <param name="byteIndex">The index of the data in the first byte.</param>
        /// <returns></returns>
        public static FormOfWay Decode(byte[] data, int byteIndex)
        {
            return FormOfWayConvertor.Decode(data, 0, byteIndex);
        }

        /// <summary>
        /// Decodes a form of way type from binary data.
        /// </summary>
        /// <param name="data">The binary data.</param>
        /// <param name="startIndex">The index of the byte in data.</param>
        /// <param name="byteIndex">The index of the data in the given byte.</param>
        public static FormOfWay Decode(byte[] data, int startIndex, int byteIndex)
        {
            if (byteIndex > 5) { throw new ArgumentOutOfRangeException("byteIndex", "byteIndex has to be a value in the range of [0-5]."); }

            byte classData = data[startIndex];

            // create mask.
            int mask = 7 << (5 - byteIndex);
            int value = (classData & mask) >> (5 - byteIndex);

            switch(value)
            {
                case 0:
                    return FormOfWay.Undefined;
                case 1:
                    return FormOfWay.Motorway;
                case 2:
                    return FormOfWay.MultipleCarriageWay;
                case 3:
                    return FormOfWay.SingleCarriageWay;
                case 4:
                    return FormOfWay.Roundabout;
                case 5:
                    return FormOfWay.TrafficSquare;
                case 6:
                    return FormOfWay.SlipRoad;
                case 7:
                    return FormOfWay.Other;
            }
            throw new InvalidOperationException("Decoded a value from three bits not in the range of [0-7]?!");
        }

        /// <summary>
        /// Encodes a form of way into binary data.
        /// </summary>
        /// <param name="formOfWay"></param>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="byteIndex"></param>
        public static void Encode(FormOfWay formOfWay, byte[] data, int startIndex, int byteIndex)
        {
            if (byteIndex > 5) { throw new ArgumentOutOfRangeException("byteIndex", "byteIndex has to be a value in the range of [0-5]."); }

            int value = 0;
            switch (formOfWay)
            {
                case FormOfWay.Undefined:
                    value = 0;
                    break;
                case FormOfWay.Motorway:
                    value = 1;
                    break;
                case FormOfWay.MultipleCarriageWay:
                    value = 2;
                    break;
                case FormOfWay.SingleCarriageWay:
                    value = 3;
                    break;
                case FormOfWay.Roundabout:
                    value = 4;
                    break;
                case FormOfWay.TrafficSquare:
                    value = 5;
                    break;
                case FormOfWay.SlipRoad:
                    value = 6;
                    break;
                case FormOfWay.Other:
                    value = 7;
                    break;
            }

            byte target = data[startIndex];

            byte mask = (byte)(7 << (5 - byteIndex));
            target = (byte)(target & ~mask); // set to zero.
            value = (byte)(value << (5 - byteIndex)); // move value to correct position.
            target = (byte)(target | value); // add to byte.

            data[startIndex] = target;
        }
    }
}