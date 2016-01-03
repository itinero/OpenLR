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

using OpenLR.Locations;
using System;

namespace OpenLR.Binary
{
    /// <summary>
    /// Abstract representation of a binary decoder.
    /// </summary>
    public abstract class BinaryLocationDecoder<TLocation> : OpenLR.Decoding.LocationDecoder<TLocation>
        where TLocation : ILocation
    {
        /// <summary>
        /// Decodes the given data into a location reference.
        /// </summary>
        public override TLocation Decode(string data)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            // the data in a binary decoder should be a base64 string.
            byte[] binaryData = null;
            try
            {
                binaryData = Convert.FromBase64String(data);
            }
            catch(FormatException ex)
            { // not a base64 string.
                throw ex;
            }

            // we have binary data now.
            return this.Decode(binaryData);
        }

        /// <summary>
        /// Decodes the given data into a location reference.
        /// </summary>
        protected abstract TLocation Decode(byte[] data);

        /// <summary>
        /// Returns true if the given data can be decoded using this decoder.
        /// </summary>
        public override bool CanDecode(string data)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            // the data in a binary decoder should be a base64 string.
            byte[] binaryData = null;
            try
            {
                binaryData = Convert.FromBase64String(data);
            }
            catch (FormatException ex)
            { // not a base64 string.
                throw ex;
            }

            // we have binary data now.
            return this.CanDecode(binaryData);
        }

        /// <summary>
        /// Returns true if the given data can be decoded using this decoder.
        /// </summary>
        protected abstract bool CanDecode(byte[] data);
    }
}