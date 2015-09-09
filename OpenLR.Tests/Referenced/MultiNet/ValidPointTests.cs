using NUnit.Framework;
using OpenLR.Referenced.Router;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Graphs;

namespace OpenLR.Tests.Referenced.MultiNet
{
    /// <summary>
    /// Contains tests to test the valid points detection on MultiNet.
    /// </summary>
    [TestFixture]
    public class ValidPointTests
    {
        /// <summary>
        /// Tests if a deadend vertex is valid.
        /// </summary>
        /// <remarks>
        /// Situation:
        ///     (1)-------(2)
        /// Result:
        ///     Both 1 and 2 are valid coming from any direction.
        /// </remarks>
        [Test]
        public void TestDeadend()
        {
            var tags = new TagsTableCollectionIndex();
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graph.AddVertex(0, 0);
            var vertex2 = graph.AddVertex(1, 1);
            var edge = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("FOW", "2"),
                    Tag.Create("FRC", "3"),
                    Tag.Create("ONEWAY", string.Empty)))
            };
            graph.AddEdge(vertex1, vertex2, edge, null);

            var encoder = new OpenLR.Referenced.MultiNet.ReferencedMultiNetEncoder(new BasicRouterDataSource<LiveEdge>(graph),
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
        public void TestTwoNeighbours()
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
                    Tag.Create("FOW", "2"),
                    Tag.Create("FRC", "3"),
                    Tag.Create("ONEWAY", string.Empty)))
            };
            graph.AddEdge(vertex1, vertex2, edge, null);
            graph.AddEdge(vertex2, vertex3, edge, null);

            var encoder = new OpenLR.Referenced.MultiNet.ReferencedMultiNetEncoder(new BasicRouterDataSource<LiveEdge>(graph),
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
        public void TestTwoNeighboursOneWay()
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
                    Tag.Create("FOW", "2"),
                    Tag.Create("FRC", "3"),
                    Tag.Create("ONEWAY", "FT")))
            };
            graph.AddEdge(vertex1, vertex2, edge, null);
            graph.AddEdge(vertex2, vertex3, edge, null);

            var encoder = new OpenLR.Referenced.MultiNet.ReferencedMultiNetEncoder(new BasicRouterDataSource<LiveEdge>(graph),
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
        public void TestTwoNeighboursHalfOneWay()
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
                    Tag.Create("FOW", "2"),
                    Tag.Create("FRC", "3"),
                    Tag.Create("ONEWAY", string.Empty)))
            };
            var onwayEdge = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("FOW", "2"),
                    Tag.Create("FRC", "3"),
                    Tag.Create("ONEWAY", "FT")))
            };
            graph.AddEdge(vertex1, vertex2, edge, null);
            graph.AddEdge(vertex2, vertex3, onwayEdge, null);

            var encoder = new OpenLR.Referenced.MultiNet.ReferencedMultiNetEncoder(new BasicRouterDataSource<LiveEdge>(graph),
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
        public void TestSplit()
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
                    Tag.Create("FOW", "2"),
                    Tag.Create("FRC", "3"),
                    Tag.Create("ONEWAY", string.Empty)))
            };
            var onwayEdge = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("FOW", "2"),
                    Tag.Create("FRC", "3"),
                    Tag.Create("ONEWAY", "FT")))
            };
            graph.AddEdge(vertex1, vertex2, edge, null);
            graph.AddEdge(vertex2, vertex3, onwayEdge, null);
            graph.AddEdge(vertex2, vertex4, onwayEdge, null);

            var encoder = new OpenLR.Referenced.MultiNet.ReferencedMultiNetEncoder(new BasicRouterDataSource<LiveEdge>(graph),
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
        public void TestJoin()
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
                    Tag.Create("FOW", "2"),
                    Tag.Create("FRC", "3"),
                    Tag.Create("ONEWAY", string.Empty)))
            };
            var onwayEdge = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("FOW", "2"),
                    Tag.Create("FRC", "3"),
                    Tag.Create("ONEWAY", "FT")))
            };
            graph.AddEdge(vertex1, vertex2, edge, null);
            graph.AddEdge(vertex2, vertex3, onwayEdge, null);
            graph.AddEdge(vertex2, vertex4, onwayEdge, null);

            var encoder = new OpenLR.Referenced.MultiNet.ReferencedMultiNetEncoder(new BasicRouterDataSource<LiveEdge>(graph),
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
        public void TestTJunction()
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
                    Tag.Create("FOW", "2"),
                    Tag.Create("FRC", "3"),
                    Tag.Create("ONEWAY", string.Empty)))
            };
            graph.AddEdge(vertex1, vertex2, edge, null);
            graph.AddEdge(vertex2, vertex3, edge, null);
            graph.AddEdge(vertex2, vertex4, edge, null);

            var encoder = new OpenLR.Referenced.MultiNet.ReferencedMultiNetEncoder(new BasicRouterDataSource<LiveEdge>(graph),
                new EncoderMock());

            Assert.IsTrue(encoder.IsVertexValid(vertex2));
        }

        /// <summary>
        /// Tests if an intermediate vertex is valid at a T-junction to a one-way street.
        /// </summary>
        /// <remarks>
        /// Situation:
        ///                (3)
        ///                 |
        ///                 |
        ///     (1)--->----(2)---->---(4)
        /// Result:
        ///     2 is valid.
        /// </remarks>
        [Test]
        public void TestTJunctionOneway()
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
                    Tag.Create("FOW", "2"),
                    Tag.Create("FRC", "3"),
                    Tag.Create("ONEWAY", string.Empty)))
            };
            var edgeOneway = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("FOW", "2"),
                    Tag.Create("FRC", "2"),
                    Tag.Create("ONEWAY", "FT")))
            };
            graph.AddEdge(vertex1, vertex2, edgeOneway, null);
            graph.AddEdge(vertex2, vertex3, edge, null);
            graph.AddEdge(vertex2, vertex4, edgeOneway, null);

            var encoder = new OpenLR.Referenced.MultiNet.ReferencedMultiNetEncoder(new BasicRouterDataSource<LiveEdge>(graph),
                new EncoderMock());

            Assert.IsTrue(encoder.IsVertexValid(vertex2));
        }

        /// <summary>
        /// Tests if an intermediate vertex is valid at a T-junction to a one-way street that has the same frc as the incoming road.
        /// </summary>
        /// <remarks>
        /// Situation:
        ///                (3)
        ///                 |
        ///                 |
        ///     (1)--->----(2)---->---(4)
        /// Result:
        ///     2 is valid.
        /// </remarks>
        [Test]
        public void TestTJunctionOnewaySameFrc()
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
                    Tag.Create("FOW", "2"),
                    Tag.Create("FRC", "3"),
                    Tag.Create("ONEWAY", string.Empty)))
            };
            var edgeOneway = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("FOW", "2"),
                    Tag.Create("FRC", "3"),
                    Tag.Create("ONEWAY", "FT")))
            };
            graph.AddEdge(vertex1, vertex2, edgeOneway, null);
            graph.AddEdge(vertex2, vertex3, edge, null);
            graph.AddEdge(vertex2, vertex4, edgeOneway, null);

            var encoder = new OpenLR.Referenced.MultiNet.ReferencedMultiNetEncoder(new BasicRouterDataSource<LiveEdge>(graph),
                new EncoderMock());

            Assert.IsTrue(encoder.IsVertexValid(vertex2));
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
        public void TestXJunctions()
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
                    Tag.Create("FOW", "2"),
                    Tag.Create("FRC", "3"),
                    Tag.Create("ONEWAY", string.Empty)))
            };
            graph.AddEdge(vertex1, vertex2, edge, null);
            graph.AddEdge(vertex2, vertex3, edge, null);
            graph.AddEdge(vertex2, vertex4, edge, null);
            graph.AddEdge(vertex2, vertex5, edge, null);

            var encoder = new OpenLR.Referenced.MultiNet.ReferencedMultiNetEncoder(new BasicRouterDataSource<LiveEdge>(graph),
                new EncoderMock());

            Assert.IsTrue(encoder.IsVertexValid(vertex2));
        }

        /// Arrival/exit roundabout:
        /// 
        /// Situation:
        ///                 (3)
        ///                 /
        ///               [2]
        ///               /
        /// (1)---[1]---(2)
        ///               \
        ///               [3]
        ///                 \
        ///                 (4)
        ///                 
        /// Result: (2) is a valid point.
        /// 
        /// (1) LAT :51.22069
        ///     LON : 5.88442
        /// (2) LAT :51.22056
        ///     LON : 5.88426
        /// (3) LAT :51.22046
        ///     LON : 5.88434
        /// (4) LAT :51.22059
        ///     LON : 5.88414
        ///     
        ///              
        /// [1] ONEWAY      :   
        ///     FRC         :   2
        ///     FOW         :   3
        /// [2] ONEWAY      :   TF
        ///     FRC         :   2
        ///     FOW         :   4
        /// [3] ONEWAY      :   FT
        ///     FRC         :   2
        ///     FOW         :   4
        [Test]
        public void TestRoundaboutExit()
        {
            var tags = new TagsTableCollectionIndex();
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graph.AddVertex(51.22069f, 5.88442f);
            var vertex2 = graph.AddVertex(51.22056f, 5.88426f);
            var vertex3 = graph.AddVertex(51.22046f, 5.88434f);
            var vertex4 = graph.AddVertex(51.22059f, 5.88414f);
            var edge1 = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("ONEWAY", string.Empty),
                    Tag.Create("FRC", "2"),
                    Tag.Create("FOW", "3")))
            };
            var edge2 = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("ONEWAY", "TF"),
                    Tag.Create("FRC", "2"),
                    Tag.Create("FOW", "4")))
            };
            var edge3 = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("ONEWAY", "FT"),
                    Tag.Create("FRC", "2"),
                    Tag.Create("FOW", "4")))
            };
            graph.AddEdge(vertex1, vertex2, edge1, null);
            graph.AddEdge(vertex2, vertex3, edge2, null);
            graph.AddEdge(vertex2, vertex4, edge3, null);

            var encoder = new OpenLR.Referenced.MultiNet.ReferencedMultiNetEncoder(new BasicRouterDataSource<LiveEdge>(graph),
                new EncoderMock());

            Assert.IsTrue(encoder.IsVertexValid(vertex2));
        }

        /// A carriage way that splits in two but there is no option to turn back:
        /// 
        /// Situation:
        ///                ---[2]---(3)
        ///               /
        /// (1)---[1]---(2)
        ///               \
        ///                ---[3]---(4)
        ///                
        /// RESULT: (2) is not a valid point.
        ///                
        /// (1) LAT :51.22218
        ///     LON : 5.87950
        /// (2) LAT :51.22183
        ///     LON : 5.88044
        /// (3) LAT :51.22149
        ///     LON : 5.88149
        /// (4) LAT :51.22144
        ///     LON : 5.88147
        ///     
        ///              
        /// [1] ONEWAY      :   
        ///     FRC         :   2
        ///     FOW         :   3
        /// [2] ONEWAY      :   FT
        ///     FRC         :   2
        ///     FOW         :   2
        /// [3] ONEWAY      :   TF
        ///     FRC         :   2
        ///     FOW         :   2
        [Test]
        public void TestCarriageWaySplit()
        {
            var tags = new TagsTableCollectionIndex();
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graph.AddVertex(51.22218f, 5.87950f);
            var vertex2 = graph.AddVertex(51.22183f, 5.88044f);
            var vertex3 = graph.AddVertex(51.22149f, 5.88149f);
            var vertex4 = graph.AddVertex(51.22144f, 5.88147f);
            var edge1 = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("ONEWAY", string.Empty),
                    Tag.Create("FRC", "2"),
                    Tag.Create("FOW", "3")))
            };
            var edge2 = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("ONEWAY", "TF"),
                    Tag.Create("FRC", "2"),
                    Tag.Create("FOW", "2")))
            };
            var edge3 = new LiveEdge()
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("ONEWAY", "FT"),
                    Tag.Create("FRC", "2"),
                    Tag.Create("FOW", "2")))
            };
            graph.AddEdge(vertex1, vertex2, edge1, null);
            graph.AddEdge(vertex2, vertex3, edge2, null);
            graph.AddEdge(vertex2, vertex4, edge3, null);

            var encoder = new OpenLR.Referenced.MultiNet.ReferencedMultiNetEncoder(new BasicRouterDataSource<LiveEdge>(graph),
                new EncoderMock());

            Assert.IsFalse(encoder.IsVertexValid(vertex2));
        }
    }
}