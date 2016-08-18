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
using Itinero.Data.Network;
using OpenLR.Referenced;
using OpenLR.Referenced.Locations;
using System.Collections.Generic;

namespace OpenLR
{
    /// <summary>
    /// Contains extension methods for some OpenLR core stuff.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Copies elements from the list and the range into the given array starting at the given index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The array to copy from.</param>
        /// <param name="index">The start of the elements </param>
        /// <param name="count"></param>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The index to start copying to in the array.</param>
        public static void CopyTo<T>(this T[] source, int index, T[] array, int arrayIndex, int count)
        {
            for (int idx = index; idx < index + count; idx++)
            {
                array[arrayIndex] = source[idx];
                arrayIndex++;
            }
        }

        ///// <summary>
        ///// Converts the circle location to features.
        ///// </summary>
        //public static FeatureCollection ToFeatures(this CircleLocation circleLocation)
        //{
        //    // create a point feature.
        //    var point = new Point(new Coordinate(circleLocation.Coordinate.Longitude, circleLocation.Coordinate.Latitude));
        //    var pointAttributes = new AttributesTable();
        //    pointAttributes.AddAttribute("radius", circleLocation.Radius);
        //    var pointFeature = new Feature(point, pointAttributes);

        //    //// create a circle feature.
        //    //var geometricShapeFactory = new GeometricShapeFactory();
        //    //geometricShapeFactory.NumPoints = 32;
        //    //geometricShapeFactory.Centre = point.Coordinate;
        //    //geometricShapeFactory.Size = circleLocation.Radius;
        //    //var circle = geometricShapeFactory.CreateCircle();
        //    //var circleAttributes = new AttributesTable();
        //    //var circleFeature = new Feature(circle, circleAttributes);

        //    // create the feature collection.
        //    var featureCollection = new FeatureCollection();
        //    featureCollection.Add(pointFeature);
        //    //featureCollection.Add(circleFeature);
        //    return featureCollection;
        //}

        ///// <summary>
        ///// Converts the closed line location to features.
        ///// </summary>
        //public static FeatureCollection ToFeatures(this ClosedLineLocation closedLineLocation)
        //{
        //    // create the feature collection.
        //    var featureCollection = new FeatureCollection();

        //    // build the coordinates list and create point features.
        //    var coordinates = new List<Coordinate>();
        //    coordinates.Add(closedLineLocation.First.Coordinate.ToGeoAPICoordinate());
        //    featureCollection.Add(closedLineLocation.First.ToFeature());
        //    if (closedLineLocation.Intermediate != null)
        //    { // there are intermediate coordinates.
        //        for (int idx = 0; idx < closedLineLocation.Intermediate.Length; idx++)
        //        {
        //            coordinates.Add(closedLineLocation.Intermediate[idx].Coordinate.ToGeoAPICoordinate());
        //            featureCollection.Add(closedLineLocation.Intermediate[idx].ToFeature());
        //        }
        //    }
        //    coordinates.Add(closedLineLocation.Last.Coordinate.ToGeoAPICoordinate());
        //    featureCollection.Add(closedLineLocation.Last.ToFeature());

        //    // create a line feature.
        //    var line = new LineString(coordinates.ToArray());
        //    var lineAttributes = new AttributesTable();
        //    var lineFeature = new Feature(line, lineAttributes);
        //    featureCollection.Add(lineFeature);

        //    return featureCollection;
        //}

        ///// <summary>
        ///// Converts the geo coordinate line location to features.
        ///// </summary>
        //public static FeatureCollection ToFeatures(this GeoCoordinateLocation geoCoordinateLocation)
        //{
        //    // create the feature collection.
        //    var featureCollection = new FeatureCollection();

        //    // create a point feature.
        //    var point = new Point(geoCoordinateLocation.Coordinate.ToGeoAPICoordinate());
        //    var pointAttributes = new AttributesTable();
        //    featureCollection.Add(new Feature(point, pointAttributes));
        //    return featureCollection;
        //}

