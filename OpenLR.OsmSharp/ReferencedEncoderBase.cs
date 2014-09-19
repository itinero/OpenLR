using OpenLR.Encoding;
using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.OsmSharp.Encoding;
using OpenLR.OsmSharp.Locations;
using OpenLR.Referenced;
using OsmSharp;
using OsmSharp.Collections.PriorityQueues;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Units.Angle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenLR.OsmSharp
{
    /// <summary>
    /// A referenced encoder implementation.
    /// </summary>
    public abstract class ReferencedEncoderBase<TEdge> : OpenLR.Referenced.Encoding.ReferencedEncoder
        where TEdge : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Holds the basic router datasource.
        /// </summary>
        private readonly IBasicRouterDataSource<TEdge> _graph;

        /// <summary>
        /// Creates a new referenced encoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="locationEncoder"></param>
        public ReferencedEncoderBase(IBasicRouterDataSource<TEdge> graph, Encoder locationEncoder)
            : base(locationEncoder)
        {
            _graph = graph;
        }

        /// <summary>
        /// Returns the router.
        /// </summary>
        /// <returns></returns>
        protected abstract IBasicRouter<TEdge> GetRouter();

        /// <summary>
        /// Returns the reference graph.
        /// </summary>
        public IBasicRouterDataSource<TEdge> Graph
        {
            get
            {
                return _graph;
            }
        }

        /// <summary>
        /// Gets the referenced point along line encoder.
        /// </summary>
        protected virtual ReferencedPointAlongLineEncoder<TEdge> GetReferencedPointAlongLineEncoder()
        {
            return new ReferencedPointAlongLineEncoder<TEdge>(this, this.LocationEncoder.CreatePointAlongLineLocationEncoder(), _graph, this.GetRouter());
        }

        /// <summary>
        /// Encodes a referenced point along line location into an unreferenced location.
        /// </summary>
        /// <param name="pointAlongLineLocation"></param>
        /// <returns></returns>
        public virtual PointAlongLineLocation EncodeReferenced(ReferencedPointAlongLine<TEdge> pointAlongLineLocation)
        {
            return this.GetReferencedPointAlongLineEncoder().EncodeReferenced(pointAlongLineLocation);
        }

        /// <summary>
        /// Encodes a point along line location.
        /// </summary>
        /// <param name="pointAlongLineLocation"></param>
        /// <returns></returns>
        public virtual string Encode(ReferencedPointAlongLine<TEdge> pointAlongLineLocation)
        {
            return this.GetReferencedPointAlongLineEncoder().Encode(pointAlongLineLocation);
        }

        /// <summary>
        /// Encodes the given location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override string Encode(ReferencedLocation location)
        {
            if (location == null) { throw new ArgumentNullException("location"); }

            if (location is ReferencedPointAlongLine<TEdge>)
            {
                return this.Encode(location as ReferencedPointAlongLine<TEdge>);
            }

            throw new ArgumentOutOfRangeException("location",
                string.Format("Location cannot be encoded by any of the encoders: {0}", location.ToString()));
        }

        /// <summary>
        /// Returns the encoder vehicle profile.
        /// </summary>
        public abstract Vehicle Vehicle
        {
            get;
        }

        /// <summary>
        /// Returns the location of the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public abstract Coordinate GetVertexLocation(long vertex);

        /// <summary>
        /// Returns the tags associated with the given tags id.
        /// </summary>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        public abstract TagsCollectionBase GetTags(uint tagsId);

        /// <summary>
        /// Tries to match the given tags and figure out a corresponding frc and fow.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="frc"></param>
        /// <param name="fow"></param>
        /// <returns>False if no matching was found.</returns>
        public abstract bool TryMatching(TagsCollectionBase tags, out FunctionalRoadClass frc, out FormOfWay fow);

        /// <summary>
        /// Returns a value if a oneway restriction is found.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns>null: no restrictions, true: forward restriction, false: backward restriction.</returns>
        /// <returns></returns>
        public abstract bool? IsOneway(TagsCollectionBase tags);

        /// <summary>
        /// Returns the bearing calculate between two given vertices along the given edge.
        /// </summary>
        /// <param name="vertexFrom"></param>
        /// <param name="edge"></param>
        /// <param name="vertexTo"></param>
        /// <param name="forward">When true the edge is forward relative to the vertices, false the edge is backward.</param>
        /// <returns></returns>
        public virtual Degree GetBearing(long vertexFrom, TEdge edge, long vertexTo, bool forward)
        {
            var coordinates = new List<GeoCoordinate>();
            float latitude, longitude;
            this.Graph.GetVertex((uint)vertexFrom, out latitude, out longitude);
            coordinates.Add(new GeoCoordinate(latitude, longitude));

            if (edge.Coordinates != null)
            { // there are intermediates, add them in the correct order.
                if (forward)
                {
                    coordinates.AddRange(edge.Coordinates.Select<GeoCoordinateSimple, GeoCoordinate>(x => { return new GeoCoordinate(x.Latitude, x.Longitude); }));
                }
                else
                {
                    coordinates.AddRange(edge.Coordinates.Reverse().Select<GeoCoordinateSimple, GeoCoordinate>(x => { return new GeoCoordinate(x.Latitude, x.Longitude); }));
                }
            }

            this.Graph.GetVertex((uint)vertexTo, out latitude, out longitude);
            coordinates.Add(new GeoCoordinate(latitude, longitude));

            return BearingEncoder.EncodeBearing(coordinates);
        }

        /// <summary>
        /// Returns true if the given vertex is a valid candidate to use as a location reference point.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool IsVertexValid(long vertex)
        {
            var arcs = this.Graph.GetArcs((uint)vertex);

            // filter out non-traversable arcs.
            var traversableArcs = arcs.Where((arc) =>
            {
                return this.Vehicle.CanTraverse(this.Graph.TagsIndex.Get(arc.Value.Tags));
            }).ToList();

            if (traversableArcs.Count > 1)
            { // if there is one incoming edge where there is only one way out then this vertex is invalid.
                foreach (var incoming in traversableArcs)
                {
                    // check if this neighbour is incoming.
                    var incomingTags = this.Graph.TagsIndex.Get(incoming.Value.Tags);
                    var incomingOneway = this.Vehicle.IsOneWay(incomingTags);
                    if (incomingOneway == null &&
                        incomingOneway.Value != incoming.Value.Forward)
                    { // ok, is not oneway or oneway is in incoming direction.
                        var outgoingCount = 0;
                        foreach (var outgoing in traversableArcs)
                        {
                            if (outgoing.Key != incoming.Key)
                            { // don't take the same edge.
                                var outgoingTags = this.Graph.TagsIndex.Get(outgoing.Value.Tags);
                                var oneway = this.Vehicle.IsOneWay(outgoingTags);
                                if (oneway == null &&
                                    oneway.Value == outgoing.Value.Forward)
                                { // ok, is not oneway or oneway is outgoing direction.
                                    outgoingCount++;
                                }
                            }
                        }
                        if (outgoingCount == 1)
                        { // there is only one option, so for some situations to vertex is invalid, mark it invalid for all.
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            { // one arc, vertex is at the end.
                return true;
            }
        }

        /// <summary>
        /// Finds a valid vertex for the given vertex but does not search in the direction of the target neighbour.
        /// </summary>
        /// <param name="vertex">The invalid vertex.</param>
        /// <param name="targetNeighbour">The neighbour of this vertex that is part of the location.</param>
        /// <param name="searchForward">When true, the search is forward, otherwise backward.</param>
        public abstract PathSegment<uint> FindValidVertexFor(long vertex, long targetNeighbour, bool searchForward);
    }

    /// <summary>
    /// Contains encoder extensions.
    /// </summary>
    public static class ReferencedEncoderBaseExtensions
    {
        /// <summary>
        /// Builds a point along line location for the given encoder.
        /// </summary>
        /// <param name="encoder"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static ReferencedPointAlongLine<LiveEdge> BuildPointAlongLine(this ReferencedEncoderBase<LiveEdge> encoder, GeoCoordinate location)
        {
            // get the closest edge that can be traversed to the given location.
            var closest = encoder.Graph.GetClosestEdge<LiveEdge>(location);

            // check oneway.
            var oneway = encoder.Vehicle.IsOneWay(encoder.Graph.TagsIndex.Get(closest.Value.Value.Tags));
            var useForward = (oneway == null) || (oneway.Value == closest.Value.Value.Forward);

            // build location and make sure the vehicle can travel from from location to to location.
            LiveEdge edge;
            uint from, to;
            if (useForward)
            { // encode first point to last point.
                edge = closest.Value.Value;
                if (!closest.Value.Value.Forward)
                { // use reverse edge.
                    var reverseEdge = new LiveEdge();
                    reverseEdge.Tags = closest.Value.Value.Tags;
                    reverseEdge.Forward = !closest.Value.Value.Forward;
                    reverseEdge.Distance = closest.Value.Value.Distance;
                    reverseEdge.Coordinates = null;
                    if (closest.Value.Value.Coordinates != null)
                    {
                        var reverse = new GeoCoordinateSimple[closest.Value.Value.Coordinates.Length];
                        closest.Value.Value.Coordinates.CopyToReverse(reverse, 0);
                        reverseEdge.Coordinates = reverse;
                    }
                    edge = reverseEdge;
                }
                from = closest.Key;
                to = closest.Value.Key;
            }
            else
            { // encode last point to first point.
                edge = closest.Value.Value;
                if (closest.Value.Value.Forward)
                { // reverse edge if needed.
                    // Next OsmSharp version: use closest.Value.Value.Reverse();
                    var reverseEdge = new LiveEdge();
                    reverseEdge.Tags = closest.Value.Value.Tags;
                    reverseEdge.Forward = !closest.Value.Value.Forward;
                    reverseEdge.Distance = closest.Value.Value.Distance;
                    reverseEdge.Coordinates = null;
                    if (closest.Value.Value.Coordinates != null)
                    {
                        var reverse = new GeoCoordinateSimple[closest.Value.Value.Coordinates.Length];
                        closest.Value.Value.Coordinates.CopyToReverse(reverse, 0);
                        reverseEdge.Coordinates = reverse;
                    }
                    edge = reverseEdge;
                }
                from = closest.Value.Key;
                to = closest.Key;
            }

            // OK, now we have a naive location, we need to check if it's valid.
            // see: §C section 6 @ http://www.tomtom.com/lib/OpenLR/OpenLR-whitepaper.pdf
            var referencedPointAlongLine = new OpenLR.OsmSharp.Locations.ReferencedPointAlongLine<LiveEdge>()
                {
                    Route = new ReferencedLine<LiveEdge>(encoder.Graph)
                    {
                        Edges = new LiveEdge[] { edge },
                        Vertices = new long[] { from, to }
                    },
                    Latitude = location.Latitude,
                    Longitude = location.Longitude
                };

            bool rule1 = false, rule4 = false;
            while(!rule1 || !rule4)
            { // keep looping until rules are valid or building fails.
                // RULE1: distance should not exceed 15km.
                var length = referencedPointAlongLine.Length(encoder);
                rule1 = length.Value > 15000;

                // RULE2: no need to check, will be rounded.

                // RULE3: ok, there are two points.

                // RULE4: choosen points should be valid network points.
                bool fromIsValid = encoder.IsVertexValid(from);
                bool toIsValid = encoder.IsVertexValid(to);

                if (fromIsValid && toIsValid)
                { // ok, this is good, the location is already valid.
                    if (!rule1)
                    { // no implented this.
                        throw new NotImplementedException("Distance between the two closest valid points is too big, should insert intermediate point.");
                    }
                }
                else
                { // try to find valid points.
                    if(!fromIsValid)
                    { // from is not valid, try to find a valid point.
                        var pathToValid = encoder.FindValidVertexFor(referencedPointAlongLine.Route.Vertices[0], referencedPointAlongLine.Route.Vertices[1], false);

                        // build edges list.
                        var vertices = pathToValid.ToArray().ToList();
                        var edges = new List<LiveEdge>();
                        for (int idx = 0; idx < vertices.Count - 1; idx++)
                        { // loop over edges.
                            edges.Add(encoder.Graph.GetArcs(vertices[idx + 1]).Where(x => x.Key == vertices[idx]).First().Value);
                        }

                        // create new location.
                        var edgesArray = new LiveEdge[edges.Count + referencedPointAlongLine.Route.Edges.Length];
                        edges.CopyTo(0, edgesArray, 0, edges.Count);
                        referencedPointAlongLine.Route.Edges.CopyTo(0, edgesArray, edges.Count, referencedPointAlongLine.Route.Edges.Length);
                        var vertexArray = new long[vertices.Count - 1 + referencedPointAlongLine.Route.Vertices.Length];
                        vertices.ConvertAll(x => (long)x).CopyTo(0, vertexArray, 0, vertices.Count - 1);
                        referencedPointAlongLine.Route.Vertices.CopyTo(0, vertexArray, vertices.Count - 1, referencedPointAlongLine.Route.Vertices.Length);

                        referencedPointAlongLine.Route.Edges = edgesArray;
                        referencedPointAlongLine.Route.Vertices = vertexArray;
                    }

                    if (!toIsValid)
                    { // from is not valid, try to find a valid point.
                        var vertexCount = referencedPointAlongLine.Route.Vertices.Length;
                        var pathToValid = encoder.FindValidVertexFor(referencedPointAlongLine.Route.Vertices[vertexCount - 1], referencedPointAlongLine.Route.Vertices[vertexCount - 2], true);

                        // build edges list.
                        var vertices = pathToValid.ToArray().ToList();
                        var edges = new List<LiveEdge>();
                        for (int idx = 0; idx < vertices.Count - 1; idx++)
                        { // loop over edges.
                            edges.Add(encoder.Graph.GetArcs(vertices[idx]).Where(x => x.Key == vertices[idx + 1]).First().Value);
                        }

                        // create new location.
                        var edgesArray = new LiveEdge[edges.Count + referencedPointAlongLine.Route.Edges.Length];
                        referencedPointAlongLine.Route.Edges.CopyTo(0, edgesArray, 0, referencedPointAlongLine.Route.Edges.Length);
                        edges.CopyTo(0, edgesArray, referencedPointAlongLine.Route.Edges.Length, edges.Count);
                        var vertexArray = new long[vertices.Count - 1 + referencedPointAlongLine.Route.Vertices.Length];
                        referencedPointAlongLine.Route.Vertices.CopyTo(0, vertexArray, 0, referencedPointAlongLine.Route.Vertices.Length);
                        vertices.ConvertAll(x => (long)x).CopyTo(1, vertexArray, referencedPointAlongLine.Route.Vertices.Length, vertices.Count - 1);

                        referencedPointAlongLine.Route.Edges = edgesArray;
                        referencedPointAlongLine.Route.Vertices = vertexArray;
                    }
                }
            }
            return null;
        }
    }
}