using NUnit.Framework;
using OpenLR.Binary.Encoders;
using OpenLR.Model;
using OpenLR.Referenced;
using OpenLR.Referenced.Encoding;
using OpenLR.Referenced.Locations;
using OpenLR.Referenced.Osm;
using OpenLR.Referenced.Router;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;

namespace OpenLR.Tests.Referenced.Osm
{
    /// <summary>
    /// Tests for referenced line location encoding.
    /// </summary>
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
            var referencedLocation  = new ReferencedLine<LiveEdge>(graph);
            referencedLocation.Edges = new LiveEdge[1];
            referencedLocation.Edges[0] = new LiveEdge()
            {
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            referencedLocation.EdgeShapes = new GeoCoordinateSimple[1][];
            referencedLocation.EdgeShapes[0] = new GeoCoordinateSimple[0];
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
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            referencedLocation.EdgeShapes = new GeoCoordinateSimple[1][];
            referencedLocation.EdgeShapes[0] = new GeoCoordinateSimple[0];
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
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            referencedLocation.EdgeShapes = new GeoCoordinateSimple[1][];
            referencedLocation.EdgeShapes[0] = new GeoCoordinateSimple[0];
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

        /// <summary>
        /// A simple referenced line location encoding test.
        /// </summary>
        /// <remarks>Tests building a line location from a path.</remarks>
        [Test]
        public void EncodeLineLocation1BuildLineLocation()
        {            
            // build a graph to encode from.
            var tags = new TagsTableCollectionIndex();
            var graphDataSource = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graphDataSource.AddVertex(49.60597f, 6.12829f);
            var vertex2 = graphDataSource.AddVertex(49.60521f, 6.12779f);
            var edge1 = new LiveEdge()
            {
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddEdge(vertex1, vertex2, edge1, null);

            // create a referenced location and encode it.
            var graph = new BasicRouterDataSource<LiveEdge>(graphDataSource);
            
            var mainEncoder = new ReferencedOsmEncoder(graph, null);
            var referencedLocation = mainEncoder.BuildLineLocation(new long[] {vertex1, vertex2}, new LiveEdge[] {edge1}, 0, 0);

            Assert.IsNotNull(referencedLocation);
            Assert.IsNotNull(referencedLocation.Vertices);
            Assert.IsNotNull(referencedLocation.Edges);
            Assert.AreEqual(2, referencedLocation.Vertices.Length);
            Assert.AreEqual(1, referencedLocation.Vertices[0]);
            Assert.AreEqual(2, referencedLocation.Vertices[1]);
            Assert.AreEqual(1, referencedLocation.Edges.Length);
            Assert.AreEqual(edge1.Distance, referencedLocation.Edges[0].Distance);
            Assert.AreEqual(edge1.Forward, referencedLocation.Edges[0].Forward);
            Assert.AreEqual(edge1.Tags, referencedLocation.Edges[0].Tags);
            Assert.AreEqual(0, referencedLocation.PositiveOffsetPercentage);
            Assert.AreEqual(0, referencedLocation.NegativeOffsetPercentage);
        }

        /// <summary>
        /// A simple referenced line location encoding test.
        /// </summary>
        /// <remarks>Tests building a line location from a path with the starting point invalid.
        /// Network:   5
        ///            |
        ///          (100m)
        ///            |
        ///            3--(100m)--1--(100m)--2
        ///            |
        ///          (100m)
        ///            |
        ///            4
        /// Toencode:           1->2
        /// Expected result:    1 is invalid, we should get 3->1->2 with a positive offset percentage of approx 50%.
        /// </remarks>
        [Test]
        public void EncodeLineLocation2BuildLineLocationInvalidStart()
        {
            // build a graph to encode from.
            var tags = new TagsTableCollectionIndex();
            var graphDataSource = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graphDataSource.AddVertex(51.05849821468899f, 3.7240000000000000f);
            var vertex2 = graphDataSource.AddVertex(51.05849821468899f, 3.7254400000000000f);
            var vertex3 = graphDataSource.AddVertex(51.05849821468899f, 3.7225627899169926f);
            var vertex4 = graphDataSource.AddVertex(51.05760000000000f, 3.7225627899169926f);
            var vertex5 = graphDataSource.AddVertex(51.05940000000000f, 3.7225627899169926f);
            var edge = new LiveEdge() // all edge are identical.
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddEdge(vertex1, vertex2, edge, null);
            graphDataSource.AddEdge(vertex1, vertex3, edge, null);
            graphDataSource.AddEdge(vertex3, vertex5, edge, null);
            graphDataSource.AddEdge(vertex3, vertex4, edge, null);

            // create a referenced location and encode it.
            var graph = new BasicRouterDataSource<LiveEdge>(graphDataSource);

            var mainEncoder = new ReferencedOsmEncoder(graph, null);
            var referencedLocation = mainEncoder.BuildLineLocation(new long[] { vertex1, vertex2 }, new LiveEdge[] { edge }, 0, 0);

            Assert.IsNotNull(referencedLocation);
            Assert.IsNotNull(referencedLocation.Vertices);
            Assert.IsNotNull(referencedLocation.Edges);
            Assert.AreEqual(3, referencedLocation.Vertices.Length);
            Assert.AreEqual(3, referencedLocation.Vertices[0]);
            Assert.AreEqual(1, referencedLocation.Vertices[1]);
            Assert.AreEqual(2, referencedLocation.Vertices[2]);
            Assert.AreEqual(2, referencedLocation.Edges.Length);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[0].Distance);
            Assert.AreEqual(!edge.Forward, referencedLocation.Edges[0].Forward); // edge was reversed.
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[0].Tags);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[1].Distance);
            Assert.AreEqual(edge.Forward, referencedLocation.Edges[1].Forward);
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[1].Tags);
            Assert.AreEqual(50, referencedLocation.PositiveOffsetPercentage, 1);
            Assert.AreEqual(0, referencedLocation.NegativeOffsetPercentage);
        }

        /// <summary>
        /// A simple referenced line location encoding test.
        /// </summary>
        /// <remarks>Tests building a line location from a path with the end point invalid.
        /// Network:                         5
        ///                                  |
        ///                                (100m)
        ///                                  |
        ///            3--(100m)--1--(100m)--2
        ///                                  |
        ///                                (100m)
        ///                                  |
        ///                                  4
        /// Toencode:           3->1
        /// Expected result:    1 is invalid, we should get 3->1->2 with a negative offset percentage of approx 50%.
        /// </remarks>
        [Test]
        public void EncodeLineLocation3BuildLineLocationInvalidEnd()
        {
            // build a graph to encode from.
            var tags = new TagsTableCollectionIndex();
            var graphDataSource = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graphDataSource.AddVertex(51.05849821468899f, 3.7240000000000000f);
            var vertex2 = graphDataSource.AddVertex(51.05849821468899f, 3.7254400000000000f);
            var vertex3 = graphDataSource.AddVertex(51.05849821468899f, 3.7225627899169926f);
            var vertex4 = graphDataSource.AddVertex(51.05760000000000f, 3.7254400000000000f);
            var vertex5 = graphDataSource.AddVertex(51.05940000000000f, 3.7254400000000000f);
            var edge = new LiveEdge() // all edge are identical.
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddEdge(vertex1, vertex2, edge, null);
            graphDataSource.AddEdge(vertex1, vertex3, edge, null);
            graphDataSource.AddEdge(vertex2, vertex5, edge, null);
            graphDataSource.AddEdge(vertex2, vertex4, edge, null);

            // create a referenced location and encode it.
            var graph = new BasicRouterDataSource<LiveEdge>(graphDataSource);

            var mainEncoder = new ReferencedOsmEncoder(graph, null);
            var referencedLocation = mainEncoder.BuildLineLocation(new long[] { vertex3, vertex1 }, new LiveEdge[] { edge.ToReverse() }, 0, 0);

            Assert.IsNotNull(referencedLocation);
            Assert.IsNotNull(referencedLocation.Vertices);
            Assert.IsNotNull(referencedLocation.Edges);
            Assert.AreEqual(3, referencedLocation.Vertices.Length);
            Assert.AreEqual(3, referencedLocation.Vertices[0]);
            Assert.AreEqual(1, referencedLocation.Vertices[1]);
            Assert.AreEqual(2, referencedLocation.Vertices[2]);
            Assert.AreEqual(2, referencedLocation.Edges.Length);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[0].Distance);
            Assert.AreEqual(!edge.Forward, referencedLocation.Edges[0].Forward); // edge was reversed.
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[0].Tags);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[1].Distance);
            Assert.AreEqual(edge.Forward, referencedLocation.Edges[1].Forward);
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[1].Tags);
            Assert.AreEqual(0, referencedLocation.PositiveOffsetPercentage, 1);
            Assert.AreEqual(50, referencedLocation.NegativeOffsetPercentage, 1);
        }

        /// <summary>
        /// A simple referenced line location encoding test.
        /// </summary>
        /// <remarks>Tests building a line location from a path with the starting point invalid.
        /// Network:   5
        ///            |
        ///          (100m)
        ///            |
        ///            3--(100m)--1--(100m)--2
        ///            |
        ///          (100m)
        ///            |
        ///            4
        /// Toencode:           1->2 with positive offset of 20%
        /// Expected result:    1 is invalid, we should get 3->1->2 with a positive offset percentage of approx 60%.
        /// </remarks>
        [Test]
        public void EncodeLineLocation4BuildLineLocationInvalidStartWithOffset()
        {
            // build a graph to encode from.
            var tags = new TagsTableCollectionIndex();
            var graphDataSource = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graphDataSource.AddVertex(51.05849821468899f, 3.7240000000000000f);
            var vertex2 = graphDataSource.AddVertex(51.05849821468899f, 3.7254400000000000f);
            var vertex3 = graphDataSource.AddVertex(51.05849821468899f, 3.7225627899169926f);
            var vertex4 = graphDataSource.AddVertex(51.05760000000000f, 3.7225627899169926f);
            var vertex5 = graphDataSource.AddVertex(51.05940000000000f, 3.7225627899169926f);
            var edge = new LiveEdge() // all edge are identical.
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddEdge(vertex1, vertex2, edge, null);
            graphDataSource.AddEdge(vertex1, vertex3, edge, null);
            graphDataSource.AddEdge(vertex3, vertex5, edge, null);
            graphDataSource.AddEdge(vertex3, vertex4, edge, null);

            // create a referenced location and encode it.
            var graph = new BasicRouterDataSource<LiveEdge>(graphDataSource);

            var mainEncoder = new ReferencedOsmEncoder(graph, null);
            var referencedLocation = mainEncoder.BuildLineLocation(new long[] { vertex1, vertex2 }, new LiveEdge[] { edge }, 20, 0);

            Assert.IsNotNull(referencedLocation);
            Assert.IsNotNull(referencedLocation.Vertices);
            Assert.IsNotNull(referencedLocation.Edges);
            Assert.AreEqual(3, referencedLocation.Vertices.Length);
            Assert.AreEqual(3, referencedLocation.Vertices[0]);
            Assert.AreEqual(1, referencedLocation.Vertices[1]);
            Assert.AreEqual(2, referencedLocation.Vertices[2]);
            Assert.AreEqual(2, referencedLocation.Edges.Length);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[0].Distance);
            Assert.AreEqual(!edge.Forward, referencedLocation.Edges[0].Forward); // edge was reversed.
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[0].Tags);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[1].Distance);
            Assert.AreEqual(edge.Forward, referencedLocation.Edges[1].Forward);
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[1].Tags);
            Assert.AreEqual(60, referencedLocation.PositiveOffsetPercentage, 1);
            Assert.AreEqual(0, referencedLocation.NegativeOffsetPercentage);
        }

        /// <summary>
        /// A simple referenced line location encoding test.
        /// </summary>
        /// <remarks>Tests building a line location from a path with the end point invalid.
        /// Network:                         5
        ///                                  |
        ///                                (100m)
        ///                                  |
        ///            3--(100m)--1--(100m)--2
        ///                                  |
        ///                                (100m)
        ///                                  |
        ///                                  4
        /// Toencode:           3->1 with negative offset of 20%
        /// Expected result:    1 is invalid, we should get 3->1->2 with a negative offset percentage of approx 60%.
        /// </remarks>
        [Test]
        public void EncodeLineLocation5BuildLineLocationInvalidEndWithOffset()
        {
            // build a graph to encode from.
            var tags = new TagsTableCollectionIndex();
            var graphDataSource = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graphDataSource.AddVertex(51.05849821468899f, 3.7240000000000000f);
            var vertex2 = graphDataSource.AddVertex(51.05849821468899f, 3.7254400000000000f);
            var vertex3 = graphDataSource.AddVertex(51.05849821468899f, 3.7225627899169926f);
            var vertex4 = graphDataSource.AddVertex(51.05760000000000f, 3.7254400000000000f);
            var vertex5 = graphDataSource.AddVertex(51.05940000000000f, 3.7254400000000000f);
            var edge = new LiveEdge() // all edge are identical.
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddEdge(vertex1, vertex2, edge, null);
            graphDataSource.AddEdge(vertex1, vertex3, edge, null);
            graphDataSource.AddEdge(vertex2, vertex5, edge, null);
            graphDataSource.AddEdge(vertex2, vertex4, edge, null);

            // create a referenced location and encode it.
            var graph = new BasicRouterDataSource<LiveEdge>(graphDataSource);

            var mainEncoder = new ReferencedOsmEncoder(graph, null);
            var referencedLocation = mainEncoder.BuildLineLocation(new long[] { vertex3, vertex1 }, new LiveEdge[] { edge.ToReverse() }, 0, 20);

            Assert.IsNotNull(referencedLocation);
            Assert.IsNotNull(referencedLocation.Vertices);
            Assert.IsNotNull(referencedLocation.Edges);
            Assert.AreEqual(3, referencedLocation.Vertices.Length);
            Assert.AreEqual(3, referencedLocation.Vertices[0]);
            Assert.AreEqual(1, referencedLocation.Vertices[1]);
            Assert.AreEqual(2, referencedLocation.Vertices[2]);
            Assert.AreEqual(2, referencedLocation.Edges.Length);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[0].Distance);
            Assert.AreEqual(!edge.Forward, referencedLocation.Edges[0].Forward); // edge was reversed.
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[0].Tags);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[1].Distance);
            Assert.AreEqual(edge.Forward, referencedLocation.Edges[1].Forward);
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[1].Tags);
            Assert.AreEqual(0, referencedLocation.PositiveOffsetPercentage, 1);
            Assert.AreEqual(60, referencedLocation.NegativeOffsetPercentage, 1);
        }

        /// <summary>
        /// A simple referenced line location encoding test.
        /// </summary>
        /// <remarks>Tests building a line location from a path and then binary encoding it.</remarks>
        [Test]
        public void EncodeLineLocation6EncodeReferenced()
        {
            // build a graph to encode from.
            var tags = new TagsTableCollectionIndex();
            var graphDataSource = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graphDataSource.AddVertex(49.60597f, 6.12829f);
            var vertex2 = graphDataSource.AddVertex(49.60521f, 6.12779f);
            var edge1 = new LiveEdge()
            {
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddEdge(vertex1, vertex2, edge1, null);

            // create a referenced location and encode it.
            var graph = new BasicRouterDataSource<LiveEdge>(graphDataSource);

            // encode location.
            var encoder = new LineEncoder();
            var router = new DykstraRoutingLive();
            var mainEncoder = new ReferencedOsmEncoder(graph, null);
            var referencedEncoder = new ReferencedLineEncoder<LiveEdge>(mainEncoder, encoder);
            var referencedLocation = mainEncoder.BuildLineLocation(new long[] { vertex1, vertex2 }, new LiveEdge[] { edge1 }, 0, 0);
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
        }

        /// <summary>
        /// A simple referenced line location encoding test.
        /// </summary>
        /// <remarks>Tests building a line location from a path with the starting point invalid and then encoding it.
        /// Network:   5
        ///            |
        ///          (100m)
        ///            |
        ///            3--(100m)--1--(100m)--2
        ///            |
        ///          (100m)
        ///            |
        ///            4
        /// Toencode:           1->2
        /// Expected result:    1 is invalid, we should get 3->1->2 with a positive offset percentage of approx 50%.
        /// </remarks>
        [Test]
        public void EncodeLineLocation7EncodeLineLocationInvalidStart()
        {
            // build a graph to encode from.
            var tags = new TagsTableCollectionIndex();
            var graphDataSource = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graphDataSource.AddVertex(51.05849821468899f, 3.7240000000000000f);
            var vertex2 = graphDataSource.AddVertex(51.05849821468899f, 3.7254400000000000f);
            var vertex3 = graphDataSource.AddVertex(51.05849821468899f, 3.7225627899169926f);
            var vertex4 = graphDataSource.AddVertex(51.05760000000000f, 3.7225627899169926f);
            var vertex5 = graphDataSource.AddVertex(51.05940000000000f, 3.7225627899169926f);
            var edge = new LiveEdge() // all edge are identical.
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddEdge(vertex1, vertex2, edge, null);
            graphDataSource.AddEdge(vertex1, vertex3, edge, null);
            graphDataSource.AddEdge(vertex3, vertex5, edge, null);
            graphDataSource.AddEdge(vertex3, vertex4, edge, null);

            // create a referenced location and encode it.
            var graph = new BasicRouterDataSource<LiveEdge>(graphDataSource);

            // encode location.
            var encoder = new LineEncoder();
            var router = new DykstraRoutingLive();
            var mainEncoder = new ReferencedOsmEncoder(graph, null);
            var referencedEncoder = new ReferencedLineEncoder<LiveEdge>(mainEncoder, encoder);
            var referencedLocation = mainEncoder.BuildLineLocation(new long[] { vertex1, vertex2 }, new LiveEdge[] { edge }, 0, 0);
            var location = referencedEncoder.EncodeReferenced(referencedLocation);

            // test result.
            Assert.IsNotNull(location);
            Assert.AreEqual(51.05849821468899f, location.First.Coordinate.Latitude, 0.0001);
            Assert.AreEqual(3.722562789916992f, location.First.Coordinate.Longitude, 0.0001);
            Assert.AreEqual(200, location.First.DistanceToNext, 5);
            Assert.AreEqual(90, location.First.Bearing, 11);
            Assert.AreEqual(FormOfWay.SingleCarriageWay, location.First.FormOfWay);
            Assert.AreEqual(FunctionalRoadClass.Frc3, location.First.FuntionalRoadClass);
            Assert.AreEqual(FunctionalRoadClass.Frc3, location.First.LowestFunctionalRoadClassToNext);

            Assert.AreEqual(51.058498214688990f, location.Last.Coordinate.Latitude, 0.0001);
            Assert.AreEqual(3.7254400000000000f, location.Last.Coordinate.Longitude, 0.0001);
            Assert.AreEqual(270, location.Last.Bearing, 11);

            Assert.AreEqual(50, location.PositiveOffsetPercentage, 2);
            Assert.AreEqual(0, location.NegativeOffsetPercentage, 2);
        }

        /// <summary>
        /// A simple referenced line location encoding test.
        /// </summary>
        /// <remarks>Tests building a line location from a path with the end point invalid.
        /// Network:                         5
        ///                                  |
        ///                                (100m)
        ///                                  |
        ///            3--(100m)--1--(100m)--2
        ///                                  |
        ///                                (100m)
        ///                                  |
        ///                                  4
        /// Toencode:           3->1
        /// Expected result:    1 is invalid, we should get 3->1->2 with a negative offset percentage of approx 50%.
        /// </remarks>
        [Test]
        public void EncodeLineLocation8EncodeLineLocationInvalidEnd()
        {
            // build a graph to encode from.
            var tags = new TagsTableCollectionIndex();
            var graphDataSource = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graphDataSource.AddVertex(51.05849821468899f, 3.7240000000000000f);
            var vertex2 = graphDataSource.AddVertex(51.05849821468899f, 3.7254400000000000f);
            var vertex3 = graphDataSource.AddVertex(51.05849821468899f, 3.7225627899169926f);
            var vertex4 = graphDataSource.AddVertex(51.05760000000000f, 3.7254400000000000f);
            var vertex5 = graphDataSource.AddVertex(51.05940000000000f, 3.7254400000000000f);
            var edge = new LiveEdge() // all edge are identical.
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddEdge(vertex1, vertex2, edge, null);
            graphDataSource.AddEdge(vertex1, vertex3, edge, null);
            graphDataSource.AddEdge(vertex2, vertex5, edge, null);
            graphDataSource.AddEdge(vertex2, vertex4, edge, null);

            // create a referenced location and encode it.
            var graph = new BasicRouterDataSource<LiveEdge>(graphDataSource);

            // encode location.
            var encoder = new LineEncoder();
            var router = new DykstraRoutingLive();
            var mainEncoder = new ReferencedOsmEncoder(graph, null);
            var referencedEncoder = new ReferencedLineEncoder<LiveEdge>(mainEncoder, encoder);
            var referencedLocation = mainEncoder.BuildLineLocation(new long[] { vertex3, vertex1 }, new LiveEdge[] { edge.ToReverse() }, 0, 0);
            var location = referencedEncoder.EncodeReferenced(referencedLocation);

            // test result.
            Assert.IsNotNull(location);
            Assert.AreEqual(51.05849821468899f, location.First.Coordinate.Latitude, 0.0001);
            Assert.AreEqual(3.7225627899169926f, location.First.Coordinate.Longitude, 0.0001);
            Assert.AreEqual(200, location.First.DistanceToNext, 5);
            Assert.AreEqual(90, location.First.Bearing, 11);
            Assert.AreEqual(FormOfWay.SingleCarriageWay, location.First.FormOfWay);
            Assert.AreEqual(FunctionalRoadClass.Frc3, location.First.FuntionalRoadClass);
            Assert.AreEqual(FunctionalRoadClass.Frc3, location.First.LowestFunctionalRoadClassToNext);

            Assert.AreEqual(51.058498214688990f, location.Last.Coordinate.Latitude, 0.0001);
            Assert.AreEqual(3.7254400000000000f, location.Last.Coordinate.Longitude, 0.0001);
            Assert.AreEqual(270, location.Last.Bearing, 11);

            Assert.AreEqual(0, location.PositiveOffsetPercentage, 2);
            Assert.AreEqual(50, location.NegativeOffsetPercentage, 2);
        }

        /// <summary>
        /// A simple referenced line location encoding test.
        /// </summary>
        /// <remarks>Tests building a line location when only given two locations.
        /// Network:  
        ///            3--(100m)--1--(100m)--2
        /// Toencode:           1->2
        /// Expected result:    1 is invalid, we should get 3->1->2 with a positive offset percentage of approx 50%.
        /// </remarks>
        [Test]
        public void EncodeLineLocation9BuildLineLocationSimple()
        {
            // build a graph to encode from.
            var tags = new TagsTableCollectionIndex();
            var graphDataSource = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graphDataSource.AddVertex(51.05849821468899f, 3.7240000000000000f);
            var vertex2 = graphDataSource.AddVertex(51.05849821468899f, 3.7254400000000000f);
            var vertex3 = graphDataSource.AddVertex(51.05849821468899f, 3.7225627899169926f);
            var edge = new LiveEdge() // all edge are identical.
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddEdge(vertex1, vertex2, edge, null);
            graphDataSource.AddEdge(vertex1, vertex3, edge, null);

            // create a referenced location and encode it.
            var graph = new BasicRouterDataSource<LiveEdge>(graphDataSource);

            var mainEncoder = new ReferencedOsmEncoder(graph, null);
            var referencedLocation = mainEncoder.BuildLineLocation(
                new GeoCoordinate(51.05849821468899f, 3.7240000000000000f),
                new GeoCoordinate(51.05849821468899f, 3.7254400000000000f));

            Assert.IsNotNull(referencedLocation);
            Assert.IsNotNull(referencedLocation.Vertices);
            Assert.IsNotNull(referencedLocation.Edges);
            Assert.AreEqual(3, referencedLocation.Vertices.Length);
            Assert.AreEqual(3, referencedLocation.Vertices[0]);
            Assert.AreEqual(1, referencedLocation.Vertices[1]);
            Assert.AreEqual(2, referencedLocation.Vertices[2]);
            Assert.AreEqual(2, referencedLocation.Edges.Length);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[0].Distance);
            Assert.AreEqual(!edge.Forward, referencedLocation.Edges[0].Forward); // edge was reversed.
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[0].Tags);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[1].Distance);
            Assert.AreEqual(edge.Forward, referencedLocation.Edges[1].Forward);
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[1].Tags);
            Assert.AreEqual(50, referencedLocation.PositiveOffsetPercentage, 1);
            Assert.AreEqual(0, referencedLocation.NegativeOffsetPercentage);
        }

        /// <summary>
        /// A simple referenced line location encoding test.
        /// </summary>
        /// <remarks>Tests building a line location when only given two locations.
        /// Network:  
        ///            3--(100m)--1--(100m)--2
        /// Toencode:           3->1
        /// Expected result:    1 is invalid, we should get 3->1->2 with a negative offset percentage of approx 50%.
        /// </remarks>
        [Test]
        public void EncodeLineLocation10BuildLineLocationSimple()
        {
            // build a graph to encode from.
            var tags = new TagsTableCollectionIndex();
            var graphDataSource = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graphDataSource.AddVertex(51.05849821468899f, 3.7240000000000000f);
            var vertex2 = graphDataSource.AddVertex(51.05849821468899f, 3.7254400000000000f);
            var vertex3 = graphDataSource.AddVertex(51.05849821468899f, 3.7225627899169926f);
            var edge = new LiveEdge() // all edge are identical.
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddEdge(vertex1, vertex2, edge, null);
            graphDataSource.AddEdge(vertex1, vertex3, edge, null);

            // create a referenced location and encode it.
            var graph = new BasicRouterDataSource<LiveEdge>(graphDataSource);

            var mainEncoder = new ReferencedOsmEncoder(graph, null);
            var referencedLocation = mainEncoder.BuildLineLocation(
                new GeoCoordinate(51.05849821468899f, 3.7225627899169926f),
                new GeoCoordinate(51.05849821468899f, 3.7240000000000000f));

            Assert.IsNotNull(referencedLocation);
            Assert.IsNotNull(referencedLocation.Vertices);
            Assert.IsNotNull(referencedLocation.Edges);
            Assert.AreEqual(3, referencedLocation.Vertices.Length);
            Assert.AreEqual(3, referencedLocation.Vertices[0]);
            Assert.AreEqual(1, referencedLocation.Vertices[1]);
            Assert.AreEqual(2, referencedLocation.Vertices[2]);
            Assert.AreEqual(2, referencedLocation.Edges.Length);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[0].Distance);
            Assert.AreEqual(!edge.Forward, referencedLocation.Edges[0].Forward); // edge was reversed.
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[0].Tags);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[1].Distance);
            Assert.AreEqual(edge.Forward, referencedLocation.Edges[1].Forward);
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[1].Tags);
            Assert.AreEqual(0, referencedLocation.PositiveOffsetPercentage, 1);
            Assert.AreEqual(50, referencedLocation.NegativeOffsetPercentage, 1);
        }

        /// <summary>
        /// A simple referenced line location encoding test.
        /// </summary>
        /// <remarks>Tests building a line location when only given two locations.
        /// Network:  
        ///            1--(100m)--(x)--(100m)--2
        /// Toencode:           x->2
        /// Expected result:    x is not a vertex, we should get 1->2 with a positive offset percentage of approx 50%.
        /// </remarks>
        [Test]
        public void EncodeLineLocation11BuildLineLocationSimple()
        {
            // build a graph to encode from.
            var tags = new TagsTableCollectionIndex();
            var graphDataSource = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graphDataSource.AddVertex(51.05849821468899f, 3.7225627899169926f);
            var vertex2 = graphDataSource.AddVertex(51.05849821468899f, 3.7254400000000000f);
            //var vertex3 = graphDataSource.AddVertex(51.05849821468899f, 3.7240000000000000f);
            var edge = new LiveEdge() // all edge are identical.
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddEdge(vertex1, vertex2, edge, null);

            // create a referenced location and encode it.
            var graph = new BasicRouterDataSource<LiveEdge>(graphDataSource);

            var mainEncoder = new ReferencedOsmEncoder(graph, null);
            var referencedLocation = mainEncoder.BuildLineLocation(
                new GeoCoordinate(51.05849821468899f, 3.7240000000000000f),
                new GeoCoordinate(51.05849821468899f, 3.7254400000000000f));

            // WARNING: direction here is actually unpredictable, it's either edge 1->2 that is found in the initial search or 2->1.
            Assert.IsNotNull(referencedLocation);
            Assert.IsNotNull(referencedLocation.Vertices);
            Assert.IsNotNull(referencedLocation.Edges);
            Assert.AreEqual(2, referencedLocation.Vertices.Length);
            Assert.AreEqual(1, referencedLocation.Vertices[0]);
            Assert.AreEqual(2, referencedLocation.Vertices[1]);
            Assert.AreEqual(1, referencedLocation.Edges.Length);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[0].Distance);
            Assert.AreEqual(edge.Forward, referencedLocation.Edges[0].Forward);
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[0].Tags);
            Assert.AreEqual(50, referencedLocation.PositiveOffsetPercentage, 1);
            Assert.AreEqual(0, referencedLocation.NegativeOffsetPercentage);
        }

        /// <summary>
        /// A simple referenced line location encoding test.
        /// </summary>
        /// <remarks>Tests building a line location when only given two locations.
        /// Network:  
        ///            1--(100m)--(x)--(100m)--2
        /// Toencode:           1->x
        /// Expected result:    x is not a vertex, we should get 1->2 with a negative offset percentage of approx 50%.
        /// </remarks>
        [Test]
        public void EncodeLineLocation12BuildLineLocationSimple()
        {
            // build a graph to encode from.
            var tags = new TagsTableCollectionIndex();
            var graphDataSource = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graphDataSource.AddVertex(51.05849821468899f, 3.7225627899169926f);
            var vertex2 = graphDataSource.AddVertex(51.05849821468899f, 3.7254400000000000f);
            //var vertex3 = graphDataSource.AddVertex(51.05849821468899f, 3.7225627899169926f);
            var edge = new LiveEdge() // all edge are identical.
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddEdge(vertex1, vertex2, edge, null);

            // create a referenced location and encode it.
            var graph = new BasicRouterDataSource<LiveEdge>(graphDataSource);

            var mainEncoder = new ReferencedOsmEncoder(graph, null);
            var referencedLocation = mainEncoder.BuildLineLocation(
                new GeoCoordinate(51.05849821468899f, 3.7225627899169926f),
                new GeoCoordinate(51.05849821468899f, 3.7240000000000000f));

            Assert.IsNotNull(referencedLocation);
            Assert.IsNotNull(referencedLocation.Vertices);
            Assert.IsNotNull(referencedLocation.Edges);
            Assert.AreEqual(2, referencedLocation.Vertices.Length);
            Assert.AreEqual(1, referencedLocation.Vertices[0]);
            Assert.AreEqual(2, referencedLocation.Vertices[1]);
            Assert.AreEqual(1, referencedLocation.Edges.Length);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[0].Distance);
            Assert.AreEqual(edge.Forward, referencedLocation.Edges[0].Forward); // edge was reversed.
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[0].Tags);
            Assert.AreEqual(0, referencedLocation.PositiveOffsetPercentage, 1);
            Assert.AreEqual(50, referencedLocation.NegativeOffsetPercentage, 1);
        }

        /// <summary>
        /// A simple referenced line location encoding test.
        /// </summary>
        /// <remarks>Tests building a line location when only given two locations.
        /// Network:  
        ///            3--(100m)->-1->-(100m)--2
        /// Toencode:           1->2
        /// Expected result:    1 is invalid, we should get 3->1->2 with a positive offset percentage of approx 50%.
        /// </remarks>
        [Test]
        public void EncodeLineLocation13BuildLineLocationSimple()
        {
            // build a graph to encode from.
            var tags = new TagsTableCollectionIndex();
            var graphDataSource = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graphDataSource.AddVertex(51.05849821468899f, 3.7240000000000000f);
            var vertex2 = graphDataSource.AddVertex(51.05849821468899f, 3.7254400000000000f);
            var vertex3 = graphDataSource.AddVertex(51.05849821468899f, 3.7225627899169926f);
            var edge = new LiveEdge() // all edge are identical.
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("highway", "tertiary"),
                    Tag.Create("oneway", "yes")))
            };
            graphDataSource.AddEdge(vertex1, vertex2, edge, null);
            graphDataSource.AddEdge(vertex3, vertex1, edge, null);

            // create a referenced location and encode it.
            var graph = new BasicRouterDataSource<LiveEdge>(graphDataSource);

            var mainEncoder = new ReferencedOsmEncoder(graph, null);
            var referencedLocation = mainEncoder.BuildLineLocation(
                new GeoCoordinate(51.05849821468899f, 3.7240000000000000f),
                new GeoCoordinate(51.05849821468899f, 3.7254400000000000f));

            Assert.IsNotNull(referencedLocation);
            Assert.IsNotNull(referencedLocation.Vertices);
            Assert.IsNotNull(referencedLocation.Edges);
            Assert.AreEqual(3, referencedLocation.Vertices.Length);
            Assert.AreEqual(3, referencedLocation.Vertices[0]);
            Assert.AreEqual(1, referencedLocation.Vertices[1]);
            Assert.AreEqual(2, referencedLocation.Vertices[2]);
            Assert.AreEqual(2, referencedLocation.Edges.Length);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[0].Distance);
            Assert.AreEqual(edge.Forward, referencedLocation.Edges[0].Forward); // edge was reversed.
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[0].Tags);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[1].Distance);
            Assert.AreEqual(edge.Forward, referencedLocation.Edges[1].Forward);
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[1].Tags);
            Assert.AreEqual(50, referencedLocation.PositiveOffsetPercentage, 1);
            Assert.AreEqual(0, referencedLocation.NegativeOffsetPercentage);
        }

        /// <summary>
        /// A simple referenced line location encoding test.
        /// </summary>
        /// <remarks>Tests building a line location when only given two locations.
        /// Network:  
        ///            3->-(50m)->-(x)--(50m)->-1->-(50m)->-(y)->-(50m)->-(2)
        /// Toencode:           x->y
        /// Expected result:    We should get 3->1->2 with a positive and negative offset percentage of approx 25%.
        /// </remarks>
        [Test]
        public void EncodeLineLocation14BuildLineLocationSimple()
        {
            // build a graph to encode from.
            var tags = new TagsTableCollectionIndex();
            var graphDataSource = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graphDataSource.AddVertex(51.05849821468899f, 3.7240000000000000f);
            var vertex2 = graphDataSource.AddVertex(51.05849821468899f, 3.7254400000000000f);
            var vertex3 = graphDataSource.AddVertex(51.05849821468899f, 3.7225627899169926f);
            var edge = new LiveEdge() // all edge are identical.
            {
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create("highway", "tertiary"),
                    Tag.Create("oneway", "yes")))
            };
            graphDataSource.AddEdge(vertex1, vertex2, edge, null);
            graphDataSource.AddEdge(vertex3, vertex1, edge, null);

            // define x and y.
            var x = new GeoCoordinate(51.05849821468899, 3.723265528678894);
            var y = new GeoCoordinate(51.05849821468899, 3.724719285964966);

            // create a referenced location and encode it.
            var graph = new BasicRouterDataSource<LiveEdge>(graphDataSource);

            var mainEncoder = new ReferencedOsmEncoder(graph, null);
            var referencedLocation = mainEncoder.BuildLineLocation(x, y);

            Assert.IsNotNull(referencedLocation);
            Assert.IsNotNull(referencedLocation.Vertices);
            Assert.IsNotNull(referencedLocation.Edges);
            Assert.AreEqual(3, referencedLocation.Vertices.Length);
            Assert.AreEqual(3, referencedLocation.Vertices[0]);
            Assert.AreEqual(1, referencedLocation.Vertices[1]);
            Assert.AreEqual(2, referencedLocation.Vertices[2]);
            Assert.AreEqual(2, referencedLocation.Edges.Length);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[0].Distance);
            Assert.AreEqual(edge.Forward, referencedLocation.Edges[0].Forward); // edge was reversed.
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[0].Tags);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[1].Distance);
            Assert.AreEqual(edge.Forward, referencedLocation.Edges[1].Forward);
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[1].Tags);
            Assert.AreEqual(25, referencedLocation.PositiveOffsetPercentage, 1);
            Assert.AreEqual(25, referencedLocation.NegativeOffsetPercentage, 1);
        }
    }
}
