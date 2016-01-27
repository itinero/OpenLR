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
using OsmSharp.Units.Distance;

namespace OpenLR.Referenced.Decoding
{
    /// <summary>
    /// Represents a dynamic graph decoder: Decodes a raw OpenLR location into a location referenced to a dynamic graph.
    /// </summary>
    public abstract class ReferencedDecoder<TReferencedLocation, TLocation> : ReferencedLocationDecoder<TReferencedLocation, TLocation>
        where TReferencedLocation : ReferencedLocation
        where TLocation : ILocation
    {
        private readonly ReferencedDecoderBase _mainDecoder;

        /// <summary>
        /// Creates a new dynamic graph decoder.
        /// </summary>
        public ReferencedDecoder(ReferencedDecoderBase mainDecoder, OpenLR.Decoding.LocationDecoder<TLocation> rawDecoder)
            : base(rawDecoder)
        {
            _mainDecoder = mainDecoder;
        }

        /// <summary>
        /// Gets the main decoder.
        /// </summary>
        public ReferencedDecoderBase MainDecoder
        {
            get
            {
                return _mainDecoder;
            }
        }

        /// <summary>
        /// Decodes an OpenLR-encoded unreferenced raw OpenLR location into a referenced Location.
        /// </summary>
        /// <returns></returns>
        public override abstract TReferencedLocation Decode(TLocation location);

        /// <summary>
        /// Returns the max vertex distance.
        /// </summary>
        protected Meter MaxVertexDistance
        {
            get
            {
                return _mainDecoder.MaxVertexDistance;
            }
        }
    }
}