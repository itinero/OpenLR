// The MIT License (MIT)

// Copyright (c) 2017 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Reflection;
using Itinero;
using Itinero.Algorithms.Search.Hilbert;
using Itinero.Attributes;
using Itinero.Data.Network.Edges;
using Itinero.Data.Network.Restrictions;
using Itinero.Osm.Vehicles;
using NUnit.Framework;
using OpenLR.Geo;
using OpenLR.Model;
using OpenLR.Model.Locations;
using OpenLR.Osm;
using OpenLR.Referenced.Codecs;
using OpenLR.Referenced.Locations;
using Coordinate = Itinero.LocalGeo.Coordinate;

namespace OpenLR.Test.Referenced
{
    /// <summary>
    /// Contains tests for decoding/encoding an OpenLR polygon location to a referenced polygon location.
    /// </summary>
    [TestFixture]
    public class ReferencedLineCodecTests
    {
        /// <summary>
        /// Tests encoding a referenced line location.
        /// </summary>
        [Test]
        public void EncodeReferencedLineLocation()
        {
            var e = 0.00001f;

            // setup a routing network to test against.
            var routerDb = new RouterDb();
            routerDb.LoadTestNetwork(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OpenLR.Test.test_data.networks.network3.geojson"));
            routerDb.Sort();
            routerDb.AddSupportedVehicle(Vehicle.Car);

            // setup test location and data to verify this.
            var start = routerDb.Network.GetVertex(7);
            var end = routerDb.Network.GetVertex(6);
            var location = new ReferencedLine()
            {
                Edges = new long[] {1, -6, 2},
                Vertices = new uint[] {7, 4, 5, 6},
                StartLocation = routerDb.CreateRouterPointForVertex(7, routerDb.GetSupportedProfile("car")),
                EndLocation = routerDb.CreateRouterPointForVertex(6, routerDb.GetSupportedProfile("car")),
                NegativeOffsetPercentage = 0,
                PositiveOffsetPercentage = 0
            };

            // encode and verify result.
            var encoded = ReferencedLineCodec.Encode(location, new Coder(routerDb, new OsmCoderProfile()));
            Assert.IsNotNull(encoded.First);
            Assert.AreEqual(start.Latitude, encoded.First.Coordinate.Latitude, e);
            Assert.AreEqual(start.Longitude, encoded.First.Coordinate.Longitude, e);
            Assert.IsTrue(encoded.Intermediate == null || encoded.Intermediate.Length == 0);
            Assert.AreEqual(end.Latitude, encoded.Last.Coordinate.Latitude, e);
            Assert.AreEqual(end.Longitude, encoded.Last.Coordinate.Longitude, e);
            Assert.AreEqual(0, encoded.NegativeOffsetPercentage);
            Assert.AreEqual(0, encoded.PositiveOffsetPercentage);
            Assert.AreEqual(FunctionalRoadClass.Frc4, encoded.First.FuntionalRoadClass);
            Assert.AreEqual(FunctionalRoadClass.Frc4, encoded.First.LowestFunctionalRoadClassToNext);
            Assert.AreEqual(Coordinate.DistanceEstimateInMeter(start, end),
                encoded.First.DistanceToNext, 1);
        }

        /// <summary>
        /// Tests decoding a referenced line location.
        /// </summary>
        [Test]
        public void DecodeReferencedLineLocation()
        {
            // setup a routing network to test against.
            var routerDb = new RouterDb();
            routerDb.LoadTestNetwork(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OpenLR.Test.test_data.networks.network3.geojson"));
            routerDb.Sort();
            routerDb.AddSupportedVehicle(Vehicle.Car);

            // setup test location and data to verify this.
            var start = routerDb.Network.GetVertex(7);
            var end = routerDb.Network.GetVertex(6);
            var location = new LineLocation()
            {
                First = new LocationReferencePoint()
                {
                    Bearing = 90,
                    Coordinate = new Model.Coordinate()
                    {
                        Latitude = start.Latitude,
                        Longitude = start.Longitude
                    },
                    DistanceToNext = (int) Coordinate.DistanceEstimateInMeter(start, end),
                    FormOfWay = FormOfWay.SingleCarriageWay,
                    FuntionalRoadClass = FunctionalRoadClass.Frc4,
                    LowestFunctionalRoadClassToNext = FunctionalRoadClass.Frc4
                },
                Intermediate = new LocationReferencePoint[0],
                Last = new LocationReferencePoint()
                {
                    Bearing = 270,
                    Coordinate = new Model.Coordinate()
                    {
                        Latitude = end.Latitude,
                        Longitude = end.Longitude
                    },
                    DistanceToNext = 0,
                    FormOfWay = FormOfWay.SingleCarriageWay,
                    FuntionalRoadClass = FunctionalRoadClass.Frc4
                },
                NegativeOffsetPercentage = 0,
                PositiveOffsetPercentage = 0
            };

            // decode and verify result.
            var decoded = ReferencedLineCodec.Decode(location, new Coder(routerDb, new OsmCoderProfile()));
            Assert.IsNotNull(decoded);
            Assert.AreEqual(0, decoded.NegativeOffsetPercentage);
            Assert.AreEqual(0, decoded.PositiveOffsetPercentage);
            Assert.IsNotNull(decoded.Vertices);
            Assert.AreEqual(new uint[] {7, 4, 5, 6}, decoded.Vertices);
            Assert.IsNotNull(decoded.Edges);
            Assert.AreEqual(new long[] {1, -6, 2}, decoded.Edges);
            Assert.IsNotNull(decoded.StartLocation);
            Assert.IsTrue(Coordinate.DistanceEstimateInMeter(
                              decoded.StartLocation.LocationOnNetwork(routerDb), start) < 1);
            Assert.IsTrue(Coordinate.DistanceEstimateInMeter(
                              decoded.EndLocation.LocationOnNetwork(routerDb), end) < 1);
        }

        /// <summary>
        /// Tests encoding a referenced line location that doesn't represent a shortest path.
        /// </summary>
        [Test]
        public void EncodeReferencedLineLocationNotShortestPath()
        {
            var e = 0.00001f;

            // setup a routing network to test against.
            var routerDb = new RouterDb();

            routerDb.LoadTestNetwork(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OpenLR.Test.test_data.networks.network3.geojson"));
            routerDb.Sort();
            routerDb.AddSupportedVehicle(Vehicle.Car);

            // setup test location and data to verify this.
            var vertex2 = routerDb.Network.GetVertex(2);
            var vertex3 = routerDb.Network.GetVertex(3);
            var vertex4 = routerDb.Network.GetVertex(4);
            var vertex5 = routerDb.Network.GetVertex(5);
            var vertex6 = routerDb.Network.GetVertex(6);
            var vertex7 = routerDb.Network.GetVertex(7);
            var location = new ReferencedLine()
            {
                Edges = new long[] {1, 3, 4, 5, 2},
                Vertices = new uint[] {7, 4, 3, 2, 5, 6},
                StartLocation = routerDb.CreateRouterPointForVertex(7, routerDb.GetSupportedProfile("car")),
                EndLocation = routerDb.CreateRouterPointForVertex(6, routerDb.GetSupportedProfile("car")),
                NegativeOffsetPercentage = 0,
                PositiveOffsetPercentage = 0
            };
            var json = location.ToFeatures(routerDb).ToGeoJson();

            var length = Coordinate.DistanceEstimateInMeter(
                new List<Coordinate>(new Coordinate[]
                {
                    vertex7,
                    vertex4,
                    vertex3,
                    vertex2,
                    vertex5,
                    vertex6
                }));

            // encode and verify result.
            var encoded = ReferencedLineCodec.Encode(location, new Coder(routerDb, new OsmCoderProfile()));
            Assert.IsNotNull(encoded.First);
            Assert.AreEqual(vertex7.Latitude, encoded.First.Coordinate.Latitude, e);
            Assert.AreEqual(vertex7.Longitude, encoded.First.Coordinate.Longitude, e);
            Assert.IsTrue(encoded.Intermediate != null && encoded.Intermediate.Length == 1);
            Assert.AreEqual(vertex6.Latitude, encoded.Last.Coordinate.Latitude, e);
            Assert.AreEqual(vertex6.Longitude, encoded.Last.Coordinate.Longitude, e);
            Assert.AreEqual(0, encoded.NegativeOffsetPercentage);
            Assert.AreEqual(0, encoded.PositiveOffsetPercentage);
            Assert.AreEqual(FunctionalRoadClass.Frc4, encoded.First.FuntionalRoadClass);
            Assert.AreEqual(FunctionalRoadClass.Frc4, encoded.First.LowestFunctionalRoadClassToNext);
            Assert.AreEqual(length, encoded.First.DistanceToNext + encoded.Intermediate[0].DistanceToNext, 1);
        }


        /// <summary>
        /// Tests encoding a line that can be navigated and is within two bollards.
        /// 
        /// </summary>
        [Test]
        public void EncodeReferencedLineLocationBetweenBollards()
        {
            var e = 0.00001f;


            // setup a routing network to test against.
            var routerDb = new RouterDb();
            routerDb.AddSupportedVehicle(Vehicle.Car);

            var bollardProfile = new AttributeCollection();
            bollardProfile.AddOrReplace("barrier", "bollard");
            bollardProfile.AddOrReplace("motor", "no");

            var restrictionsDb = new RestrictionsDb();
             restrictionsDb.Add(1);
            routerDb.AddRestrictions("motor_car", 
                restrictionsDb);


            routerDb.Network.AddVertex(0, 51.0f, 4.0f);
            routerDb.Network.AddVertex(1, 51.1f, 4.1f);
            routerDb.Network.AddVertex(2, 51.2f, 4.2f);
            routerDb.Network.AddVertex(3, 51.3f, 4.3f);

            routerDb.VertexMeta[1] = bollardProfile;
            routerDb.VertexMeta[2] = bollardProfile;

            // to make junctions
            routerDb.Network.AddVertex(4, 51.0f, 4.3f);
            routerDb.Network.AddVertex(5, 51.3f, 4.0f);
            
            var profile = new AttributeCollection();

            profile.AddOrReplace("highway", "residential");
            var residential = routerDb.EdgeProfiles.Add(profile);


            for (var i = (uint) 0; i < 3; i++)
            {
                var edgeId = routerDb.Network.AddEdge(i, i + 1, new EdgeData()
                {
                    Distance = (float) 100,
                    MetaId = 0,
                    Profile = (ushort) residential
                }, null);
                Console.WriteLine($"Created edge {edgeId} between vertex {i} and {i + 1}");
            }

            Console.WriteLine(routerDb.GetGeoJson());

            routerDb.Network.AddEdge(0, 4, new EdgeData()
            {
                Distance = (float) 100,
                MetaId = 0,
                Profile = (ushort) residential
            }, null);

            routerDb.Network.AddEdge(4, 3, new EdgeData()
            {
                Distance = (float) 100,
                MetaId = 0,
                Profile = (ushort) residential
            }, null);

            routerDb.Network.AddEdge(0, 5, new EdgeData()
            {
                Distance = (float) 100,
                MetaId = 0,
                Profile = (ushort) residential
            }, null);

            routerDb.Network.AddEdge(3, 5, new EdgeData()
            {
                Distance = (float) 100,
                MetaId = 0,
                Profile = (ushort) residential
            }, null);

            Console.WriteLine(routerDb.GetGeoJson());

            var location = new ReferencedLine()
            {
                Edges = new long[] {1,2}, // Edge-id +1, see https://github.com/itinero/routing/issues/95
                Vertices = new uint[] {0, 1, 2},
                StartLocation = routerDb.CreateRouterPointForVertex(0, routerDb.GetSupportedProfile("car")),
                EndLocation = routerDb.CreateRouterPointForVertex(2 , routerDb.GetSupportedProfile("car")),
                NegativeOffsetPercentage = 0,
                PositiveOffsetPercentage = 0
            };

            // encode and verify result.

            var encoded = ReferencedLineCodec.Encode(location, new Coder(routerDb, new OsmCoderProfile()));
            Console.WriteLine("Encoded OpenLR-reference:");
            Console.WriteLine(encoded.ToFeatures().ToGeoJson());
            Assert.IsNotNull(encoded.First);
            Assert.AreEqual(0, encoded.NegativeOffsetPercentage);
            Assert.AreEqual(0, encoded.PositiveOffsetPercentage);
            Assert.AreEqual(FunctionalRoadClass.Frc4, encoded.First.FuntionalRoadClass);
        }
    }
}