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
using Itinero.LocalGeo;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace OpenLR.Tests.Functional
{
    public static class Extensions
    {
        public static string ToGeoJson(this FeatureCollection featureCollection)
        {
            var jsonSerializer = new NetTopologySuite.IO.GeoJsonSerializer();
            var jsonStream = new StringWriter();
            jsonSerializer.Serialize(jsonStream, featureCollection);
            var json = jsonStream.ToInvariantString();
            return json;
        }

        public static FeatureCollection FromGeoJson(string geoJson)
        {
            var jsonSerializer = new NetTopologySuite.IO.GeoJsonSerializer();
            var jsonStream = new StringReader(geoJson);
            return jsonSerializer.Deserialize<FeatureCollection>(new JsonTextReader(jsonStream)) as FeatureCollection;
        }
        
        public static FeatureCollection FromGeoJsonFile(string geoJsonFile)
        {
            return FromGeoJson(File.ReadAllText(geoJsonFile));
        }

        public static Tuple<Coordinate, IAttributesTable>[] PointsFromGeoJsonFile(string geoJsonFile)
        {
            var coordinates = new List<Tuple<Coordinate, IAttributesTable>>();
            var features = FromGeoJsonFile(geoJsonFile);
            foreach(var feature in features.Features)
            {
                if (feature.Geometry is Point)
                {
                    var p = feature.Geometry as Point;
                    coordinates.Add(new Tuple<Coordinate, IAttributesTable>(
                        new Coordinate((float)p.Coordinate.Y, (float)p.Coordinate.X),
                        feature.Attributes));
                }
            }
            return coordinates.ToArray();
        }

        /// <summary>
        /// Returns true if the given table contains the given attribute with the given value.
        /// </summary>
        public static bool Contains(this IAttributesTable table, string name, object value)
        {
            var names = table.GetNames();
            for (var i = 0; i < names.Length; i++)
            {
                if (names[i] == name)
                {
                    return value.Equals(table.GetValues()[i]);
                }
            }
            return false;
        }
    }
}