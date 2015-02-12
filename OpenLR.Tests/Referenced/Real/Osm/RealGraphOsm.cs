using OpenLR.Referenced.Router;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm.PBF.Streams;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams.Graphs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.Tests.Referenced.Real.Osm
{
    /// <summary>
    /// Class resposible for loading a real graph.
    /// </summary>
    public static class RealGraphOsm
    {
        /// <summary>
        /// Returns a routing graph.
        /// </summary>
        /// <param name="pbf">The name of the OSM pbf-test file.</param>
        /// <returns></returns>
        public static BasicRouterDataSource<LiveEdge> GetRoutingGraph(string pbf)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format(
                "OpenLR.Tests.Data.{0}.osm.pbf", pbf)))
            {
                return new BasicRouterDataSource<LiveEdge>(LiveGraphOsmStreamTarget.Preprocess(new PBFOsmStreamSource(stream), new OsmRoutingInterpreter()));
            }
        }
    }
}