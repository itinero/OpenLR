using NUnit.Framework;
using OpenLR.Binary.Encoders;
using OpenLR.Model;
using OpenLR.OsmSharp.Encoding;
using OpenLR.OsmSharp.Locations;
using OpenLR.OsmSharp.Osm;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;

namespace OpenLR.Tests.Referenced.Osm
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
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            }, null);
            graph.AddArc(vertex2, vertex1, new LiveEdge()
            {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            }, null);

            // create a referenced location and encode it.
            var referencedPointAlongLineLocation = new ReferencedPointAlongLine<LiveEdge>();
            referencedPointAlongLineLocation.Edge = new LiveEdge()
            {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            referencedPointAlongLineLocation.VertexFrom = vertex1;
            referencedPointAlongLineLocation.VertexTo = vertex2;
            referencedPointAlongLineLocation.Latitude = (49.60597f + 49.60521f) / 2f;
            referencedPointAlongLineLocation.Longitude = (6.12829f + 6.12779f) / 2f;

            // encode location.
            var encoder = new PointAlongLineEncoder();
            var router = new DykstraRoutingLive();
            var mainEncoder = new ReferencedOsmEncoder(graph, null);
            var referencedEncoder = new ReferencedPointAlongLineEncoder<LiveEdge>(mainEncoder, encoder, graph, router);
            var location = referencedEncoder.EncodeReferenced(referencedPointAlongLineLocation);

            // test result.
            Assert.IsNotNull(location);
            Assert.AreEqual(SideOfRoad.OnOrAbove, location.SideOfRoad);
            Assert.AreEqual(Orientation.NoOrientation, location.Orientation);
            Assert.AreEqual(45, location.PositiveOffset);

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
            referencedPointAlongLineLocation = new ReferencedPointAlongLine<LiveEdge>();
            referencedPointAlongLineLocation.Edge = new LiveEdge()
            {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            referencedPointAlongLineLocation.VertexFrom = vertex1;
            referencedPointAlongLineLocation.VertexTo = vertex2;
            referencedPointAlongLineLocation.Latitude = 49.60597f;
            referencedPointAlongLineLocation.Longitude = 6.12829f;

            // encode location.
            location = referencedEncoder.EncodeReferenced(referencedPointAlongLineLocation);

            // test result.
            Assert.IsNotNull(location);
            Assert.AreEqual(SideOfRoad.OnOrAbove, location.SideOfRoad);
            Assert.AreEqual(Orientation.NoOrientation, location.Orientation);
            Assert.AreEqual(0, location.PositiveOffset);

            Assert.AreEqual(49.60597f, location.First.Coordinate.Latitude);
            Assert.AreEqual(6.12829f, location.First.Coordinate.Longitude);
            Assert.AreEqual(91, location.First.DistanceToNext);
            Assert.AreEqual(FormOfWay.SingleCarriageWay, location.First.FormOfWay);
            Assert.AreEqual(FunctionalRoadClass.Frc3, location.First.FuntionalRoadClass);
            Assert.AreEqual(FunctionalRoadClass.Frc3, location.First.LowestFunctionalRoadClassToNext);

            Assert.AreEqual(49.60521f, location.Last.Coordinate.Latitude);
            Assert.AreEqual(6.12779f, location.Last.Coordinate.Longitude);

            // encode location with a point on the last point.
            referencedPointAlongLineLocation = new ReferencedPointAlongLine<LiveEdge>();
            referencedPointAlongLineLocation.Edge = new LiveEdge()
            {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            referencedPointAlongLineLocation.VertexFrom = vertex1;
            referencedPointAlongLineLocation.VertexTo = vertex2;
            referencedPointAlongLineLocation.Latitude = 49.60521f;
            referencedPointAlongLineLocation.Longitude = 6.12779f;

            // encode location.
            location = referencedEncoder.EncodeReferenced(referencedPointAlongLineLocation);

            // test result.
            Assert.IsNotNull(location);
            Assert.AreEqual(SideOfRoad.OnOrAbove, location.SideOfRoad);
            Assert.AreEqual(Orientation.NoOrientation, location.Orientation);
            Assert.AreEqual(91, location.PositiveOffset);

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
