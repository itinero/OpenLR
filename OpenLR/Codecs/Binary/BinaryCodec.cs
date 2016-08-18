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
using OpenLR.Codecs.Binary.Decoders;
using OpenLR.Model.Locations;

namespace OpenLR.Codecs.Binary
{
    /// <summary>
    /// A binary codec to encode/decode OpenLR binary format.
    /// </summary>
    public class BinaryCodec : CodecBase
    {
        /// <summary>
        /// Decodes the given string.
        /// </summary>
        public override ILocation Decode(string encoded)
        {
            if (encoded == null) { throw new ArgumentNullException("encoded"); }

            // the data in a binary decoder should be a base64 string.
            byte[] binaryData = null;
            try
            {
                binaryData = Convert.FromBase64String(encoded);
            }
            catch (FormatException ex)
            { // not a base64 string.
                throw ex;
            }

            if (CircleLocationCodec.CanDecode(binaryData))
            {
                return CircleLocationCodec.Decode(binaryData);
            }
            if (ClosedLineLocationCodec.CanDecode(binaryData))
            {
                return ClosedLineLocationCodec.Decode(binaryData);
            }
            if (GeoCoordinateLocationCodec.CanDecode(binaryData))
            {
                return GeoCoordinateLocationCodec.Decode(binaryData);
            }
            if (GridLocationCodec.CanDecode(binaryData))
            {
                return GridLocationCodec.Decode(binaryData);
            }
            if (LineLocationCodec.CanDecode(binaryData))
            {
                return LineLocationCodec.Decode(binaryData);
            }
            if (PointAlongLineLocationCodec.CanDecode(binaryData))
            {
                return PointAlongLineLocationCodec.Decode(binaryData);
            }
            if (PoiWithAccessPointLocationCodec.CanDecode(binaryData))
            {
                return PoiWithAccessPointLocationCodec.Decode(binaryData);
            }
            if (PolygonLocationCodec.CanDecode(binaryData))
            {
                return PolygonLocationCodec.Decode(binaryData);
            }
            if (RectangleLocationCodec.CanDecode(binaryData))
            {
                return RectangleLocationCodec.Decode(binaryData);
            }
            throw new ArgumentException(string.Format("Cannot decode string, no codec found: {0}", encoded));
        }

        /// <summary>
        /// Encodes the given location.
        /// </summary>
        public override string Encode(ILocation location)
        {
            if (location == null) { throw new ArgumentNullException("location"); }

            byte[] encoded = null;
            if (location is LineLocation)
            {
                encoded = LineLocationCodec.Encode(location as LineLocation);
            }
            else if(location is PointAlongLineLocation)
            {
                encoded = PointAlongLineLocationCodec.Encode(location as PointAlongLineLocation);
            }
            else
            {
                throw new ArgumentException("Encoding failed, this type of location is not supported.");
            }

            // decode into a byte array and convert to a base64 string.
            return Convert.ToBase64String(encoded);
        }
    }
}