//// The MIT License (MIT)

//// Copyright (c) 2016 Ben Abelshausen

//// Permission is hereby granted, free of charge, to any person obtaining a copy
//// of this software and associated documentation files (the "Software"), to deal
//// in the Software without restriction, including without limitation the rights
//// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//// copies of the Software, and to permit persons to whom the Software is
//// furnished to do so, subject to the following conditions:

//// The above copyright notice and this permission notice shall be included in
//// all copies or substantial portions of the Software.

//// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//// THE SOFTWARE.

//using OpenLR.Locations;
//using OpenLR.Referenced.Locations;
//using OsmSharp.Collections.Tags;
//using OsmSharp.Geo;
//using OsmSharp.Geo.Attributes;
//using OsmSharp.Geo.Features;
//using OsmSharp.Geo.Geometries;
//using OsmSharp.Math.Geo;
//using OsmSharp.Math.Geo.Simple;
//using OsmSharp.Math.Primitives;
//using OsmSharp.Routing.Network;
//using OsmSharp.Units.Distance;
//using System;
//using System.Collections.Generic;

//namespace OpenLR.Referenced
//{
//    /// <summary>
//    /// Contains extension methods for some OpenLR core stuff.
//    /// </summary>
//    public static class Extensions
//    {
//        /// <summary>
//        /// Converts the circle location to features.
//        /// </summary>
//        public static FeatureCollection ToFeatures(this CircleLocation circleLocation)
//        {
//            // create a point feature.
//            var point = new Point(new GeoCoordinate(circleLocation.Coordinate.Latitude, circleLocation.Coordinate.Longitude));
//            var pointAttributes = new SimpleGeometryAttributeCollection();
//            pointAttributes.Add("radius", circleLocation.Radius);
//            var pointFeature = new Feature(point, pointAttributes);
            
//            //// create a circle feature.
//            //var geometricShapeFactory = new GeometricShapeFactory();
//            //geometricShapeFactory.NumPoints = 32;
//            //geometricShapeFactory.Centre = point.Coordinate;
//            //geometricShapeFactory.Size = circleLocation.Radius;
//            //var circle = geometricShapeFactory.CreateCircle();
//            //var circleAttributes = new AttributesTable();
//            //var circleFeature = new Feature(circle, circleAttributes);

//            // create the feature collection.
//            var featureCollection = new FeatureCollection();
//            featureCollection.Add(pointFeature);
//            //featureCollection.Add(circleFeature);
//            return featureCollection;
//        }

//        /// <summary>
//        /// Converts the closed line location to features.
//        /// </summary>
//        public static FeatureCollection ToFeatures(this ClosedLineLocation closedLineLocation)
//        {
//            // create the feature collection.
//            var featureCollection = new FeatureCollection();

//            // build the coordinates list and create point features.
//            var coordinates = new List<GeoCoordinate>();
//            coordinates.Add(new GeoCoordinate(closedLineLocation.First.Coordinate.Latitude, 
//                closedLineLocation.First.Coordinate.Longitude));
//            featureCollection.Add(closedLineLocation.First.ToFeature());
//            if (closedLineLocation.Intermediate != null)
//            { // there are intermediate coordinates.
//                for (int idx = 0; idx < closedLineLocation.Intermediate.Length; idx++)
//                {
//                    coordinates.Add(new GeoCoordinate(closedLineLocation.Intermediate[idx].Coordinate.Latitude,
//                        closedLineLocation.Intermediate[idx].Coordinate.Longitude));
//                    featureCollection.Add(closedLineLocation.Intermediate[idx].ToFeature());
//                }
//            }
//            coordinates.Add(new GeoCoordinate(closedLineLocation.Last.Coordinate.Latitude,
//                closedLineLocation.Last.Coordinate.Longitude));
//            featureCollection.Add(closedLineLocation.Last.ToFeature());

//            // create a line feature.
//            var line = new LineString(coordinates.ToArray());
//            var lineAttributes = new SimpleGeometryAttributeCollection();
//            var lineFeature = new Feature(line, lineAttributes);
//            featureCollection.Add(lineFeature);

//            return featureCollection;
//        }

//        /// <summary>
//        /// Converts the geo coordinate line location to features.
//        /// </summary>
//        public static FeatureCollection ToFeatures(this GeoCoordinateLocation geoCoordinateLocation)
//        {
//            // create the feature collection.
//            var featureCollection = new FeatureCollection();

