using Itinero;
using Itinero.Data.Edges;
using Itinero.IO.Shape;
using Itinero.LocalGeo;
using OpenLR;
using OpenLR.Geo;
using OpenLR.Referenced.Locations;
using Samples.EncodeRoute.NWB;
using System;
using System.IO;

namespace Samples.EncodeRoute
{
    class Program
    {
        static void Main(string[] args)
        {
            Itinero.Logging.Logger.LogAction = (o, level, message, parameters) =>
            {
                Console.WriteLine(string.Format("[{0}] {1} - {2}", o, level, message));
            };

            RouterDb routerDb;
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nwb.routerdb")))
            {
                // download test data and extract to 'temp' directory relative to application base directory.
                Download.DownloadAndExtractShape("http://files.itinero.tech/data/open-data/NWB/WGS84_2016-09-01.zip", "WGS84_2016-09-01.zip");

                // create a new router db and load the shapefile.
                var vehicle = new NWBCar(); // load data for the car profile.
                routerDb = new RouterDb(EdgeDataSerializer.MAX_DISTANCE);
                routerDb.LoadFromShape(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp"), "wegvakken.shp", "JTE_ID_BEG", "JTE_ID_END", vehicle);

                // write the router db to disk for later use.
                using (var ouputStream = File.OpenWrite(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nwb.routerdb")))
                {
                    routerDb.Serialize(ouputStream);
                }
            }
            else
            {
                routerDb = RouterDb.Deserialize(File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nwb.routerdb")));
            }

            // create test case.
            var testCase = new Coordinate[]
                {
                    new Coordinate(51.95833631000295f, 4.946079254150391f),
                    new Coordinate(51.94288923910573f, 4.954748153686523f)
                };

            // create coder (can decode and encode all location types).
            var coder = new Coder(routerDb, new NWBCoderProfile());

            // build line location from shortest path.
            var line = coder.BuildLine(testCase[0], testCase[1]);
            var lineJson = line.ToFeatures(routerDb).ToGeoJson();

            // encode.
            var encoded = coder.Encode(line);

            // decode again.
            var decodedLine = coder.Decode(encoded) as ReferencedLine;
            var decodedLineJson = decodedLine.ToFeatures(routerDb).ToGeoJson();
        }
    }
}