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

using GeoAPI.Geometries;
using Itinero;
using Itinero.Attributes;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OpenLR.Referenced;
using OpenLR.Referenced.Locations;
using System.Collections.Generic;

namespace OpenLR.Geo
{
    /// <summary>
    /// Contains extension methods related to NTS.
    /// </summary>
    public static class NtsExtensions
    {
        /// <summary>
        /// Converts the given coordinate to corresponding GeoAPI coordinate.
        /// </summary>
        public static Coordinate ToGeoAPICoordinate(this Itinero.LocalGeo.Coordinate coordinate)
        {
            return new Coordinate(coordinate.Longitude, coordinate.Latitude);
        }

        /// <summary>
        /// Converts the attribute collection to an NTS attributes table.
        /// </summary>
        public static AttributesTable ToAttributes(this IAttributeCollection attributes)
        {
            var table = new AttributesTable();
            foreach(var att in attributes)
            {
                table.AddAttribute(att.Key, att.Value);
            }
            return table;
        }

        /// <summary>
        /// Converts this line location to features.
        /// </summary>
        public static FeatureCollection ToFeatures(this ReferencedLine line, RouterDb routerDb)
        {
            var featureCollection = new FeatureCollection();

            // build coordinates list.
            var coordinates = new List<Coordinate>();
            for (int idx = 0; idx < line.Edges.Length; idx++)
            {
                var edge = routerDb.Network.GetEdge(line.Edges[idx]);
                coordinates.Add(routerDb.Network.GetVertex(line.Vertices[idx]).ToGeoAPICoordinate());
                if (edge.Shape != null)
                {
                    var shape = edge.Shape;
                    if (line.Edges[idx] < 0)
                    {
                        shape = shape.Reverse();
                    }
                    foreach (var c in shape)
                    {
                        coordinates.Add(c.ToGeoAPICoordinate());
                    }
                }
                coordinates.Add(routerDb.Network.GetVertex(line.Vertices[idx + 1]).ToGeoAPICoordinate());

                var tags = new AttributeCollection();
                tags.AddOrReplace(routerDb.EdgeProfiles.Get(edge.Data.Profile));
                tags.AddOrReplace(routerDb.EdgeMeta.Get(edge.Data.MetaId));
                var table = tags.ToAttributes();
                
                featureCollection.Add(new Feature(new LineString(coordinates.ToArray()), table));
                coordinates.Clear();
            }
            return featureCollection;
        }
    }
}