using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.Utilities;
using OpenLR.Locations;
using OpenLR.OsmSharp.Router;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Units.Distance;
using System;
using System.Collections.Generic;

namespace OpenLR.OsmSharp
{
    /// <summary>
    /// Contains extension methods for some OpenLR core stuff.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converts the circle location to features.
        /// </summary>
        /// <param name="circleLocation"></param>
        /// <returns></returns>
        public static FeatureCollection ToFeatures(this CircleLocation circleLocation)
        {
            var geometryFactory = new GeometryFactory();

            // create a point feature.
            var point = geometryFactory.CreatePoint(new Coordinate(circleLocation.Coordinate.Longitude, circleLocation.Coordinate.Latitude));
            var pointAttributes = new AttributesTable();
            pointAttributes.AddAttribute("radius", circleLocation.Radius);
            var pointFeature = new Feature(point, pointAttributes);
            
            // create a circle feature.
            var geometricShapeFactory = new GeometricShapeFactory();
            geometricShapeFactory.NumPoints = 32;
            geometricShapeFactory.Centre = point.Coordinate;
            geometricShapeFactory.Size = circleLocation.Radius;
            var circle = geometricShapeFactory.CreateCircle();
            var circleAttributes = new AttributesTable();
            var circleFeature = new Feature(circle, circleAttributes);

            // create the feature collection.
            var featureCollection = new FeatureCollection();
            featureCollection.Add(pointFeature);
            featureCollection.Add(circleFeature);
            return featureCollection;
        }

        /// <summary>
        /// Converts the closed line location to features.
        /// </summary>
        /// <param name="closedLineLocation"></param>
        /// <returns></returns>
        public static FeatureCollection ToFeatures(this ClosedLineLocation closedLineLocation)
        {
            // create the geometry factory.
            var geometryFactory = new GeometryFactory();

            // create the feature collection.
            var featureCollection = new FeatureCollection();

            // build the coordinates list and create point features.
            var coordinates = new List<Coordinate>();
            coordinates.Add(new Coordinate(closedLineLocation.First.Coordinate.Longitude, closedLineLocation.First.Coordinate.Latitude));
            featureCollection.Add(closedLineLocation.First.ToFeature());
            if (closedLineLocation.Intermediate != null)
            { // there are intermediate coordinates.
                for (int idx = 0; idx < closedLineLocation.Intermediate.Length; idx++)
                {
                    coordinates.Add(new Coordinate(closedLineLocation.Intermediate[idx].Coordinate.Longitude, closedLineLocation.Intermediate[idx].Coordinate.Latitude));
                    featureCollection.Add(closedLineLocation.Intermediate[idx].ToFeature());
                }
            }
            coordinates.Add(new Coordinate(closedLineLocation.Last.Coordinate.Longitude, closedLineLocation.Last.Coordinate.Latitude));
            featureCollection.Add(closedLineLocation.Last.ToFeature());

            // create a line feature.
            var line = geometryFactory.CreateLineString(coordinates.ToArray());
            var lineAttributes = new AttributesTable();
            var lineFeature = new Feature(line, lineAttributes);
            featureCollection.Add(lineFeature);

            return featureCollection;
        }

        /// <summary>
        /// Converts the geo coordinate line location to features.
        /// </summary>
        /// <param name="geoCoordinateLocation"></param>
        /// <returns></returns>
        public static FeatureCollection ToFeatures(this GeoCoordinateLocation geoCoordinateLocation)
        {
            // create the geometry factory.
            var geometryFactory = new GeometryFactory();

            // create the feature collection.
            var featureCollection = new FeatureCollection();

            // create a point feature.
            var point = geometryFactory.CreatePoint(new Coordinate(geoCoordinateLocation.Coordinate.Longitude, geoCoordinateLocation.Coordinate.Latitude));
            var pointAttributes = new AttributesTable();
            featureCollection.Add(new Feature(point, pointAttributes));
            return featureCollection;
        }

