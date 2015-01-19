using NUnit.Framework;
using OpenLR.Binary.Encoders;
using OpenLR.Model;
using OpenLR.OsmSharp;
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
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddArc(vertex1, vertex2, edge1, null);
            graphDataSource.AddArc(vertex2, vertex1, new LiveEdge()
            {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            }, null);

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
            Assert.AreEqual(edge1.Coordinates, referencedLocation.Edges[0].Coordinates);
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
                Coordinates = null,
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddArc(vertex1, vertex2, edge, null);
            graphDataSource.AddArc(vertex2, vertex1, edge.ToReverse(), null);
            graphDataSource.AddArc(vertex1, vertex3, edge, null);
            graphDataSource.AddArc(vertex3, vertex1, edge.ToReverse(), null);
            graphDataSource.AddArc(vertex3, vertex5, edge, null);
            graphDataSource.AddArc(vertex5, vertex3, edge.ToReverse(), null);
            graphDataSource.AddArc(vertex3, vertex4, edge, null);
            graphDataSource.AddArc(vertex4, vertex3, edge.ToReverse(), null);

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
            Assert.AreEqual(edge.Coordinates, referencedLocation.Edges[0].Coordinates);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[0].Distance);
            Assert.AreEqual(!edge.Forward, referencedLocation.Edges[0].Forward); // edge was reversed.
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[0].Tags);
            Assert.AreEqual(edge.Coordinates, referencedLocation.Edges[1].Coordinates);
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
                Coordinates = null,
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddArc(vertex1, vertex2, edge, null);
            graphDataSource.AddArc(vertex2, vertex1, edge.ToReverse(), null);
            graphDataSource.AddArc(vertex1, vertex3, edge, null);
            graphDataSource.AddArc(vertex3, vertex1, edge.ToReverse(), null);
            graphDataSource.AddArc(vertex2, vertex5, edge, null);
            graphDataSource.AddArc(vertex5, vertex2, edge.ToReverse(), null);
            graphDataSource.AddArc(vertex2, vertex4, edge, null);
            graphDataSource.AddArc(vertex4, vertex2, edge.ToReverse(), null);

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
            Assert.AreEqual(edge.Coordinates, referencedLocation.Edges[0].Coordinates);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[0].Distance);
            Assert.AreEqual(!edge.Forward, referencedLocation.Edges[0].Forward); // edge was reversed.
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[0].Tags);
            Assert.AreEqual(edge.Coordinates, referencedLocation.Edges[1].Coordinates);
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
                Coordinates = null,
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddArc(vertex1, vertex2, edge, null);
            graphDataSource.AddArc(vertex2, vertex1, edge.ToReverse(), null);
            graphDataSource.AddArc(vertex1, vertex3, edge, null);
            graphDataSource.AddArc(vertex3, vertex1, edge.ToReverse(), null);
            graphDataSource.AddArc(vertex3, vertex5, edge, null);
            graphDataSource.AddArc(vertex5, vertex3, edge.ToReverse(), null);
            graphDataSource.AddArc(vertex3, vertex4, edge, null);
            graphDataSource.AddArc(vertex4, vertex3, edge.ToReverse(), null);

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
            Assert.AreEqual(edge.Coordinates, referencedLocation.Edges[0].Coordinates);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[0].Distance);
            Assert.AreEqual(!edge.Forward, referencedLocation.Edges[0].Forward); // edge was reversed.
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[0].Tags);
            Assert.AreEqual(edge.Coordinates, referencedLocation.Edges[1].Coordinates);
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
                Coordinates = null,
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddArc(vertex1, vertex2, edge, null);
            graphDataSource.AddArc(vertex2, vertex1, edge.ToReverse(), null);
            graphDataSource.AddArc(vertex1, vertex3, edge, null);
            graphDataSource.AddArc(vertex3, vertex1, edge.ToReverse(), null);
            graphDataSource.AddArc(vertex2, vertex5, edge, null);
            graphDataSource.AddArc(vertex5, vertex2, edge.ToReverse(), null);
            graphDataSource.AddArc(vertex2, vertex4, edge, null);
            graphDataSource.AddArc(vertex4, vertex2, edge.ToReverse(), null);

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
            Assert.AreEqual(edge.Coordinates, referencedLocation.Edges[0].Coordinates);
            Assert.AreEqual(edge.Distance, referencedLocation.Edges[0].Distance);
            Assert.AreEqual(!edge.Forward, referencedLocation.Edges[0].Forward); // edge was reversed.
            Assert.AreEqual(edge.Tags, referencedLocation.Edges[0].Tags);
            Assert.AreEqual(edge.Coordinates, referencedLocation.Edges[1].Coordinates);
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
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddArc(vertex1, vertex2, edge1, null);
            graphDataSource.AddArc(vertex2, vertex1, new LiveEdge()
            {
                Coordinates = null,
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            }, null);

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
                Coordinates = null,
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddArc(vertex1, vertex2, edge, null);
            graphDataSource.AddArc(vertex2, vertex1, edge.ToReverse(), null);
            graphDataSource.AddArc(vertex1, vertex3, edge, null);
            graphDataSource.AddArc(vertex3, vertex1, edge.ToReverse(), null);
            graphDataSource.AddArc(vertex3, vertex5, edge, null);
            graphDataSource.AddArc(vertex5, vertex3, edge.ToReverse(), null);
            graphDataSource.AddArc(vertex3, vertex4, edge, null);
            graphDataSource.AddArc(vertex4, vertex3, edge.ToReverse(), null);

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
                Coordinates = null,
                Distance = 100,
                Forward = true,
                Tags = tags.Add(new TagsCollection(Tag.Create("highway", "tertiary")))
            };
            graphDataSource.AddArc(vertex1, vertex2, edge, null);
            graphDataSource.AddArc(vertex2, vertex1, edge.ToReverse(), null);
            graphDataSource.AddArc(vertex1, vertex3, edge, null);
            graphDataSource.AddArc(vertex3, vertex1, edge.ToReverse(), null);
            graphDataSource.AddArc(vertex2, vertex5, edge, null);
            graphDataSource.AddArc(vertex5, vertex2, edge.ToReverse(), null);
            graphDataSource.AddArc(vertex2, vertex4, edge, null);
            graphDataSource.AddArc(vertex4, vertex2, edge.ToReverse(), null);

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
    }
}
