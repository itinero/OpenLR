using NUnit.Framework;
using OpenLR.Codecs.Binary.Decoders;
using OpenLR.Model;
using OpenLR.Model.Locations;
using System;

namespace OpenLR.Tests.Binary
{
    /// <summary>
    /// Contains tests for decoding/encoding a point along location to/from OpenLR binary representation.
    /// </summary>
    [TestFixture]
    public class PointAlongLineLocationTests
    {
        /// <summary>
        /// A simple test decoding from a base64 string.
        /// </summary>
        [Test]
        public void DecodeBase64Test()
        {
            double delta = 0.0001;

            // define a base64 string we are sure is a line location.
            var stringData = Convert.FromBase64String("KwRbnyNGfhJBAv/P/7WSAEw=");

            // decode.
            Assert.IsTrue(PointAlongLineLocationCodec.CanDecode(stringData));
            var location = PointAlongLineLocationCodec.Decode(stringData);

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
            Assert.AreEqual(17, pointAlongLineLocation.First.Bearing.Value, 11.25); // binary encode loses accuracy for bearing.

            // check second reference.
            Assert.IsNotNull(pointAlongLineLocation.Last);
            Assert.AreEqual(6.12779, pointAlongLineLocation.Last.Coordinate.Longitude, delta); // 6.12779°
            Assert.AreEqual(49.60521, pointAlongLineLocation.Last.Coordinate.Latitude, delta); // 49.60521°
            Assert.AreEqual(FunctionalRoadClass.Frc2, pointAlongLineLocation.Last.FuntionalRoadClass);
            Assert.AreEqual(FormOfWay.MultipleCarriageWay, pointAlongLineLocation.Last.FormOfWay);
            Assert.AreEqual(3, pointAlongLineLocation.Last.Bearing.Value, 11.25); // binary encode loses accuracy for bearing.

            // check other properties.
            Assert.AreEqual(Orientation.NoOrientation, pointAlongLineLocation.Orientation);
            Assert.AreEqual(SideOfRoad.Left, pointAlongLineLocation.SideOfRoad);
            Assert.AreEqual(30.19, pointAlongLineLocation.PositiveOffsetPercentage, 0.5); // binary encode loses accuracy.
        }

