﻿// The MIT License (MIT)

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
using Itinero.LocalGeo;
using NetTopologySuite.Algorithm.Distance;
using NUnit.Framework;
using OpenLR.Geo;
using OpenLR.Osm;
using OpenLR.Referenced.Locations;
using System;
using System.Collections.Generic;
using System.IO;
using Serilog;

namespace OpenLR.Test.Functional.Osm
{
    /// <summary>
    /// Contains tests for the netherlands.
    /// </summary>
    public static class Netherlands
    {
        /// <summary>
        /// Downloads and builds the nwb router db.
        /// </summary>
        public static RouterDb DownloadAndBuildRouterDb()
        {
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "netherlands.c.routerdb")))
            {
                // download test data and extract to 'temp' directory relative to application base directory.
                Log.Logger.Verbose($"Download the netherlands pbf...");
                Download.DownloadPbf("netherlands-latest.osm.pbf");

                // create a new router db and load the osm data.
                Log.Logger.Verbose($"Building routerdb...");
                var routerDb = new RouterDb();
                using (var stream = File.OpenRead("netherlands-latest.osm.pbf"))
                {
                    routerDb.LoadOsmData(stream, Itinero.Osm.Vehicles.Vehicle.Car);
                }

                // write the router db to disk for later use.
                Log.Logger.Verbose($"Writing routerdb...");
                using (var ouputStream = File.OpenWrite(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "netherlands.c.routerdb")))
                {
                    routerDb.Serialize(ouputStream);
                }
                return routerDb;
            }
            else
            {
                Log.Logger.Verbose($"Loading routerdb...");
                return RouterDb.Deserialize(File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "netherlands.c.routerdb")));
            }
        }

        /// <summary>
        /// Tests encoding/decoding on OSM-data.
        /// </summary>
        public static void TestAll(RouterDb routerDb)
        {
            TestEncodeDecodePointAlongLine(routerDb);
            TestEncodeDecodeRoutes(routerDb);
        }

        /// <summary>
        /// Tests encoding/decoding short routes.
        /// </summary>
        public static void TestEncodeDecodeRoutes(RouterDb routerDb)
        {
            var coder = new Coder(routerDb, new OsmCoderProfile(0.3f), new OpenLR.Codecs.Binary.BinaryCodec());
            var features = Extensions.FromGeoJsonFile(Path.Combine(".", "Data", "line_locations.geojson"));

            var i = 0;
            foreach (var feature in features)
            {
                var points = new List<Coordinate>();
                var coordinates = (feature.Geometry as NetTopologySuite.Geometries.LineString).Coordinates;

                foreach (var c in coordinates)
                {
                    points.Add(new Coordinate((float)c.Y, (float)c.X));
                }

                Log.Logger.Verbose($"Testing line location {i + 1}/{features.Count}" +
                                   $" @ {points[0].ToInvariantString()}->{points[1].ToInvariantString()}");
                TestEncodeDecoderRoute(coder, points.ToArray());

                i++;
            }
        }

        /// <summary>
        /// Tests encoding/decoding a route.
        /// </summary>
        public static void TestEncodeDecoderRoute(Coder coder, Coordinate[] points)
        {
            var referencedLine = coder.BuildLine(points);
            var referencedLineJson = referencedLine.ToFeatures(coder.Router.Db).ToGeoJson();

            float positiveOffset, negativeOffset;
            RouterPoint source, target;
            var path = referencedLine.BuildPathFromLine(coder.Router.Db, out source, out positiveOffset, out target, out negativeOffset);
            var route = coder.Router.BuildRoute(coder.Profile.Profile, coder.Router.GetDefaultWeightHandler(coder.Profile.Profile), source, target, path);

            var encoded = coder.Encode(referencedLine);

            var decodedReferencedLine = coder.Decode(encoded) as ReferencedLine;
            var decodedReferencedLineJson = decodedReferencedLine.ToFeatures(coder.Router.Db).ToGeoJson();

            var distance = DiscreteHausdorffDistance.Distance(referencedLine.ToLineString(coder.Router.Db),
                decodedReferencedLine.ToLineString(coder.Router.Db));

            Assert.IsTrue(distance < .1);
        }

        /// <summary>
        /// Tests encoding/decoding point along line locations.
        /// </summary>
        public static void TestEncodeDecodePointAlongLine(RouterDb routerDb)
        {
            var coder = new Coder(routerDb, new OsmCoderProfile(0.3f), new OpenLR.Codecs.Binary.BinaryCodec());
            
            var locations = Extensions.PointsFromGeoJsonFile(Path.Combine(".", "Data", "locations.geojson"));

            for (var i = 0; i < locations.Length; i++)
            {
                try
                {
                    Log.Logger.Verbose($"Testing location {i + 1}/{locations.Length}" +
                                       $" @ {locations[i].ToInvariantString()}");
                    Netherlands.TestEncodeDecodePointAlongLine(coder, locations[i].Item1.Latitude, locations[i].Item1.Longitude, 30);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Encoding/decoding failed:" + ex.ToInvariantString());
                }
            }
        }

        /// <summary>
        /// Tests encoding/decoding a point along line location.
        /// </summary>
        public static void TestEncodeDecodePointAlongLine(Coder coder, float latitude, float longitude, float tolerance)
        {
            RouterPoint routerPoint;
            var decoded = EncodeDecodePointAlongLine(coder, latitude, longitude, out routerPoint);
            var encodedLocation = routerPoint.LocationOnNetwork(coder.Router.Db); // the actual encoded location.

            var distance = Itinero.LocalGeo.Coordinate.DistanceEstimateInMeter(encodedLocation.Latitude, encodedLocation.Longitude, 
                decoded.Latitude, decoded.Longitude);

            Assert.IsTrue(distance < tolerance);
        }

        /// <summary>
        /// Encode/decode a point along line location.
        /// </summary>
        private static ReferencedPointAlongLine EncodeDecodePointAlongLine(Coder coder, float latitude, float longitude, 
            out RouterPoint routerPoint)
        {
            var location = coder.BuildPointAlongLine(latitude, longitude, out routerPoint);
            var locationGeoJson = location.ToFeatures(coder.Router.Db).ToGeoJson();

            var encoded = coder.Encode(location);

            var decoded = coder.Decode(encoded) as ReferencedPointAlongLine;
            var decodedGeoJson = decoded.ToFeatures(coder.Router.Db).ToGeoJson();
            return decoded;
        }
    }
}