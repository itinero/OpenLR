using NUnit.Framework;
using OpenLR.Model.Locations;
using OpenLR.Referenced.Codecs;

namespace OpenLR.Test.Referenced
{
    /// <summary>
    /// Contains tests for decoding/encoding an OpenLR circle location to a referenced circle location.
    /// </summary>
    [TestFixture]
    public class ReferencedCircleDecoderTests
    {
        /// <summary>
        /// An extremely simple referenced circle location decoding test.
        /// </summary>
        [Test]
        public void DecodeReferencedCircleLocation()
        {
            // build the location to decode.
            var location = new CircleLocation();
            location.Coordinate = new Model.Coordinate();
            location.Coordinate.Latitude = 49.60728;
            location.Coordinate.Longitude = 6.12699;
            location.Radius = 170;

            // decode the location
            var referencedLocation = ReferencedCircleCodec.Decode(location);

            // confirm result.
            Assert.IsNotNull(referencedLocation);
            Assert.AreEqual(referencedLocation.Longitude, location.Coordinate.Longitude);
            Assert.AreEqual(referencedLocation.Latitude, location.Coordinate.Latitude);
            Assert.AreEqual(referencedLocation.Radius, location.Radius);
        }
    }
}