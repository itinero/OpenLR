using NUnit.Framework;
using OpenLR.Binary.Encoders;
using OpenLR.Model;
using OpenLR.Referenced.Encoding;
using OpenLR.Referenced.Locations;
using OpenLR.Referenced.MultiNet;
using OpenLR.Referenced.Router;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Graphs;

namespace OpenLR.Tests.Referenced.MultiNet
{
    [TestFixture]
    public class ReferencedPointAlongLineEncoderTests
    {
        /// <summary>
        /// A simple referenced point along line location encoding test.
        /// </summary>
        [Test]
        public void EncodedReferencedPointAlongLineLocation()
        {
            // build a graph to encode from.
            var tags = new TagsTableCollectionIndex();
            var graphDataSource = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graphDataSource.AddVertex(49.60597f, 6.12829f);
            var vertex2 = graphDataSource.AddVertex(49.60521f, 6.12779f);
            graphDataSource.AddEdge(vertex1, vertex2, new LiveEdge()
            {
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("FRC", "2"),
                    Tag.Create("FOW", "3")))
            }, null);
            graphDataSource.AddEdge(vertex2, vertex1, new LiveEdge()
            {
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("FRC", "2"),
                    Tag.Create("FOW", "3")))
            }, null);

            // create a referenced location and encode it.
            var graph = new BasicRouterDataSource<LiveEdge>(graphDataSource);
            var referencedPointAlongLineLocation = new ReferencedPointAlongLine();
            referencedPointAlongLineLocation.Route = new ReferencedLine(graph);
            referencedPointAlongLineLocation.Route.Edges = new LiveEdge[1];
            referencedPointAlongLineLocation.Route.Edges[0] = new LiveEdge()
            {
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("FRC", "2"),
                    Tag.Create("FOW", "3")))
            };
            referencedPointAlongLineLocation.Route.EdgeShapes = new GeoCoordinateSimple[1][];
            referencedPointAlongLineLocation.Route.EdgeShapes[0] = new GeoCoordinateSimple[0];
            referencedPointAlongLineLocation.Route.Vertices = new long[2];
            referencedPointAlongLineLocation.Route.Vertices[0] = vertex1;
            referencedPointAlongLineLocation.Route.Vertices[1] = vertex2;
            referencedPointAlongLineLocation.Latitude = (49.60597f + 49.60521f) / 2f;
            referencedPointAlongLineLocation.Longitude = (6.12829f + 6.12779f) / 2f;
            referencedPointAlongLineLocation.Orientation = Orientation.FirstToSecond;

            // encode location.
            var encoder = new PointAlongLineEncoder();
            var mainEncoder = new ReferencedMultiNetEncoder(graph, null);
            var referencedEncoder = new ReferencedPointAlongLineEncoder(mainEncoder, encoder);
            var location = referencedEncoder.EncodeReferenced(referencedPointAlongLineLocation);

            // test result.
            Assert.IsNotNull(location);
            Assert.AreEqual(SideOfRoad.OnOrAbove, location.SideOfRoad);
            Assert.AreEqual(Orientation.FirstToSecond, location.Orientation);
            Assert.AreEqual(50, location.PositiveOffsetPercentage.Value, 0.5f);

            Assert.AreEqual(49.60597f, location.First.Coordinate.Latitude);
            Assert.AreEqual(6.12829f, location.First.Coordinate.Longitude);
            Assert.AreEqual(91, location.First.DistanceToNext);
            Assert.AreEqual(FormOfWay.SingleCarriageWay, location.First.FormOfWay);
            Assert.AreEqual(FunctionalRoadClass.Frc2, location.First.FuntionalRoadClass);
            Assert.AreEqual(FunctionalRoadClass.Frc2, location.First.LowestFunctionalRoadClassToNext);

            Assert.AreEqual(49.60521f, location.Last.Coordinate.Latitude);
            Assert.AreEqual(6.12779f, location.Last.Coordinate.Longitude);

            // TODO: encode location with a point on or at the first and last points.
        }
    }
}