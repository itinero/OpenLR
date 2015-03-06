using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.Utilities;
using OpenLR.Locations;
using OpenLR.Referenced.Locations;
using OpenLR.Referenced.Router;
using OsmSharp;
using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Units.Distance;
using System;
using System.Collections.Generic;

namespace OpenLR.Referenced
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
            lineAttributes.AddAttribute("negative_offset", lineLocation.NegativeOffsetPercentage.HasValue ? lineLocation.NegativeOffsetPercentage.Value : 0);
            lineAttributes.AddAttribute("positive_offset", lineLocation.PositiveOffsetPercentage.HasValue ? lineLocation.PositiveOffsetPercentage.Value : 0);
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
        public static FeatureCollection ToFeatures(this ReferencedPointAlongLine referencedPointALongLineLocation, ReferencedEncoderBase baseEncoder)
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
        public static Meter Length(this ReferencedPointAlongLine referencedPointALongLineLocation, ReferencedEncoderBase baseEncoder)
        {
            return referencedPointALongLineLocation.Route.Length(baseEncoder);
        }

        /// <summary>
        /// Converts the referenced point along the line location to features.
        /// </summary>
        /// <param name="referencedLine"></param>
        /// <param name="baseEncoder"></param>
        /// <returns></returns>
        public static Meter Length(this ReferencedLine referencedLine, ReferencedEncoderBase baseEncoder)
        {
            var length = 0.0;
            for (int idx = 0; idx < referencedLine.Edges.Length; idx++)
            {
                length = length + baseEncoder.Graph.GetCoordinates(new Tuple<long, long, LiveEdge>(
                    referencedLine.Vertices[idx], referencedLine.Vertices[idx + 1],
                    referencedLine.Edges[idx])).Length().Value;
            }
            return length;
        }

        /// <summary>
        /// Converts the referenced point along the line location to features.
        /// </summary>
        /// <param name="referencedPointALongLineLocation"></param>
        /// <param name="baseEncoder"></param>
        /// <returns></returns>
        public static List<GeoCoordinate> GetCoordinates(this ReferencedPointAlongLine referencedPointALongLineLocation, ReferencedEncoderBase baseEncoder)
        {
            var coordinates = new List<GeoCoordinate>();
            for (int idx = 0; idx < referencedPointALongLineLocation.Route.Edges.Length; idx++)
            {
                var from = baseEncoder.GetVertexLocation(referencedPointALongLineLocation.Route.Vertices[idx]).ToGeoCoordinate();
                var to = baseEncoder.GetVertexLocation(referencedPointALongLineLocation.Route.Vertices[idx + 1]).ToGeoCoordinate();
                var edgeCoordinates = referencedPointALongLineLocation.Route.Edges[idx].GetCoordinates(
                    referencedPointALongLineLocation.Route.EdgeShapes[idx], from, to);
                if (coordinates.Count > 0)
                {
                    coordinates.RemoveAt(coordinates.Count - 1);

                }
                coordinates.AddRange(edgeCoordinates);
            }
            return coordinates;
        }

        /// <summary>
        /// Converts the referenced line location to features.
        /// </summary>
        /// <param name="referencedLine">The referenced line.</param>
        /// <param name="encoder">The encoder.</param>
        /// <returns></returns>
        public static List<GeoCoordinate> GetCoordinates(this ReferencedLine referencedLine, ReferencedEncoderBase encoder)
        {
            return referencedLine.GetCoordinates(encoder, 0, referencedLine.Vertices.Length);
        }

        /// <summary>
        /// Converts the referenced line location to features.
        /// </summary>
        /// <param name="referencedLine">The referenced line.</param>
        /// <param name="encoder">The encoder.</param>
        /// <param name="start">The start vertex.</param>
        /// <param name="count">The vertices to return coordinates for.</param>
        /// <returns></returns>
        public static List<GeoCoordinate> GetCoordinates(this ReferencedLine referencedLine, ReferencedEncoderBase encoder, int start, int count)
        {
            var coordinates = new List<GeoCoordinate>();
            for (int idx = start; idx < start + count - 1; idx++)
            {
                var fromLocation = encoder.GetVertexLocation(referencedLine.Vertices[idx]).ToGeoCoordinate();
                var toLocation = encoder.GetVertexLocation(referencedLine.Vertices[idx + 1]).ToGeoCoordinate();
                var edgeCoordinates = referencedLine.Edges[idx].GetCoordinates(
                    referencedLine.EdgeShapes[idx], fromLocation, toLocation);
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
        public static FeatureCollection ToFeatures(this ReferencedPointAlongLine referencedPointALongLineLocation, ReferencedDecoderBase baseDecoder)
        {
            // create the geometry factory.
            var geometryFactory = new GeometryFactory();

            // create the feature collection.
            var featureCollection = referencedPointALongLineLocation.Route.ToFeatures();

            // create the coordinates.
            featureCollection.Add(baseDecoder.GetVertexLocation(referencedPointALongLineLocation.Route.Vertices[0]).ToFeature());
            featureCollection.Add(baseDecoder.GetVertexLocation(referencedPointALongLineLocation.Route.Vertices[referencedPointALongLineLocation.Route.Vertices.Length - 1]).ToFeature());

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
        /// <param name="graph">The graph to search.</param>
        /// <param name="location">The location.</param>
        /// <returns>Returns an edge or an edge from 0 to 0 if none is found.</returns>
        public static Tuple<long, long, LiveEdge> GetClosestEdge(this BasicRouterDataSource<LiveEdge> graph, GeoCoordinate location)
        {
            return graph.GetClosestEdge(location, double.MaxValue);
        }

        /// <summary>
        /// Returns the edge that is closest to the given location.
        /// </summary>
        /// <param name="graph">The graph to search.</param>
        /// <param name="location">The location.</param>
        /// <param name="maxDistance">The maximum distance.</param>
        /// <returns>Returns an edge or an edge from 0 to 0 if none is found.</returns>
        public static Tuple<long, long, LiveEdge> GetClosestEdge(this BasicRouterDataSource<LiveEdge> graph, GeoCoordinate location, Meter maxDistance)
        { // create the search box.
            var boxSizeStart = 0.0001;
            var boxSizeMax = 0.2;
            var result = graph.GetClosestEdge(location, maxDistance, boxSizeStart);
            while(result == null && boxSizeStart <= boxSizeMax)
            {
                boxSizeStart = boxSizeStart * 2;
                result = graph.GetClosestEdge(location, maxDistance, boxSizeStart);
            }
            return result;
        }

        /// <summary>
        /// Returns the edge that is closest to the given location.
        /// </summary>
        /// <param name="graph">The graph to search.</param>
        /// <param name="location">The location.</param>
        /// <param name="maxDistance">The maximum distance.</param>
        /// <param name="boxSize">The search box size.</param>
        /// <returns>Returns an edge or an edge from 0 to 0 if none is found.</returns>
        public static Tuple<long, long, LiveEdge> GetClosestEdge(this BasicRouterDataSource<LiveEdge> graph, GeoCoordinate location, Meter maxDistance, double boxSize)
        {
            Tuple<long, long, LiveEdge> bestEdge = null;

            var searchBox = new GeoCoordinateBox(
                new GeoCoordinate(location.Latitude - boxSize, location.Longitude - boxSize),
                new GeoCoordinate(location.Latitude + boxSize, location.Longitude + boxSize));

            var maxDistanceBox = searchBox;
            if (maxDistance.Value < double.MaxValue)
            {
                maxDistanceBox = new GeoCoordinateBox(
                    location.OffsetWithDirection(maxDistance, OsmSharp.Math.Geo.Meta.DirectionEnum.East).OffsetWithDirection(maxDistance, OsmSharp.Math.Geo.Meta.DirectionEnum.South),
                    location.OffsetWithDirection(maxDistance, OsmSharp.Math.Geo.Meta.DirectionEnum.West).OffsetWithDirection(maxDistance, OsmSharp.Math.Geo.Meta.DirectionEnum.North));
            }
            var arcs = graph.GetEdges(searchBox);

            float latitude, longitude;
            var bestDistance = maxDistance.Value;
            foreach (var arc in arcs)
            {
                graph.GetVertex(arc.Item1, out latitude, out longitude);
                var from = new GeoCoordinate(latitude, longitude);
                graph.GetVertex(arc.Item2, out latitude, out longitude);
                var to = new GeoCoordinate(latitude, longitude);

                var coordinates = new List<GeoCoordinate>();
                ICoordinateCollection shape = arc.Item4;
                if (shape != null)
                { // a non-null shape.
                    coordinates.Add(from);
                    coordinates.AddRange(shape.ToArray());
                    coordinates.Add(to);
                }
                else
                { // no shape, only add from/to.
                    coordinates.Add(from);
                    coordinates.Add(to);
                }
                if (coordinates != null && coordinates.Count > 0)
                {
                    for (int idx = 1; idx < coordinates.Count; idx++)
                    {
                        if (maxDistanceBox.IntersectsPotentially(coordinates[idx - 1], coordinates[idx]))
                        {
                            var line = new GeoCoordinateLine(coordinates[idx - 1], coordinates[idx], true, true);
                            var distance = line.Distance(location);
                            if (distance < bestDistance)
                            {
                                bestEdge = new Tuple<long, long, LiveEdge>(arc.Item1, arc.Item2, arc.Item3);
                                bestDistance = distance;
                            }
                        }
                    }
                }
            }
            return bestEdge;
        }

        /// <summary>
        /// Returns an array containing all coordinates in the given collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static GeoCoordinate[] ToArray(this ICoordinateCollection collection)
        {
            var coordinates = new List<GeoCoordinate>();
            collection.Reset();
            while(collection.MoveNext())
            {
                coordinates.Add(new GeoCoordinate(collection.Latitude, collection.Longitude));
            }
            return coordinates.ToArray();
        }

        /// <summary>
        /// Returns a list of coordinates representing the geometry of the edge.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="edge">The edge-tuple with (from-vertex, to-vertex, edgedata).</param>
        /// <returns></returns>
        public static List<GeoCoordinate> GetCoordinates(this BasicRouterDataSource<LiveEdge> graph, Tuple<long, long, LiveEdge> edge)
        {
            float latitude, longitude;
            graph.GetVertex(edge.Item1, out latitude, out longitude);
            var from = new GeoCoordinate(latitude, longitude);
            graph.GetVertex(edge.Item2, out latitude, out longitude);
            var to = new GeoCoordinate(latitude, longitude);
            ICoordinateCollection shape;
            if(graph.GetEdgeShape(edge.Item1, edge.Item2, out shape) &&
                shape != null)
            {
                return edge.Item3.GetCoordinates(shape.ToSimpleArray(), from, to);
            }
            return edge.Item3.GetCoordinates(new GeoCoordinateSimple[0], from, to);
        }

        /// <summary>
        /// Returns a list of coordinates representing the geometry of the edge.
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="edgeShape"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static List<GeoCoordinate> GetCoordinates(this LiveEdge edge, GeoCoordinateSimple[] edgeShape, GeoCoordinate from, GeoCoordinate to)
        {
            var coordinates = new List<GeoCoordinate>();
            coordinates.Add(from);
            if (edgeShape != null)
            {
                if (edge.Forward)
                {
                    for (int idx = 0; idx < edgeShape.Length; idx++)
                    {
                        coordinates.Add(new GeoCoordinate(edgeShape[idx].Latitude, edgeShape[idx].Longitude));
                    }
                }
                else
                {
                    for (int idx = edgeShape.Length - 1; idx >= 0; idx--)
                    {
                        coordinates.Add(new GeoCoordinate(edgeShape[idx].Latitude, edgeShape[idx].Longitude));
                    }
                }
            }
            coordinates.Add(to);
            return coordinates;
        }

        /// <summary>
        /// Calculates the real length in meters.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <returns></returns>
        public static Meter Length(this List<GeoCoordinate> coordinates)
        {
            return Extensions.Length(coordinates, 0, coordinates.Count);
        }

        /// <summary>
        /// Calculates the real length in meters.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <param name="start">The start index.</param>
        /// <param name="count">The length.</param>
        /// <returns></returns>
        public static Meter Length(this List<GeoCoordinate> coordinates, int start, int count)
        {
            var meter = 0.0;
            for (int idx = start; idx < start + count - 1; idx++)
            {
                meter = meter + GeoCoordinate.DistanceEstimateInMeter(
                    coordinates[idx], coordinates[idx + 1]);
            }
            return meter;
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

        /// <summary>
        /// Converts the edge to it's reverse version.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public static LiveEdge ToReverse(this LiveEdge edge)
        {
            var reverseEdge = new LiveEdge();
            reverseEdge.Tags = edge.Tags;
            reverseEdge.Forward = !edge.Forward;
            reverseEdge.Distance = edge.Distance;
            //reverseEdge.Coordinates = null;
            //if (edge.Coordinates != null)
            //{
            //    var reverse = new GeoCoordinateSimple[edge.Coordinates.Length];
            //    edge.Coordinates.CopyToReverse(reverse, 0);
            //    reverseEdge.Coordinates = reverse;
            //}
            return reverseEdge;
        }
    }
}