//            // create a point feature.
//            var point = new Point(new GeoCoordinate(geoCoordinateLocation.Coordinate.Latitude,
//                geoCoordinateLocation.Coordinate.Longitude));
//            var pointAttributes = new SimpleGeometryAttributeCollection();
//            featureCollection.Add(new Feature(point, pointAttributes));
//            return featureCollection;
//        }

//        /// <summary>
//        /// Converts the grid location to features.
//        /// </summary>
//        public static FeatureCollection ToFeatures(this GridLocation gridLocation)
//        {
//            // create the feature collection.
//            var featureCollection = new FeatureCollection();

//            // create a point feature at each point in the grid.
//            double lonDiff = (gridLocation.LowerLeft.Longitude - gridLocation.UpperRight.Longitude) / gridLocation.Columns;
//            double latDiff = (gridLocation.UpperRight.Latitude - gridLocation.LowerLeft.Latitude) / gridLocation.Rows;
//            for(int column = 0; column < gridLocation.Columns; column++)
//            {
//                double longitude = gridLocation.LowerLeft.Longitude - (column * lonDiff);
//                for (int row = 0; row < gridLocation.Rows; row++)
//                {
//                    double latitude = gridLocation.UpperRight.Latitude - (row * latDiff);
//                    var point = new Point(new GeoCoordinate(latitude, longitude));
//                    var pointAttributes = new SimpleGeometryAttributeCollection();
//                    featureCollection.Add(new Feature(point, pointAttributes));
//                }
//            }

//            // create a lineair ring.
//            var lineairRingAttributes = new SimpleGeometryAttributeCollection();
//            featureCollection.Add(new Feature(new LineString(new GeoCoordinate[] {
//                new GeoCoordinate(gridLocation.LowerLeft.Latitude, gridLocation.LowerLeft.Longitude),
//                new GeoCoordinate(gridLocation.LowerLeft.Latitude, gridLocation.UpperRight.Longitude),
//                new GeoCoordinate(gridLocation.UpperRight.Latitude, gridLocation.UpperRight.Longitude),
//                new GeoCoordinate(gridLocation.UpperRight.Latitude, gridLocation.LowerLeft.Longitude)
//            }), lineairRingAttributes));
//            return featureCollection;
//        }

//        /// <summary>
//        /// Converts the line location to features.
//        /// </summary>
//        public static FeatureCollection ToFeatures(this LineLocation lineLocation)
//        {
//            // create the feature collection.
//            var featureCollection = new FeatureCollection();

//            // build the coordinates list and create point features.
//            var coordinates = new List<GeoCoordinate>();
//            coordinates.Add(new GeoCoordinate(lineLocation.First.Coordinate.Latitude, 
//                lineLocation.First.Coordinate.Longitude));
//            featureCollection.Add(lineLocation.First.ToFeature());
//            if (lineLocation.Intermediate != null)
//            { // there are intermediate coordinates.
//                for (int idx = 0; idx < lineLocation.Intermediate.Length; idx++)
//                {
//                    coordinates.Add(new GeoCoordinate(lineLocation.Intermediate[idx].Coordinate.Latitude, 
//                        lineLocation.Intermediate[idx].Coordinate.Longitude));
//                    featureCollection.Add(lineLocation.Intermediate[idx].ToFeature());
//                }
//            }
//            coordinates.Add(new GeoCoordinate(lineLocation.Last.Coordinate.Latitude, 
//                lineLocation.Last.Coordinate.Longitude));
//            featureCollection.Add(lineLocation.Last.ToFeature());

//            // create a line feature.
//            var line = new LineString(coordinates.ToArray());
//            var lineAttributes = new SimpleGeometryAttributeCollection();
//            lineAttributes.Add("negative_offset", lineLocation.NegativeOffsetPercentage.HasValue ? 
//                lineLocation.NegativeOffsetPercentage.Value : 0);
//            lineAttributes.Add("positive_offset", lineLocation.PositiveOffsetPercentage.HasValue ? 
//                lineLocation.PositiveOffsetPercentage.Value : 0);
//            var lineFeature = new Feature(line, lineAttributes);
//            featureCollection.Add(lineFeature);

//            return featureCollection;
//        }

//        /// <summary>
//        /// Converts the point along the line location to features.
//        /// </summary>
//        public static FeatureCollection ToFeatures(this PointAlongLineLocation pointAlongLineLocation)
//        {
//            // create the feature collection.
//            var featureCollection = new FeatureCollection();

