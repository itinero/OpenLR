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
using Itinero.Data.Edges;
using Itinero.IO.Shape;
using Itinero.LocalGeo;
using Itinero.Profiles;
using NetTopologySuite.Algorithm.Distance;
using OpenLR;
using OpenLR.Geo;
using OpenLR.Referenced.Locations;
using System;
using System.IO;

namespace Samples.NWB
{
    class Program
    {
        /// <summary>
        /// Demo's using NWB as a routing network for encoding/decoding.
        /// </summary>
        static void Main(string[] args)
        {
            Itinero.Logging.Logger.LogAction = (o, level, message, parameters) =>
            {
                Console.WriteLine($"[{o}] {level} - {message}");
            };
            
            // download and build router db from NWB-data if needed.
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nwb.routerdb")))
            {
                DownloadExtractAndBuildRouterDb();
            }

            // load routerDb.
            var routerDb = RouterDb.Deserialize(File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nwb.routerdb")));

            // create 'coder'.
            var vehicle = routerDb.GetSupportedVehicle("nwb.car");
            var coder = new Coder(routerDb, new INwbCoderSettingsExtensions(vehicle.Shortest()));

            // encode/decode some shortest paths.
            EncodeDecodeRoute(coder, new Coordinate(52.41239352799169f, 5.832839012145995f), new Coordinate(52.41021421939001f, 5.848240256309509f));
            EncodeDecodeRoute(coder, new Coordinate(52.40624144888954f, 6.239939332008362f), new Coordinate(52.38836282749006f, 6.246392726898193f));

            // encode/decode some point along line locations.
            EncodeDecodePointAlongLine(coder, new Coordinate(52.41239352799169f, 5.832839012145995f));
            EncodeDecodePointAlongLine(coder, new Coordinate(52.40624144888954f, 6.239939332008362f));
            EncodeDecodePointAlongLine(coder, new Coordinate(52.41021421939001f, 5.848240256309509f));
            EncodeDecodePointAlongLine(coder, new Coordinate(52.38836282749006f, 6.246392726898193f));

            // decode a string.
            var decoded = coder.Decode("KwMvwyTrWi+5Av9S/+kvBgA=");
        }

        static void EncodeDecodeRoute(Coder coder, Coordinate coordinate1, Coordinate coordinate2)
        {
            // build referenced line and calculate shortest path.
            var referencedLine = coder.BuildLine(coordinate1, coordinate2, out _);

            // encode.
            var encoded = coder.Encode(referencedLine);

            // decoded.
            var decodedReferencedLine = coder.Decode(encoded) as ReferencedLine;
        }

        static void EncodeDecodePointAlongLine(Coder coder, Coordinate coordinate)
        {
            // build point along line based on the closest line.
            var referencedPointAlongLine = coder.BuildPointAlongLine(coordinate);

            // encode.
            var encoded = coder.Encode(referencedPointAlongLine);

            // decode.
            var decodedReferencedLine = coder.Decode(encoded) as ReferencedPointAlongLine;
        }

        static void DownloadExtractAndBuildRouterDb()
        {
            // download test data and extract to 'temp' directory relative to application base directory.
            if (!File.Exists("WGS84_2016-09-01.zip"))
            {
                Download.DownloadAndExtractShape("http://files.itinero.tech/data/open-data/NWB/WGS84_2016-09-01.zip", "WGS84_2016-09-01.zip");
            }

            // create a new router db and load the shapefile.
            var vehicle = DynamicVehicle.LoadFromStream(File.OpenRead("car.lua")); // load data for the car profile.
            var routerDb = new RouterDb(EdgeDataSerializer.MAX_DISTANCE);
            routerDb.LoadFromShape(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp"), "wegvakken.shp", "JTE_ID_BEG", "JTE_ID_END", vehicle);

            // write the router db to disk for later use.
            using var outputStream = File.OpenWrite(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nwb.routerdb"));
            routerDb.Serialize(outputStream);
        }
    }
}
