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
using Itinero.Data.Network;
using Itinero.Geo;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OpenLR.Referenced;
using OpenLR.Referenced.Codecs.Candidates;
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
            var coordinates = new List<Itinero.LocalGeo.Coordinate>();
            for (var i = 0; i < line.Edges.Length; i++)
            {
                var edge = routerDb.Network.GetEdge(line.Edges[i]);

                List<Itinero.LocalGeo.Coordinate> shape = null;
                if (i == 0 && line.Vertices[0] == Itinero.Constants.NO_VERTEX)
                { // shape from startlocation -> vertex1.
                    if (line.Edges.Length == 1)
                    { // only 1 edge, shape from startLocation -> endLocation.
                        shape = line.StartLocation.ShapePointsTo(routerDb, line.EndLocation);
                        shape.Insert(0, line.StartLocation.LocationOnNetwork(routerDb));
                        shape.Add(line.EndLocation.LocationOnNetwork(routerDb));
                    }
                    else
                    { // just get shape to first vertex.
                        shape = line.StartLocation.ShapePointsTo(routerDb, line.Vertices[1]);
                        shape.Insert(0, line.StartLocation.LocationOnNetwork(routerDb));
                        shape.Add(routerDb.Network.GetVertex(line.Vertices[1]));
                    }
                }
                else if (i == line.Edges.Length - 1 && line.Vertices[line.Vertices.Length - 1] == Itinero.Constants.NO_VERTEX)
                { // shape from second last vertex -> endlocation.
                    shape = line.StartLocation.ShapePointsTo(routerDb, line.Vertices[line.Vertices.Length - 1]);
                    shape.Reverse();
                    shape.Insert(0, routerDb.Network.GetVertex(line.Vertices[line.Vertices.Length - 1]));
                    shape.Add(line.EndLocation.LocationOnNetwork(routerDb));
                }
                else
                { // regular 2 vertices and edge.
                    shape = routerDb.Network.GetShape(routerDb.Network.GetEdge(line.Edges[i]));
                    if (line.Edges[i] < 0)
                    {
                        shape.Reverse();
                    }
                }
                if (shape != null)
                {
                    if (coordinates.Count > 0)
                    {
                        coordinates.RemoveAt(coordinates.Count - 1);
                    }
                    for (var j = 0; j < shape.Count; j++)
                    {
                        coordinates.Add(shape[j]);
                    }
                }

                var tags = new AttributeCollection();
                tags.AddOrReplace(routerDb.EdgeProfiles.Get(edge.Data.Profile));
                tags.AddOrReplace(routerDb.EdgeMeta.Get(edge.Data.MetaId));
                tags.AddOrReplace("edge_id", edge.IdDirected().ToInvariantString());
                var table = tags.ToAttributes();

                featureCollection.Add(new Feature(new LineString(shape.ToCoordinates().ToArray()), table));
                coordinates.Clear();
            }

            var positiveLocation = line.GetPositiveOffsetLocation(routerDb).ToGeoAPICoordinate();
            featureCollection.Add(new Feature(new Point(positiveLocation), (new AttributeCollection(new Attribute("type", "positive_offset_location"))).ToAttributes()));
            var negativeLocation = line.GetNegativeOffsetLocation(routerDb).ToGeoAPICoordinate();
            featureCollection.Add(new Feature(new Point(negativeLocation), (new AttributeCollection(new Attribute("type", "negative_offset_location"))).ToAttributes()));

            return featureCollection;
        }

        /// <summary>
        /// Converts this line location to a line string.
        /// </summary>
        public static LineString ToLineString(this ReferencedLine line, RouterDb routerDb)
        {
            // build coordinates list.
            var coordinates = new List<Coordinate>();
            for (int idx = 0; idx < line.Edges.Length; idx++)
            {
                var shape = routerDb.Network.GetShape(
                    routerDb.Network.GetEdge(line.Edges[idx]));
                if (idx > 0)
                {
                    coordinates.RemoveAt(coordinates.Count - 1);
                }
                if (line.Edges[idx] > 0)
                {
                    coordinates.AddRange(shape.ToCoordinates());
                }
                else
                {
                    for(var i = shape.Count - 1; i >= 0; i--)
                    {
                        coordinates.Add(shape[i].ToGeoAPICoordinate());
                    }
                }
            }
            
            return new LineString(coordinates.ToArray());
        }
        
        /// <summary>
        /// Converts this line location to features.
        /// </summary>
        public static FeatureCollection ToFeatures(this ReferencedPointAlongLine line, RouterDb routerDb)
        {
            var features = line.Route.ToFeatures(routerDb);

            features.Add(new Feature(new Point(new Coordinate(line.Longitude, line.Latitude)),
                new AttributesTable()));

            return features;
        }
    }
}