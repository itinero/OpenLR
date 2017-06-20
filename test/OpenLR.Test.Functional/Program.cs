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
using Itinero.LocalGeo;
using Itinero.Osm.Vehicles;
using NetTopologySuite.Features;
using OpenLR.Geo;
using OpenLR.Model.Locations;
using System;
using System.IO;

namespace OpenLR.Tests.Functional
{
    class Program
    {
        static void Main(string[] args)
        {
            Itinero.Logging.Logger.LogAction = (o, level, message, parameters) =>
            {
                Console.WriteLine(string.Format("[{0}] {1} - {2}", o, level, message));
            };

            Download.DownloadAll();

            // executes the netherlands tests based on OSM.
            var routerDb = RouterDb.Deserialize(File.OpenRead(@"netherlands.c.cf.routerdb"));
            routerDb.RemoveContracted(Vehicle.Car.Shortest());
            Action netherlandsTest = () => { Osm.Netherlands.TestEncodeDecodePointAlongLine(routerDb); };
            netherlandsTest.TestPerf("Testing netherlands point along line performance");
            netherlandsTest = () => { Osm.Netherlands.TestEncodeDecodeRoutes(routerDb); };
            netherlandsTest.TestPerf("Testing netherlands line performance");

            // executes the netherlands tests based on NWB.
            routerDb = NWB.Netherlands.DownloadExtractAndBuildRouterDb();
            netherlandsTest = () => { NWB.Netherlands.TestEncodeDecodePointAlongLine(routerDb); };
            netherlandsTest.TestPerf("Testing netherlands point along line performance");
            netherlandsTest = () => { NWB.Netherlands.TestEncodeDecodeRoutes(routerDb); };
            netherlandsTest.TestPerf("Testing netherlands line performance");
#if DEBUG
            Console.ReadLine();
#endif
        }

        private static string ToJson(FeatureCollection featureCollection)
        {
            var jsonSerializer = new NetTopologySuite.IO.GeoJsonSerializer();
            var jsonStream = new StringWriter();
            jsonSerializer.Serialize(jsonStream, featureCollection);
            var json = jsonStream.ToInvariantString();
            return json;
        }
    }
}