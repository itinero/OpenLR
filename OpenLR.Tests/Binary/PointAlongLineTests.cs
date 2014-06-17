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
        }
    }
}