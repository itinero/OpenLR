using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.Utilities;
using OpenLR.Locations;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
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
            featureCollection.Add(pointAlongLineLocation.First.ToFeature());
            featureCollection.Add(pointAlongLineLocation.Last.ToFeature());
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
            pointAttributes.AddAttribute("bearing_distance", locationReferencePoint.BearingDistance);
            pointAttributes.AddAttribute("distance_to_next", locationReferencePoint.DistanceToNext);
            pointAttributes.AddAttribute("form_of_way", locationReferencePoint.FormOfWay);
            pointAttributes.AddAttribute("functional_road_class", locationReferencePoint.FuntionalRoadClass);
            pointAttributes.AddAttribute("lowest_functional_road_class_to_next", locationReferencePoint.LowestFunctionalRoadClassToNext);
            return new Feature(point, pointAttributes);
        }

        /// <summary>
        /// Returns the edge that is closest to the given location.
        /// </summary>
        /// <typeparam name="TEdge"></typeparam>
        /// <param name="graph"></param>
        /// <param name="location"></param>
        /// <returns>Returns an edge or an edge from 0 to 0 if none is found.</returns>
        public static KeyValuePair<uint, KeyValuePair<uint, TEdge>> GetClosestEdge<TEdge>(this IBasicRouterDataSource<TEdge> graph, GeoCoordinate location)
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
            var bestEdge = new KeyValuePair<uint,KeyValuePair<uint,TEdge>>(0, new KeyValuePair<uint,TEdge>(0, default(TEdge)));
            foreach(var arc in arcs)
            {
                graph.GetVertex(arc.Key, out latitude, out longitude);
                var from = new GeoCoordinate(latitude, longitude);
                graph.GetVertex(arc.Value.Key, out latitude, out longitude);
                var to = new GeoCoordinate(latitude, longitude);

                var line = new GeoCoordinateLine(from, to, true, true);

                var distance = line.Distance(location);
                if(distance <  bestDistance)
                {
                    bestEdge = arc;
                    bestDistance = distance;
                }
            }
            return bestEdge;
        }
    }
}