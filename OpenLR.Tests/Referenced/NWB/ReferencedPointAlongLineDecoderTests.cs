using NUnit.Framework;
using OpenLR.Binary;
using OpenLR.Binary.Decoders;
using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.Referenced.Decoding;
using OpenLR.Referenced.NWB;
using OpenLR.Referenced.Router;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Graphs;

namespace OpenLR.Tests.Referenced.NWB
{
    /// <summary>
    /// Contains tests for decoding/encoding a OpenLR line location to a referenced line location.
    /// </summary>
    [TestFixture]
    public class ReferencedPointAlongLineDecoderTests
    {
        /// <summary>
        /// A simple referenced point along line location decoding test.
        /// </summary>
        [Test]
        public void DecodeReferencedPointAlongLineLocation()
        {
            var delta = 0.0001;

            // build the location to decode.
            var location = new PointAlongLineLocation();
            location.First = new LocationReferencePoint();
            location.First.Coordinate = new Coordinate() { Latitude = 49.60597, Longitude = 6.12829 };
            location.First.DistanceToNext = 92;
            location.First.FuntionalRoadClass = FunctionalRoadClass.Frc2;
            location.First.FormOfWay = FormOfWay.MultipleCarriageWay;
            location.First.LowestFunctionalRoadClassToNext = FunctionalRoadClass.Frc2;
            location.First.Bearing = 203;
            location.Last = new LocationReferencePoint();
            location.Last.Coordinate = new Coordinate() { Latitude = 49.60521, Longitude = 6.12779 };
            location.Last.DistanceToNext = 10;
            location.Last.FuntionalRoadClass = FunctionalRoadClass.Frc2;
            location.Last.FormOfWay = FormOfWay.MultipleCarriageWay;
            location.Last.Bearing = 23;
            location.PositiveOffsetPercentage = (float)((28.0 / 92.0) * 100.0);
            location.Orientation = Orientation.NoOrientation;
            location.SideOfRoad = SideOfRoad.Left;

            // build a graph to decode onto.
            var tags = new TagsTableCollectionIndex();
            var graphDataSource = new DynamicGraphRouterDataSource<LiveEdge>(tags);
            var vertex1 = graphDataSource.AddVertex(49.60597f, 6.12829f);
            var vertex2 = graphDataSource.AddVertex(49.60521f, 6.12779f);
            graphDataSource.AddEdge(vertex1, vertex2, new LiveEdge()
            {
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create(NWBMapping.BAANSUBSRT, "VBD"), 
                    Tag.Create(NWBMapping.WEGBEHSRT, "R"), 
                    Tag.Create(NWBMapping.WEGNUMMER, string.Empty),
                    Tag.Create(NWBMapping.RIJRICHTNG, string.Empty),
                    Tag.Create(NWBMapping.HECTOLTTR, string.Empty)))
            }, null);
            graphDataSource.AddEdge(vertex2, vertex1, new LiveEdge()
            {
                Distance = 10,
                Forward = true,
                Tags = tags.Add(new TagsCollection(
                    Tag.Create(NWBMapping.BAANSUBSRT, "VBD"),
                    Tag.Create(NWBMapping.WEGBEHSRT, "R"),
                    Tag.Create(NWBMapping.WEGNUMMER, string.Empty),
                    Tag.Create(NWBMapping.RIJRICHTNG, string.Empty),
                    Tag.Create(NWBMapping.HECTOLTTR, string.Empty)))
            }, null);

            // decode the location
            var graph = new BasicRouterDataSource<LiveEdge>(graphDataSource);
            var decoder = new PointAlongLineDecoder();
            var router = new BasicRouter();
            var mainDecoder = new ReferencedNWBDecoder(graph, new BinaryDecoder());
            var referencedDecoder = new ReferencedPointAlongLineDecoder(mainDecoder, decoder);
            var referencedLocation = referencedDecoder.Decode(location);

            // confirm result.
            Assert.IsNotNull(referencedLocation);
            Assert.IsNotNull(referencedLocation.Route.Edges);
            Assert.IsNotNull(referencedLocation.Route.Edges[0]);
            Assert.AreEqual(vertex1, referencedLocation.Route.Vertices[0]);
            Assert.AreEqual(vertex2, referencedLocation.Route.Vertices[1]);
            var longitudeReference = (location.Last.Coordinate.Longitude - location.First.Coordinate.Longitude) * (location.PositiveOffsetPercentage.Value / 100.0) + location.First.Coordinate.Longitude;
            var latitudeReference = (location.Last.Coordinate.Latitude - location.First.Coordinate.Latitude) * (location.PositiveOffsetPercentage.Value / 100.0) + location.First.Coordinate.Latitude;
            Assert.AreEqual(longitudeReference, referencedLocation.Longitude, delta);
            Assert.AreEqual(latitudeReference, referencedLocation.Latitude, delta);
        }
    }
}