        ///// <summary>
        ///// Converts the grid location to features.
        ///// </summary>
        //public static FeatureCollection ToFeatures(this GridLocation gridLocation)
        //{
        //    // create the feature collection.
        //    var featureCollection = new FeatureCollection();

        //    // create a point feature at each point in the grid.
        //    var lonDiff = (gridLocation.LowerLeft.Longitude - gridLocation.UpperRight.Longitude) / gridLocation.Columns;
        //    var latDiff = (gridLocation.UpperRight.Latitude - gridLocation.LowerLeft.Latitude) / gridLocation.Rows;
        //    for (var column = 0; column < gridLocation.Columns; column++)
        //    {
        //        var longitude = gridLocation.LowerLeft.Longitude - (column * lonDiff);
        //        for (int row = 0; row < gridLocation.Rows; row++)
        //        {
        //            var latitude = gridLocation.UpperRight.Latitude - (row * latDiff);
        //            var point = new Point(new Coordinate(longitude, latitude));
        //            var pointAttributes = new AttributesTable();
        //            featureCollection.Add(new Feature(point, pointAttributes));
        //        }
        //    }

        //    // create a lineair ring.
        //    var lineairRingAttributes = new AttributesTable();
        //    featureCollection.Add(new Feature(new LineString(new Coordinate[] {
        //        new Coordinate(gridLocation.LowerLeft.Longitude, gridLocation.LowerLeft.Latitude),
        //        new Coordinate(gridLocation.UpperRight.Longitude, gridLocation.LowerLeft.Latitude),
        //        new Coordinate(gridLocation.UpperRight.Longitude, gridLocation.UpperRight.Latitude),
        //        new Coordinate(gridLocation.LowerLeft.Longitude, gridLocation.UpperRight.Latitude)
        //    }), lineairRingAttributes));
        //    return featureCollection;
        //}

        ///// <summary>
        ///// Converts the line location to features.
        ///// </summary>
        //public static FeatureCollection ToFeatures(this LineLocation lineLocation)
        //{
        //    // create the feature collection.
        //    var featureCollection = new FeatureCollection();

        //    // build the coordinates list and create point features.
        //    var coordinates = new List<Coordinate>();
        //    coordinates.Add(lineLocation.First.Coordinate.ToGeoAPICoordinate());
        //    featureCollection.Add(lineLocation.First.ToFeature());
        //    if (lineLocation.Intermediate != null)
        //    { // there are intermediate coordinates.
        //        for (int idx = 0; idx < lineLocation.Intermediate.Length; idx++)
        //        {
        //            coordinates.Add(lineLocation.Intermediate[idx].Coordinate.ToGeoAPICoordinate());
        //            featureCollection.Add(lineLocation.Intermediate[idx].ToFeature());
        //        }
        //    }
        //    coordinates.Add(lineLocation.Last.Coordinate.ToGeoAPICoordinate());
        //    featureCollection.Add(lineLocation.Last.ToFeature());

        //    // create a line feature.
        //    var line = new LineString(coordinates.ToArray());
        //    var lineAttributes = new AttributesTable();
        //    lineAttributes.AddAttribute("negative_offset", lineLocation.NegativeOffsetPercentage.HasValue ?
        //        lineLocation.NegativeOffsetPercentage.Value : 0);
        //    lineAttributes.AddAttribute("positive_offset", lineLocation.PositiveOffsetPercentage.HasValue ?
        //        lineLocation.PositiveOffsetPercentage.Value : 0);
        //    var lineFeature = new Feature(line, lineAttributes);
        //    featureCollection.Add(lineFeature);

        //    return featureCollection;
        //}

        ///// <summary>
        ///// Converts the point along the line location to features.
        ///// </summary>
        //public static FeatureCollection ToFeatures(this PointAlongLineLocation pointAlongLineLocation)
        //{
        //    // create the feature collection.
        //    var featureCollection = new FeatureCollection();

