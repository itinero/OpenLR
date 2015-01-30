using NUnit.Framework;
using OpenLR.Binary.Encoders;
using OpenLR.Model;
using OpenLR.Referenced.Encoding;
using OpenLR.Referenced.Locations;
using OpenLR.Referenced.Osm;
using OpenLR.Referenced.Router;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Math.Geo.Simple;
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
        public void EncodeReferencedPointAlongLineLocation()
        {
            // build a graph to encode from.
            var tags = new TagsTableCollectionIndex();
            var graphDataSource = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            uint vertex1 = graphDataSource.AddVertex(49.60597f, 6.12829f);
            uint vertex2 = graphDataSource.AddVertex(49.60521f, 6.12779f);
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

            // create a referenced location and encode it.
            var graph = new BasicRouterDataSource<LiveEdge>(graphDataSource);
            var referencedPointAlongLineLocation = new ReferencedPointAlongLine<LiveEdge>();
            referencedPointAlongLineLocation.Route = new ReferencedLine<LiveEdge>(graph);
            referencedPointAlongLineLocation.Route.Edges = new LiveEdge[1];
            referencedPointAlongLineLocation.Route.Edges[0] = new LiveEdge()
            {
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            referencedPointAlongLineLocation.Route.EdgeShapes = new GeoCoordinateSimple[1][];
            referencedPointAlongLineLocation.Route.EdgeShapes[0] = new GeoCoordinateSimple[0];
            referencedPointAlongLineLocation.Route.Vertices = new long[2];
            referencedPointAlongLineLocation.Route.Vertices[0] = vertex1;
            referencedPointAlongLineLocation.Route.Vertices[1] = vertex2;
            referencedPointAlongLineLocation.Latitude = (49.60597f + 49.60521f) / 2f;
            referencedPointAlongLineLocation.Longitude = (6.12829f + 6.12779f) / 2f;

            // encode location.
            var encoder = new PointAlongLineEncoder();
            var router = new DykstraRoutingLive();
            var mainEncoder = new ReferencedOsmEncoder(graph, null);
            var referencedEncoder = new ReferencedPointAlongLineEncoder<LiveEdge>(mainEncoder, encoder);
            var location = referencedEncoder.EncodeReferenced(referencedPointAlongLineLocation);

            // test result.
            Assert.IsNotNull(location);
            Assert.AreEqual(SideOfRoad.OnOrAbove, location.SideOfRoad);
            Assert.AreEqual(Orientation.NoOrientation, location.Orientation);
            Assert.AreEqual(50, location.PositiveOffsetPercentage.Value, 0.5f);

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
            referencedPointAlongLineLocation.Route = new ReferencedLine<LiveEdge>(graph);
            referencedPointAlongLineLocation.Route.Edges = new LiveEdge[1];
            referencedPointAlongLineLocation.Route.Edges[0] = new LiveEdge()
            {
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            referencedPointAlongLineLocation.Route.EdgeShapes = new GeoCoordinateSimple[1][];
            referencedPointAlongLineLocation.Route.EdgeShapes[0] = new GeoCoordinateSimple[0];
            referencedPointAlongLineLocation.Route.Vertices = new long[2];
            referencedPointAlongLineLocation.Route.Vertices[0] = vertex1;
            referencedPointAlongLineLocation.Route.Vertices[1] = vertex2;
            referencedPointAlongLineLocation.Latitude = 49.60597f;
            referencedPointAlongLineLocation.Longitude = 6.12829f;

            // encode location.
            location = referencedEncoder.EncodeReferenced(referencedPointAlongLineLocation);

            // test result.
            Assert.IsNotNull(location);
            Assert.AreEqual(SideOfRoad.OnOrAbove, location.SideOfRoad);
            Assert.AreEqual(Orientation.NoOrientation, location.Orientation);
            Assert.AreEqual(0, location.PositiveOffsetPercentage);

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
            referencedPointAlongLineLocation.Route = new ReferencedLine<LiveEdge>(graph);
            referencedPointAlongLineLocation.Route.Edges = new LiveEdge[1];
            referencedPointAlongLineLocation.Route.Edges[0] = new LiveEdge()
            {
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            referencedPointAlongLineLocation.Route.EdgeShapes = new GeoCoordinateSimple[1][];
            referencedPointAlongLineLocation.Route.EdgeShapes[0] = new GeoCoordinateSimple[0];
            referencedPointAlongLineLocation.Route.Vertices = new long[2];
            referencedPointAlongLineLocation.Route.Vertices[0] = vertex1;
            referencedPointAlongLineLocation.Route.Vertices[1] = vertex2;
            referencedPointAlongLineLocation.Latitude = 49.60521f;
            referencedPointAlongLineLocation.Longitude = 6.12779f;

            // encode location.
            location = referencedEncoder.EncodeReferenced(referencedPointAlongLineLocation);

            // test result.
            Assert.IsNotNull(location);
            Assert.AreEqual(SideOfRoad.OnOrAbove, location.SideOfRoad);
            Assert.AreEqual(Orientation.NoOrientation, location.Orientation);
            Assert.AreEqual(100, location.PositiveOffsetPercentage.Value, 0.5f);

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
