using NUnit.Framework;
using OpenLR.Binary.Decoders;
using OpenLR.Binary.Encoders;
using OpenLR.Locations;
using OpenLR.Model;

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
            Assert.IsTrue(decoder.CanDecode(stringData));
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
            Assert.AreEqual(17, pointAlongLineLocation.First.BearingDistance);

            // check second reference.
            Assert.IsNotNull(pointAlongLineLocation.Last);
            Assert.AreEqual(6.12779, pointAlongLineLocation.Last.Coordinate.Longitude, delta); // 6.12779°
            Assert.AreEqual(49.60521, pointAlongLineLocation.Last.Coordinate.Latitude, delta); // 49.60521°
            Assert.AreEqual(FunctionalRoadClass.Frc2, pointAlongLineLocation.Last.FuntionalRoadClass);
            Assert.AreEqual(FormOfWay.MultipleCarriageWay, pointAlongLineLocation.Last.FormOfWay);
            Assert.AreEqual(3, pointAlongLineLocation.Last.BearingDistance);

            // check other properties.
            Assert.AreEqual(Orientation.NoOrientation, pointAlongLineLocation.Orientation);
            Assert.AreEqual(SideOfRoad.Left, pointAlongLineLocation.SideOfRoad);
            Assert.AreEqual(77, pointAlongLineLocation.PositiveOffset);
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
            location.First.BearingDistance = 17;

            location.Last = new LocationReferencePoint();
            location.Last.Coordinate = new Coordinate()
            {
                Latitude = 49.60521,
                Longitude = 6.12779
            };
            location.Last.FuntionalRoadClass = FunctionalRoadClass.Frc2;
            location.Last.FormOfWay = FormOfWay.MultipleCarriageWay;
            location.Last.BearingDistance = 3;

            location.Orientation = Orientation.NoOrientation;
            location.SideOfRoad = SideOfRoad.Left;
            location.PositiveOffset = 77;

            // encode.
            var encoder = new PointAlongLineEncoder();
            var stringData = encoder.Encode(location);

            // decode again (decoding was tested above).
            var decoder = new PointAlongLineDecoder();
            var decodedLocation = decoder.Decode(stringData);

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
            Assert.AreEqual(location.First.BearingDistance, pointAlongLineLocation.First.BearingDistance);

            // check second reference.
            Assert.IsNotNull(pointAlongLineLocation.Last);
            Assert.AreEqual(location.Last.Coordinate.Longitude, pointAlongLineLocation.Last.Coordinate.Longitude, delta); // 6.12779°
            Assert.AreEqual(location.Last.Coordinate.Latitude, pointAlongLineLocation.Last.Coordinate.Latitude, delta); // 49.60521°
            Assert.AreEqual(location.Last.FuntionalRoadClass, pointAlongLineLocation.Last.FuntionalRoadClass);
            Assert.AreEqual(location.Last.FormOfWay, pointAlongLineLocation.Last.FormOfWay);
            Assert.AreEqual(location.Last.BearingDistance, pointAlongLineLocation.Last.BearingDistance);

            // check other properties.
            Assert.AreEqual(location.Orientation, pointAlongLineLocation.Orientation);
            Assert.AreEqual(location.SideOfRoad, pointAlongLineLocation.SideOfRoad);
            Assert.AreEqual(location.PositiveOffset, pointAlongLineLocation.PositiveOffset);

        }
    }
}