//            // create the coordinates.
//            var coordinates = new List<GeoCoordinate>();
//            coordinates.Add(new GeoCoordinate(pointAlongLineLocation.First.Coordinate.Latitude,
//                pointAlongLineLocation.First.Coordinate.Longitude));
//            featureCollection.Add(pointAlongLineLocation.First.ToFeature());
//            coordinates.Add(new GeoCoordinate(pointAlongLineLocation.Last.Coordinate.Latitude,
//                pointAlongLineLocation.Last.Coordinate.Longitude));
//            featureCollection.Add(pointAlongLineLocation.Last.ToFeature());

//            // create a line feature.
//            var line = new LineString(coordinates.ToArray());
//            var lineAttributes = new SimpleGeometryAttributeCollection();
//            lineAttributes.Add("orientation", pointAlongLineLocation.Orientation.ToString());
//            lineAttributes.Add("positive_offset_percentage", pointAlongLineLocation.PositiveOffsetPercentage);
//            lineAttributes.Add("side_of_road", pointAlongLineLocation.SideOfRoad.ToString());
//            var lineFeature = new Feature(line, lineAttributes);
//            featureCollection.Add(lineFeature);

//            return featureCollection;
//        }

//        /// <summary>
//        /// Converts the referenced point along the line location to features.
//        /// </summary>
//        public static FeatureCollection ToFeatures(this ReferencedPointAlongLine referencedPointALongLineLocation, ReferencedEncoderBase baseEncoder)
//        {
//            // create the feature collection.
//            var featureCollection = referencedPointALongLineLocation.Route.ToFeatures();

//            // create the coordinates.
//            var feature = baseEncoder.GetVertexLocation(referencedPointALongLineLocation.Route.Vertices[0]).ToFeature();
//            feature.Attributes.Add("type", "start");
//            featureCollection.Add(feature);
//            feature = baseEncoder.GetVertexLocation(referencedPointALongLineLocation.Route.Vertices[
//                referencedPointALongLineLocation.Route.Vertices.Length - 1]).ToFeature();
//            feature.Attributes.Add("type", "end");
//            featureCollection.Add(feature);

//            // create a feature for the actual location.
//            var locationCoordinate = new GeoCoordinate(referencedPointALongLineLocation.Latitude, 
//                referencedPointALongLineLocation.Longitude);
//            var locationCoordinateFeature = new Feature(new Point(locationCoordinate), 
//                new SimpleGeometryAttributeCollection());
//            featureCollection.Add(locationCoordinateFeature);

//            return featureCollection;
//        }

//        /// <summary>
//        /// Converts the referenced point along the line location to features.
//        /// </summary>
//        public static Meter Length(this ReferencedPointAlongLine referencedPointALongLineLocation, ReferencedEncoderBase baseEncoder)
//        {
//            return referencedPointALongLineLocation.Route.Length(baseEncoder);
//        }

//        /// <summary>
//        /// Converts the referenced point along the line location to features.
//        /// </summary>
//        public static Meter Length(this ReferencedLine referencedLine, ReferencedEncoderBase baseEncoder)
//        {
//            var length = 0.0;
//            for (int idx = 0; idx < referencedLine.Edges.Length; idx++)
//            {
//                length = length + baseEncoder.RouterDb.Network.GetShape(
//                    baseEncoder.RouterDb.Network.GetEdge(referencedLine.Edges[idx])).;
//            }
//            return length;
//        }

//        /// <summary>
//        /// Converts the referenced line location to features.
//        /// </summary>
//        /// <param name="referencedLine">The referenced line.</param>
//        /// <param name="encoder">The encoder.</param>
//        /// <returns></returns>
//        public static List<GeoCoordinate> GetCoordinates(this ReferencedLine referencedLine, ReferencedEncoderBase encoder)
//        {
//            return referencedLine.GetCoordinates(encoder, 0, referencedLine.Vertices.Length);
//        }

