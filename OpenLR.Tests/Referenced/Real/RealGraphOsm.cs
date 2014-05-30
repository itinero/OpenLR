using OsmSharp.Collections.Tags;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Osm.Graphs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.Tests.Referenced.Real
{
    /// <summary>
    /// Class resposible for loading a real graph.
    /// </summary>
    public static class RealGraphOsm
    {
        /// <summary>
        /// Holds the routing graph.
        /// </summary>
        private static IBasicRouterDataSource<LiveEdge> _routingGraph;

        /// <summary>
        /// Returns a routing graph.
        /// </summary>
        /// <returns></returns>
        public static IBasicRouterDataSource<LiveEdge> GetRoutingGraph()
        {
            if (_routingGraph == null)
            {
                TagsCollectionBase metaTags;
                var serializer = new global::OsmSharp.Routing.Osm.Graphs.Serialization.LiveEdgeFlatfileSerializer();
                _routingGraph = serializer.Deserialize(
                    new FileInfo(@"C:\OSM\bin\belgium-latest.simple.flat.routing").OpenRead(), out metaTags, false);
            }
            return _routingGraph;
        }
    }
}