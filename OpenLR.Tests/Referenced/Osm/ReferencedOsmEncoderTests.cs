using NUnit.Framework;
using OpenLR.Referenced.Router;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.Tests.Referenced.Osm
{
    [TestFixture]
    public class ReferencedOsmEncoderTests
    {
        /// <summary>
        /// Tests if a deadend vertex is valid.
        /// </summary>
        /// <remarks>
        /// Situation:
        ///     (1)-------(2)
        /// Expected result:
        ///     Both 1 and 2 are valid coming from any direction.
        /// </remarks>
        [Test]
        public void TestIsVertexValidDeadend()
        {
            var tags = new TagsTableCollectionIndex();
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graph.AddVertex(0, 0);
            var vertex2 = graph.AddVertex(1, 1);
            var edge = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graph.AddEdge(vertex1, vertex2, edge, null);

            var encoder = new OpenLR.Referenced.Osm.ReferencedOsmEncoder(new BasicRouterDataSource<LiveEdge>(graph),
                new EncoderMock());

            Assert.IsTrue(encoder.IsVertexValid(vertex1));
        }

        /// <summary>
        /// Tests if an intermediate vertex is valid when it has just two neighbours.
        /// </summary>
        /// <remarks>
        /// Situation:
        ///     (1)-------(2)------(3)
        /// Result:
        ///     2 is never valid.
        /// </remarks>
        [Test]
        public void TestIsVertexValidTwoNeighbours()
        {
            var tags = new TagsTableCollectionIndex();
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graph.AddVertex(0, 0);
            var vertex2 = graph.AddVertex(1, 1);
            var vertex3 = graph.AddVertex(2, 2);
            var edge = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graph.AddEdge(vertex1, vertex2, edge, null);
            graph.AddEdge(vertex2, vertex3, edge, null);

            var encoder = new OpenLR.Referenced.Osm.ReferencedOsmEncoder(new BasicRouterDataSource<LiveEdge>(graph),
                new EncoderMock());

            Assert.IsFalse(encoder.IsVertexValid(vertex2));
        }

        /// <summary>
        /// Tests if an intermediate vertex is invalid when it has just two neighbours on a oneway road.
        /// </summary>
        /// <remarks>
        /// Situation:
        ///     (1)--->---(2)--->---(3)
        /// Result:
        ///     2 is never valid.
        /// </remarks>
        [Test]
        public void TestIsVertexValidTwoNeighboursOneWay()
        {
            var tags = new TagsTableCollectionIndex();
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graph.AddVertex(0, 0);
            var vertex2 = graph.AddVertex(1, 1);
            var vertex3 = graph.AddVertex(2, 2);
            var edge = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("highway", "tertiary"),
                    Tag.Create("oneway", "yes")))
            };
            graph.AddEdge(vertex1, vertex2, edge, null);
            graph.AddEdge(vertex2, vertex3, edge, null);

            var encoder = new OpenLR.Referenced.Osm.ReferencedOsmEncoder(new BasicRouterDataSource<LiveEdge>(graph),
                new EncoderMock());

            Assert.IsFalse(encoder.IsVertexValid(vertex2));
        }

        /// <summary>
        /// Tests if an intermediate vertex is invalid when it has just two neighbours on a oneway road.
        /// </summary>
        /// <remarks>
        /// Situation:
        ///     (1)-------(2)--->---(3)
        /// Result:
        ///     2 is never valid.
        /// </remarks>
        [Test]
        public void TestIsVertexValidTwoNeighboursHalfOneWay()
        {
            var tags = new TagsTableCollectionIndex();
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graph.AddVertex(0, 0);
            var vertex2 = graph.AddVertex(1, 1);
            var vertex3 = graph.AddVertex(2, 2);
            var edge = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("highway", "tertiary")))
            };
            var onwayEdge = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("highway", "tertiary"),
                    Tag.Create("oneway", "yes")))
            };
            graph.AddEdge(vertex1, vertex2, edge, null);
            graph.AddEdge(vertex2, vertex3, onwayEdge, null);

            var encoder = new OpenLR.Referenced.Osm.ReferencedOsmEncoder(new BasicRouterDataSource<LiveEdge>(graph),
                new EncoderMock());

            Assert.IsFalse(encoder.IsVertexValid(vertex2));
        }

        /// <summary>
        /// Tests if an intermediate vertex is valid when a road splits in two oneway segments.
        /// </summary>
        /// <remarks>
        /// Situation:
        ///                  ---->---(4)
        ///                 /
        ///     (1)--------(2)--->---(3)
        /// Result:
        ///     2 is valid.
        /// </remarks>
        [Test]
        public void TestIsVertexValidSplit()
        {
            var tags = new TagsTableCollectionIndex();
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graph.AddVertex(0, 0);
            var vertex2 = graph.AddVertex(1, 1);
            var vertex3 = graph.AddVertex(2, 2);
            var vertex4 = graph.AddVertex(3, 3);
            var edge = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("highway", "tertiary")))
            };
            var onwayEdge = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("highway", "tertiary"),
                    Tag.Create("oneway", "yes")))
            };
            graph.AddEdge(vertex1, vertex2, edge, null);
            graph.AddEdge(vertex2, vertex3, onwayEdge, null);
            graph.AddEdge(vertex2, vertex4, onwayEdge, null);

            var encoder = new OpenLR.Referenced.Osm.ReferencedOsmEncoder(new BasicRouterDataSource<LiveEdge>(graph),
                new EncoderMock());

            Assert.IsTrue(encoder.IsVertexValid(vertex2));
        }

        /// <summary>
        /// Tests if an intermediate vertex is valid when a road joins.
        /// </summary>
        /// <remarks>
        /// Situation:
        ///     (2)--->---- 
        ///                \
        ///     (1)--->----(3)--->---(4)
        /// Result:
        ///     3 is valid.
        /// </remarks>
        [Test]
        public void TestIsVertexValidJoin()
        {
            var tags = new TagsTableCollectionIndex();
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graph.AddVertex(0, 0);
            var vertex2 = graph.AddVertex(1, 1);
            var vertex3 = graph.AddVertex(2, 2);
            var vertex4 = graph.AddVertex(3, 3);
            var edge = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("highway", "tertiary")))
            };
            var onwayEdge = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("highway", "tertiary"),
                    Tag.Create("oneway", "yes")))
            };
            graph.AddEdge(vertex1, vertex2, edge, null);
            graph.AddEdge(vertex2, vertex3, onwayEdge, null);
            graph.AddEdge(vertex2, vertex4, onwayEdge, null);

            var encoder = new OpenLR.Referenced.Osm.ReferencedOsmEncoder(new BasicRouterDataSource<LiveEdge>(graph),
                new EncoderMock());

            Assert.IsTrue(encoder.IsVertexValid(vertex3));
        }

        /// <summary>
        /// Tests if an intermediate vertex is valid at a T-junction.
        /// </summary>
        /// <remarks>
        /// Situation:
        ///                (3)
        ///                 |
        ///                 |
        ///     (1)--------(2)--------(4)
        /// Result:
        ///     2 is valid.
        /// </remarks>
        [Test]
        public void TestIsVertexValidTJunctions()
        {
            var tags = new TagsTableCollectionIndex();
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graph.AddVertex(0, 0);
            var vertex2 = graph.AddVertex(1, 1);
            var vertex3 = graph.AddVertex(2, 2);
            var vertex4 = graph.AddVertex(3, 3);
            var edge = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("highway", "tertiary")))
            };
            graph.AddEdge(vertex1, vertex2, edge, null);
            graph.AddEdge(vertex2, vertex3, edge, null);
            graph.AddEdge(vertex2, vertex4, edge, null);

            var encoder = new OpenLR.Referenced.Osm.ReferencedOsmEncoder(new BasicRouterDataSource<LiveEdge>(graph),
                new EncoderMock());

            Assert.IsTrue(encoder.IsVertexValid(vertex3));
        }

        /// <summary>
        /// Tests if an intermediate vertex is valid at a X-junction.
        /// </summary>
        /// <remarks>
        /// Situation:
        ///                (3)
        ///                 |
        ///                 |
        ///     (1)--------(2)--------(4)
        ///                 |
        ///                 |
        ///                (5)
        /// Result:
        ///     2 is valid.
        /// </remarks>
        [Test]
        public void TestIsVertexValidXJunctions()
        {
            var tags = new TagsTableCollectionIndex();
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graph.AddVertex(0, 0);
            var vertex2 = graph.AddVertex(1, 1);
            var vertex3 = graph.AddVertex(2, 2);
            var vertex4 = graph.AddVertex(3, 3);
            var vertex5 = graph.AddVertex(4, 4);
            var edge = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("highway", "tertiary")))
            };
            graph.AddEdge(vertex1, vertex2, edge, null);
            graph.AddEdge(vertex2, vertex3, edge, null);
            graph.AddEdge(vertex2, vertex4, edge, null);
            graph.AddEdge(vertex2, vertex5, edge, null);

            var encoder = new OpenLR.Referenced.Osm.ReferencedOsmEncoder(new BasicRouterDataSource<LiveEdge>(graph),
                new EncoderMock());

            Assert.IsTrue(encoder.IsVertexValid(vertex3));
        }

        /// <summary>
        /// Tests if an intermediate vertex is valid at a rounabout exit.
        /// </summary>
        /// <remarks>
        /// Situation:
        /// 
        ///     (1)---->---(2)---->---(4)
        ///                 |
        ///                 |
        ///                (3)
        /// Result:
        ///     2 is valid.
        /// </remarks>
        [Test]
        public void TestIsVertexValidRoundaboutExit()
        {
            var tags = new TagsTableCollectionIndex();
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graph.AddVertex(0, 0);
            var vertex2 = graph.AddVertex(1, 1);
            var vertex3 = graph.AddVertex(2, 2);
            var vertex4 = graph.AddVertex(3, 3);
            var edge = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("highway", "tertiary")))
            };
            var onwayEdge = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("highway", "tertiary"),
                    Tag.Create("oneway", "yes")))
            };
            graph.AddEdge(vertex1, vertex2, onwayEdge, null);
            graph.AddEdge(vertex2, vertex3, edge, null);
            graph.AddEdge(vertex2, vertex4, onwayEdge, null);

            var encoder = new OpenLR.Referenced.Osm.ReferencedOsmEncoder(new BasicRouterDataSource<LiveEdge>(graph),
                new EncoderMock());

            Assert.IsTrue(encoder.IsVertexValid(vertex3));
        }
    }
}