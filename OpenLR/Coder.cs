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

using Itinero;
using OpenLR.Model.Locations;
using OpenLR.Referenced;
using OpenLR.Referenced.Locations;
using System;

namespace OpenLR
{
    /// <summary>
    /// The OpenLR encoder/decoder.
    /// </summary>
    public class Coder
    {
        private readonly Router _router;
        private readonly Codecs.CodecBase _rawCodec;
        private readonly CoderProfile _profile;

        /// <summary>
        /// Creates a new coder with a default binary codec.
        /// </summary>
        /// <param name="routerDb"></param>
        /// <param name="profile"></param>
        public Coder(RouterDb routerDb, CoderProfile profile)
            : this(routerDb, profile, new Codecs.Binary.BinaryCodec())
        {

        }
        
        /// <summary>
        /// Creates a new coder.
        /// </summary>
        public Coder(RouterDb routerDb, CoderProfile profile, Codecs.CodecBase rawCodec)
        {
            _router = new Router(routerDb);
            _rawCodec = rawCodec;
            _profile = profile;
        }

        /// <summary>
        /// Gets the router.
        /// </summary>
        public Router Router
        {
            get
            {
                return _router;
            }
        }

        /// <summary>
        /// Gets the profile.
        /// </summary>
        public CoderProfile Profile
        {
            get
            {
                return _profile;
            }
        }

        /// <summary>
        /// Encodes a location into an OpenLR string.
        /// </summary>
        public string Encode(ReferencedLocation location)
        {
            if (location is ReferencedCircle)
            {
                return _rawCodec.Encode(Referenced.Codecs.ReferencedCircleCodec.Encode(location as ReferencedCircle));
            }
            if (location is ReferencedGeoCoordinate)
            {
                return _rawCodec.Encode(Referenced.Codecs.ReferencedGeoCoordinateCodec.Encode(location as ReferencedGeoCoordinate));
            }
            if (location is ReferencedGrid)
            {
                return _rawCodec.Encode(Referenced.Codecs.ReferencedGridCodec.Encode(location as ReferencedGrid));
            }
            if (location is ReferencedLine)
            {
                return _rawCodec.Encode(Referenced.Codecs.ReferencedLineCodec.Encode(location as ReferencedLine, this));
            }
            if (location is ReferencedPointAlongLine)
            {
                return _rawCodec.Encode(Referenced.Codecs.ReferencedPointAlongLineCodec.Encode(location as ReferencedPointAlongLine, this));
            }
            if (location is ReferencedPolygon)
            {
                return _rawCodec.Encode(Referenced.Codecs.ReferencedPolygonCodec.Encode(location as ReferencedPolygon));
            }
            if (location is ReferencedRectangle)
            {
                return _rawCodec.Encode(Referenced.Codecs.ReferencedRectangleCodec.Encode(location as ReferencedRectangle));
            }
            throw new ArgumentOutOfRangeException("location", "Unknow location type.");
        }

        /// <summary>
        /// Decoders an OpenLR string into a location.
        /// </summary>
        public ReferencedLocation Decode(string encoded)
        {
            var location = _rawCodec.Decode(encoded);

            if (location is CircleLocation)
            {
                return Referenced.Codecs.ReferencedCircleCodec.Decode(location as CircleLocation);
            }
            if (location is GeoCoordinateLocation)
            {
                return Referenced.Codecs.ReferencedGeoCoordinateCodec.Decode(location as GeoCoordinateLocation);
            }
            if (location is GridLocation)
            {
                return Referenced.Codecs.ReferencedGridCodec.Decode(location as GridLocation);
            }
            if (location is LineLocation)
            {
                return Referenced.Codecs.ReferencedLineCodec.Decode(location as LineLocation, this);
            }
            if (location is PointAlongLineLocation)
            {
                return Referenced.Codecs.ReferencedPointAlongLineCodec.Decode(location as PointAlongLineLocation, this);
            }
            if (location is PolygonLocation)
            {
                return Referenced.Codecs.ReferencedPolygonCodec.Decode(location as PolygonLocation);
            }
            if (location is RectangleLocation)
            {
                return Referenced.Codecs.ReferencedRectangleCodec.Decode(location as RectangleLocation);
            }
            throw new ArgumentOutOfRangeException("encoded", "Unknow encoded string.");
        }
    }
}