        //    // create the coordinates.
        //    var coordinates = new List<Coordinate>();
        //    coordinates.Add(pointAlongLineLocation.First.Coordinate.ToGeoAPICoordinate());
        //    featureCollection.Add(pointAlongLineLocation.First.ToFeature());
        //    coordinates.Add(pointAlongLineLocation.Last.Coordinate.ToGeoAPICoordinate());
        //    featureCollection.Add(pointAlongLineLocation.Last.ToFeature());

        //    // create a line feature.
        //    var line = new LineString(coordinates.ToArray());
        //    var lineAttributes = new AttributesTable();
        //    lineAttributes.AddAttribute("orientation", pointAlongLineLocation.Orientation.ToString());
        //    lineAttributes.AddAttribute("positive_offset_percentage", pointAlongLineLocation.PositiveOffsetPercentage);
        //    lineAttributes.AddAttribute("side_of_road", pointAlongLineLocation.SideOfRoad.ToString());
        //    var lineFeature = new Feature(line, lineAttributes);
        //    featureCollection.Add(lineFeature);

        //    return featureCollection;
        //}

        ///// <summary>
        ///// Converts the referenced point along the line location to features.
        ///// </summary>
        //public static FeatureCollection ToFeatures(this ReferencedPointAlongLine referencedPointALongLineLocation, RouterDb routerDb)
        //{
        //    // create the feature collection.
        //    var featureCollection = referencedPointALongLineLocation.Route.ToFeatures();

        //    // create the coordinates.
        //    var feature = routerDb.Network.GetVertex(referencedPointALongLineLocation.Route.Vertices[0]).ToFeature();
        //    feature.Attributes.AddAttribute("type", "start");
        //    featureCollection.Add(feature);
        //    feature = routerDb.Network.GetVertex(referencedPointALongLineLocation.Route.Vertices[
        //        referencedPointALongLineLocation.Route.Vertices.Length - 1]).ToFeature();
        //    feature.Attributes.AddAttribute("type", "end");
        //    featureCollection.Add(feature);

        //    // create a feature for the actual location.
        //    var locationCoordinate = new Coordinate(referencedPointALongLineLocation.Longitude,
        //        referencedPointALongLineLocation.Latitude);
        //    var locationCoordinateFeature = new Feature(new Point(locationCoordinate),
        //        new AttributesTable());
        //    featureCollection.Add(locationCoordinateFeature);

        //    return featureCollection;
        //}

        /// <summary>
        /// Converts the referenced point along the line location to features.
        /// </summary>
        public static float Length(this ReferencedPointAlongLine referencedPointALongLineLocation, RouterDb routerDb)
        {
            return referencedPointALongLineLocation.Route.Length(routerDb);
        }

        /// <summary>
        /// Converts the referenced point along the line location to features.
        /// </summary>
        public static float Length(this ReferencedLine referencedLine, RouterDb routerDb)
        {
            var length = 0.0f;
            for (int idx = 0; idx < referencedLine.Edges.Length; idx++)
            {
                length = length + routerDb.Network.GetShape(
                    routerDb.Network.GetEdge(referencedLine.Edges[idx])).Length();
            }
            return length;
        }

        /// <summary>
        /// Calculates the length of the shape formed by the given coordinates.
        /// </summary>
        public static float Length(this IEnumerable<Itinero.LocalGeo.Coordinate> enumerable)
        {
            var length = 0.0f;
            Itinero.LocalGeo.Coordinate? previous = null;
            foreach (var c in enumerable)
            {
                if (previous != null)
                {
                    length += Itinero.LocalGeo.Coordinate.DistanceEstimateInMeter(previous.Value, c);
                }
                previous = c;
            }
            return length;
        }
   

        ///// <summary>
        ///// Converts the referenced point along the line location to features.
        ///// </summary>
        ///// <returns></returns>
        //public static FeatureCollection ToFeatures(this ReferencedPointAlongLine referencedPointALongLineLocation, ReferencedDecoderBase baseDecoder)
        //{
        //    // create the feature collection.
        //    var featureCollection = referencedPointALongLineLocation.Route.ToFeatures();

