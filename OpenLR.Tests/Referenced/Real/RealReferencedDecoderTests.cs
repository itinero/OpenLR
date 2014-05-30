using NetTopologySuite.IO;
using NUnit.Framework;
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

            var router = new DykstraRoutingLive();
            Facade.RegisterDecoder(new ReferencedGeoCoordinateDecoder<LiveEdge>(new OpenLR.Binary.Decoders.GeoCoordinateLocationDecoder(), RealGraphOsm.GetRoutingGraph(), router));
            Facade.RegisterDecoder(new ReferencedCircleDecoder<LiveEdge>(new OpenLR.Binary.Decoders.CircleLocationDecoder(), RealGraphOsm.GetRoutingGraph(), router));
            Facade.RegisterDecoder(new ReferencedGridDecoder<LiveEdge>(new OpenLR.Binary.Decoders.GridLocationDecoder(), RealGraphOsm.GetRoutingGraph(), router));
            Facade.RegisterDecoder(new ReferencedLineDecoder<LiveEdge>(new OpenLR.Binary.Decoders.LineLocationDecoder(), RealGraphOsm.GetRoutingGraph(), router));
            Facade.RegisterDecoder(new ReferencedPointAlongLineDecoder<LiveEdge>(new OpenLR.Binary.Decoders.PointAlongLineDecoder(), RealGraphOsm.GetRoutingGraph(), router));
            Facade.RegisterDecoder(new ReferencedPolygonDecoder<LiveEdge>(new OpenLR.Binary.Decoders.PolygonLocationDecoder(), RealGraphOsm.GetRoutingGraph(), router));
            Facade.RegisterDecoder(new ReferencedRectangleDecoder<LiveEdge>(new OpenLR.Binary.Decoders.RectangleLocationDecoder(), RealGraphOsm.GetRoutingGraph(), router));

            var location = Facade.DecodeBinary(data);
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