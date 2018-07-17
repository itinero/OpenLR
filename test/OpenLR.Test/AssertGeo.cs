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

using GeoAPI.Geometries;
using NetTopologySuite.Algorithm.Distance;
using NetTopologySuite.IO;
using NUnit.Framework;

namespace OpenLR.Test
{
    /// <summary>
    /// Holds extra assert functionality.
    /// </summary>
    public static class AssertGeo
    {
        /// <summary>
        /// Compares two geometries.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="delta"></param>
        public static void AreEqual(IGeometry expected, IGeometry actual, double delta)
        {
            if(expected is ILineString && actual is ILineString)
            {
                AssertGeo.AreEqual(expected as ILineString, actual as ILineString, delta);
            }
            //else if (expected is ILineString && actual is ILineString)
            //{
            //    Assert.AreEqual(expected as ILineString, actual as ILineString, delta);
            //}
        }

        /// <summary>
        /// Compares two linestrings.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="delta"></param>
        public static void AreEqual(ILineString expected, ILineString actual, double delta)
        {
            var distance = new PointPairDistance();
            foreach(Coordinate actualCoordinate in actual.Coordinates)
		    {
                DistanceToPoint.ComputeDistance(expected, actualCoordinate, distance);
                Assert.AreEqual(0.0, distance.Distance, delta);
            }
            foreach (Coordinate expectedCoordinate in expected.Coordinates)
            {
                DistanceToPoint.ComputeDistance(actual, expectedCoordinate, distance);
                Assert.AreEqual(0.0, distance.Distance, delta);
            }
        }

        /// <summary>
        /// Compares two linestrings.
        /// </summary>
        /// <param name="geoJsonActual"></param>
        /// <param name="geoJson"></param>
        public static void AreEqual(string geoJsonActual, string geoJson, double delta)
        {
            var geoJsonReader = new GeoJsonReader();
            var actual = geoJsonReader.Read<IGeometry>(geoJsonActual);
            var expected = geoJsonReader.Read<IGeometry>(geoJson);

            AssertGeo.AreEqual(actual, expected, delta);
        }
    }
}