//        /// <summary>
//        /// Converts the referenced line location to features.
//        /// </summary>
//        /// <param name="referencedLine">The referenced line.</param>
//        /// <param name="encoder">The encoder.</param>
//        /// <param name="start">The start vertex.</param>
//        /// <param name="count">The vertices to return coordinates for.</param>
//        /// <returns></returns>
//        public static List<GeoCoordinate> GetCoordinates(this ReferencedLine referencedLine, ReferencedEncoderBase encoder, int start, int count)
//        {
//            var coordinates = new List<GeoCoordinate>();
//            if(count <= 0)
//            {
//                return coordinates;
//            }
//            coordinates.Add(encoder.GetVertexLocation(referencedLine.Vertices[start]).ToGeoCoordinate());
//            for (var i = start; i < start + count - 1; i++)
//            {
//                if (referencedLine.EdgeShapes[i] != null)
//                {
//                    for (var j = 0; j < referencedLine.EdgeShapes[i].Length; j++)
//                    {
//                        coordinates.Add(new GeoCoordinate(
//                            referencedLine.EdgeShapes[i][j].Latitude, referencedLine.EdgeShapes[i][j].Longitude));
//                    }
//                }
//                coordinates.Add(encoder.GetVertexLocation(referencedLine.Vertices[i + 1]).ToGeoCoordinate());
//            }
//            return coordinates;
//        }

//        /// <summary>
//        /// Converts the referenced line location to a list of sorted coordinates.
//        /// </summary>
//        /// <param name="referencedLine">The referenced line.</param>
//        /// <param name="decoder">The decoder.</param>
//        /// <returns></returns>
//        public static List<GeoCoordinate> GetCoordinates(this ReferencedLine referencedLine, ReferencedDecoderBase decoder)
//        {
//            return referencedLine.GetCoordinates(decoder, 0, referencedLine.Vertices.Length);
//        }

//        /// <summary>
//        /// Converts the referenced line location to a list of sorted coordinates.
//        /// </summary>
//        /// <param name="referencedLine">The referenced line.</param>
//        /// <param name="decoder">The decoder.</param>
//        /// <param name="start">The start vertex.</param>
//        /// <param name="count">The vertices to return coordinates for.</param>
//        /// <returns></returns>
//        public static List<GeoCoordinate> GetCoordinates(this ReferencedLine referencedLine, ReferencedDecoderBase decoder, int start, int count)
//        {
//            var coordinates = new List<GeoCoordinate>();
//            if (count <= 0)
//            {
//                return coordinates;
//            }
//            coordinates.Add(decoder.GetVertexLocation(referencedLine.Vertices[start]).ToGeoCoordinate());
//            for (var i = start; i < start + count - 1; i++)
//            {
//                if (referencedLine.EdgeShapes[i] != null)
//                {
//                    for (var j = 0; j < referencedLine.EdgeShapes[i].Length; j++)
//                    {
//                        coordinates.Add(new GeoCoordinate(
//                            referencedLine.EdgeShapes[i][j].Latitude, referencedLine.EdgeShapes[i][j].Longitude));
//                    }
//                }
//                coordinates.Add(decoder.GetVertexLocation(referencedLine.Vertices[i + 1]).ToGeoCoordinate());
//            }
//            return coordinates;
//        }

//        /// <summary>
//        /// Converts the referenced line location to a list of sorted coordinates.
//        /// </summary>
//        /// <returns></returns>
//        public static List<GeoCoordinate> GetCoordinates(this ReferencedLine route, ReferencedDecoderBase decoder, double offsetRatio, 
//            out int offsetEdgeIdx, out GeoCoordinate offsetLocation, out Meter offsetLength, out Meter offsetEdgeLength, out Meter edgeLength)
//        {            
//            if (route == null) { throw new ArgumentNullException("route"); }
//            if (route.Edges == null || route.Edges.Length == 0) { throw new ArgumentOutOfRangeException("route", "Route has no edges."); }
//            if (route.Vertices == null || route.Vertices.Length == 0) { throw new ArgumentOutOfRangeException("route", "Route has no vertices."); }
//            if (route.Vertices.Length != route.Edges.Length + 1) { throw new ArgumentOutOfRangeException("route", "Route is invalid: there should be n vertices and n-1 edges."); }

//            // calculate the total length first.
//            var totalLength = route.GetCoordinates(decoder).Length();

//            // calculate the lenght at the offst.
//            offsetLength = (Meter)(totalLength.Value * offsetRatio);
//            offsetEdgeLength = -1;
//            offsetEdgeIdx = -1;
//            edgeLength = -1;

