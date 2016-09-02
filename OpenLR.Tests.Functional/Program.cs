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
            
            // executes the netherlands tests.
            var routerDb = RouterDb.Deserialize(File.OpenRead(@"netherlands.c.cf.routerdb"));
            routerDb.RemoveContracted(Vehicle.Car.Shortest());
            Action netherlandsTest = () => { Netherlands.TestAll(routerDb); };
            netherlandsTest.TestPerf("Testing netherlands performance");

            Console.ReadLine();
        }
    }
}