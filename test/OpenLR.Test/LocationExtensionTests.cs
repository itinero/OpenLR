// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

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

using Itinero;
using Itinero.Algorithms.Search.Hilbert;
using Itinero.Data.Network;
using NUnit.Framework;
using OpenLR.Referenced.Locations;

namespace OpenLR.Tests
{
    /// <summary>
    /// Contains tests for the location extensions.
    /// </summary>
    [TestFixture]
    public class LocationExtensionTests
    { 
        /// <summary>
        /// Tests getting coordinates on a simple referenced line.
        /// </summary>
        [Test]
        public void TestReferencedLineGetCoordinates1()
        {
            var routerDb = new RouterDb();
            routerDb.LoadTestNetwork(
                System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OpenLR.Test.test_data.networks.network1.geojson"));

            var line = new ReferencedLine()
            {
                Edges = new long[] { 1 },
                Vertices = new uint[] { 0, 1 },
                StartLocation = new RouterPoint(0, 0, 0, 0),
                EndLocation = new RouterPoint(1, 1, 0, ushort.MaxValue)
            };

            var shape = line.GetCoordinates(routerDb);
            Assert.IsNotNull(shape);
            Assert.AreEqual(2, shape.Count);
            var vertex0 = routerDb.Network.GetVertex(0);
            Assert.AreEqual(vertex0.Latitude, shape[0].Latitude);
            Assert.AreEqual(vertex0.Longitude, shape[0].Longitude);
            var vertex1 = routerDb.Network.GetVertex(1);
            Assert.AreEqual(vertex1.Latitude, shape[1].Latitude);
            Assert.AreEqual(vertex1.Longitude, shape[1].Longitude);

            line = new ReferencedLine()
            {
                Edges = new long[] { 1 },
                Vertices = new uint[] { Itinero.Constants.NO_VERTEX, 1 },
                StartLocation = new RouterPoint(0, 0, 0, (ushort)(ushort.MaxValue * 0.25)),
                EndLocation = new RouterPoint(1, 1, 0, ushort.MaxValue)
            };

            shape = line.GetCoordinates(routerDb);
            Assert.IsNotNull(shape);
            Assert.AreEqual(2, shape.Count);
            var resolvedLocation = line.StartLocation.LocationOnNetwork(routerDb);
            Assert.AreEqual(resolvedLocation.Latitude, shape[0].Latitude);
            Assert.AreEqual(resolvedLocation.Longitude, shape[0].Longitude);
            Assert.AreEqual(vertex1.Latitude, shape[1].Latitude);
            Assert.AreEqual(vertex1.Longitude, shape[1].Longitude);

            line = new ReferencedLine()
            {
                Edges = new long[] { 1 },
                Vertices = new uint[] { Itinero.Constants.NO_VERTEX, 1 },
                StartLocation = new RouterPoint(0, 0, 0, 0),
                EndLocation = new RouterPoint(1, 1, 0, (ushort)(ushort.MaxValue * 0.75))
            };

            shape = line.GetCoordinates(routerDb);
            Assert.IsNotNull(shape);
            Assert.AreEqual(2, shape.Count);
            Assert.AreEqual(vertex0.Latitude, shape[0].Latitude);
            Assert.AreEqual(vertex0.Longitude, shape[0].Longitude);
            resolvedLocation = line.EndLocation.LocationOnNetwork(routerDb);
            Assert.AreEqual(resolvedLocation.Latitude, shape[1].Latitude);
            Assert.AreEqual(resolvedLocation.Longitude, shape[1].Longitude);

            line = new ReferencedLine()
            {
                Edges = new long[] { 1 },
                Vertices = new uint[] { Itinero.Constants.NO_VERTEX, 1 },
                StartLocation = new RouterPoint(0, 0, 0, (ushort)(ushort.MaxValue * 0.25)),
                EndLocation = new RouterPoint(1, 1, 0, (ushort)(ushort.MaxValue * 0.75))
            };

            shape = line.GetCoordinates(routerDb);
            Assert.IsNotNull(shape);
            Assert.AreEqual(2, shape.Count);
            resolvedLocation = line.StartLocation.LocationOnNetwork(routerDb);
            Assert.AreEqual(resolvedLocation.Latitude, shape[0].Latitude);
            Assert.AreEqual(resolvedLocation.Longitude, shape[0].Longitude);
            resolvedLocation = line.EndLocation.LocationOnNetwork(routerDb);
            Assert.AreEqual(resolvedLocation.Latitude, shape[1].Latitude);
            Assert.AreEqual(resolvedLocation.Longitude, shape[1].Longitude);
        }

        /// <summary>
        /// Tests getting coordinates on a referenced line with more than one edge.
        /// </summary>
        [Test]
        public void TestReferencedLineGetCoordinates2()
        {
            var routerDb = new RouterDb();
            routerDb.LoadTestNetwork(
                System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OpenLR.Test.test_data.networks.network2.geojson"));

            // build a referenced line 0->1->2->5.
            var enumerator = routerDb.Network.GetEdgeEnumerator();
            enumerator.MoveTo(0);
            enumerator.MoveNextUntil(x => x.To == 1);
            var edge01Directed = enumerator.IdDirected();
            var edge01 = enumerator.Id;
            enumerator.MoveTo(1);
            enumerator.MoveNextUntil(x => x.To == 2);
            var edge12Directed = enumerator.IdDirected();
            var edge12 = enumerator.Id;
            enumerator.MoveTo(2);
            enumerator.MoveNextUntil(x => x.To == 5);
            var edge25Directed = enumerator.IdDirected();
            var edge25 = enumerator.Id;

            var vertex0 = routerDb.Network.GetVertex(0);
            var vertex1 = routerDb.Network.GetVertex(1);
            var vertex2 = routerDb.Network.GetVertex(2);
            var vertex5 = routerDb.Network.GetVertex(5);

            var line = new ReferencedLine()
            {
                Edges = new long[] { edge01Directed, edge12Directed, edge25Directed },
                Vertices = new uint[] { 0, 1, 2, 5 },
                StartLocation = routerDb.CreateRouterPointForVertex(0, Itinero.Osm.Vehicles.Vehicle.Car.Shortest()),
                EndLocation = routerDb.CreateRouterPointForVertex(5, Itinero.Osm.Vehicles.Vehicle.Car.Shortest())
            };

            var shape = line.GetCoordinates(routerDb);
            Assert.IsNotNull(shape);
            Assert.AreEqual(4, shape.Count);
            Assert.AreEqual(vertex0.Latitude, shape[0].Latitude);
            Assert.AreEqual(vertex0.Longitude, shape[0].Longitude);
            Assert.AreEqual(vertex1.Latitude, shape[1].Latitude);
            Assert.AreEqual(vertex1.Longitude, shape[1].Longitude);
            Assert.AreEqual(vertex2.Latitude, shape[2].Latitude);
            Assert.AreEqual(vertex2.Longitude, shape[2].Longitude);
            Assert.AreEqual(vertex5.Latitude, shape[3].Latitude);
            Assert.AreEqual(vertex5.Longitude, shape[3].Longitude);
        }
    }
}