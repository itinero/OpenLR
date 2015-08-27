using OpenLR.Model;
using OsmSharp.Math;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Primitives;
using OsmSharp.Units.Angle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.Referenced.Encoding
{
    /// <summary>
    /// Holds bearing encoder code.
    /// </summary>
    public static class BearingEncoder
    {
        /// <summary>
        /// Encodes a bearing based on the list of coordinates and the BEARDIST parameter.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public static Degree EncodeBearing(List<GeoCoordinate> coordinates)
        {
            var distance = 0.0;
            var previous = coordinates[0];
            PointF2D bearingPosition = null;
            for (int idx = 1; idx < coordinates.Count; idx++)
            {
                var current = new GeoCoordinate(coordinates[idx].Latitude, coordinates[idx].Longitude);
                var currentSegmentDistance = current.DistanceReal(previous).Value;
                var currentDistance = currentSegmentDistance + distance;
                if (currentDistance > Parameters.BEARDIST)
                { // the coordinate to calculate the beardist is in this segment!
                    // calculate where.
                    var relativeDistance = Parameters.BEARDIST - distance;
                    var relativeOffset = relativeDistance / currentSegmentDistance;

                    bearingPosition = previous + ((current - previous) * relativeOffset);
                    break;
                }
                distance = currentDistance;
                previous = current;
            }
            if (bearingPosition == null)
            { // use the toCoordinate as the last 'current'.
                // if edge is too short use target coordinate.
                bearingPosition = coordinates[coordinates.Count - 1];
            }

            // calculate offset.
            var offset = (bearingPosition - coordinates[0]);

            // convert offset to meters.
            var north = new VectorF2D(0, 1); // north.
            var xMeters = new GeoCoordinate(coordinates[0].Latitude, coordinates[0].Longitude + offset[0]).DistanceReal(
                coordinates[0]).Value;
            if(offset[0] < 0)
            { // invert offset.
                xMeters = -xMeters;
            }
            var yMeters = new GeoCoordinate(coordinates[0].Latitude + offset[1], coordinates[0].Longitude).DistanceReal(
                coordinates[0]).Value;
            if (offset[1] < 0)
            { // invert offset.
                yMeters = -yMeters;
            }
            var direction = new VectorF2D(xMeters, yMeters).Normalize();

            return direction.Angle(north);
        }

        /// <summary>
        /// Encodes a bearing based on the list of coordinates and the BEARDIST parameter.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="startAtEnd"></param>
        /// <returns></returns>
        public static Degree EncodeBearing(List<GeoCoordinate> coordinates, bool startAtEnd)
        {
            if(startAtEnd)
            {
                return BearingEncoder.EncodeBearing(new List<GeoCoordinate>(coordinates.Reverse<GeoCoordinate>()));
            }
            return BearingEncoder.EncodeBearing(coordinates);
        }
    }
}