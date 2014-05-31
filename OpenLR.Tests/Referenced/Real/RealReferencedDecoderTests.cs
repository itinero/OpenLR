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
    }
}