        /// <summary>
        /// Converts the grid location to features.
        /// </summary>
        /// <param name="gridLocation"></param>
        /// <returns></returns>
        public static FeatureCollection ToFeatures(this GridLocation gridLocation)
        {
            // create the geometry factory.
            var geometryFactory = new GeometryFactory();

            // create the feature collection.
            var featureCollection = new FeatureCollection();

            // create a point feature at each point in the grid.
            double lonDiff = (gridLocation.LowerLeft.Longitude - gridLocation.UpperRight.Longitude) / gridLocation.Columns;
            double latDiff = (gridLocation.UpperRight.Latitude - gridLocation.LowerLeft.Latitude) / gridLocation.Rows;
            for(int column = 0; column < gridLocation.Columns; column++)
            {
                double longitude = gridLocation.LowerLeft.Longitude - (column * lonDiff);
                for (int row = 0; row < gridLocation.Rows; row++)
                {
                    double latitude = gridLocation.UpperRight.Latitude - (row * latDiff);
                    var point = geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
                    var pointAttributes = new AttributesTable();
                    featureCollection.Add(new Feature(point, pointAttributes));
                }
            }

            // create a lineair ring.
            var lineairRingAttributes = new AttributesTable();
            featureCollection.Add(new Feature(geometryFactory.CreateLinearRing(new Coordinate[] {
                new Coordinate(gridLocation.LowerLeft.Longitude, gridLocation.LowerLeft.Latitude),
                new Coordinate(gridLocation.LowerLeft.Longitude, gridLocation.UpperRight.Latitude),
                new Coordinate(gridLocation.UpperRight.Longitude, gridLocation.UpperRight.Latitude),
                new Coordinate(gridLocation.UpperRight.Longitude, gridLocation.LowerLeft.Latitude)
            }), lineairRingAttributes));
            return featureCollection;
        }

        /// <summary>
        /// Converts the line location to features.
        /// </summary>
        /// <param name="lineLocation"></param>
        /// <returns></returns>
        public static FeatureCollection ToFeatures(this LineLocation lineLocation)
        {
            // create the geometry factory.
            var geometryFactory = new GeometryFactory();

            // create the feature collection.
            var featureCollection = new FeatureCollection();

            // build the coordinates list and create point features.
            var coordinates = new List<Coordinate>();
            coordinates.Add(new Coordinate(lineLocation.First.Coordinate.Longitude, lineLocation.First.Coordinate.Latitude));
            featureCollection.Add(lineLocation.First.ToFeature());
            if (lineLocation.Intermediate != null)
            { // there are intermediate coordinates.
                for (int idx = 0; idx < lineLocation.Intermediate.Length; idx++)
                {
                    coordinates.Add(new Coordinate(lineLocation.Intermediate[idx].Coordinate.Longitude, lineLocation.Intermediate[idx].Coordinate.Latitude));
                    featureCollection.Add(lineLocation.Intermediate[idx].ToFeature());
                }
            }
            coordinates.Add(new Coordinate(lineLocation.Last.Coordinate.Longitude, lineLocation.Last.Coordinate.Latitude));
            featureCollection.Add(lineLocation.Last.ToFeature());

            // create a line feature.
            var line = geometryFactory.CreateLineString(coordinates.ToArray());
            var lineAttributes = new AttributesTable();
            lineAttributes.AddAttribute("negative_offset", lineLocation.NegativeOffset);
            lineAttributes.AddAttribute("positive_offset", lineLocation.PositiveOffset);
            var lineFeature = new Feature(line, lineAttributes);
            featureCollection.Add(lineFeature);

            return featureCollection;
        }

