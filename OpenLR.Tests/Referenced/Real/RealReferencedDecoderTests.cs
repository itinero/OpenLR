using NetTopologySuite.IO;
using NUnit.Framework;
using OpenLR.Binary;
using OpenLR.OsmSharp;
using OpenLR.OsmSharp.Decoding;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.Tests.Referenced.Real
{
    /// <summary>
    /// Contains tests for decoding/encoding an OpenLR circle location to a referenced circle location.
    /// </summary>
    [TestFixture]
    public class RealReferencedDecoderTests
    {
        /// <summary>
        /// A referenced decoding with real data.
        /// </summary>
        [Test]
        public void TestDecode1()
        {
            // this is an encoded line-location, but assume we don't know this.
            string data = "CwNIUCQL5gs8Ef/FA6sLDw==";
            string geoJsonActual = "{\"type\":\"LineString\",\"coordinates\":[[4.6161060333251953,50.690467834472656],[4.615666389465332,50.690864562988281],[4.6154966354370117,50.696247100830078],[4.615412712097168,50.697616577148438],[4.6153979301452637,50.698280334472656],[4.6153759956359863,50.698684692382812],[4.6153225898742676,50.699268341064453],[4.6153054237365723,50.699459075927734],[4.6152987480163574,50.699722290039062]]}";

            // create a referenced decoder.
            var referencedDecoder = new ReferencedLiveEdgeDecoder(RealGraphOsm.GetRoutingGraph(), new BinaryDecoder());

            // decodes a location.
            var location = referencedDecoder.Decode(data);
            var lineLocation = location as ReferencedLine<LiveEdge>;
            var lineLocationGeometry = lineLocation.ToGeometry();

            // write GeoJSON.
            var geoJsonWriter = new GeoJsonWriter();
            var geoJson = geoJsonWriter.Write(lineLocationGeometry);
            Assert.IsNotNull(geoJson);
            AssertGeo.AreEqual(geoJsonActual, geoJson, 1);
        }

        /// <summary>
        /// A referenced decoding with real data.
        /// </summary>
        [Test]
        public void TestDecode2()
        {
            // this is an encoded line-location, but assume we don't know this.
            string data = "CwM+IiQYtws7DPzyAbILCg==";
            string geoJsonActual = "{\"type\":\"LineString\",\"coordinates\":[[4.5598363876342773,50.760837554931641],[4.5597963333129883,50.760860443115234],[4.5595493316650391,50.760982513427734],[4.55853796005249,50.761508941650391],[4.5582680702209473,50.761661529541016],[4.5523185729980469,50.7650146484375]]}";

            // create a referenced decoder.
            var referencedDecoder = new ReferencedLiveEdgeDecoder(RealGraphOsm.GetRoutingGraph(), new BinaryDecoder());

            // decodes a location.
            var location = referencedDecoder.Decode(data);
            var lineLocation = location as ReferencedLine<LiveEdge>;
            var lineLocationGeometry = lineLocation.ToGeometry();

            // write GeoJSON.
            var geoJsonWriter = new GeoJsonWriter();
            var geoJson = geoJsonWriter.Write(lineLocationGeometry);
            Assert.IsNotNull(geoJson);
            AssertGeo.AreEqual(geoJsonActual, geoJson, 1);
        }

        /// <summary>
        /// A referenced decoding with real data.
        /// </summary>
        [Test]
        public void TestDecode3()
        {
            // this is an encoded line-location, but assume we don't know this.
            string data = "CwM8tSQZgQs7EvvvAr4LCQ==";
            string geoJsonActual = "{\"type\":\"LineString\",\"coordinates\":[[4.5523185729980469,50.7650146484375],[4.5522303581237793,50.765064239501953],[4.55216121673584,50.765102386474609],[4.5513834953308105,50.765544891357422],[4.5506992340087891,50.765937805175781],[4.549346923828125,50.7667121887207],[4.5487337112426758,50.767177581787109],[4.54760217666626,50.767967224121094],[4.5467185974121094,50.768653869628906],[4.5454902648925781,50.769634246826172],[4.5442113876342773,50.770816802978516],[4.543121337890625,50.771892547607422],[4.5426936149597168,50.772018432617188],[4.5425186157226562,50.772056579589844],[4.5423049926757812,50.772087097167969],[4.5419602394104,50.772125244140625],[4.5418844223022461,50.772132873535156]]}";

            // create a referenced decoder.
            var referencedDecoder = new ReferencedLiveEdgeDecoder(RealGraphOsm.GetRoutingGraph(), new BinaryDecoder());

            // decodes a location.
            var location = referencedDecoder.Decode(data);
            var lineLocation = location as ReferencedLine<LiveEdge>;
            var lineLocationGeometry = lineLocation.ToGeometry();

            // write GeoJSON.
            var geoJsonWriter = new GeoJsonWriter();
            var geoJson = geoJsonWriter.Write(lineLocationGeometry);
            Assert.IsNotNull(geoJson);
            AssertGeo.AreEqual(geoJsonActual, geoJson, 1);
        }
    }
}