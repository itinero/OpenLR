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
                Latitude  = coordinate.Latitude,
                Longitude = coordinate.Longitude
            };
        }
    }
}