        /// <summary>
        /// Converts the point along the line location to features.
        /// </summary>
        /// <param name="pointAlongLineLocation"></param>
        /// <returns></returns>
        public static FeatureCollection ToFeatures(this PointAlongLineLocation pointAlongLineLocation)
        {
            // create the geometry factory.
            var geometryFactory = new GeometryFactory();

            // create the feature collection.
            var featureCollection = new FeatureCollection();

            // create the coordinates.
            var coordinates = new List<Coordinate>();
            coordinates.Add(new Coordinate(pointAlongLineLocation.First.Coordinate.Longitude, pointAlongLineLocation.First.Coordinate.Latitude));
            featureCollection.Add(pointAlongLineLocation.First.ToFeature());
            coordinates.Add(new Coordinate(pointAlongLineLocation.Last.Coordinate.Longitude, pointAlongLineLocation.Last.Coordinate.Latitude));
            featureCollection.Add(pointAlongLineLocation.Last.ToFeature());

            // create a line feature.
            var line = geometryFactory.CreateLineString(coordinates.ToArray());
            var lineAttributes = new AttributesTable();
            lineAttributes.AddAttribute("orientation", pointAlongLineLocation.Orientation.ToString());
            lineAttributes.AddAttribute("positive_offset_percentage", pointAlongLineLocation.PositiveOffsetPercentage);
            lineAttributes.AddAttribute("side_of_road", pointAlongLineLocation.SideOfRoad.ToString());
            var lineFeature = new Feature(line, lineAttributes);
            featureCollection.Add(lineFeature);

            return featureCollection;
        }

        /// <summary>
        /// Converts the referenced point along the line location to features.
        /// </summary>
        /// <param name="referencedPointALongLineLocation"></param>
        /// <param name="baseEncoder"></param>
        /// <returns></returns>
        public static FeatureCollection ToFeatures<TEdge>(this OpenLR.OsmSharp.Locations.ReferencedPointAlongLine<TEdge> referencedPointALongLineLocation, ReferencedEncoderBase<TEdge> baseEncoder)
            where TEdge : IDynamicGraphEdgeData
        {
            // create the geometry factory.
            var geometryFactory = new GeometryFactory();

            // create the feature collection.
            var featureCollection = referencedPointALongLineLocation.Route.ToFeatures();

            // create the coordinates.
            featureCollection.Add(baseEncoder.GetVertexLocation(referencedPointALongLineLocation.Route.Vertices[0]).ToFeature());
            featureCollection.Add(baseEncoder.GetVertexLocation(referencedPointALongLineLocation.Route.Vertices[referencedPointALongLineLocation.Route.Vertices.Length - 1]).ToFeature());

            // create a feature for the actual location.
            var locationCoordinate = new Coordinate(referencedPointALongLineLocation.Longitude, referencedPointALongLineLocation.Latitude);
            var locationCoordinateFeature = new Feature(new Point(locationCoordinate), new AttributesTable());
            featureCollection.Add(locationCoordinateFeature);

            return featureCollection;
        }

        /// <summary>
        /// Converts the referenced point along the line location to features.
        /// </summary>
        /// <param name="referencedPointALongLineLocation"></param>
        /// <param name="baseEncoder"></param>
        /// <returns></returns>
        public static Meter Length<TEdge>(this OpenLR.OsmSharp.Locations.ReferencedPointAlongLine<TEdge> referencedPointALongLineLocation, ReferencedEncoderBase<TEdge> baseEncoder)
            where TEdge : IDynamicGraphEdgeData
        {
            var length = 0.0;
            for(int idx = 0; idx < referencedPointALongLineLocation.Route.Edges.Length; idx++)
            {
                var from = baseEncoder.GetVertexLocation(referencedPointALongLineLocation.Route.Vertices[idx]).ToGeoCoordinate();
                var to = baseEncoder.GetVertexLocation(referencedPointALongLineLocation.Route.Vertices[idx + 1]).ToGeoCoordinate();
                length = length + referencedPointALongLineLocation.Route.Edges[idx].Length(from, to).Value;
            }
            return length;
        }

