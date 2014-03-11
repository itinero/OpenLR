using NUnit.Framework;
using OpenLR.Binary.Decoders;
using OpenLR.Locations;

namespace OpenLR.Tests.Binary
{
    /// <summary>
    /// Contains tests for decoding/encoding a geo coordinate to/from OpenLR binary representation.
    /// </summary>
    [TestFixture]
    public class GeoCoordinateTests
    {
        /// <summary>
        /// A simple test decoding from a base64 string.
        /// </summary>
        [Test]
        public void DecodeBase64Test()
        {
            double delta = 0.0001;

            // define a base64 string.
            string stringData = "IwRbYyNGuw==";

            // decode.
            var decoder = new GeoCoordinateLocationDecoder();
            var location = decoder.Decode(stringData);

            Assert.IsNotNull(location);
            Assert.IsInstanceOf<GeoCoordinateLocation>(location);
            var geoCoordinate = (location as GeoCoordinateLocation);

            // check coordinate.
            Assert.IsNotNull(geoCoordinate.Coordinate);
            Assert.AreEqual(6.12699, geoCoordinate.Coordinate.Longitude, delta); // 6.12699°
            Assert.AreEqual(49.60728, geoCoordinate.Coordinate.Latitude, delta); // 49.60728°
        }
    }
}