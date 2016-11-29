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
using Itinero.IO.Osm;
using Itinero.Osm.Vehicles;
using OpenLR;
using OpenLR.Geo;
using OpenLR.Osm;
using OpenLR.Referenced.Locations;
using System;
using System.IO;

namespace Samples.OSM
{
    class Program
    {
        static void Main(string[] args)
        {
            Download.ToFile("http://files.itinero.tech/data/OSM/planet/europe/luxembourg-latest.osm.pbf", "luxembourg-latest.osm.pbf").Wait();

            // build routerdb from raw OSM data.
            // check this for more info on RouterDb's: https://github.com/itinero/routing/wiki/RouterDb
            var routerDb = new RouterDb();
            using (var sourceStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "luxembourg-latest.osm.pbf")))
            {
                routerDb.LoadOsmData(sourceStream, Vehicle.Car);
            }

            // create coder.
            var coder = new Coder(routerDb, new OsmCoderProfile());

            // build a line location from a shortest path.
            // REMARK: this functionality is NOT part of the OpenLR-spec, just a convenient way to build a line location.
            var line = coder.BuildLine(new Itinero.LocalGeo.Coordinate(49.67218282319583f, 6.142280101776122f),
                new Itinero.LocalGeo.Coordinate(49.67776489459803f, 6.1342549324035645f));
            var lineGeoJson = line.ToFeatures(coder.Router.Db).ToGeoJson();

            // encode this location.
            var encoded = coder.Encode(line);
            Console.WriteLine(encoded);

            // decode this location.
            var decodedLine = coder.Decode(encoded) as ReferencedLine;
            var decodedLineGeoJson = decodedLine.ToFeatures(coder.Router.Db).ToGeoJson();

            Console.ReadLine();
        }
    }
}
