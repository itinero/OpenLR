using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.Utilities;
using OpenLR.Locations;
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
        /// <param name="closeLineLocation"></param>
        /// <returns></returns>
        public static FeatureCollection ToFeatures(this ClosedLineLocation closeLineLocation)
        {
            // create the geometry factory.
            var geometryFactory = new GeometryFactory();

            // create the feature collection.
            var featureCollection = new FeatureCollection();

            // build the coordinates list and create point features.
            var coordinates = new List<Coordinate>();
            coordinates.Add(new Coordinate(closeLineLocation.First.Coordinate.Longitude, closeLineLocation.First.Coordinate.Latitude));
            featureCollection.Add(closeLineLocation.First.ToFeature());
            if(closeLineLocation.Intermediate != null)
            { // there are intermediate coordinates.
                for(int idx = 0; idx < closeLineLocation.Intermediate.Length; idx++)
                {
                    coordinates.Add(new Coordinate(closeLineLocation.Intermediate[idx].Coordinate.Longitude, closeLineLocation.Intermediate[idx].Coordinate.Latitude));
                    featureCollection.Add(closeLineLocation.Intermediate[idx].ToFeature());
                }
            }
            coordinates.Add(new Coordinate(closeLineLocation.Last.Coordinate.Longitude, closeLineLocation.Last.Coordinate.Latitude));
            featureCollection.Add(closeLineLocation.Last.ToFeature());

            // create a line feature.
            var line = geometryFactory.CreateLineString(coordinates.ToArray());
            var lineAttributes = new AttributesTable();
            var lineFeature = new Feature(line, lineAttributes);
            featureCollection.Add(lineFeature);

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
    }
}