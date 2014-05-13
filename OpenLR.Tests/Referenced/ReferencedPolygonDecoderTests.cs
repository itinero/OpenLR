using NUnit.Framework;
using OpenLR.Locations;
using OpenLR.OsmSharp.Decoding;
using OsmSharp.Routing.Osm.Graphs;

namespace OpenLR.Tests.Referenced
{
    /// <summary>
    /// Contains tests for decoding/encoding an OpenLR polygon location to a referenced polygon location.
    /// </summary>
    [TestFixture]
    public class ReferencedPolygonDecoderTests
    {
        /// <summary>
        /// An extremely simple referenced polygon location decoding test.
        /// </summary>
        [Test]
        public void DecodeReferencedPolygonLocation()
        {
            // build the location to decode.
            var location = new PolygonLocation();
            location.Coordinates = new Model.Coordinate[5];
            location.Coordinates[0] = new Model.Coordinate() { Latitude = 49.60576, Longitude = 6.12549 };
            location.Coordinates[1] = new Model.Coordinate() { Latitude = 49.60591, Longitude = 6.12903 };
            location.Coordinates[2] = new Model.Coordinate() { Latitude = 49.60834, Longitude = 6.12739 };
            location.Coordinates[3] = new Model.Coordinate() { Latitude = 49.60870, Longitude = 6.12657 };
            location.Coordinates[4] = new Model.Coordinate() { Latitude = 49.60795, Longitude = 6.12492 };

            // decode the location
            //var decoder = new GeoCoordinateLocationDecoder();
            var referencedDecoder = new ReferencedPolygonDecoder<LiveEdge>(null, null, null);
            var referencedLocation = referencedDecoder.Decode(location);

            // confirm result.
            Assert.IsNotNull(referencedLocation);
            Assert.AreEqual(referencedLocation.Coordinates[0].Latitude, location.Coordinates[0].Latitude);
            Assert.AreEqual(referencedLocation.Coordinates[0].Longitude, location.Coordinates[0].Longitude);
            Assert.AreEqual(referencedLocation.Coordinates[1].Latitude, location.Coordinates[1].Latitude);
            Assert.AreEqual(referencedLocation.Coordinates[1].Longitude, location.Coordinates[1].Longitude);
            Assert.AreEqual(referencedLocation.Coordinates[2].Latitude, location.Coordinates[2].Latitude);
            Assert.AreEqual(referencedLocation.Coordinates[2].Longitude, location.Coordinates[2].Longitude);
            Assert.AreEqual(referencedLocation.Coordinates[3].Latitude, location.Coordinates[3].Latitude);
            Assert.AreEqual(referencedLocation.Coordinates[3].Longitude, location.Coordinates[3].Longitude);
            Assert.AreEqual(referencedLocation.Coordinates[4].Latitude, location.Coordinates[4].Latitude);
            Assert.AreEqual(referencedLocation.Coordinates[4].Longitude, location.Coordinates[4].Longitude);
        }
    }
}
