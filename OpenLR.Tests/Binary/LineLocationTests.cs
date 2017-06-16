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

        /// <summary>
        /// A regression tests for issue:
        /// https://github.com/itinero/OpenLR/issues/76
        /// </summary>
        [Test]
        public void RegressionEncodeDecodeNegativeLongitude()
        {
            double delta = 0.0001;

            var location = new LineLocation()
            {
                First = new LocationReferencePoint()
                {
                    Bearing = 101,
                    Coordinate = new Coordinate()
                    {
                        Latitude = 52.932136535644531f,
                        Longitude = -1.5213972330093384f
                    },
                    DistanceToNext = 1111,
                    FormOfWay = FormOfWay.Motorway,
                    FuntionalRoadClass = FunctionalRoadClass.Frc0,
                    LowestFunctionalRoadClassToNext = FunctionalRoadClass.Frc0
                },
                Last = new LocationReferencePoint()
                {
                    Bearing = 276,
                    Coordinate = new Coordinate()
                    {
                        Latitude = 52.929317474365234f,
                        Longitude = -1.5055110454559326
                    },
                    DistanceToNext = 0,
                    FormOfWay = FormOfWay.Motorway,
                    FuntionalRoadClass = FunctionalRoadClass.Frc0
                },
                NegativeOffsetPercentage = 2.12239432f,
                PositiveOffsetPercentage = 1.46861947f
            };

            // encode.
            var stringData = LineLocationCodec.Encode(location);

            // decode again (decoding was tested above).
            var decodedLocation = LineLocationCodec.Decode(stringData);
            
            Assert.IsNotNull(decodedLocation);
            Assert.IsInstanceOf<LineLocation>(decodedLocation);
            var lineLocation = (decodedLocation as LineLocation);

            // check first reference.
            Assert.IsNotNull(lineLocation.First);
            Assert.AreEqual(52.932136535644531f, lineLocation.First.Coordinate.Latitude, delta);
            Assert.AreEqual(-1.5213972330093384f, lineLocation.First.Coordinate.Longitude, delta);
            Assert.AreEqual(FunctionalRoadClass.Frc0, lineLocation.First.FuntionalRoadClass);
            Assert.AreEqual(FormOfWay.Motorway, lineLocation.First.FormOfWay);
            Assert.AreEqual(FunctionalRoadClass.Frc0, lineLocation.First.LowestFunctionalRoadClassToNext);

            // check second reference.
            Assert.IsNotNull(lineLocation.Last);
            Assert.AreEqual(-1.5055110454559326f, lineLocation.Last.Coordinate.Longitude, delta); // 6.12817°
            Assert.AreEqual(52.929317474365234f, lineLocation.Last.Coordinate.Latitude, delta); // 49.60305°
            Assert.AreEqual(FunctionalRoadClass.Frc0, lineLocation.Last.FuntionalRoadClass);
            Assert.AreEqual(FormOfWay.Motorway, lineLocation.Last.FormOfWay);
        }

        /// <summary>
        /// A regression test for encoding a location with intermediate points.
        /// </summary>
        [Test]
        public void RegressionEncodeDecode()
        {
            var e = 0.0001f;

            var json = "{\r\n  \"First\": {\r\n    \"Coordinate\": {\r\n      \"Latitude\": 52.275665283203125,\r\n      \"Longitude\": 6.825572490692139\r\n    },\r\n    \"Bearing\": 83,\r\n    \"FuntionalRoadClass\": 5,\r\n    \"FormOfWay\": 7,\r\n    \"LowestFunctionalRoadClassToNext\": 5,\r\n    \"DistanceToNext\": 7012\r\n  },\r\n  \"Intermediate\": [\r\n    {\r\n      \"Coordinate\": {\r\n        \"Latitude\": 52.30723190307617,\r\n        \"Longitude\": 6.911031246185303\r\n      },\r\n      \"Bearing\": 95,\r\n      \"FuntionalRoadClass\": 5,\r\n      \"FormOfWay\": 7,\r\n      \"LowestFunctionalRoadClassToNext\": 5,\r\n      \"DistanceToNext\": 70\r\n    },\r\n    {\r\n      \"Coordinate\": {\r\n        \"Latitude\": 52.307552337646484,\r\n        \"Longitude\": 6.9113054275512695\r\n      },\r\n      \"Bearing\": 27,\r\n      \"FuntionalRoadClass\": 3,\r\n      \"FormOfWay\": 6,\r\n      \"LowestFunctionalRoadClassToNext\": 3,\r\n      \"DistanceToNext\": 1924\r\n    },\r\n    {\r\n      \"Coordinate\": {\r\n        \"Latitude\": 52.32027053833008,\r\n        \"Longitude\": 6.928611755371094\r\n      },\r\n      \"Bearing\": 73,\r\n      \"FuntionalRoadClass\": 3,\r\n      \"FormOfWay\": 2,\r\n      \"LowestFunctionalRoadClassToNext\": 5,\r\n      \"DistanceToNext\": 7658\r\n    }\r\n  ],\r\n  \"Last\": {\r\n    \"Coordinate\": {\r\n      \"Latitude\": 52.36980438232422,\r\n      \"Longitude\": 6.999289512634277\r\n    },\r\n    \"Bearing\": 230,\r\n    \"FuntionalRoadClass\": 5,\r\n    \"FormOfWay\": 7,\r\n    \"LowestFunctionalRoadClassToNext\": null,\r\n    \"DistanceToNext\": 0\r\n  },\r\n  \"PositiveOffsetPercentage\": 0,\r\n  \"NegativeOffsetPercentage\": 0.638994753\r\n}";
            var location = Newtonsoft.Json.JsonConvert.DeserializeObject<LineLocation>(json);
            
            // encode.
            var stringData = LineLocationCodec.Encode(location);

            // decode again (decoding was tested above).
            var decodedLocation = LineLocationCodec.Decode(stringData);

            Assert.AreEqual(location.First.Coordinate.Latitude, decodedLocation.First.Coordinate.Latitude, e);
            Assert.AreEqual(location.First.Coordinate.Longitude, decodedLocation.First.Coordinate.Longitude, e);

            Assert.AreEqual(location.Intermediate.Length, decodedLocation.Intermediate.Length);
            for(var i = 0; i < location.Intermediate.Length; i++)
            {
                Assert.AreEqual(location.Intermediate[i].Coordinate.Latitude, decodedLocation.Intermediate[i].Coordinate.Latitude, e);
                Assert.AreEqual(location.Intermediate[i].Coordinate.Longitude, decodedLocation.Intermediate[i].Coordinate.Longitude, e);
            }

            Assert.AreEqual(location.Last.Coordinate.Latitude, decodedLocation.Last.Coordinate.Latitude, e);
            Assert.AreEqual(location.Last.Coordinate.Longitude, decodedLocation.Last.Coordinate.Longitude, e);
        }
    }
}