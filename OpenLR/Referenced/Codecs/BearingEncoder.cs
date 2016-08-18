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

using Itinero.LocalGeo;
using Itinero.Navigation.Directions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace OpenLR.Referenced.Codecs
{
    /// <summary>
    /// Holds bearing encoder code.
    /// </summary>
    public static class BearingEncoder
    {
        /// <summary>
        /// Encodes a bearing based on the list of coordinates and the BEARDIST parameter.
        /// </summary>
        public static float EncodeBearing(List<Coordinate> coordinates)
        {
            var distance = 0.0;
            var previous = coordinates[0];
            Coordinate? bearingPosition = null;
            for (int idx = 1; idx < coordinates.Count; idx++)
            {
                var current = new Coordinate(coordinates[idx].Latitude, coordinates[idx].Longitude);
                var currentSegmentDistance = Coordinate.DistanceEstimateInMeter(current, previous);
                var currentDistance = currentSegmentDistance + distance;
                if (currentDistance > Parameters.BEARDIST)
                { // the coordinate to calculate the beardist is in this segment!
                    // calculate where.
                    var relativeDistance = Parameters.BEARDIST - distance;
                    var relativeOffset = relativeDistance / currentSegmentDistance;

                    bearingPosition = new Coordinate()
                    {
                        Latitude = (float)(previous.Latitude + ((current.Latitude - previous.Latitude) * relativeOffset)),
                        Longitude = (float)(previous.Longitude + ((current.Longitude - previous.Longitude) * relativeOffset))
                    };
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
            
            var north = new Coordinate(coordinates[0].Latitude + 1, coordinates[0].Longitude);

            var angleRadians = DirectionCalculator.Angle(bearingPosition.Value, coordinates[0], north);
            var angleDegrees = (float)(angleRadians * (180 / Math.PI));
            return angleDegrees;
        }

        /// <summary>
        /// Encodes a bearing based on the list of coordinates and the BEARDIST parameter.
        /// </summary>
        public static float EncodeBearing(List<Coordinate> coordinates, bool startAtEnd)
        {
            if(startAtEnd)
            {
                return BearingEncoder.EncodeBearing(new List<Coordinate>(coordinates.Reverse<Coordinate>()));
            }
            return BearingEncoder.EncodeBearing(coordinates);
        }

        /// <summary>
        /// Encodes a bearing based on the list of coordinates and the BEARDIST parameter.
        /// </summary>
        public static float EncodeBearing(List<OpenLR.Model.Coordinate> coordinates)
        {
            var distance = 0.0;
            var previous = coordinates[0];
            OpenLR.Model.Coordinate bearingPosition = null;
            for (int idx = 1; idx < coordinates.Count; idx++)
            {
                var current = new OpenLR.Model.Coordinate()
                {
                    Latitude = coordinates[idx].Latitude,
                    Longitude = coordinates[idx].Longitude
                };
                var currentSegmentDistance = Coordinate.DistanceEstimateInMeter((float)current.Latitude, (float)current.Longitude, 
                    (float)previous.Latitude, (float)previous.Longitude);
                var currentDistance = currentSegmentDistance + distance;
                if (currentDistance > Parameters.BEARDIST)
                { // the coordinate to calculate the beardist is in this segment!
                    // calculate where.
                    var relativeDistance = Parameters.BEARDIST - distance;
                    var relativeOffset = relativeDistance / currentSegmentDistance;

                    bearingPosition = new OpenLR.Model.Coordinate()
                    {
                        Latitude = (float)(previous.Latitude + ((current.Latitude - previous.Latitude) * relativeOffset)),
                        Longitude = (float)(previous.Longitude + ((current.Longitude - previous.Longitude) * relativeOffset))
                    };
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

            var north = new Coordinate((float)coordinates[0].Latitude + 1, (float)coordinates[0].Longitude);

            var angleRadians = DirectionCalculator.Angle(new Coordinate((float)bearingPosition.Latitude, (float)bearingPosition.Longitude), 
                new Coordinate((float)coordinates[0].Latitude, (float)coordinates[0].Longitude), north);
            var angleDegrees = (float)(angleRadians * (180 / Math.PI));
            return angleDegrees;
        }

        /// <summary>
        /// Encodes a bearing based on the list of coordinates and the BEARDIST parameter.
        /// </summary>
        public static float EncodeBearing(List<OpenLR.Model.Coordinate> coordinates, bool startAtEnd)
        {
            if (startAtEnd)
            {
                return BearingEncoder.EncodeBearing(new List<OpenLR.Model.Coordinate>(coordinates.Reverse<OpenLR.Model.Coordinate>()));
            }
            return BearingEncoder.EncodeBearing(coordinates);
        }
    }
}