using Itinero;
using Itinero.Osm.Vehicles;
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

            // TODO: fix the NWB tests.
            //// executes the netherlands tests based on NWB.
            //routerDb = NWB.Netherlands.DownloadExtractAndBuildRouterDb();
            //netherlandsTest = () => { NWB.Netherlands.TestEncodeDecodePointAlongLine(routerDb); };
            //netherlandsTest.TestPerf("Testing netherlands point along line performance");
            //netherlandsTest = () => { NWB.Netherlands.TestEncodeDecodeRoutes(routerDb); };
            //netherlandsTest.TestPerf("Testing netherlands line performance");

            Console.ReadLine();
        }
    }
}