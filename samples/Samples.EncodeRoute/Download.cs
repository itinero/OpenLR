using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Samples.EncodeRoute
{
    public static class Download
    {
        public static string NetherlandsRouterDbUrl = "http://files.itinero.tech/data/itinero/routerdbs/planet/europe/netherlands.c.cf.routerdb";

        /// <summary>
        /// Downloads all test-data.
        /// </summary>
        public static void DownloadAll()
        {
            if (!File.Exists("netherlands.c.cf.routerdb"))
            {
                var client = new WebClient();
                client.DownloadFile(Download.NetherlandsRouterDbUrl,
                    "netherlands.c.cf.routerdb");
            }
        }

        /// <summary>
        /// Downloads a file if it doesn't exist yet.
        /// </summary>
        public static async Task ToFile(string url, string filename)
        {
            if (!File.Exists(filename))
            {
                var client = new HttpClient();
                using (var stream = await client.GetStreamAsync(url))
                using (var outputStream = File.OpenWrite(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename)))
                {
                    stream.CopyTo(outputStream);
                }
            }
        }

        /// <summary>
        /// Downloads and extracts the given file.
        /// </summary>
        public static void DownloadAndExtractShape(string url, string filename)
        {
            Download.ToFile(url, filename).Wait();
            Extract(filename);
        }

        /// <summary>
        /// Extracts the given file to a 'temp' directory.
        /// </summary>
        /// <param name="file"></param>
        public static void Extract(string file)
        {
            var archive = new ZipArchive(File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file)));
            var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }
            foreach (var entry in archive.Entries)
            {
                if (!string.IsNullOrWhiteSpace(entry.Name))
                {
                    var entryFile = Path.Combine(baseDir, entry.FullName);
                    using (var entryStream = entry.Open())
                    using (var outputStream = File.OpenWrite(entryFile))
                    {
                        entryStream.CopyTo(outputStream);
                    }
                }
                else
                {
                    var dir = Path.Combine(baseDir, entry.FullName);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }
            }
        }
    }
}
