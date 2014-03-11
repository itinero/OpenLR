using NUnit.Framework;
using OpenLR.Binary.Decoders;
using OpenLR.Locations;

namespace OpenLR.Tests.Binary
{
    /// <summary>
    /// Contains tests for decoding/encoding a circle location to/from OpenLR binary representation.
    /// </summary>
    [TestFixture]
    public class CircleLocationTests
    {
        /// <summary>
        /// A simple test decoding from a base64 string.
        /// </summary>
        [Test]
        public void DecodeBase64Test()
        {
            double delta = 0.0001;

            // define a base64 string.
            string stringData = "AwRbYyNGu6o=";

            // decode.
            var decoder = new CircleLocationDecoder();
            var location = decoder.Decode(stringData);

            Assert.IsNotNull(location);
            Assert.IsInstanceOf<CircleLocation>(location);
            var circleLocation = (location as CircleLocation);

            // check coordinate.
            Assert.IsNotNull(circleLocation.Coordinates);
            Assert.AreEqual(6.12699, circleLocation.Coordinates.Longitude, delta); // 6.12699°
            Assert.AreEqual(49.60728, circleLocation.Coordinates.Latitude, delta); // 49.60728°
            Assert.AreEqual(170, circleLocation.Radius);
        }
    }
}