        //    // create the coordinates.
        //    var feature = baseDecoder.RouterDb.Network.GetVertex(referencedPointALongLineLocation.Route.Vertices[0]).ToFeature();
        //    feature.Attributes.AddAttribute("type", "start");
        //    featureCollection.Add(feature);
        //    feature = baseDecoder.RouterDb.Network.GetVertex(referencedPointALongLineLocation.Route.Vertices[
        //        referencedPointALongLineLocation.Route.Vertices.Length - 1]).ToFeature();
        //    feature.Attributes.AddAttribute("type", "end");
        //    featureCollection.Add(feature);

        //    // create a feature for the actual location.
        //    var locationCoordinate = new Coordinate(referencedPointALongLineLocation.Longitude, referencedPointALongLineLocation.Latitude);
        //    var locationCoordinateFeature = new Feature(new Point(locationCoordinate), new AttributesTable());
        //    featureCollection.Add(locationCoordinateFeature);

        //    return featureCollection;
        //}

        ///// <summary>
        ///// Converts the poi with access point location.
        ///// </summary>
        ///// <param name="poiWithAccessPointLocation"></param>
        ///// <returns></returns>
        //public static FeatureCollection ToFeatures(this PoiWithAccessPointLocation poiWithAccessPointLocation)
        //{
        //    // create the geometry factory.
        //    var geometryFactory = new GeometryFactory();

        //    // create the feature collection.
        //    var featureCollection = new FeatureCollection();

        //    // create a point for the poi.
        //    var point = geometryFactory.CreatePoint(new Coordinate(poiWithAccessPointLocation.Coordinate.Longitude, poiWithAccessPointLocation.Coordinate.Latitude));
        //    var pointAttributes = new AttributesTable();
        //    pointAttributes.AddAttribute("orientation", poiWithAccessPointLocation.Orientation);
        //    pointAttributes.AddAttribute("positive_offset", poiWithAccessPointLocation.PositiveOffset);
        //    pointAttributes.AddAttribute("side_of_road", poiWithAccessPointLocation.SideOfRoad);
        //    var pointFeature = new Feature(point, pointAttributes);
        //    featureCollection.Add(pointFeature);
        //    featureCollection.Add(poiWithAccessPointLocation.First.ToFeature());
        //    featureCollection.Add(poiWithAccessPointLocation.Last.ToFeature());
        //    return featureCollection;
        //}

        ///// <summary>
        ///// Converts the polygon location.
        ///// </summary>
        //public static FeatureCollection ToFeatures(this PolygonLocation polygonLocation)
        //{
        //    // create the geometry factory.
        //    var geometryFactory = new GeometryFactory();

        //    // create the feature collection.
        //    var featureCollection = new FeatureCollection();

        //    // build the coordinates list and create point features.
        //    var coordinates = new List<Coordinate>();
        //    if (polygonLocation.Coordinates != null)
        //    { // there are intermediate coordinates.
        //        for (int idx = 0; idx < polygonLocation.Coordinates.Length; idx++)
        //        {
        //            coordinates.Add(new Coordinate(polygonLocation.Coordinates[idx].Longitude, polygonLocation.Coordinates[idx].Latitude));
        //        }
        //    }

        //    // create a line feature.
        //    var line = geometryFactory.CreateLinearRing(coordinates.ToArray());
        //    var lineAttributes = new AttributesTable();
        //    var lineFeature = new Feature(line, lineAttributes);
        //    featureCollection.Add(lineFeature);

        //    return featureCollection;
        //}

        ///// <summary>
        ///// Converts the rectangle location.
        ///// </summary>
        //public static FeatureCollection ToFeatures(this RectangleLocation rectangleLocation)
        //{
        //    // create the feature collection.
        //    var featureCollection = new FeatureCollection();

