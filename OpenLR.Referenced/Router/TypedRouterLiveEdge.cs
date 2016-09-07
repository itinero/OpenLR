using OsmSharp.Routing;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Routers;

namespace OpenLR.Referenced.Router
{
    /// <summary>
    /// A version of the typedrouter using edges of type LiveEdge.
    /// </summary>
    internal class TypedRouterLiveEdge : TypedRouter<LiveEdge>
    {
        /// <summary>
        /// Creates a new type router using edges of type LiveEdge.
        /// </summary>
        public TypedRouterLiveEdge(IBasicRouterDataSource<LiveEdge> graph, IRoutingInterpreter interpreter,
                           IRoutingAlgorithm<LiveEdge> router)
            : base(graph, interpreter, router)
        {

        }

        /// <summary>
        /// Returns true if the given vehicle is supported.
        /// </summary>
        public override bool SupportsVehicle(OsmSharp.Routing.Vehicle vehicle)
        {
            // TODO: ask interpreter.
            return true;
        }

        /// <summary>
        /// Calculates all routes from a given resolved point to the routable graph.
        /// </summary>
        public new PathSegmentVisitList RouteResolvedGraph(OsmSharp.Routing.Vehicle vehicle, RouterPoint resolvedPoint, bool? backwards)
        {
            return base.RouteResolvedGraph(vehicle, resolvedPoint, backwards);
        }

        /// <summary>
        /// Constructs a route from a given path.
        /// </summary>
        public Route ConstructRouteFromPath(OsmSharp.Routing.Vehicle vehicle, PathSegment<long> route, RouterPoint source, RouterPoint target, bool geometryOnly)
        {
            return this.ConstructRoute(vehicle, route, source, target, geometryOnly);
        }
    }
}