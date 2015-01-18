using NUnit.Framework;
using OpenLR.Binary.Encoders;
using OpenLR.Model;
using OpenLR.OsmSharp.Encoding;
using OpenLR.OsmSharp.Locations;
using OpenLR.OsmSharp.Osm;
using OpenLR.OsmSharp.Router;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;

namespace OpenLR.Tests.Referenced.Osm
{
    [TestFixture]
    public class ReferencedLineEncoderTests
    {
        /// <summary>
        /// A simple referenced line location encoding test.
        /// </summary>
        [Test]
        public void EncodedLineLocation()
        {
            // build a graph to encode from.
            var tags = new TagsTableCollectionIndex();
            var graphDataSource = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            uint vertex1 = graphDataSource.AddVertex(49.60597f, 6.12829f);
            uint vertex2 = graphDataSource.AddVertex(49.60521f, 6.12779f);
            graphDataSource.AddArc(vertex1, vertex2, new LiveEdge()
            {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            }, null);
            graphDataSource.AddArc(vertex2, vertex1, new LiveEdge()
            {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            }, null);

            // create a referenced location and encode it.
            var graph = new BasicRouterDataSource<LiveEdge>(graphDataSource);
            var referencedLocation  = new ReferencedLine<LiveEdge>(graph);
            referencedLocation.Edges = new LiveEdge[1];
            referencedLocation.Edges[0] = new LiveEdge()
            {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            referencedLocation.Vertices = new long[2];
            referencedLocation.Vertices[0] = vertex1;
            referencedLocation.Vertices[1] = vertex2;

            // encode location.
            var encoder = new LineEncoder();
            var router = new DykstraRoutingLive();
            var mainEncoder = new ReferencedOsmEncoder(graph, null);
            var referencedEncoder = new ReferencedLineEncoder<LiveEdge>(mainEncoder, encoder);
            var location = referencedEncoder.EncodeReferenced(referencedLocation);

            // test result.
            Assert.IsNotNull(location);
            Assert.AreEqual(49.60597f, location.First.Coordinate.Latitude);
            Assert.AreEqual(6.12829f, location.First.Coordinate.Longitude);
            Assert.AreEqual(91, location.First.DistanceToNext);
            Assert.AreEqual(203, location.First.Bearing);
            Assert.AreEqual(FormOfWay.SingleCarriageWay, location.First.FormOfWay);
            Assert.AreEqual(FunctionalRoadClass.Frc3, location.First.FuntionalRoadClass);
            Assert.AreEqual(FunctionalRoadClass.Frc3, location.First.LowestFunctionalRoadClassToNext);

            Assert.AreEqual(49.60521f, location.Last.Coordinate.Latitude);
            Assert.AreEqual(6.12779f, location.Last.Coordinate.Longitude);
            Assert.AreEqual(23, location.Last.Bearing);

            // encode location with a point on the first point.
            referencedLocation = new ReferencedLine<LiveEdge>(graph);
            referencedLocation.Edges = new LiveEdge[1];
            referencedLocation.Edges[0] = new LiveEdge()
            {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            referencedLocation.Vertices = new long[2];
            referencedLocation.Vertices[0] = vertex1;
            referencedLocation.Vertices[1] = vertex2;

            // encode location.
            location = referencedEncoder.EncodeReferenced(referencedLocation);

            // test result.
            Assert.IsNotNull(location);

            Assert.AreEqual(49.60597f, location.First.Coordinate.Latitude);
            Assert.AreEqual(6.12829f, location.First.Coordinate.Longitude);
            Assert.AreEqual(91, location.First.DistanceToNext);
            Assert.AreEqual(FormOfWay.SingleCarriageWay, location.First.FormOfWay);
            Assert.AreEqual(FunctionalRoadClass.Frc3, location.First.FuntionalRoadClass);
            Assert.AreEqual(FunctionalRoadClass.Frc3, location.First.LowestFunctionalRoadClassToNext);

            Assert.AreEqual(49.60521f, location.Last.Coordinate.Latitude);
            Assert.AreEqual(6.12779f, location.Last.Coordinate.Longitude);

            // encode location with a point on the last point.
            referencedLocation = new ReferencedLine<LiveEdge>(graph);
            referencedLocation.Edges = new LiveEdge[1];
            referencedLocation.Edges[0] = new LiveEdge()
            {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            referencedLocation.Vertices = new long[2];
            referencedLocation.Vertices[0] = vertex1;
            referencedLocation.Vertices[1] = vertex2;

            // encode location.
            location = referencedEncoder.EncodeReferenced(referencedLocation);

            // test result.
            Assert.IsNotNull(location);

            Assert.AreEqual(49.60597f, location.First.Coordinate.Latitude);
            Assert.AreEqual(6.12829f, location.First.Coordinate.Longitude);
            Assert.AreEqual(91, location.First.DistanceToNext);
            Assert.AreEqual(FormOfWay.SingleCarriageWay, location.First.FormOfWay);
            Assert.AreEqual(FunctionalRoadClass.Frc3, location.First.FuntionalRoadClass);
            Assert.AreEqual(FunctionalRoadClass.Frc3, location.First.LowestFunctionalRoadClassToNext);

            Assert.AreEqual(49.60521f, location.Last.Coordinate.Latitude);
            Assert.AreEqual(6.12779f, location.Last.Coordinate.Longitude);
        }
    }
}
