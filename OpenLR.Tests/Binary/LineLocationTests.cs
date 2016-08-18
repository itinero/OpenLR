using NUnit.Framework;
using OpenLR.Codecs.Binary.Decoders;
using OpenLR.Model;
using OpenLR.Model.Locations;
using System;

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
            var stringData = Convert.FromBase64String("CwRbWyNG9RpsCQCb/jsbtAT/6/+jK1lE");

            // decode.
            Assert.IsTrue(LineLocationCodec.CanDecode(stringData));
            var location = LineLocationCodec.Decode(stringData);

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

        /// <summary>
        /// A simple test encoding from a base64 string.
        /// </summary>
        [Test]
        public void EncodeBase64Test()
        {
            double delta = 0.0001;

            // build the location to decode.
            var location = new LineLocation();
            location.First = new LocationReferencePoint();
            location.First.Coordinate = new Coordinate() { Latitude = 49.60851, Longitude = 6.12683 };
            location.First.DistanceToNext = 10;
            location.First.FuntionalRoadClass = FunctionalRoadClass.Frc3;
            location.First.FormOfWay = FormOfWay.MultipleCarriageWay;
            location.First.LowestFunctionalRoadClassToNext = FunctionalRoadClass.Frc3;
            location.First.Bearing = 0;
            location.Intermediate = new LocationReferencePoint[1];
            location.Intermediate[0] = new LocationReferencePoint();
            location.Intermediate[0].Coordinate = new Coordinate() { Latitude = 49.60398, Longitude = 6.12838 };
            location.Intermediate[0].DistanceToNext = 10;
            location.Intermediate[0].FuntionalRoadClass = FunctionalRoadClass.Frc3;
            location.Intermediate[0].FormOfWay = FormOfWay.SingleCarriageWay;
            location.Intermediate[0].LowestFunctionalRoadClassToNext = FunctionalRoadClass.Frc5;
            location.Intermediate[0].Bearing = 0;
            location.Last = new LocationReferencePoint();
            location.Last.Coordinate = new Coordinate() { Latitude = 49.60305, Longitude = 6.12817 };
            location.Last.DistanceToNext = 10;
            location.Last.FuntionalRoadClass = FunctionalRoadClass.Frc5;
            location.Last.FormOfWay = FormOfWay.SingleCarriageWay;
            location.Last.Bearing = 0;

            // encode.
            var stringData = LineLocationCodec.Encode(location);

            // decode again (decoding was tested above).
            var decodedLocation = LineLocationCodec.Decode(stringData);

            Assert.IsNotNull(decodedLocation);
            Assert.IsInstanceOf<LineLocation>(decodedLocation);
            var lineLocation = (decodedLocation as LineLocation);

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

            // compare again with reference encoded string.
            var referenceBinary = Convert.FromBase64String("CwRbWyNG9BpgAACa/jsboAD/6/+kKwAAAA==");
            var referenceDecodedLocation = LineLocationCodec.Decode(referenceBinary);

            // check first reference.
            Assert.IsNotNull(lineLocation.First);
            Assert.AreEqual(referenceDecodedLocation.First.Coordinate.Longitude, lineLocation.First.Coordinate.Longitude, delta); // 6.12829°
            Assert.AreEqual(referenceDecodedLocation.First.Coordinate.Latitude, lineLocation.First.Coordinate.Latitude, delta); // 49.60597°
            Assert.AreEqual(referenceDecodedLocation.First.FuntionalRoadClass, lineLocation.First.FuntionalRoadClass);
            Assert.AreEqual(referenceDecodedLocation.First.FormOfWay, lineLocation.First.FormOfWay);
            Assert.AreEqual(referenceDecodedLocation.First.LowestFunctionalRoadClassToNext, lineLocation.First.LowestFunctionalRoadClassToNext);
            Assert.AreEqual(referenceDecodedLocation.First.Bearing.Value, lineLocation.First.Bearing.Value, 11.25); // binary encode loses accuracy for bearing.

            // check intermediates.
            Assert.IsNotNull(referenceDecodedLocation.Intermediate);
            Assert.AreEqual(referenceDecodedLocation.Intermediate.Length, lineLocation.Intermediate.Length);
            Assert.AreEqual(referenceDecodedLocation.Intermediate[0].Coordinate.Longitude, lineLocation.Intermediate[0].Coordinate.Longitude, delta); // 6.12838°
            Assert.AreEqual(referenceDecodedLocation.Intermediate[0].Coordinate.Latitude, lineLocation.Intermediate[0].Coordinate.Latitude, delta); // 49.60398°
            Assert.AreEqual(referenceDecodedLocation.Intermediate[0].FuntionalRoadClass, lineLocation.Intermediate[0].FuntionalRoadClass);
            Assert.AreEqual(referenceDecodedLocation.Intermediate[0].FormOfWay, lineLocation.Intermediate[0].FormOfWay);
            Assert.AreEqual(referenceDecodedLocation.Intermediate[0].LowestFunctionalRoadClassToNext, lineLocation.Intermediate[0].LowestFunctionalRoadClassToNext);

            // check second reference.
            Assert.IsNotNull(lineLocation.Last);
            Assert.AreEqual(referenceDecodedLocation.Last.Coordinate.Longitude, lineLocation.Last.Coordinate.Longitude, delta); // 6.12779°
            Assert.AreEqual(referenceDecodedLocation.Last.Coordinate.Latitude, lineLocation.Last.Coordinate.Latitude, delta); // 49.60521°
            Assert.AreEqual(referenceDecodedLocation.Last.FuntionalRoadClass, lineLocation.Last.FuntionalRoadClass);
            Assert.AreEqual(referenceDecodedLocation.Last.FormOfWay, lineLocation.Last.FormOfWay);
            Assert.AreEqual(referenceDecodedLocation.Last.Bearing.Value, lineLocation.Last.Bearing.Value, 11.25); // binary encode loses accuracy for bearing.
        }
    }
}