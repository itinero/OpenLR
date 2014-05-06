using NUnit.Framework;
using OpenLR.Binary.Decoders;
using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.OsmSharp.Decoding;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.Tests.Referenced
{
    /// <summary>
    /// Contains tests for decoding/encoding a OpenLR line location to a referenced line location.
    /// </summary>
    [TestFixture]
    public class LineLocationGraphDecoderTests
    {
        /// <summary>
        /// A simple referenced line location decoding test.
        /// </summary>
        [Test]
        public void DecodeReferencedLineLocation()
        {
            // build the location to decode.
            var location = new LineLocation();
            location.First = new LocationReferencePoint();
            location.First.Coordinate = new Coordinate() { Latitude = 49.60851, Longitude = 6.12683 };
            location.First.DistanceToNext = 10;
            location.First.FuntionalRoadClass = FunctionalRoadClass.Frc3;
            location.First.FormOfWay = FormOfWay.MultipleCarriageWay;
            location.First.LowestFunctionalRoadClassToNext = FunctionalRoadClass.Frc3;
            location.Intermediate = new LocationReferencePoint[1];
            location.Intermediate[0] = new LocationReferencePoint();
            location.Intermediate[0].Coordinate = new Coordinate() { Latitude = 49.60398, Longitude = 6.12838 };
            location.Intermediate[0].DistanceToNext = 10;
            location.Intermediate[0].FuntionalRoadClass = FunctionalRoadClass.Frc3;
            location.Intermediate[0].FormOfWay = FormOfWay.SingleCarriageWay;
            location.Intermediate[0].LowestFunctionalRoadClassToNext = FunctionalRoadClass.Frc5;
            location.Last = new LocationReferencePoint();
            location.Last = new LocationReferencePoint();
            location.Last.Coordinate = new Coordinate() { Latitude = 49.60305, Longitude = 6.12817 };
            location.Last.DistanceToNext = 10;
            location.Last.FuntionalRoadClass = FunctionalRoadClass.Frc5;
            location.Last.FormOfWay = FormOfWay.SingleCarriageWay;

            // build a graph to decode onto.
            var tags = new TagsTableCollectionIndex();
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            uint vertex1 = graph.AddVertex(49.60851f, 6.12683f);
            uint vertex2 = graph.AddVertex(49.60398f, 6.12838f);
            uint vertex3 = graph.AddVertex(49.60305f, 6.12817f);
            graph.AddArc(vertex1, vertex2, new LiveEdge() {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            }, null);
            graph.AddArc(vertex2, vertex1, new LiveEdge()
            {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            }, null);
            graph.AddArc(vertex2, vertex3, new LiveEdge() {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            }, null);
            graph.AddArc(vertex3, vertex2, new LiveEdge()
            {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            }, null);

            // decode the location
            var decoder = new LineLocationDecoder();
            var router = new DykstraRoutingLive();
            var referencedDecoder = new LineLocationGraphDecoder<LiveEdge>(decoder, graph, router);
            var referencedLocation = referencedDecoder.Decode(location);

            // test result.
            Assert.IsNotNull(referencedLocation);
            Assert.IsNotNull(referencedLocation.Vertices);
            Assert.AreEqual(3, referencedLocation.Vertices.Length);
            Assert.IsNotNull(referencedLocation.Edges);
            Assert.AreEqual(2, referencedLocation.Edges.Length);
        }
    }
}