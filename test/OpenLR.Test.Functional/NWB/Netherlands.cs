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
using NUnit.Framework;
using OpenLR.Geo;
using OpenLR.Osm;
using OpenLR.Referenced.Locations;
using System;
using System.Collections.Generic;
using System.IO;

namespace OpenLR.Tests.Functional.NWB
{
    /// <summary>
    /// Contains tests for the netherlands.
    /// </summary>
    public static class Netherlands
    {
        /// <summary>
        /// Downloads, extracts and builds the nwb router db.
        /// </summary>
        public static RouterDb DownloadExtractAndBuildRouterDb()
        {
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nwb.routerdb")))
            {
                // download test data and extract to 'temp' directory relative to application base directory.
                Download.DownloadAndExtractShape("http://files.itinero.tech/data/open-data/NWB/WGS84_2016-09-01.zip", "WGS84_2016-09-01.zip");

                // create a new router db and load the shapefile.
                var vehicle = DynamicVehicle.LoadFromEmbeddedResource(System.Reflection.Assembly.GetExecutingAssembly(),
                    "OpenLR.Test.Functional.NWB.car.lua"); // load data for the car profile.
                var routerDb = new RouterDb(EdgeDataSerializer.MAX_DISTANCE);
                routerDb.LoadFromShape(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp"), "wegvakken.shp", "JTE_ID_BEG", "JTE_ID_END", vehicle);

                // write the router db to disk for later use.
                using (var ouputStream = File.OpenWrite(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nwb.routerdb")))
                {
                    routerDb.Serialize(ouputStream);
                }
                return routerDb;
            }
            else
            {
                return RouterDb.Deserialize(File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nwb.routerdb")));
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
            var coder = new Coder(routerDb, new NWBCoderProfile(routerDb.GetSupportedVehicle("nwb.car")));

            var features = Extensions.FromGeoJsonFile(@".\Data\line_locations.geojson");

            var i = 0;
            foreach (var feature in features.Features)
            {
                var points = new List<Coordinate>();
                var coordinates = (feature.Geometry as NetTopologySuite.Geometries.LineString).Coordinates;

                foreach (var c in coordinates)
                {
                    points.Add(new Coordinate((float)c.Y, (float)c.X));
                }

                if (!feature.Attributes.Contains("nwb", "no"))
                {
                    System.Console.WriteLine("Testing line location {0}/{1} @ {2}->{3}", i + 1, features.Features.Count,
                        points[0].ToInvariantString(), points[1].ToInvariantString());
                    TestEncodeDecoderRoute(coder, points.ToArray());
                }

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
            var coder = new Coder(routerDb, new NWBCoderProfile(routerDb.GetSupportedVehicle("nwb.car")));
            
            var locations = Extensions.PointsFromGeoJsonFile(@".\Data\locations.geojson");
            
            for (var i = 0; i < locations.Length; i++)
            {
                try
                {
                    System.Console.WriteLine("Testing location {0}/{1} @ {2}", i + 1, locations.Length, locations[i].ToInvariantString());
                    var location = locations[i].Item1;
                    var attributes = locations[i].Item2;
                    if (!attributes.Contains("nwb", "no"))
                    {
                        Netherlands.TestEncodeDecodePointAlongLine(coder, location.Latitude, location.Longitude, 30);
                    }
                }
                catch (Exception ex)
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