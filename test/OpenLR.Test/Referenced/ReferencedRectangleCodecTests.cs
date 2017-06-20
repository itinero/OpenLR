using NUnit.Framework;
using OpenLR.Model.Locations;
using OpenLR.Referenced.Codecs;

namespace OpenLR.Tests.Referenced
{
    /// <summary>
    /// Contains tests for decoding/encoding an OpenLR rectangle location to a referenced rectangle location.
    /// </summary>
    [TestFixture]
    public class ReferencedRectangleCodecTests
    {
        /// <summary>
        /// An extremely simple referenced rectangle location decoding test.
        /// </summary>
        [Test]
        public void DecodeReferencedRectangleLocation()
        {
            // build the location to decode.
            var location = new RectangleLocation();
            location.LowerLeft = new Model.Coordinate();
            location.LowerLeft.Latitude = 49.60586;
            location.LowerLeft.Longitude = 6.12555;
            location.UpperRight = new Model.Coordinate();
            location.UpperRight.Longitude = 6.12875;
            location.UpperRight.Latitude = 49.60711;

            // decode the location
            //var decoder = new GeoCoordinateLocationDecoder();
            var referencedLocation = ReferencedRectangleCodec.Decode(location);

            // confirm result.
            Assert.IsNotNull(referencedLocation);
            Assert.AreEqual(referencedLocation.LowerLeftLatitude, location.LowerLeft.Latitude);
            Assert.AreEqual(referencedLocation.LowerLeftLongitude, location.LowerLeft.Longitude);
            Assert.AreEqual(referencedLocation.UpperRightLatitude, location.UpperRight.Latitude);
            Assert.AreEqual(referencedLocation.UpperRightLongitude, location.UpperRight.Longitude);
        }
    }
}