using NUnit.Framework;
using OpenLR.Referenced.Router;
using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Graphs;

namespace OpenLR.Tests.Referenced.Router
{
    /// <summary>
    /// Contains tests for the basic router implementation.
    /// </summary>
    [TestFixture]
    public class BasicRouterTests
    {
        /// <summary>
        /// Tests a one-hop route.
        /// </summary>
        [Test]
        public void TestOneHop()
        {
            // build test data.
            var tagsIndex = new TagsTableCollectionIndex();
            var tags = new TagsCollection(new Tag() { Key = "highway", Value = "residential" });
            var tagsId = tagsIndex.Add(tags);
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            var vertex1 = graph.AddVertex(50.909909938361324f, 4.452434778213501f);
            var vertex2 = graph.AddVertex(50.916698097895550f, 4.459059834480286f);
            graph.AddEdge(vertex1, vertex2, 
                new LiveEdge()
                {
                    Distance = 890.65f,
                    Forward = true,
                    Tags = tagsId
                });

            // calculate route.
            var router = new BasicRouter();
            var route = router.Calculate(new BasicRouterDataSource<LiveEdge>(graph), 
                OsmSharp.Routing.Vehicle.Car, vertex1, vertex2, true);

            // verify result.
            Assert.IsNotNull(route);
            Assert.AreEqual(vertex2, route.Vertex);
            Assert.AreEqual(OsmSharp.Routing.Vehicle.Car.Weight(tags, 890.65f), route.Weight);
            Assert.AreEqual(vertex1, route.From.Vertex);
            Assert.AreEqual(0, route.From.Weight);
        }

        /// <summary>
        /// Tests a one-hop route with a shape.
        /// </summary>
        [Test]
        public void TestOneHopShape()
        {
            // build test data.
            var tagsIndex = new TagsTableCollectionIndex();
            var tags = new TagsCollection(new Tag() { Key = "highway", Value = "residential" });
            var tagsId = tagsIndex.Add(tags);
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            var vertex1 = graph.AddVertex(50.909909938361324f, 4.452434778213501f);
            var vertex2 = graph.AddVertex(50.916698097895550f, 4.459059834480286f);
            graph.AddEdge(vertex1, vertex2,
                new LiveEdge()
                {
                    Distance = 890.65f,
                    Forward = true,
                    Tags = tagsId
                },
                new CoordinateArrayCollection<GeoCoordinate>(
                    new GeoCoordinate [] {
                        new GeoCoordinate(50.910408852752770, 4.4531670212745670),
                        new GeoCoordinate(50.911362694534270, 4.4543391466140750),
                        new GeoCoordinate(50.912018872212215, 4.4550552964210500),
                        new GeoCoordinate(50.913280466188610, 4.4562461972236630),
                        new GeoCoordinate(50.914614742177996, 4.4574183225631705),
                        new GeoCoordinate(50.915686870323930, 4.4582659006118770),
                        new GeoCoordinate(50.916354824109910, 4.4587942957878110)
                    }));

            // calculate route.
            var router = new BasicRouter();
            var route = router.Calculate(new BasicRouterDataSource<LiveEdge>(graph),
                OsmSharp.Routing.Vehicle.Car, vertex1, vertex2, true);

            // verify result.
            Assert.IsNotNull(route);
            Assert.AreEqual(vertex2, route.Vertex);
            Assert.AreEqual(OsmSharp.Routing.Vehicle.Car.Weight(tags, 890.65f), route.Weight);
            Assert.AreEqual(vertex1, route.From.Vertex);
            Assert.AreEqual(0, route.From.Weight);
        }
    }
}