        /// <summary>
        /// Converts the referenced point along the line location to features.
        /// </summary>
        /// <param name="referencedPointALongLineLocation"></param>
        /// <param name="baseEncoder"></param>
        /// <returns></returns>
        public static List<GeoCoordinate> GetCoordinates<TEdge>(this OpenLR.OsmSharp.Locations.ReferencedPointAlongLine<TEdge> referencedPointALongLineLocation, ReferencedEncoderBase<TEdge> baseEncoder)
            where TEdge : IDynamicGraphEdgeData
        {
            var coordinates = new List<GeoCoordinate>();
            for (int idx = 0; idx < referencedPointALongLineLocation.Route.Edges.Length; idx++)
            {
                var from = baseEncoder.GetVertexLocation(referencedPointALongLineLocation.Route.Vertices[idx]).ToGeoCoordinate();
                var to = baseEncoder.GetVertexLocation(referencedPointALongLineLocation.Route.Vertices[idx + 1]).ToGeoCoordinate();
                var edgeCoordinates = referencedPointALongLineLocation.Route.Edges[idx].GetCoordinates(from, to);
                if (coordinates.Count > 0)
                {
                    coordinates.RemoveAt(coordinates.Count - 1);

                }
                coordinates.AddRange(edgeCoordinates);
            }
            return coordinates;
        }

        /// <summary>
        /// Converts the referenced point along the line location to features.
        /// </summary>
        /// <param name="referencedPointALongLineLocation"></param>
        /// <param name="baseDecoder"></param>
        /// <returns></returns>
        public static FeatureCollection ToFeatures<TEdge>(this OpenLR.OsmSharp.Locations.ReferencedPointAlongLine<TEdge> referencedPointALongLineLocation, ReferencedDecoderBase<TEdge> baseDecoder)
            where TEdge : IDynamicGraphEdgeData
        {
            // create the geometry factory.
            var geometryFactory = new GeometryFactory();

            // create the feature collection.
            var featureCollection = new FeatureCollection();

            // create the coordinates.
            var geoCoordinates = baseDecoder.GetCoordinates(referencedPointALongLineLocation.Route);
            featureCollection.Add(baseDecoder.GetCoordinate(referencedPointALongLineLocation.Route.Vertices[0]).ToFeature());
            featureCollection.Add(baseDecoder.GetCoordinate(referencedPointALongLineLocation.Route.Vertices[referencedPointALongLineLocation.Route.Vertices.Length - 1]).ToFeature());
            var coordinates = new List<Coordinate>(geoCoordinates.Count);
            for(int idx = 0; idx < geoCoordinates.Count; idx++)
            {
                coordinates.Add(new Coordinate(geoCoordinates[idx].Longitude, geoCoordinates[idx].Latitude));
            }

            // create a line feature.
            var line = geometryFactory.CreateLineString(coordinates.ToArray());
            var lineAttributes = new AttributesTable();
            lineAttributes.AddAttribute("orientation", referencedPointALongLineLocation.Orientation.ToString());
            var lineFeature = new Feature(line, lineAttributes);
            featureCollection.Add(lineFeature);

            // create a feature for the actual location.
            var locationCoordinate = new Coordinate(referencedPointALongLineLocation.Longitude, referencedPointALongLineLocation.Latitude);
            var locationCoordinateFeature = new Feature(new Point(locationCoordinate), new AttributesTable());
            featureCollection.Add(locationCoordinateFeature);

            return featureCollection;
        }

        /// <summary>
        /// Converts the poi with access point location.
        /// </summary>
        /// <param name="poiWithAccessPointLocation"></param>
        /// <returns></returns>
        public static FeatureCollection ToFeatures(this PoiWithAccessPointLocation poiWithAccessPointLocation)
        {
            // create the geometry factory.
            var geometryFactory = new GeometryFactory();

            // create the feature collection.
            var featureCollection = new FeatureCollection();

            // create a point for the poi.
            var point = geometryFactory.CreatePoint(new Coordinate(poiWithAccessPointLocation.Coordinate.Longitude, poiWithAccessPointLocation.Coordinate.Latitude));
            var pointAttributes = new AttributesTable();
            pointAttributes.AddAttribute("orientation", poiWithAccessPointLocation.Orientation);
            pointAttributes.AddAttribute("positive_offset", poiWithAccessPointLocation.PositiveOffset);
            pointAttributes.AddAttribute("side_of_road", poiWithAccessPointLocation.SideOfRoad);
            var pointFeature = new Feature(point, pointAttributes);
            featureCollection.Add(pointFeature);
            featureCollection.Add(poiWithAccessPointLocation.First.ToFeature());
            featureCollection.Add(poiWithAccessPointLocation.Last.ToFeature());
            return featureCollection;
        }

