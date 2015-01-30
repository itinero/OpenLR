using NUnit.Framework;
using OpenLR.Locations;
using OpenLR.Referenced.Decoding;
using OpenLR.Referenced.Osm;
using OsmSharp.Routing.Osm.Graphs;

namespace OpenLR.Tests.Referenced
{
    /// <summary>
    /// Contains tests for decoding/encoding an OpenLR grid location to a referenced grid location.
    /// </summary>
    [TestFixture]
    public class ReferencedGridDecoderTests
    {
        /// <summary>
        /// An extremely simple referenced grid location decoding test.
        /// </summary>
        [Test]
        public void DecodeReferencedGridLocation()
        {
            // build the location to decode.
            var location = new GridLocation();
            location.LowerLeft = new Model.Coordinate();
            location.LowerLeft.Latitude = 49.60586;
            location.LowerLeft.Longitude = 6.12555;
            location.UpperRight = new Model.Coordinate();
            location.UpperRight.Longitude = 6.12875;
            location.UpperRight.Latitude = 49.60711;
            location.Rows = 3;
            location.Columns = 5;

            // decode the location
            var referencedDecoder = new ReferencedGridDecoder<LiveEdge>(null, null);
            var referencedLocation = referencedDecoder.Decode(location);

            // confirm result.
            Assert.IsNotNull(referencedLocation);
            Assert.AreEqual(referencedLocation.LowerLeftLatitude, location.LowerLeft.Latitude);
            Assert.AreEqual(referencedLocation.LowerLeftLongitude, location.LowerLeft.Longitude);
            Assert.AreEqual(referencedLocation.UpperRightLatitude, location.UpperRight.Latitude);
            Assert.AreEqual(referencedLocation.UpperRightLongitude, location.UpperRight.Longitude);
            Assert.AreEqual(referencedLocation.Rows, location.Rows);
            Assert.AreEqual(referencedLocation.Columns, location.Columns);
        }
    }
}
