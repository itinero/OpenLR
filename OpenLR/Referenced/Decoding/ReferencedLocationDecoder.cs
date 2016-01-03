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

using OpenLR.Decoding;
using OpenLR.Locations;

namespace OpenLR.Referenced.Decoding
{
    /// <summary>
    /// Decodes an OpenLR-encoded location into a Location: References an OpenLR location and decodes this into a Location with a depency on the routing network.
    /// </summary>
    public abstract class ReferencedLocationDecoder<TReferencedLocation, TLocation>
        where TReferencedLocation : ReferencedLocation
        where TLocation : ILocation
    {
        private readonly OpenLR.Decoding.LocationDecoder<TLocation> _rawDecoder; // Holds the decoder to decode the raw OpenLR-data.

        /// <summary>
        /// Creates a new referenced location decoder.
        /// </summary>
        public ReferencedLocationDecoder(OpenLR.Decoding.LocationDecoder<TLocation> rawDecoder)
        {
            _rawDecoder = rawDecoder;
        }

        /// <summary>
        /// Returns true if the given data can be decoded using this decoder.
        /// </summary>
        public bool CanDecode(string data)
        {
            return _rawDecoder.CanDecode(data);
        }

        /// <summary>
        /// Decodes an encoded OpenLR location into a raw location.
        /// </summary>
        public TLocation DecodeRaw(string data)
        {
            return _rawDecoder.Decode(data);
        }

        /// <summary>
        /// Decodes an encoded OpenLR location into a referenced Location.
        /// </summary>
        public TReferencedLocation Decode(string data)
        {
            return this.Decode(_rawDecoder.Decode(data));
        }

        /// <summary>
        /// Decodes an unreferenced raw OpenLR location into a referenced Location.
        /// </summary>
        public abstract TReferencedLocation Decode(TLocation location);
    }
}