        /// <summary>
        /// Converts the polygon location.
        /// </summary>
        /// <param name="polygonLocation"></param>
        /// <returns></returns>
        public static FeatureCollection ToFeatures(this PolygonLocation polygonLocation)
        {
            // create the geometry factory.
            var geometryFactory = new GeometryFactory();

            // create the feature collection.
            var featureCollection = new FeatureCollection();

            // build the coordinates list and create point features.
            var coordinates = new List<Coordinate>();
            if (polygonLocation.Coordinates != null)
            { // there are intermediate coordinates.
                for (int idx = 0; idx < polygonLocation.Coordinates.Length; idx++)
                {
                    coordinates.Add(new Coordinate(polygonLocation.Coordinates[idx].Longitude, polygonLocation.Coordinates[idx].Latitude));
                }
            }

            // create a line feature.
            var line = geometryFactory.CreateLinearRing(coordinates.ToArray());
            var lineAttributes = new AttributesTable();
            var lineFeature = new Feature(line, lineAttributes);
            featureCollection.Add(lineFeature);

            return featureCollection;
        }

        /// <summary>
        /// Converts the rectangle location.
        /// </summary>
        /// <param name="rectangleLocation"></param>
        /// <returns></returns>
        public static FeatureCollection ToFeatures(this RectangleLocation rectangleLocation)
        {
            // create the geometry factory.
            var geometryFactory = new GeometryFactory();

            // create the feature collection.
            var featureCollection = new FeatureCollection();

            // create a lineair ring.
            var lineairRingAttributes = new AttributesTable();
            featureCollection.Add(new Feature(geometryFactory.CreateLinearRing(new Coordinate[] {
                new Coordinate(rectangleLocation.LowerLeft.Longitude, rectangleLocation.LowerLeft.Latitude),
                new Coordinate(rectangleLocation.LowerLeft.Longitude, rectangleLocation.UpperRight.Latitude),
                new Coordinate(rectangleLocation.UpperRight.Longitude, rectangleLocation.UpperRight.Latitude),
                new Coordinate(rectangleLocation.UpperRight.Longitude, rectangleLocation.LowerLeft.Latitude)
            }), lineairRingAttributes));
            return featureCollection;
        }

        /// <summary>
        /// Converts the location reference point to 
        /// </summary>
        /// <param name="locationReferencePoint"></param>
        /// <returns></returns>
        public static Feature ToFeature(this OpenLR.Model.LocationReferencePoint locationReferencePoint)
        {
            // create the geometry factory.
            var geometryFactory = new GeometryFactory();

            // create a point feature.
            var point = geometryFactory.CreatePoint(new Coordinate(locationReferencePoint.Coordinate.Longitude, locationReferencePoint.Coordinate.Latitude));
            var pointAttributes = new AttributesTable();
            pointAttributes.AddAttribute("bearing_distance", locationReferencePoint.Bearing);
            pointAttributes.AddAttribute("distance_to_next", locationReferencePoint.DistanceToNext);
            pointAttributes.AddAttribute("form_of_way", locationReferencePoint.FormOfWay);
            pointAttributes.AddAttribute("functional_road_class", locationReferencePoint.FuntionalRoadClass);
            pointAttributes.AddAttribute("lowest_functional_road_class_to_next", locationReferencePoint.LowestFunctionalRoadClassToNext);
            return new Feature(point, pointAttributes);
        }

