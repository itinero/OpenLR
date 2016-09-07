using OpenLR.Referenced;
using OpenLR.Referenced.NWB;
using OpenLR.Referenced.Router;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Graphs.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.EncodeRoute
{
    class Program
    {
        static void Main(string[] args)
        {
            // make sure the output is logged to console.
            OsmSharp.Logging.Log.Enable();
            OsmSharp.Logging.Log.RegisterListener(new OsmSharp.WinForms.UI.Logging.ConsoleTraceListener());

            var nwbDump = @"C:\work\via\data\NWB\nwb.dump";

            // load data.
            OsmSharp.Logging.Log.TraceEvent("Nederlands.Test", OsmSharp.Logging.TraceEventType.Information,
                string.Format("Loading {0}...", nwbDump));
            var serializer = new LiveEdgeFlatfileSerializer();
            IBasicRouterDataSource<LiveEdge> nwbGraph = null;
            using (var stream = new FileInfo(nwbDump).OpenRead())
            {
                nwbGraph = serializer.Deserialize(stream);
            }

            NWBMapping.BAANSUBSRT = "BST_CODE";
            BasicRouter.MaxSettles = 65536;

            // create test case.
            var testCase = new GeoCoordinate[]
                {
                    new GeoCoordinate(51.95833631000295f, 4.946079254150391f),
                    new GeoCoordinate(51.94288923910573f, 4.954748153686523f)
                };

            // create encoder.
            var encoder = ReferencedNWBEncoder.CreateBinary(nwbGraph);

            // build line location from shortest path.
            OsmSharp.Routing.Route route;
            var line = encoder.BuildLineLocationFromShortestPath(testCase[0], testCase[1], out route);
            var lineJson = line.ToFeatures().ToGeoJson(); // create geojson to view output.

            // encode the line location.
            var encoded = encoder.Encode(line);

            // create decoder.
            var decoder = ReferencedNWBDecoder.CreateBinary(nwbGraph);

            // decode line location.
            var decodedLine = decoder.Decode(encoded);
            var decodedLineJson = line.ToFeatures().ToGeoJson(); // create geojson to view output.
        }
    }
}
