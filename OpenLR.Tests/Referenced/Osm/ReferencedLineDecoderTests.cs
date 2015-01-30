using NUnit.Framework;
using OpenLR.Binary;
using OpenLR.Binary.Decoders;
using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.OsmSharp.Decoding;
using OpenLR.OsmSharp.Osm;
using OpenLR.OsmSharp.Router;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;

namespace OpenLR.Tests.Referenced.Osm
{
    /// <summary>
    /// Contains tests for decoding/encoding a OpenLR line location to a referenced line location.
    /// </summary>
    [TestFixture]
    public class ReferencedLineDecoderTests
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
            location.First.Bearing = 0;
            location.Intermediate = new LocationReferencePoint[1];
            location.Intermediate[0] = new LocationReferencePoint();
            location.Intermediate[0].Coordinate = new Coordinate() { Latitude = 49.60398, Longitude = 6.12838 };
            location.Intermediate[0].DistanceToNext = 10;
            location.Intermediate[0].FuntionalRoadClass = FunctionalRoadClass.Frc3;
            location.Intermediate[0].FormOfWay = FormOfWay.SingleCarriageWay;
            location.Intermediate[0].LowestFunctionalRoadClassToNext = FunctionalRoadClass.Frc5;
            location.Intermediate[0].Bearing = 0;
            location.Last = new LocationReferencePoint();
            location.Last = new LocationReferencePoint();
            location.Last.Coordinate = new Coordinate() { Latitude = 49.60305, Longitude = 6.12817 };
            location.Last.DistanceToNext = 10;
            location.Last.FuntionalRoadClass = FunctionalRoadClass.Frc5;
            location.Last.FormOfWay = FormOfWay.SingleCarriageWay;
            location.Last.Bearing = 0;

            // build a graph to decode onto.
            var tags = new TagsTableCollectionIndex();
            var graphDataSource = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            uint vertex1 = graphDataSource.AddVertex(49.60851f, 6.12683f);
            uint vertex2 = graphDataSource.AddVertex(49.60398f, 6.12838f);
            uint vertex3 = graphDataSource.AddVertex(49.60305f, 6.12817f);
            graphDataSource.AddEdge(vertex1, vertex2, new LiveEdge()
            {
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            }, null);
            graphDataSource.AddEdge(vertex2, vertex1, new LiveEdge()
            {
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            }, null);
            graphDataSource.AddEdge(vertex2, vertex3, new LiveEdge()
            {
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            }, null);
            graphDataSource.AddEdge(vertex3, vertex2, new LiveEdge()
            {
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            }, null);

            // decode the location
            var graph = new BasicRouterDataSource<LiveEdge>(graphDataSource);
            var decoder = new LineLocationDecoder();
            var router = new BasicRouter();
            var mainDecoder = new ReferencedOsmDecoder(graph, new BinaryDecoder());
            var referencedDecoder = new ReferencedLineDecoder<LiveEdge>(mainDecoder, decoder);
            var referencedLocation = referencedDecoder.Decode(location);

            // confirm result.
            Assert.IsNotNull(referencedLocation);
            Assert.IsNotNull(referencedLocation.Vertices);
            Assert.AreEqual(3, referencedLocation.Vertices.Length);
            Assert.IsNotNull(referencedLocation.Edges);
            Assert.AreEqual(2, referencedLocation.Edges.Length);
        }
    }
}