        //    // create a lineair ring.
        //    var lineairRingAttributes = new AttributesTable();
        //    featureCollection.Add(new Feature(new LinearRing(new Coordinate[] {
        //        new Coordinate(rectangleLocation.LowerLeft.Longitude, rectangleLocation.LowerLeft.Latitude),
        //        new Coordinate(rectangleLocation.UpperRight.Longitude, rectangleLocation.LowerLeft.Latitude),
        //        new Coordinate(rectangleLocation.UpperRight.Longitude, rectangleLocation.UpperRight.Latitude),
        //        new Coordinate(rectangleLocation.LowerLeft.Longitude, rectangleLocation.UpperRight.Latitude)
        //    }), lineairRingAttributes));
        //    return featureCollection;
        //}

        ///// <summary>
        ///// Converts the location reference point to 
        ///// </summary>
        //public static Feature ToFeature(this OpenLR.Model.LocationReferencePoint locationReferencePoint)
        //{
        //    var point = new Point(new Coordinate(locationReferencePoint.Coordinate.Latitude,
        //        locationReferencePoint.Coordinate.Longitude));
        //    var pointAttributes = new AttributesTable();
        //    pointAttributes.AddAttribute("bearing_distance", locationReferencePoint.Bearing);
        //    pointAttributes.AddAttribute("distance_to_next", locationReferencePoint.DistanceToNext);
        //    pointAttributes.AddAttribute("form_of_way", locationReferencePoint.FormOfWay);
        //    pointAttributes.AddAttribute("functional_road_class", locationReferencePoint.FuntionalRoadClass);
        //    pointAttributes.AddAttribute("lowest_functional_road_class_to_next", locationReferencePoint.LowestFunctionalRoadClassToNext);
        //    return new Feature(point, pointAttributes);
        //}

        ///// <summary>
        ///// Converts the given coordinate to a geocoordinate.
        ///// </summary>
        //public static Coordinate ToGeoAPICoordinate(this OpenLR.Model.Coordinate coordinate)
        //{
        //    return new Coordinate(coordinate.Longitude, coordinate.Latitude);
        //}

        ///// <summary>
        ///// Converts the given coordinate to a geocoordinate.
        ///// </summary>
        //public static Coordinate ToGeoAPICoordinate(this Itinero.LocalGeo.Coordinate coordinate)
        //{
        //    return new Coordinate(coordinate.Longitude, coordinate.Latitude);
        //}

        ///// <summary>
        ///// Converts the given coordinate to an NTS feature.
        ///// </summary>
        //public static Feature ToFeature(this OpenLR.Model.Coordinate coordinate)
        //{
        //    return new Feature(new Point(coordinate.ToGeoAPICoordinate()), new AttributesTable());
        //}

        ///// <summary>
        ///// Converts the given coordinate to an NTS feature.
        ///// </summary>
        //public static Feature ToFeature(this Itinero.LocalGeo.Coordinate coordinate)
        //{
        //    return new Feature(new Point(coordinate.ToGeoAPICoordinate()), new AttributesTable());
        //}

        /// <summary>
        /// Substracts the two angles returning an angle in the range -180, +180 
        /// </summary>
        public static float AngleSmallestDifference(float angle1, float angle2)
        {
            var diff = angle1 - angle2;

            if (diff > 180)
            {
                return diff - 360;
            }
            return diff;
        }

        /// <summary>
        /// Converts the given coordinate to and OpenLR coordinate.
        /// </summary>
        public static OpenLR.Model.Coordinate ToCoordinate(this Itinero.LocalGeo.Coordinate coordinate)
        {
            return new OpenLR.Model.Coordinate()
            {
                Latitude  = coordinate.Longitude,
                Longitude = coordinate.Latitude
            };
        }

        ///// <summary>
        ///// Converts the given coordinates to intinero coordinates.
        ///// </summary>
        //public static List<Itinero.LocalGeo.Coordinate> ToItineroCoordinates(this List<Coordinate> coordinates)
        //{
        //    var itineroCoordinates = new List<Itinero.LocalGeo.Coordinate>();
        //    foreach(var coordinate in coordinates)
        //    {
        //        itineroCoordinates.Add(new Itinero.LocalGeo.Coordinate()
        //        {
        //            Latitude = (float)coordinate.Y,
        //            Longitude = (float)coordinate.X
        //        });
        //    }
        //    return itineroCoordinates;
        //}
    }
}