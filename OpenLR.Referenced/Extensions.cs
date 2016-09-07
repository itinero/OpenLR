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
            var feature = baseEncoder.GetVertexLocation(referencedPointALongLineLocation.Route.Vertices[0]).ToFeature();
            feature.Attributes.AddAttribute("type", "start");
            featureCollection.Add(feature);
            feature = baseEncoder.GetVertexLocation(referencedPointALongLineLocation.Route.Vertices[
                referencedPointALongLineLocation.Route.Vertices.Length - 1]).ToFeature();
            feature.Attributes.AddAttribute("type", "end");
            featureCollection.Add(feature);

            // create a feature for the actual location.
            var locationCoordinate = new Coordinate(referencedPointALongLineLocation.Longitude, referencedPointALongLineLocation.Latitude);
            var locationCoordinateFeature = new Feature(new Point(locationCoordinate), new AttributesTable());
            featureCollection.Add(locationCoordinateFeature);

            return featureCollection;
        }

        private static NetTopologySuite.IO.GeoJsonWriter _geojsonWriter = null; 

        /// <summary>
        /// Converts the given feature collection into it's equivalent GeoJSON representation.
        /// </summary>
        /// <returns></returns>
        public static string ToGeoJson(this FeatureCollection features)
        {
            if (_geojsonWriter == null)
            {
                _geojsonWriter = new NetTopologySuite.IO.GeoJsonWriter();
            }
            return _geojsonWriter.Write(features);
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
        /// Converts the referenced line location to a list of sorted coordinates.
        /// </summary>
        public static List<GeoCoordinate> GetCoordinates(this ReferencedLine referencedLine, Router.BasicRouterDataSource<LiveEdge> data)
        {
            return referencedLine.GetCoordinates(data, 0, referencedLine.Vertices.Length);
        }

        /// <summary>
        /// Converts the referenced line location to a list of sorted coordinates.
        /// </summary>
        public static List<GeoCoordinate> GetCoordinates(this ReferencedLine referencedLine, Router.BasicRouterDataSource<LiveEdge> data, int start, int count)
        {
            var coordinates = new List<GeoCoordinate>();
            if (count <= 0)
            {
                return coordinates;
            }
            coordinates.Add(data.GetVertexLocation(referencedLine.Vertices[start]));
            for (var i = start; i < start + count - 1; i++)
            {
                if (referencedLine.EdgeShapes[i] != null)
                {
                    for (var j = 0; j < referencedLine.EdgeShapes[i].Length; j++)
                    {
                        coordinates.Add(new GeoCoordinate(
                            referencedLine.EdgeShapes[i][j].Latitude, referencedLine.EdgeShapes[i][j].Longitude));
                    }
                }
                coordinates.Add(data.GetVertexLocation(referencedLine.Vertices[i + 1]));
            }
            return coordinates;
        }
        
        /// <summary>
        /// Returns the location of the given vertex.
        /// </summary>
        public static GeoCoordinate GetVertexLocation(this Router.BasicRouterDataSource<LiveEdge> data, long vertex)
        {
            float latitude, longitude;
            if (!data.GetVertex(vertex, out latitude, out longitude))
            { // oeps, vertex does not exist!
                throw new ArgumentOutOfRangeException("vertex", string.Format("Vertex {0} not found!", vertex));
            }
            return new GeoCoordinate(latitude, longitude);
        }

        /// <summary>
        /// Converts the referenced line location to a list of sorted coordinates.
        /// </summary>
        /// <returns></returns>
        public static List<GeoCoordinate> GetCoordinates(this ReferencedLine route, ReferencedDecoderBase decoder, double offsetRatio, 
            out int offsetEdgeIdx, out GeoCoordinate offsetLocation, out Meter offsetLength, out Meter offsetEdgeLength, out Meter edgeLength)
        {            
            if (route == null) { throw new ArgumentNullException("route"); }
            if (route.Edges == null || route.Edges.Length == 0) { throw new ArgumentOutOfRangeException("route", "Route has no edges."); }
            if (route.Vertices == null || route.Vertices.Length == 0) { throw new ArgumentOutOfRangeException("route", "Route has no vertices."); }
            if (route.Vertices.Length != route.Edges.Length + 1) { throw new ArgumentOutOfRangeException("route", "Route is invalid: there should be n vertices and n-1 edges."); }

            // calculate the total length first.
            var totalLength = route.GetCoordinates(decoder.Graph).Length();

            // calculate the lenght at the offst.
            offsetLength = (Meter)(totalLength.Value * offsetRatio);
            offsetEdgeLength = -1;
            offsetEdgeIdx = -1;
            edgeLength = -1;

            // loop over all coordinates and collect offsetLocation and offsetEdgeIdx.
            double currentOffsetLength = 0;
            double currentEdgeLength = 0;
            var coordinates = new List<GeoCoordinate>();
            coordinates.Add(decoder.GetCoordinate(route.Vertices[0]).ToGeoCoordinate());
            for (int edgeIdx = 0; edgeIdx < route.Edges.Length; edgeIdx++)
            {
                currentEdgeLength = 0;
                var edge = route.Edges[edgeIdx];
                var edgeShapes = route.EdgeShapes[edgeIdx];
                if (edgeShapes != null)
                { // there are intermediate coordinates.
                    for (int idx = 0; idx < edgeShapes.Length; idx++)
                    {
                        coordinates.Add(new GeoCoordinate(edgeShapes[idx].Latitude, edgeShapes[idx].Longitude));
                        currentEdgeLength = currentEdgeLength + coordinates[coordinates.Count - 2].DistanceEstimate(coordinates[coordinates.Count - 1]).Value;
                    }
                }
                coordinates.Add(decoder.GetCoordinate(route.Vertices[edgeIdx + 1]).ToGeoCoordinate());
                currentEdgeLength = currentEdgeLength + coordinates[coordinates.Count - 2].DistanceEstimate(coordinates[coordinates.Count - 1]).Value;

                // add current edge length to current offset.
                if ((currentOffsetLength + currentEdgeLength) >= offsetLength.Value &&
                    edgeLength.Value < 0)
                { // it's this edge that has the valuable info.
                    offsetEdgeIdx = edgeIdx;
                    offsetEdgeLength = offsetLength - currentOffsetLength;
                    edgeLength = currentEdgeLength;
                }
                currentOffsetLength = currentOffsetLength + currentEdgeLength;
            }

            // choose the last edge.
            if (edgeLength.Value < 0)
            { // it's this edge that has the valuable info.
                offsetEdgeIdx = route.Edges.Length - 1;
                offsetEdgeLength = offsetLength - currentOffsetLength;
                edgeLength = currentEdgeLength;
            }

            // calculate actual offset position.
            offsetLocation = coordinates.GetPositionLocation(offsetRatio);
            return coordinates;
        }

        /// <summary>
        /// Converts the referenced point along the line location to features.
        /// </summary>
        /// <returns></returns>
        public static FeatureCollection ToFeatures(this ReferencedPointAlongLine referencedPointALongLineLocation, ReferencedDecoderBase baseDecoder)
        {
            // create the geometry factory.
            var geometryFactory = new GeometryFactory();

            // create the feature collection.
            var featureCollection = referencedPointALongLineLocation.Route.ToFeatures();

            // create the coordinates.
            var feature = baseDecoder.GetVertexLocation(referencedPointALongLineLocation.Route.Vertices[0]).ToFeature();
            feature.Attributes.AddAttribute("type", "start");
            featureCollection.Add(feature);
            feature = baseDecoder.GetVertexLocation(referencedPointALongLineLocation.Route.Vertices[
                referencedPointALongLineLocation.Route.Vertices.Length - 1]).ToFeature();
            feature.Attributes.AddAttribute("type", "end");
            featureCollection.Add(feature);

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
            var boxSizeStart = 0.001;
            var boxSizeMax = 0.02;
            var result = graph.GetClosestEdge(location, maxDistance, boxSizeStart);
            while (result == null && boxSizeStart <= boxSizeMax)
            {
                boxSizeStart = boxSizeStart * 2;
                result = graph.GetClosestEdge(location, maxDistance, boxSizeStart);
            }
            return result;
        }

        /// <summary>
        /// Gets the location of the given vertex.
        /// </summary>
        /// <returns></returns>
        public static GeoCoordinate GetCoordinate(this BasicRouterDataSource<LiveEdge> graph, uint vertex)
        {
            float latitude, longitude;
            if(graph.GetVertex(vertex, out latitude, out longitude))
            {
                return new GeoCoordinate(latitude, longitude);
            }
            throw new ArgumentOutOfRangeException("Vertex does not exit.");
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

            while (boxSize < 1 && bestEdge == null)
            {
                var searchBox = new GeoCoordinateBox(
                    new GeoCoordinate(location.Latitude - (boxSize / 2), location.Longitude - boxSize),
                    new GeoCoordinate(location.Latitude + (boxSize / 2), location.Longitude + boxSize));

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
                    var distance = from.DistanceEstimate(location).Value;
                    if (distance < bestDistance)
                    { // first point is closer.
                        bestEdge = new Tuple<long, long, LiveEdge>(arc.Item1, arc.Item2, arc.Item3);
                        bestDistance = distance;
                    }

                    graph.GetVertex(arc.Item2, out latitude, out longitude);
                    var to = new GeoCoordinate(latitude, longitude);
                    distance = to.DistanceEstimate(location).Value;
                    if (distance < bestDistance)
                    { // second point is closer.
                        bestEdge = new Tuple<long, long, LiveEdge>(arc.Item1, arc.Item2, arc.Item3);
                        bestDistance = distance;
                    }

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
                            distance = coordinates[idx].DistanceEstimate(location).Value;
                            if (distance < bestDistance)
                            {
                                bestEdge = new Tuple<long, long, LiveEdge>(arc.Item1, arc.Item2, arc.Item3);
                                bestDistance = distance;
                            }
                            if (maxDistanceBox.IntersectsPotentially(coordinates[idx - 1], coordinates[idx]))
                            {
                                var line = new GeoCoordinateLine(coordinates[idx - 1], coordinates[idx], true, true);
                                distance = line.DistanceReal(location).Value;
                                if (distance < bestDistance)
                                {
                                    bestEdge = new Tuple<long, long, LiveEdge>(arc.Item1, arc.Item2, arc.Item3);
                                    bestDistance = distance;
                                }
                            }
                        }
                    }
                }

                boxSize *= 2;
            }
            return bestEdge;
        }

        /// <summary>
        /// Returns the edge that is closest to the given location.
        /// </summary>
        /// <param name="graph">The graph to search.</param>
        /// <param name="location1">The location of one of the points of the edge.</param>
        /// <param name="location2">The location of one of the points of the edge.</param>
        /// <param name="maxDistance">The maximum distance.</param>
        /// <returns>Returns an edge or an edge from 0 to 0 if none is found.</returns>
        public static Tuple<long, long, LiveEdge> GetClosestEdge(this BasicRouterDataSource<LiveEdge> graph, GeoCoordinate location1, 
            GeoCoordinate location2, Meter maxDistance)
        { // create the search box.
            var boxSizeStart = 0.001;
            var boxSizeMax = 0.02;
            var result = graph.GetClosestEdge(location1, location2, maxDistance, boxSizeStart);
            while (result == null && boxSizeStart <= boxSizeMax)
            {
                boxSizeStart = boxSizeStart * 2;
                result = graph.GetClosestEdge(location1, location2, maxDistance, boxSizeStart);
            }
            return result;
        }

        /// <summary>
        /// Returns the edge that is closest to the given location.
        /// </summary>
        /// <param name="graph">The graph to search.</param>
        /// <param name="location1">The location of one of the points of the edge.</param>
        /// <param name="location2">The location of one of the points of the edge.</param>
        /// <param name="maxDistance">The maximum distance.</param>
        /// <param name="boxSize">The search box size.</param>
        /// <returns>Returns an edge or an edge from 0 to 0 if none is found.</returns>
        /// <returns></returns>
        public static Tuple<long, long, LiveEdge> GetClosestEdge(this BasicRouterDataSource<LiveEdge> graph, GeoCoordinate location1, 
            GeoCoordinate location2, Meter maxDistance, double boxSize)
        {
            Tuple<long, long, LiveEdge> bestEdge = null;

            var searchBox1 = new GeoCoordinateBox(
                new GeoCoordinate(location1.Latitude - boxSize, location1.Longitude - boxSize),
                new GeoCoordinate(location1.Latitude + boxSize, location1.Longitude + boxSize));
            var searchBox2 = new GeoCoordinateBox(
                new GeoCoordinate(location2.Latitude - boxSize, location2.Longitude - boxSize),
                new GeoCoordinate(location2.Latitude + boxSize, location2.Longitude + boxSize));
            var searchBox = searchBox1 + searchBox2;

            var maxDistanceBox1 = searchBox1;
            var maxDistanceBox2 = searchBox2;
            if (maxDistance.Value < double.MaxValue)
            {
                maxDistanceBox1 = new GeoCoordinateBox(
                    location1.OffsetWithDirection(maxDistance, OsmSharp.Math.Geo.Meta.DirectionEnum.East).OffsetWithDirection(
                    maxDistance, OsmSharp.Math.Geo.Meta.DirectionEnum.South),
                    location1.OffsetWithDirection(maxDistance, OsmSharp.Math.Geo.Meta.DirectionEnum.West).OffsetWithDirection(
                    maxDistance, OsmSharp.Math.Geo.Meta.DirectionEnum.North));
                maxDistanceBox2 = new GeoCoordinateBox(
                    location2.OffsetWithDirection(maxDistance, OsmSharp.Math.Geo.Meta.DirectionEnum.East).OffsetWithDirection(
                    maxDistance, OsmSharp.Math.Geo.Meta.DirectionEnum.South),
                    location2.OffsetWithDirection(maxDistance, OsmSharp.Math.Geo.Meta.DirectionEnum.West).OffsetWithDirection(
                    maxDistance, OsmSharp.Math.Geo.Meta.DirectionEnum.North));
            }
            var arcs = graph.GetEdges(searchBox);

            float latitude, longitude;
            var bestDistance = double.MaxValue;
            foreach (var arc in arcs)
            {
                graph.GetVertex(arc.Item1, out latitude, out longitude);
                var vertex1Location = new GeoCoordinate(latitude, longitude);
                graph.GetVertex(arc.Item2, out latitude, out longitude);
                var vertex2Location = new GeoCoordinate(latitude, longitude);

                if(maxDistanceBox1.Contains(vertex1Location) &&
                    maxDistanceBox2.Contains(vertex2Location))
                { // both vertices within tolerance, add distances and check if better than the current.
                    var distance = vertex1Location.Distance(location1) + vertex2Location.Distance(location2);
                    if(distance < bestDistance)
                    {
                        bestEdge = new Tuple<long, long, LiveEdge>(arc.Item1, arc.Item2, arc.Item3);
                        bestDistance = distance;
                    }
                }
                else if (maxDistanceBox1.Contains(vertex2Location) &&
                    maxDistanceBox2.Contains(vertex1Location))
                { // both vertices within tolerance, add distances and check if better than the current.
                    var distance = vertex1Location.Distance(location2) + vertex2Location.Distance(location1);
                    if (distance < bestDistance)
                    {
                        bestEdge = new Tuple<long, long, LiveEdge>(arc.Item2, arc.Item1, (LiveEdge)arc.Item3.Reverse());
                        bestDistance = distance;
                    }
                }
            }
            return bestEdge;
        }

        /// <summary>
        /// Returns the vertex that is closest to the given location.
        /// </summary>
        public static Tuple<long, double> GetClosestVertex(this BasicRouterDataSource<LiveEdge> graph, GeoCoordinate location)
        {
            return graph.GetClosestVertex(location, 10000); // default of 10km.
        }

        /// <summary>
        /// Returns the vertex that is closest to the given location.
        /// </summary>
        public static Tuple<long, double> GetClosestVertex(this BasicRouterDataSource<LiveEdge> graph, GeoCoordinate location, 
            Meter maxDistance)
        {
            var offsetLocationNorth = location.OffsetWithDirection(maxDistance, OsmSharp.Math.Geo.Meta.DirectionEnum.North);
            var offsetLocationEast = location.OffsetWithDirection(maxDistance, OsmSharp.Math.Geo.Meta.DirectionEnum.East);
            var boxSize = (System.Math.Max(
                System.Math.Abs(location.Latitude - offsetLocationNorth.Latitude),
                System.Math.Abs(location.Longitude - offsetLocationEast.Longitude))) * 1.5;
            return graph.GetClosestVertex(location, maxDistance, boxSize);
        }

        /// <summary>
        /// Returns the vertex that is closest to the given location.
        /// </summary>
        public static Tuple<long, double> GetClosestVertex(this BasicRouterDataSource<LiveEdge> graph, GeoCoordinate location, 
            Meter maxDistance, double boxSize)
        {
            Tuple<long, double> bestVertex = null;

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
                var vertexLocation = new GeoCoordinate(latitude, longitude);
                var distance = vertexLocation.DistanceEstimate(location).Value;
                if (distance < maxDistance.Value)
                { // distance within tolerance.
                    if(bestVertex == null || distance < bestVertex.Item2)
                    { // better vertex.
                        bestVertex = new Tuple<long, double>(arc.Item1, distance);
                    }
                }
                graph.GetVertex(arc.Item2, out latitude, out longitude);
                vertexLocation = new GeoCoordinate(latitude, longitude);
                distance = vertexLocation.DistanceEstimate(location).Value;
                if (distance < maxDistance.Value)
                { // distance within tolerance.
                    if (bestVertex == null || distance < bestVertex.Item2)
                    { // better vertex.
                        bestVertex = new Tuple<long, double>(arc.Item2, distance);
                    }
                }
            }
            return bestVertex;
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
                if(edge.Item3.Forward)
                {
                    return edge.Item3.GetCoordinates(shape.ToSimpleArray(), from, to);
                }
                return edge.Item3.GetCoordinates(shape.Reverse().ToSimpleArray(), from, to);
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
                meter = meter + coordinates[idx].DistanceReal(coordinates[idx + 1]).Value;
            }
            return meter;
        }

        /// <summary>
        /// Projects the given point on the line represented by the coordinates.
        /// </summary>
        /// <returns></returns>
        public static bool ProjectOn(this List<GeoCoordinate> coordinates, PointF2D point, out PointF2D bestProjected, 
            out LinePointPosition bestPosition, out Meter bestOffset, out int bestIndex)
        {
            // check first point.
            var pointGeo = new GeoCoordinate(point);
            var distance = coordinates[0].DistanceReal(pointGeo).Value;
            bestProjected = coordinates[0];
            bestOffset = 0;
            bestPosition = LinePointPosition.On;
            bestIndex = -1;

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
                    distance = (new GeoCoordinate(projected)).DistanceReal(new GeoCoordinate(point)).Value;
                    if(distance < bestDistance)
                    { // this point is closer.
                        bestDistance = distance;
                        bestProjected = projected;
                        bestOffset = currentOffset + offset;
                        bestIndex = idx;
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
                    bestIndex = idx + 1;
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
        /// Returns a feature representing the geometry of the edge.
        /// </summary>
        /// <returns></returns>
        public static Feature ToFeature(this BasicRouterDataSource<LiveEdge> graph, Tuple<long, long, LiveEdge> edge)
        {
            var coordinates = graph.GetCoordinates(edge);
            var geoApiCoordinates = new Coordinate[coordinates.Count];
            for(var  i = 0; i < coordinates.Count; i++)
            {
                geoApiCoordinates[i] = new Coordinate(coordinates[i].Longitude, coordinates[i].Latitude);
            }
            return new Feature(new LineString(geoApiCoordinates), new AttributesTable());
        }

        /// <summary>
        /// Returns a feature representing the geometry of the edge.
        /// </summary>
        /// <returns></returns>
        public static FeatureCollection ToFeatures(this BasicRouterDataSource<LiveEdge> graph, Tuple<long, long, LiveEdge> edge)
        {
            var coordinates = graph.GetCoordinates(edge);
            var geoApiCoordinates = new Coordinate[coordinates.Count];
            for (var i = 0; i < coordinates.Count; i++)
            {
                geoApiCoordinates[i] = new Coordinate(coordinates[i].Longitude, coordinates[i].Latitude);
            }
            var feature = new Feature(new LineString(geoApiCoordinates), new AttributesTable());
            var features = new FeatureCollection();
            features.Add(feature);
            return features;
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

        /// <summary>
        /// Gets the positive offset location.
        /// </summary>
        public static GeoCoordinate GetPositiveOffsetLocation(this ReferencedLine referencedLine, Router.BasicRouterDataSource<LiveEdge> data)
        {
            var coordinates = referencedLine.GetCoordinates(data);

            return coordinates.GetPositionLocation(referencedLine.PositiveOffsetPercentage / 100.0f);
        }

        /// <summary>
        /// Gets the negative offset location.
        /// </summary>
        public static GeoCoordinate GetNegativeOffsetLocation(this ReferencedLine referencedLine, Router.BasicRouterDataSource<LiveEdge> data)
        {
            var coordinates = referencedLine.GetCoordinates(data);

            return coordinates.GetPositionLocation(1f - (referencedLine.NegativeOffsetPercentage / 100.0f));
        }

        /// <summary>
        /// Converts the coordinate to a geo api coordinate.
        /// </summary>
        public static GeoAPI.Geometries.Coordinate ToGeoAPICoordinate(this GeoCoordinate coordinate)
        {
            return new Coordinate(coordinate.Longitude, coordinate.Latitude);
        }
    }
}