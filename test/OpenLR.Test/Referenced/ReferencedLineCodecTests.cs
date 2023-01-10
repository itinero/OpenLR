using System.Reflection;
using Itinero;
using Itinero.Geo;
using Itinero.IO.Json.GeoJson;
using Itinero.Routing;
using Itinero.Snapping;
using NUnit.Framework;
using OpenLR.Model;
using OpenLR.Model.Locations;
using OpenLR.Networks.Osm;
using OpenLR.Referenced.Codecs;

namespace OpenLR.Test.Referenced;

/// <summary>
/// Contains tests for decoding/encoding an OpenLR polygon location to a referenced polygon location.
/// </summary>
[TestFixture]
public class ReferencedLineCodecTests
{
    // /// <summary>
    // /// Tests encoding a referenced line location.
    // /// </summary>
    // [Test]
    // public async Task EncodeReferencedLineLocation()
    // {
    //     const double e = 0.00001;
    //
    //     // setup a routing network to test against.
    //     var routerDb = new RouterDb();
    //     await routerDb.LoadTestNetworkAsync(
    //         Assembly.GetExecutingAssembly().GetManifestResourceStream(
    //             "OpenLR.Test.test_data.networks.network3.geojson"));
    //
    //     // setup test location and data to verify this.
    //     var location = new ReferencedLine()
    //     {
    //         Edges = new long[] {1, -6, 2},
    //         Vertices = new uint[] {7, 4, 5, 6},
    //         StartLocation = routerDb.CreateRouterPointForVertex(7, routerDb.GetSupportedProfile("car")),
    //         EndLocation = routerDb.CreateRouterPointForVertex(6, routerDb.GetSupportedProfile("car")),
    //         NegativeOffsetPercentage = 0,
    //         PositiveOffsetPercentage = 0
    //     };
    //
    //     // encode and verify result.
    //     var encoded = ReferencedLineCodec.Encode(location, new Coder(routerDb, new OsmCoderProfile()));
    //     Assert.IsNotNull(encoded.First);
    //     Assert.AreEqual(start.Latitude, encoded.First.Coordinate.Latitude, e);
    //     Assert.AreEqual(start.Longitude, encoded.First.Coordinate.Longitude, e);
    //     Assert.IsTrue(encoded.Intermediate == null || encoded.Intermediate.Length == 0);
    //     Assert.AreEqual(end.Latitude, encoded.Last.Coordinate.Latitude, e);
    //     Assert.AreEqual(end.Longitude, encoded.Last.Coordinate.Longitude, e);
    //     Assert.AreEqual(0, encoded.NegativeOffsetPercentage);
    //     Assert.AreEqual(0, encoded.PositiveOffsetPercentage);
    //     Assert.AreEqual(FunctionalRoadClass.Frc4, encoded.First.FuntionalRoadClass);
    //     Assert.AreEqual(FunctionalRoadClass.Frc4, encoded.First.LowestFunctionalRoadClassToNext);
    //     Assert.AreEqual(Coordinate.DistanceEstimateInMeter(start, end),
    //         encoded.First.DistanceToNext, 1);
    // }

