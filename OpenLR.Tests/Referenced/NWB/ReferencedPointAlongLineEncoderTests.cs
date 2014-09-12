using NUnit.Framework;
using OpenLR.Binary.Encoders;
using OpenLR.Model;
using OpenLR.OsmSharp.Encoding;
using OpenLR.OsmSharp.Locations;
using OpenLR.OsmSharp.NWB;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;

namespace OpenLR.Tests.Referenced.NWB
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
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            uint vertex1 = graph.AddVertex(49.60597f, 6.12829f);
            uint vertex2 = graph.AddVertex(49.60521f, 6.12779f);
            graph.AddArc(vertex1, vertex2, new LiveEdge()
            {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("BAANSUBSRT", "VBD"),
                    Tag.Create("WEGBEHSRT", "R"),
                    Tag.Create("WEGNUMMER", string.Empty),
                    Tag.Create("RIJRICHTNG", "N")))
            }, null);
            graph.AddArc(vertex2, vertex1, new LiveEdge()
            {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("BAANSUBSRT", "VBD"),
                    Tag.Create("WEGBEHSRT", "R"),
                    Tag.Create("WEGNUMMER", string.Empty),
                    Tag.Create("RIJRICHTNG", "N"),
                    Tag.Create("HECTOLTTR", string.Empty)))
            }, null);

            // create a referenced location and encode it.
            var referencedPointAlongLineLocation = new ReferencedPointAlongLine<LiveEdge>();
            referencedPointAlongLineLocation.Route = new ReferencedLine<LiveEdge>(graph);
            referencedPointAlongLineLocation.Route.Edges = new LiveEdge[1];
            referencedPointAlongLineLocation.Route.Edges[0] = new LiveEdge()
            {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("BAANSUBSRT", "VBD"),
                    Tag.Create("WEGBEHSRT", "R"),
                    Tag.Create("WEGNUMMER", string.Empty),
                    Tag.Create("RIJRICHTNG", "N"),
                    Tag.Create("HECTOLTTR", string.Empty)))
            };
            referencedPointAlongLineLocation.Route.Vertices = new long[2];
            referencedPointAlongLineLocation.Route.Vertices[0] = vertex1;
            referencedPointAlongLineLocation.Route.Vertices[1] = vertex2;
            referencedPointAlongLineLocation.Latitude = (49.60597f + 49.60521f) / 2f;
            referencedPointAlongLineLocation.Longitude = (6.12829f + 6.12779f) / 2f;

            // encode location.
            var encoder = new PointAlongLineEncoder();
            var router = new DykstraRoutingLive();
            var mainEncoder = new ReferencedNWBEncoder(graph, null);
            var referencedEncoder = new ReferencedPointAlongLineEncoder<LiveEdge>(mainEncoder, encoder, graph, router);
            var location = referencedEncoder.EncodeReferenced(referencedPointAlongLineLocation);

            // test result.
            Assert.IsNotNull(location);
            Assert.AreEqual(SideOfRoad.OnOrAbove, location.SideOfRoad);
            Assert.AreEqual(Orientation.NoOrientation, location.Orientation);
            Assert.AreEqual(50, location.PositiveOffsetPercentage.Value, 0.5f);

            Assert.AreEqual(49.60597f, location.First.Coordinate.Latitude);
            Assert.AreEqual(6.12829f, location.First.Coordinate.Longitude);
            Assert.AreEqual(91, location.First.DistanceToNext);
            Assert.AreEqual(FormOfWay.SlipRoad, location.First.FormOfWay);
            Assert.AreEqual(FunctionalRoadClass.Frc0, location.First.FuntionalRoadClass);
            Assert.AreEqual(FunctionalRoadClass.Frc0, location.First.LowestFunctionalRoadClassToNext);

            Assert.AreEqual(49.60521f, location.Last.Coordinate.Latitude);
            Assert.AreEqual(6.12779f, location.Last.Coordinate.Longitude);

            // TODO: encode location with a point on or at the first and last points.
        }
    }
}