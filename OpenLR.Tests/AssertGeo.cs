using GeoAPI.Geometries;
using NetTopologySuite.Algorithm.Distance;
using NetTopologySuite.IO;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.Tests
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