        /// <summary>
        /// A simple test encoding from a base64 string.
        /// </summary>
        [Test]
        public void EncodeBase64Test()
        {
            double delta = 0.0001;

            // create a location.
            var location = new PointAlongLineLocation();
            location.First = new LocationReferencePoint();
            location.First.Coordinate = new Coordinate()
            {
                Latitude = 49.60597,
                Longitude = 6.12829
            };
            location.First.FuntionalRoadClass = FunctionalRoadClass.Frc2;
            location.First.FormOfWay = FormOfWay.MultipleCarriageWay;
            location.First.LowestFunctionalRoadClassToNext = FunctionalRoadClass.Frc2;
            location.First.Bearing = 17;
            location.First.DistanceToNext = (int)(58.6 * 2) + 1;

            location.Last = new LocationReferencePoint();
            location.Last.Coordinate = new Coordinate()
            {
                Latitude = 49.60521,
                Longitude = 6.12779
            };
            location.Last.FuntionalRoadClass = FunctionalRoadClass.Frc2;
            location.Last.FormOfWay = FormOfWay.MultipleCarriageWay;
            location.Last.Bearing = 3;
            location.Last.DistanceToNext = 0;

            location.Orientation = Orientation.NoOrientation;
            location.SideOfRoad = SideOfRoad.Left;
            location.PositiveOffsetPercentage = 30.19f;

            // encode.
            var stringData = PointAlongLineLocationCodec.Encode(location);

            // decode again (decoding was tested above).
            var decodedLocation = PointAlongLineLocationCodec.Decode(stringData);

            Assert.IsNotNull(decodedLocation);
            Assert.IsInstanceOf<PointAlongLineLocation>(decodedLocation);
            var pointAlongLineLocation = (decodedLocation as PointAlongLineLocation);

            // check first reference.
            Assert.IsNotNull(pointAlongLineLocation.First);
            Assert.AreEqual(location.First.Coordinate.Longitude, pointAlongLineLocation.First.Coordinate.Longitude, delta); // 6.12829°
            Assert.AreEqual(location.First.Coordinate.Latitude, pointAlongLineLocation.First.Coordinate.Latitude, delta); // 49.60597°
            Assert.AreEqual(location.First.FuntionalRoadClass, pointAlongLineLocation.First.FuntionalRoadClass);
            Assert.AreEqual(location.First.FormOfWay, pointAlongLineLocation.First.FormOfWay);
            Assert.AreEqual(location.First.LowestFunctionalRoadClassToNext, pointAlongLineLocation.First.LowestFunctionalRoadClassToNext);
            Assert.AreEqual(location.First.Bearing.Value, pointAlongLineLocation.First.Bearing.Value, 11.25); // binary encode loses accuracy for bearing.
            Assert.AreEqual(location.First.DistanceToNext, pointAlongLineLocation.First.DistanceToNext);

            // check second reference.
            Assert.IsNotNull(pointAlongLineLocation.Last);
            Assert.AreEqual(location.Last.Coordinate.Longitude, pointAlongLineLocation.Last.Coordinate.Longitude, delta); // 6.12779°
            Assert.AreEqual(location.Last.Coordinate.Latitude, pointAlongLineLocation.Last.Coordinate.Latitude, delta); // 49.60521°
            Assert.AreEqual(location.Last.FuntionalRoadClass, pointAlongLineLocation.Last.FuntionalRoadClass);
            Assert.AreEqual(location.Last.FormOfWay, pointAlongLineLocation.Last.FormOfWay);
            Assert.AreEqual(location.Last.Bearing.Value, pointAlongLineLocation.Last.Bearing.Value, 11.25); // binary encode loses accuracy for bearing.
            Assert.AreEqual(location.Last.DistanceToNext, pointAlongLineLocation.Last.DistanceToNext);

            // check other properties.
            Assert.AreEqual(location.Orientation, pointAlongLineLocation.Orientation);
            Assert.AreEqual(location.SideOfRoad, pointAlongLineLocation.SideOfRoad);
            Assert.AreEqual(location.PositiveOffsetPercentage.Value, pointAlongLineLocation.PositiveOffsetPercentage.Value, 0.5f); // binary encode loses accuracy.

            // compare again with reference encoded string.
            var referenceStringData = "KwRbnyNGfhJBAv/P/7WSAEw=";
            var referenceDecodedLocation = PointAlongLineLocationCodec.Decode(Convert.FromBase64String(referenceStringData));

            var referenceBinary = System.Convert.FromBase64String(referenceStringData);

            // check first reference.
            Assert.IsNotNull(pointAlongLineLocation.First);
            Assert.AreEqual(referenceDecodedLocation.First.Coordinate.Longitude, pointAlongLineLocation.First.Coordinate.Longitude, delta); // 6.12829°
            Assert.AreEqual(referenceDecodedLocation.First.Coordinate.Latitude, pointAlongLineLocation.First.Coordinate.Latitude, delta); // 49.60597°
            Assert.AreEqual(referenceDecodedLocation.First.FuntionalRoadClass, pointAlongLineLocation.First.FuntionalRoadClass);
            Assert.AreEqual(referenceDecodedLocation.First.FormOfWay, pointAlongLineLocation.First.FormOfWay);
            Assert.AreEqual(referenceDecodedLocation.First.LowestFunctionalRoadClassToNext, pointAlongLineLocation.First.LowestFunctionalRoadClassToNext);
            Assert.AreEqual(referenceDecodedLocation.First.Bearing.Value, pointAlongLineLocation.First.Bearing.Value, 11.25); // binary encode loses accuracy for bearing.

            // check second reference.
            Assert.IsNotNull(pointAlongLineLocation.Last);
            Assert.AreEqual(referenceDecodedLocation.Last.Coordinate.Longitude, pointAlongLineLocation.Last.Coordinate.Longitude, delta); // 6.12779°
            Assert.AreEqual(referenceDecodedLocation.Last.Coordinate.Latitude, pointAlongLineLocation.Last.Coordinate.Latitude, delta); // 49.60521°
            Assert.AreEqual(referenceDecodedLocation.Last.FuntionalRoadClass, pointAlongLineLocation.Last.FuntionalRoadClass);
            Assert.AreEqual(referenceDecodedLocation.Last.FormOfWay, pointAlongLineLocation.Last.FormOfWay);
            Assert.AreEqual(referenceDecodedLocation.Last.Bearing.Value, pointAlongLineLocation.Last.Bearing.Value, 11.25); // binary encode loses accuracy for bearing.

            // check other properties.
            Assert.AreEqual(referenceDecodedLocation.Orientation, pointAlongLineLocation.Orientation);
            Assert.AreEqual(referenceDecodedLocation.SideOfRoad, pointAlongLineLocation.SideOfRoad);
            Assert.AreEqual(referenceDecodedLocation.PositiveOffsetPercentage.Value, pointAlongLineLocation.PositiveOffsetPercentage.Value, 0.5f); // binary encode loses accuracy.
        }
    }
}