    /// <summary>
    /// Tests decoding a referenced line location.
    /// </summary>
    [Test]
    public async Task DecodeReferencedLineLocation()
    {
        // setup a routing network to test against.
        var coder = await TestCoders.Coder3();

        // setup test location and data to verify this.
        (double longitude, double latitude, float? e) start = (
            4.7957682609558105,
            51.268252139690674, null);
        (double longitude, double latitude, float? e) end = (
            4.791841506958008,
            51.268158160891474, null);
        var location = new LineLocation
        {
            First = new LocationReferencePoint
            {
                Bearing = 180,
                Coordinate = new Coordinate
                {
                    Latitude = start.latitude,
                    Longitude = start.longitude
                },
                DistanceToNext = (int)start.DistanceEstimateInMeter(end),
                FormOfWay = FormOfWay.SingleCarriageWay,
                FunctionalRoadClass = FunctionalRoadClass.Frc4,
                LowestFunctionalRoadClassToNext = FunctionalRoadClass.Frc4
            },
            Intermediate = Array.Empty<LocationReferencePoint>(),
            Last = new LocationReferencePoint()
            {
                Bearing = 0,
                Coordinate = new Coordinate()
                {
                    Latitude = end.latitude,
                    Longitude = end.longitude
                },
                DistanceToNext = 0,
                FormOfWay = FormOfWay.SingleCarriageWay,
                FunctionalRoadClass = FunctionalRoadClass.Frc4
            },
            NegativeOffsetPercentage = 0,
            PositiveOffsetPercentage = 0
        };

        // decode.
        var decodedResult = await ReferencedLineCodec.Decode(location,coder);
        var decoded = decodedResult.Value;
        
        // calculate shortest path.
        var path = (await coder.Network.Route(coder.Settings.RoutingSettings.Profile).From(
            await coder.Network.Snap().ToAsync(start)).To(
            await coder.Network.Snap().ToAsync(end)).PathAsync()).Value;
        
        // verify result, should be equal to shortest path.
        Assert.That(decoded, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(decoded.NegativeOffsetPercentage, Is.EqualTo(0));
            Assert.That(decoded.PositiveOffsetPercentage, Is.EqualTo(0));
        });
        PathAsserts.AreEqual(path.Select(x => (x.edge, x.forward)), decoded);
    }

    // /// <summary>
    // /// Tests encoding a referenced line location that doesn't represent a shortest path.
    // /// </summary>
    // [Test]
    // public void EncodeReferencedLineLocationNotShortestPath()
    // {
    //     var e = 0.00001f;
    //
    //     // setup a routing network to test against.
    //     var routerDb = new RouterDb();
    //
    //     routerDb.LoadTestNetwork(
    //         Assembly.GetExecutingAssembly().GetManifestResourceStream(
    //             "OpenLR.Test.test_data.networks.network3.geojson"));
    //     routerDb.Sort();
    //     routerDb.AddSupportedVehicle(Vehicle.Car);
    //
    //     // setup test location and data to verify this.
    //     var vertex2 = routerDb.Network.GetVertex(2);
    //     var vertex3 = routerDb.Network.GetVertex(3);
    //     var vertex4 = routerDb.Network.GetVertex(4);
    //     var vertex5 = routerDb.Network.GetVertex(5);
    //     var vertex6 = routerDb.Network.GetVertex(6);
    //     var vertex7 = routerDb.Network.GetVertex(7);
    //     var location = new ReferencedLine()
    //     {
    //         Edges = new long[] {1, 3, 4, 5, 2},
    //         Vertices = new uint[] {7, 4, 3, 2, 5, 6},
    //         StartLocation = routerDb.CreateRouterPointForVertex(7, routerDb.GetSupportedProfile("car")),
    //         EndLocation = routerDb.CreateRouterPointForVertex(6, routerDb.GetSupportedProfile("car")),
    //         NegativeOffsetPercentage = 0,
    //         PositiveOffsetPercentage = 0
    //     };
    //     var json = location.ToFeatures(routerDb).ToGeoJson();
    //
    //     var length = Coordinate.DistanceEstimateInMeter(
    //         new List<Coordinate>(new Coordinate[]
    //         {
    //             vertex7,
    //             vertex4,
    //             vertex3,
    //             vertex2,
    //             vertex5,
    //             vertex6
    //         }));
    //
    //     // encode and verify result.
    //     var encoded = ReferencedLineCodec.Encode(location, new Coder(routerDb, new OsmCoderProfile()));
    //     Assert.IsNotNull(encoded.First);
    //     Assert.AreEqual(vertex7.Latitude, encoded.First.Coordinate.Latitude, e);
    //     Assert.AreEqual(vertex7.Longitude, encoded.First.Coordinate.Longitude, e);
    //     Assert.IsTrue(encoded.Intermediate != null && encoded.Intermediate.Length == 1);
    //     Assert.AreEqual(vertex6.Latitude, encoded.Last.Coordinate.Latitude, e);
    //     Assert.AreEqual(vertex6.Longitude, encoded.Last.Coordinate.Longitude, e);
    //     Assert.AreEqual(0, encoded.NegativeOffsetPercentage);
    //     Assert.AreEqual(0, encoded.PositiveOffsetPercentage);
    //     Assert.AreEqual(FunctionalRoadClass.Frc4, encoded.First.FuntionalRoadClass);
    //     Assert.AreEqual(FunctionalRoadClass.Frc4, encoded.First.LowestFunctionalRoadClassToNext);
    //     Assert.AreEqual(length, encoded.First.DistanceToNext + encoded.Intermediate[0].DistanceToNext, 1);
    // }
    //
    //
    // /// <summary>
    // /// Tests encoding a line that can be navigated and is within two bollards.
    // /// 
    // /// </summary>
    // [Test]
    // public void EncodeReferencedLineLocationBetweenBollards()
    // {
    //     // setup a routing network to test against.
    //     var routerDb = new RouterDb();
    //     routerDb.AddSupportedVehicle(Vehicle.Car);
    //
    //     var restrictionsDb = new RestrictionsDb();
    //     restrictionsDb.Add(1);
    //     routerDb.AddRestrictions("motorcar",
    //         restrictionsDb);
    //
    //     routerDb.Network.AddVertex(0, 51.0f, 4.0f);
    //     routerDb.Network.AddVertex(1, 51.0001f, 4.0001f);
    //     routerDb.Network.AddVertex(2, 51.0002f, 4.0002f);
    //
    //     var profile = new AttributeCollection();
    //
    //     profile.AddOrReplace("highway", "residential");
    //     var residential = routerDb.EdgeProfiles.Add(profile);
    //
    //
    //     routerDb.Network.AddEdge(0, 1, new EdgeData()
    //     {
    //         Distance = Coordinate.DistanceEstimateInMeter(51.0f, 4.0f, 51.0001f, 4.0001f),
    //         MetaId = 0,
    //         Profile = (ushort) residential
    //     }, null);
    //     routerDb.Network.AddEdge(1, 2, new EdgeData()
    //     {
    //         Distance = Coordinate.DistanceEstimateInMeter(51.0001f, 4.0001f, 51.0002f, 4.0002f),
    //         MetaId = 0,
    //         Profile = (ushort) residential
    //     }, null);
    //
    //     var location = new ReferencedLine()
    //     {
    //         Edges = new long[] {1}, // Edge-id +1, see https://github.com/itinero/routing/issues/95
    //         Vertices = new uint[] {0, 1},
    //         StartLocation = routerDb.CreateRouterPointForVertex(0, routerDb.GetSupportedProfile("car")),
    //         EndLocation = routerDb.CreateRouterPointForVertex(1, routerDb.GetSupportedProfile("car")),
    //         NegativeOffsetPercentage = 0,
    //         PositiveOffsetPercentage = 0
    //     };
    //
    //     // encode and verify result.
    //     var encoded = ReferencedLineCodec.Encode(location, new Coder(routerDb, new OsmCoderProfile()));
    //     Assert.IsNotNull(encoded.First);
    //     Assert.AreEqual(FunctionalRoadClass.Frc4, encoded.First.FuntionalRoadClass);
    //
    //     var location1 = new ReferencedLine()
    //     {
    //         Edges = new long[] {1}, // Edge-id +1, see https://github.com/itinero/routing/issues/95
    //         Vertices = new uint[] {1, 2},
    //         StartLocation = routerDb.CreateRouterPointForVertex(0, routerDb.GetSupportedProfile("car")),
    //         EndLocation = routerDb.CreateRouterPointForVertex(1, routerDb.GetSupportedProfile("car")),
    //         NegativeOffsetPercentage = 0,
    //         PositiveOffsetPercentage = 0
    //     };
    //
    //     var encoded1 = ReferencedLineCodec.Encode(location, new Coder(routerDb, new OsmCoderProfile()));
    //     Assert.IsNotNull(encoded1.First);
    //     Assert.AreEqual(FunctionalRoadClass.Frc4, encoded1.First.FuntionalRoadClass);
    //
    //     var locationFull = new ReferencedLine()
    //     {
    //         Edges = new long[] {1, 2}, // Edge-id +1, see https://github.com/itinero/routing/issues/95
    //         Vertices = new uint[] {0, 1, 2},
    //         StartLocation = routerDb.CreateRouterPointForVertex(0, routerDb.GetSupportedProfile("car")),
    //         EndLocation = routerDb.CreateRouterPointForVertex(2, routerDb.GetSupportedProfile("car")),
    //         NegativeOffsetPercentage = 0,
    //         PositiveOffsetPercentage = 0
    //     };
    //
    //     try
    //     {
    //         ReferencedLineCodec.Encode(locationFull, new Coder(routerDb, new OsmCoderProfile()));
    //         Assert.Fail("This should have crashed. It didn't, that is an error.");
    //     }
    //     catch (Exception e)
    //     {
    //         if (e.InnerException == null)
    //         {
    //             throw e;
    //         }
    //         Assert.AreEqual("No path found between two edges of the line location.", e.InnerException.Message);
    //     }
    // }
}
