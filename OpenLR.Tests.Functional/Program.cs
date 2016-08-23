using System;

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

            Netherlands.Test();
        }
    }
}