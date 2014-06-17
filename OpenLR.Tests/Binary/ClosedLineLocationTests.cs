using NUnit.Framework;
using OpenLR.Binary.Decoders;
using OpenLR.Locations;
using OpenLR.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.Tests.Binary
{
    /// <summary>
    /// Contains tests for decoding/encoding a closed line location to/from OpenLR binary representation.
    /// </summary>
    [TestFixture]
    public class ClosedLineLocationTests
    {
        /// <summary>
        /// A simple test decoding from a base64 string.
        /// </summary>
        [Test]
        public void DecodeBase64Test()
        {
            double delta = 0.0001;

            // define a base64 string.
            string stringData = "WwRboCNGfhJrBAAJ/zkb9AgTFQ==";

            // decode.
            var decoder = new ClosedLineLocationDecoder();
            Assert.IsTrue(decoder.CanDecode(stringData));
            var location = decoder.Decode(stringData);

            Assert.IsNotNull(location);
            Assert.IsInstanceOf<ClosedLineLocation>(location);
            var closedLineLocation = (location as ClosedLineLocation);

            // check coordinate.
            Assert.IsNotNull(closedLineLocation);
            Assert.AreEqual(6.1283, closedLineLocation.First.Coordinate.Longitude, delta); // 6.1283°
            Assert.AreEqual(49.60596, closedLineLocation.First.Coordinate.Latitude, delta); // 49.60596°
            Assert.IsNotNull(closedLineLocation.Intermediate);
            Assert.AreEqual(1, closedLineLocation.Intermediate.Length);
            Assert.AreEqual(6.12839, closedLineLocation.Intermediate[0].Coordinate.Longitude, delta); // 6.12839°
            Assert.AreEqual(49.60397, closedLineLocation.Intermediate[0].Coordinate.Latitude, delta); // 49.60397°
            Assert.AreEqual(6.1283, closedLineLocation.Last.Coordinate.Longitude, delta); // 6.1283°
            Assert.AreEqual(49.60596, closedLineLocation.Last.Coordinate.Latitude, delta); // 49.60596°

            Assert.AreEqual(FunctionalRoadClass.Frc2, closedLineLocation.First.FuntionalRoadClass);
            Assert.AreEqual(FormOfWay.MultipleCarriageWay, closedLineLocation.First.FormOfWay);
            Assert.AreEqual(FunctionalRoadClass.Frc3, closedLineLocation.First.LowestFunctionalRoadClassToNext);
            // Assert.AreEqual(246, closedLineLocation.First.DistanceToNext);
            // Assert.AreEqual(134, closedLineLocation.First.BearingDistance.Value);

            Assert.AreEqual(FunctionalRoadClass.Frc3, closedLineLocation.Intermediate[0].FuntionalRoadClass);
            Assert.AreEqual(FormOfWay.SingleCarriageWay, closedLineLocation.Intermediate[0].FormOfWay);
            Assert.AreEqual(FunctionalRoadClass.Frc7, closedLineLocation.Intermediate[0].LowestFunctionalRoadClassToNext);
            //Assert.AreEqual(246, closedLineLocation.Intermediate[0].DistanceToNext);
            //Assert.AreEqual(227, closedLineLocation.Intermediate[0].BearingDistance.Value);

            Assert.AreEqual(FunctionalRoadClass.Frc2, closedLineLocation.Last.FuntionalRoadClass);
            Assert.AreEqual(FormOfWay.SingleCarriageWay, closedLineLocation.Last.FormOfWay);
            //Assert.AreEqual(FunctionalRoadClass.Frc3, closedLineLocation.Last.LowestFunctionalRoadClassToNext);
            //Assert.AreEqual(239, closedLineLocation.Last.BearingDistance.Value);
        }
    }
}