//            // loop over all coordinates and collect offsetLocation and offsetEdgeIdx.
//            double currentOffsetLength = 0;
//            double currentEdgeLength = 0;
//            var coordinates = new List<GeoCoordinate>();
//            coordinates.Add(decoder.GetCoordinate(route.Vertices[0]).ToGeoCoordinate());
//            for (int edgeIdx = 0; edgeIdx < route.Edges.Length; edgeIdx++)
//            {
//                currentEdgeLength = 0;
//                var edge = route.Edges[edgeIdx];
//                var edgeShapes = route.EdgeShapes[edgeIdx];
//                if (edgeShapes != null)
//                { // there are intermediate coordinates.
//                    for (int idx = 0; idx < edgeShapes.Length; idx++)
//                    {
//                        coordinates.Add(new GeoCoordinate(edgeShapes[idx].Latitude, edgeShapes[idx].Longitude));
//                        currentEdgeLength = currentEdgeLength + coordinates[coordinates.Count - 2].DistanceEstimate(coordinates[coordinates.Count - 1]).Value;
//                    }
//                }
//                coordinates.Add(decoder.GetCoordinate(route.Vertices[edgeIdx + 1]).ToGeoCoordinate());
//                currentEdgeLength = currentEdgeLength + coordinates[coordinates.Count - 2].DistanceEstimate(coordinates[coordinates.Count - 1]).Value;

//                // add current edge length to current offset.
//                if ((currentOffsetLength + currentEdgeLength) >= offsetLength.Value &&
//                    edgeLength.Value < 0)
//                { // it's this edge that has the valuable info.
//                    offsetEdgeIdx = edgeIdx;
//                    offsetEdgeLength = offsetLength - currentOffsetLength;
//                    edgeLength = currentEdgeLength;
//                }
//                currentOffsetLength = currentOffsetLength + currentEdgeLength;
//            }

//            // choose the last edge.
//            if (edgeLength.Value < 0)
//            { // it's this edge that has the valuable info.
//                offsetEdgeIdx = route.Edges.Length - 1;
//                offsetEdgeLength = offsetLength - currentOffsetLength;
//                edgeLength = currentEdgeLength;
//            }

//            // calculate actual offset position.
//            offsetLocation = coordinates.GetPositionLocation(offsetRatio);
//            return coordinates;
//        }

//        /// <summary>
//        /// Converts the referenced point along the line location to features.
//        /// </summary>
//        /// <returns></returns>
//        public static FeatureCollection ToFeatures(this ReferencedPointAlongLine referencedPointALongLineLocation, ReferencedDecoderBase baseDecoder)
//        {
//            // create the geometry factory.
//            var geometryFactory = new GeometryFactory();

//            // create the feature collection.
//            var featureCollection = referencedPointALongLineLocation.Route.ToFeatures();

//            // create the coordinates.
//            var feature = baseDecoder.GetVertexLocation(referencedPointALongLineLocation.Route.Vertices[0]).ToFeature();
//            feature.Attributes.AddAttribute("type", "start");
//            featureCollection.Add(feature);
//            feature = baseDecoder.GetVertexLocation(referencedPointALongLineLocation.Route.Vertices[
//                referencedPointALongLineLocation.Route.Vertices.Length - 1]).ToFeature();
//            feature.Attributes.AddAttribute("type", "end");
//            featureCollection.Add(feature);

//            // create a feature for the actual location.
//            var locationCoordinate = new GeoCoordinate(referencedPointALongLineLocation.Latitude, referencedPointALongLineLocation.Longitude);
//            var locationCoordinateFeature = new Feature(new Point(locationCoordinate), new SimpleGeometryAttributeCollection());
//            featureCollection.Add(locationCoordinateFeature);

//            return featureCollection;
//        }

//        /// <summary>
//        /// Converts the poi with access point location.
//        /// </summary>
//        /// <param name="poiWithAccessPointLocation"></param>
//        /// <returns></returns>
//        public static FeatureCollection ToFeatures(this PoiWithAccessPointLocation poiWithAccessPointLocation)
//        {
//            // create the geometry factory.
//            var geometryFactory = new GeometryFactory();

//            // create the feature collection.
//            var featureCollection = new FeatureCollection();

