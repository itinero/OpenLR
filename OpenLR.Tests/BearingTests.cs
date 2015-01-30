using NUnit.Framework;
using OpenLR.Binary.Data;
using OpenLR.Model;
using OpenLR.Referenced.Encoding;
using OsmSharp.Math.Geo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.Tests
{
    /// <summary>
    /// Holds tests for converting bearings to/from angles.
    /// </summary>
    [TestFixture]
    public class BearingTests
    {

        /// <summary>
        /// Tests bearing decoding.
        /// </summary>
        [Test]
        public void TestBearingEncoding()
        {
            // (topLeft) ---- 12.5m ---- (topMiddle) ---- 12.5m ---- (topRight)
            //  |                             |                             |
            //  |                             |                             |
            //  |                             |                             |
            // 10m                           10m                           10m
            //  |                             |                             |
            //  |                             |                             |
            //  |                             |                             |
            // (left)    ---- 12.5m ----   (middle)  ---- 12.5m ----    (right)

            var left = new GeoCoordinate(50.84431698343915, 3.144611937981996);
            var right = new GeoCoordinate(50.84431699551239, 3.1449685962923635);
            var topLeft = new GeoCoordinate(50.84440717501314, 3.1446117265095705);
            var topRight = new GeoCoordinate(50.84440717501314, 3.1449675435791398);
            var middle = new GeoCoordinate(50.8443169894425, 3.144789284159275);
            var topMiddle = new GeoCoordinate(50.84440717501314, 3.1447892841399474);

            // encode left-right.
            var coordinates = new List<GeoCoordinate>();
            coordinates.Add(left);
            coordinates.Add(right);
            Assert.AreEqual(90, BearingEncoder.EncodeBearing(coordinates).Value, 1);

            // encode right-left.
            coordinates = new List<GeoCoordinate>();
            coordinates.Add(right);
            coordinates.Add(left);
            Assert.AreEqual(270, BearingEncoder.EncodeBearing(coordinates).Value, 1);

            // encode left-topLeft-topMiddle.
            coordinates = new List<GeoCoordinate>();
            coordinates.Add(left);
            coordinates.Add(topLeft);
            coordinates.Add(topMiddle);
            Assert.AreEqual(45, BearingEncoder.EncodeBearing(coordinates).Value, 1);

            // encode middle-topMiddle-topLeft.
            coordinates = new List<GeoCoordinate>();
            coordinates.Add(middle);
            coordinates.Add(topMiddle);
            coordinates.Add(topLeft);
            Assert.AreEqual(315, BearingEncoder.EncodeBearing(coordinates).Value, 1);
        }
    }
}