        /// <summary>
        /// Converts the given tags collection to an attributes table.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static AttributesTable ToAttributes(this TagsCollectionBase tags)
        {
            var attributes = new AttributesTable();
            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    attributes.AddAttribute(tag.Key, tag.Value);
                }
            }
            return attributes;
        }

        /// <summary>
        /// Returns the edge that is closest to the given location.
        /// </summary>
        /// <typeparam name="TEdge"></typeparam>
        /// <param name="graph"></param>
        /// <param name="location"></param>
        /// <returns>Returns an edge or an edge from 0 to 0 if none is found.</returns>
        public static KeyValuePair<long, KeyValuePair<long, TEdge>> GetClosestEdge<TEdge>(this BasicRouterDataSource<TEdge> graph, GeoCoordinate location)
            where TEdge : IDynamicGraphEdgeData
        {     
            // create the search box.
            var searchBoxSize = 0.1;
            var searchBox = new GeoCoordinateBox(new GeoCoordinate(
                location.Latitude - searchBoxSize, location.Longitude - searchBoxSize),
                                                               new GeoCoordinate(
                location.Latitude + searchBoxSize, location.Longitude + searchBoxSize));
            var arcs = graph.GetArcs(searchBox);

            float latitude, longitude;
            double bestDistance = double.MaxValue;
            var bestEdge = new KeyValuePair<long, KeyValuePair<long, TEdge>>(0, new KeyValuePair<long, TEdge>(0, default(TEdge)));
            foreach(var arc in arcs)
            {
                graph.GetVertex(arc.Key, out latitude, out longitude);
                var from = new GeoCoordinate(latitude, longitude);
                graph.GetVertex(arc.Value.Key, out latitude, out longitude);
                var to = new GeoCoordinate(latitude, longitude);

                List<GeoCoordinate> coordinates = null;
                if (arc.Value.Value.Forward)
                {
                    coordinates = arc.Value.Value.GetCoordinates(from, to);
                }
                else
                {
                    coordinates = arc.Value.Value.GetCoordinates(to, from);
                }

                if(coordinates != null && coordinates.Count > 0)
                {
                    for(int idx = 1; idx < coordinates.Count; idx++)
                    {
                        var line = new GeoCoordinateLine(coordinates[idx - 1], coordinates[idx], true, true);
                        var distance = line.Distance(location);
                        if (distance < bestDistance)
                        {
                            bestEdge = arc;
                            bestDistance = distance;
                        }
                    }
                }
            }
            return bestEdge;
        }

        /// <summary>
        /// Returns a list of coordinates representing the geometry of the edge.
        /// </summary>
        /// <typeparam name="TEdge"></typeparam>
        /// <param name="edge"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static List<GeoCoordinate> GetCoordinates<TEdge>(this TEdge edge, GeoCoordinate from, GeoCoordinate to)
            where TEdge : IDynamicGraphEdgeData
        {
            var coordinates = new List<GeoCoordinate>();
            coordinates.Add(from);
            if (edge.Coordinates != null)
            {
                for (int idx = 0; idx < edge.Coordinates.Length; idx++)
                {
                    coordinates.Add(new GeoCoordinate(edge.Coordinates[idx].Latitude, edge.Coordinates[idx].Longitude));
                }
            }
            coordinates.Add(to);
            return coordinates;
        }

        /// <summary>
        /// Returns a list of coordinates representing the geometry of the edge.
        /// </summary>
        /// <typeparam name="TEdge"></typeparam>
        /// <param name="edge"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Meter Length<TEdge>(this TEdge edge, GeoCoordinate from, GeoCoordinate to)
            where TEdge : IDynamicGraphEdgeData
        {
            var coordinates = edge.GetCoordinates(from, to);
            return coordinates.Length();
        }

        /// <summary>
        /// Calculates the real length in meters.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public static Meter Length(this List<GeoCoordinate> coordinates)
        {
            Meter length = 0;
            for (int idx = 0; idx < coordinates.Count - 1; idx++)
            {
                length = length + coordinates[idx].DistanceReal(coordinates[idx + 1]);
            }
            return length;
        }

        /// <summary>
        /// Projects the given point on the line represented by the coordinates.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="point"></param>
        /// <param name="bestProjected"></param>
        /// <param name="bestPosition"></param>
        /// <param name="bestOffset"></param>
        /// <returns></returns>
        public static bool ProjectOn(this List<GeoCoordinate> coordinates, PointF2D point, out PointF2D bestProjected, out LinePointPosition bestPosition, out Meter bestOffset)
        {
            // check first point.
            var pointGeo = new GeoCoordinate(point);
            var distance = coordinates[0].DistanceReal(pointGeo).Value;
            bestProjected = coordinates[0];
            bestOffset = 0;
            bestPosition = LinePointPosition.On;

            // check intermediate points.
            var bestDistance = distance;
            var currentOffset = 0.0;
            for (int idx = 0; idx < coordinates.Count - 1; idx++)
            {
                var line = new GeoCoordinateLine(coordinates[idx], coordinates[idx + 1], true, true);
                var projected = line.ProjectOn(point);
                var position = line.PositionOfPoint(point);

                if(projected != null)
                { // there is a valid projected point.
                    var offset = coordinates[idx].DistanceReal(new GeoCoordinate(projected)).Value;
                    distance = projected.Distance(point);
                    if(distance < bestDistance)
                    { // this point is closer.
                        bestDistance = distance;
                        bestProjected = projected;
                        bestOffset = currentOffset + offset;
                    }
                }
                currentOffset = currentOffset + line.LengthReal.Value;

                // check point at idx + 1 (point at idx already done).
                distance = coordinates[idx + 1].DistanceReal(pointGeo).Value;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestProjected = coordinates[idx + 1];
                    bestOffset = currentOffset;
                    bestPosition = LinePointPosition.On;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the location of the given position using the given coordinates as the polyline.
        /// </summary>
        /// <param name="coordinates">The polyline coordinates.</param>
        /// <param name="position">The position parameter [0-1].</param>
        /// <returns></returns>
        public static GeoCoordinate GetPositionLocation(this List<GeoCoordinate> coordinates, double position)
        {
            if (coordinates == null || coordinates.Count == 0) { throw new ArgumentOutOfRangeException("coordinates","List of coordinates cannot be empty!"); }
            if (position < 0 || position > 1) { throw new ArgumentOutOfRangeException("position", "Position has to be in range [0-1]."); }

            if(coordinates.Count == 1)
            { // only one point, location is always the point itself.
                return coordinates[0];
            }

            var lengthAtPosition = coordinates.Length().Value * position;
            var lengthAtPrevious = 0.0;
            var previous = coordinates[0];
            for(int idx = 1; idx < coordinates.Count; idx++)
            {
                var current = coordinates[idx];
                var segmentLength = current.DistanceReal(previous).Value;
                if(lengthAtPrevious + segmentLength > lengthAtPosition)
                { // the point is in this segment.
                    var localPosition = (lengthAtPosition - lengthAtPrevious) / segmentLength;
                    var direction = current - previous;
                    var location = previous + (direction * localPosition);
                    return new GeoCoordinate(location);
                }
                lengthAtPrevious = lengthAtPrevious + segmentLength;
                previous = current;
            }
            return coordinates[coordinates.Count - 1];
        }

        /// <summary>
        /// Converts the given coordinate to a geocoordinate.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public static GeoCoordinate ToGeoCoordinate(this OpenLR.Model.Coordinate coordinate)
        {
            return new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
        }

        /// <summary>
        /// Converts the given coordinate to a GeoAPI coordinate.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public static Coordinate ToGeoAPICoordinate(this OpenLR.Model.Coordinate coordinate)
        {
            return new Coordinate(coordinate.Longitude, coordinate.Latitude);
        }

        /// <summary>
        /// Converts the given coordinate to an NTS feature.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public static Feature ToFeature(this OpenLR.Model.Coordinate coordinate)
        {
            return new Feature(new Point(coordinate.ToGeoAPICoordinate()), new AttributesTable());
        }
    }
}