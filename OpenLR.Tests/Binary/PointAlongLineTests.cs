using NUnit.Framework;
using OpenLR.Binary.Decoders;
using OpenLR.Locations;
using OpenLR.Model;
using OsmSharp.Math.Geo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenLR.Tests.Binary
{
    /// <summary>
    /// Contains tests for decoding/encoding a point along location to/from OpenLR binary representation.
    /// </summary>
    [TestFixture]
    public class PointAlongLineTests
    {
        /// <summary>
        /// A simple test decoding from a base64 string.
        /// </summary>
        [Test]
        public void DecodeBase64Test()
        {
            double delta = 0.0001;

            // define a base64 string we are sure is a line location.
            string stringData = "KwRboCNGfhJRAf/O/7SSQ00=";

            // decode.
            var decoder = new PointAlongLineDecoder();
            var location = decoder.Decode(stringData);

            Assert.IsNotNull(location);
            Assert.IsInstanceOf<PointAlongLineLocation>(location);
            var pointAlongLineLocation = (location as PointAlongLineLocation);

            // check first reference.
            Assert.IsNotNull(pointAlongLineLocation.First);
            Assert.AreEqual(6.12829, pointAlongLineLocation.First.Coordinate.Longitude, delta); // 6.12829°
            Assert.AreEqual(49.60597, pointAlongLineLocation.First.Coordinate.Latitude, delta); // 49.60597°
            Assert.AreEqual(FunctionalRoadClass.Frc2, pointAlongLineLocation.First.FuntionalRoadClass);
            Assert.AreEqual(FormOfWay.MultipleCarriageWay, pointAlongLineLocation.First.FormOfWay);
            Assert.AreEqual(FunctionalRoadClass.Frc2, pointAlongLineLocation.First.LowestFunctionalRoadClassToNext);

            // check second reference.
            Assert.IsNotNull(pointAlongLineLocation.Last);
            Assert.AreEqual(6.12779, pointAlongLineLocation.Last.Coordinate.Longitude, delta); // 6.12779°
            Assert.AreEqual(49.60521, pointAlongLineLocation.Last.Coordinate.Latitude, delta); // 49.60521°
            Assert.AreEqual(FunctionalRoadClass.Frc2, pointAlongLineLocation.Last.FuntionalRoadClass);
            Assert.AreEqual(FormOfWay.MultipleCarriageWay, pointAlongLineLocation.Last.FormOfWay);

            // check other properties.
            Assert.AreEqual(Orientation.NoOrientation, pointAlongLineLocation.Orientation);
            Assert.AreEqual(SideOfRoad.Left, pointAlongLineLocation.SideOfRoad);
        }
    }
}