//            // create a point for the poi.
//            var point = geometryFactory.CreatePoint(new Coordinate(poiWithAccessPointLocation.Coordinate.Longitude, poiWithAccessPointLocation.Coordinate.Latitude));
//            var pointAttributes = new AttributesTable();
//            pointAttributes.AddAttribute("orientation", poiWithAccessPointLocation.Orientation);
//            pointAttributes.AddAttribute("positive_offset", poiWithAccessPointLocation.PositiveOffset);
//            pointAttributes.AddAttribute("side_of_road", poiWithAccessPointLocation.SideOfRoad);
//            var pointFeature = new Feature(point, pointAttributes);
//            featureCollection.Add(pointFeature);
//            featureCollection.Add(poiWithAccessPointLocation.First.ToFeature());
//            featureCollection.Add(poiWithAccessPointLocation.Last.ToFeature());
//            return featureCollection;
//        }

//        /// <summary>
//        /// Converts the polygon location.
//        /// </summary>
//        /// <param name="polygonLocation"></param>
//        /// <returns></returns>
//        public static FeatureCollection ToFeatures(this PolygonLocation polygonLocation)
//        {
//            // create the geometry factory.
//            var geometryFactory = new GeometryFactory();

//            // create the feature collection.
//            var featureCollection = new FeatureCollection();

//            // build the coordinates list and create point features.
//            var coordinates = new List<Coordinate>();
//            if (polygonLocation.Coordinates != null)
//            { // there are intermediate coordinates.
//                for (int idx = 0; idx < polygonLocation.Coordinates.Length; idx++)
//                {
//                    coordinates.Add(new Coordinate(polygonLocation.Coordinates[idx].Longitude, polygonLocation.Coordinates[idx].Latitude));
//                }
//            }

//            // create a line feature.
//            var line = geometryFactory.CreateLinearRing(coordinates.ToArray());
//            var lineAttributes = new AttributesTable();
//            var lineFeature = new Feature(line, lineAttributes);
//            featureCollection.Add(lineFeature);

//            return featureCollection;
//        }

//        /// <summary>
//        /// Converts the rectangle location.
//        /// </summary>
//        /// <param name="rectangleLocation"></param>
//        /// <returns></returns>
//        public static FeatureCollection ToFeatures(this RectangleLocation rectangleLocation)
//        {
//            // create the geometry factory.
//            var geometryFactory = new GeometryFactory();

//            // create the feature collection.
//            var featureCollection = new FeatureCollection();

//            // create a lineair ring.
//            var lineairRingAttributes = new SimpleGeometryAttributeCollection();
//            featureCollection.Add(new Feature(geometryFactory.CreateLinearRing(new GeoCoordinate[] {
//                new GeoCoordinate(rectangleLocation.LowerLeft.Latitude, rectangleLocation.LowerLeft.Longitude),
//                new GeoCoordinate(rectangleLocation.LowerLeft.Latitude, rectangleLocation.UpperRight.Longitude),
//                new GeoCoordinate(rectangleLocation.UpperRight.Latitude, rectangleLocation.UpperRight.Longitude),
//                new GeoCoordinate(rectangleLocation.UpperRight.Latitude, rectangleLocation.LowerLeft.Longitude)
//            }), lineairRingAttributes));
//            return featureCollection;
//        }

//        /// <summary>
//        /// Converts the location reference point to 
//        /// </summary>
//        public static Feature ToFeature(this OpenLR.Model.LocationReferencePoint locationReferencePoint)
//        {
//            var point = new Point(new GeoCoordinate(locationReferencePoint.Coordinate.Latitude, 
//                locationReferencePoint.Coordinate.Longitude));
//            var pointAttributes = new SimpleGeometryAttributeCollection();
//            pointAttributes.Add("bearing_distance", locationReferencePoint.Bearing);
//            pointAttributes.Add("distance_to_next", locationReferencePoint.DistanceToNext);
//            pointAttributes.Add("form_of_way", locationReferencePoint.FormOfWay);
//            pointAttributes.Add("functional_road_class", locationReferencePoint.FuntionalRoadClass);
//            pointAttributes.Add("lowest_functional_road_class_to_next", locationReferencePoint.LowestFunctionalRoadClassToNext);
//            return new Feature(point, pointAttributes);
//        }

//        /// <summary>
//        /// Converts the given coordinate to a geocoordinate.
//        /// </summary>
//        public static GeoCoordinate ToGeoCoordinate(this OpenLR.Model.Coordinate coordinate)
//        {
//            return new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
//        }

//        /// <summary>
//        /// Converts the given coordinate to an NTS feature.
//        /// </summary>
//        public static Feature ToFeature(this OpenLR.Model.Coordinate coordinate)
//        {
//            return new Feature(new Point(coordinate), new SimpleGeometryAttributeCollection());
//        }
//    }
//}