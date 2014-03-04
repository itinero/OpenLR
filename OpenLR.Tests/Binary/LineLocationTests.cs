using NUnit.Framework;
using OpenLR.Binary.Decoders;
using OpenLR.Locations;
using OpenLR.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenLR.Tests.Binary
{
    /// <summary>
    /// Contains tests for decoding/encoding a line location to/from OpenLR binary representation.
    /// </summary>
    [TestFixture]
    public class LineLocationTests
    {
        /// <summary>
        /// A simple test decoding from a base64 string.
        /// </summary>
        [Test]
        public void DecodeBase64Test()
        {
            double delta = 0.0001;

            // define a base64 string we are sure is a line location.
            string stringData = "CwRbWyNG9RpsCQCb/jsbtAT/6/+jK1lE";

            // decode.
            var decoder = new LineLocationDecoder();
            var location = decoder.Decode(stringData);

            Assert.IsNotNull(location);
            Assert.IsInstanceOf<LineLocation>(location);
            var lineLocation = (location as LineLocation);

            // check first reference.
            Assert.IsNotNull(lineLocation.First);
            Assert.AreEqual(6.12683, lineLocation.First.Coordinate.Longitude, delta); // 6.12683°
            Assert.AreEqual(49.60851, lineLocation.First.Coordinate.Latitude, delta); // 49.60851°
            Assert.AreEqual(FunctionalRoadClass.Frc3, lineLocation.First.FuntionalRoadClass);
            Assert.AreEqual(FormOfWay.MultipleCarriageWay, lineLocation.First.FormOfWay);
            Assert.AreEqual(FunctionalRoadClass.Frc3, lineLocation.First.LowestFunctionalRoadClassToNext);

            // check intermediates.
            Assert.IsNotNull(lineLocation.Intermediate);
            Assert.AreEqual(1, lineLocation.Intermediate.Length);
            Assert.AreEqual(6.12838, lineLocation.Intermediate[0].Coordinate.Longitude, delta); // 6.12838°
            Assert.AreEqual(49.60398, lineLocation.Intermediate[0].Coordinate.Latitude, delta); // 49.60398°
            Assert.AreEqual(FunctionalRoadClass.Frc3, lineLocation.Intermediate[0].FuntionalRoadClass);
            Assert.AreEqual(FormOfWay.SingleCarriageWay, lineLocation.Intermediate[0].FormOfWay);
            Assert.AreEqual(FunctionalRoadClass.Frc5, lineLocation.Intermediate[0].LowestFunctionalRoadClassToNext);

            // check second reference.
            Assert.IsNotNull(lineLocation.Last);
            Assert.AreEqual(6.12817, lineLocation.Last.Coordinate.Longitude, delta); // 6.12817°
            Assert.AreEqual(49.60305, lineLocation.Last.Coordinate.Latitude, delta); // 49.60305°
            Assert.AreEqual(FunctionalRoadClass.Frc5, lineLocation.Last.FuntionalRoadClass);
            Assert.AreEqual(FormOfWay.SingleCarriageWay, lineLocation.Last.FormOfWay);
        }
    }
}