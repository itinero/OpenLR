﻿using OpenLR.Encoding;
using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.Referenced.Encoding;
using OpenLR.Referenced.Exceptions;
using OpenLR.Referenced.Locations;
using OpenLR.Referenced.Router;
using OpenLR.Referenced;
using OsmSharp;
using OsmSharp.Collections.PriorityQueues;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Units.Angle;
using OsmSharp.Units.Distance;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenLR.Referenced
{
    /// <summary>
    /// A referenced encoder implementation.
    /// </summary>
    public abstract class ReferencedEncoderBase : OpenLR.Referenced.Encoding.ReferencedEncoder
    {
        /// <summary>
        /// Holds the basic router datasource.
        /// </summary>
        private readonly BasicRouterDataSource<LiveEdge> _graph;

        /// <summary>
        /// Creates a new referenced encoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="locationEncoder"></param>
        public ReferencedEncoderBase(BasicRouterDataSource<LiveEdge> graph, Encoder locationEncoder)
            : base(locationEncoder)
        {
            _graph = graph;
        }

        /// <summary>
        /// Gets a new router.
        /// </summary>
        /// <returns></returns>
        protected virtual BasicRouter GetRouter()
        {
            return new BasicRouter();
        }

        /// <summary>
        /// Returns the reference graph.
        /// </summary>
        public BasicRouterDataSource<LiveEdge> Graph
        {
            get
            {
                return _graph;
            }
        }

        /// <summary>
        /// Gets the referenced point along line encoder.
        /// </summary>
        protected virtual ReferencedPointAlongLineEncoder GetReferencedPointAlongLineEncoder()
        {
            return new ReferencedPointAlongLineEncoder(this, this.LocationEncoder.CreatePointAlongLineLocationEncoder());
        }

        /// <summary>
        /// Encodes a referenced point along line location into an unreferenced location.
        /// </summary>
        /// <param name="pointAlongLineLocation"></param>
        /// <returns></returns>
        public virtual PointAlongLineLocation EncodeReferenced(ReferencedPointAlongLine pointAlongLineLocation)
        {
            return this.GetReferencedPointAlongLineEncoder().EncodeReferenced(pointAlongLineLocation);
        }

        /// <summary>
        /// Encodes a point along line location.
        /// </summary>
        /// <param name="pointAlongLineLocation"></param>
        /// <returns></returns>
        public virtual string Encode(ReferencedPointAlongLine pointAlongLineLocation)
        {
            return this.GetReferencedPointAlongLineEncoder().Encode(pointAlongLineLocation);
        }

        /// <summary>
        /// Gets the referenced line encoder.
        /// </summary>
        protected virtual ReferencedLineEncoder GetReferencedLineEncoder()
        {
            return new ReferencedLineEncoder(this, this.LocationEncoder.CreateLineLocationEncoder());
        }

        /// <summary>
        /// Encodes a referenced line location into an unreferenced location.
        /// </summary>
        /// <param name="lineLocation"></param>
        /// <returns></returns>
        public virtual LineLocation EncodeReferenced(ReferencedLine lineLocation)
        {
            return this.GetReferencedLineEncoder().EncodeReferenced(lineLocation);
        }

        /// <summary>
        /// Encodes a line location.
        /// </summary>
        /// <param name="lineLocation"></param>
        /// <returns></returns>
        public virtual string Encode(ReferencedLine lineLocation)
        {
            return this.GetReferencedLineEncoder().Encode(lineLocation);
        }

        /// <summary>
        /// Encodes the given location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override string Encode(ReferencedLocation location)
        {
            if (location == null) { throw new ArgumentNullException("location"); }

            if (location is ReferencedLine)
            {
                return this.Encode(location as ReferencedLine);
            }
            else if (location is ReferencedPointAlongLine)
            {
                return this.Encode(location as ReferencedPointAlongLine);
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
        public virtual Coordinate GetVertexLocation(long vertex)
        {
            float latitude, longitude;
            if (!this.Graph.GetVertex(vertex, out latitude, out longitude))
            { // oeps, vertex does not exist!
                throw new ArgumentOutOfRangeException("vertex", string.Format("Vertex {0} not found!", vertex));
            }
            return new Coordinate()
            {
                Latitude = latitude,
                Longitude = longitude
            };
        }

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
        /// Builds a location referenced point for the vertex at the given start-index.
        /// </summary>
        /// <returns></returns>
        public LocationReferencePoint BuildLocationReferencePoint(ReferencedLine referencedLocation, int start, int end)
        {
            FormOfWay fow;
            FunctionalRoadClass frc;

            // get all coordinates along the sequence starting at 'start' and ending at 'end'.
            var coordinates = referencedLocation.GetCoordinates(this, start, end - start + 1);

            // create location reference point.
            var locationReferencePoint = new LocationReferencePoint();
            locationReferencePoint.Coordinate = this.GetVertexLocation(referencedLocation.Vertices[start]);
            var tags = this.GetTags(referencedLocation.Edges[start].Tags);
            if (!this.TryMatching(tags, out frc, out fow))
            {
                throw new ReferencedEncodingException(referencedLocation,
                    "Could not find frc and/or fow for the given tags.");
            }
            locationReferencePoint.FormOfWay = fow;
            locationReferencePoint.FuntionalRoadClass = frc;
            locationReferencePoint.Bearing = (int)BearingEncoder.EncodeBearing(coordinates).Value;
            locationReferencePoint.DistanceToNext = (int)coordinates.Length().Value;
            FunctionalRoadClass? lowest = null;
            for (var edge = start; edge < end; edge++)
            {
                tags = this.GetTags(referencedLocation.Edges[edge].Tags);
                if (!this.TryMatching(tags, out frc, out fow))
                {
                    throw new ReferencedEncodingException(referencedLocation,
                        "Could not find frc and/or fow for the given tags.");
                }

                if (!lowest.HasValue ||
                    frc < lowest)
                {
                    lowest = frc;
                }
            }
            locationReferencePoint.LowestFunctionalRoadClassToNext = lowest;

            return locationReferencePoint;
        }

        /// <summary>
        /// Builds a location referenced point for the last vertex.
        /// </summary>
        /// <returns></returns>
        public LocationReferencePoint BuildLocationReferencePointLast(ReferencedLine referencedLocation, int before)
        {
            FormOfWay fow;
            FunctionalRoadClass frc;

            var end = referencedLocation.Vertices.Length - 1;

            // get all coordinates along the sequence starting at 'before' and ending at 'end'.
            var coordinates = referencedLocation.GetCoordinates(this, before, end - before + 1);

            // create location reference point.
            var locationReferencedPoint = new LocationReferencePoint();
            locationReferencedPoint.Coordinate = this.GetVertexLocation(referencedLocation.Vertices[end]);
            var tags = this.GetTags(referencedLocation.Edges[end - 1].Tags);
            if (!this.TryMatching(tags, out frc, out fow))
            {
                throw new ReferencedEncodingException(referencedLocation,
                    "Could not find frc and/or fow for the given tags.");
            }
            locationReferencedPoint.FormOfWay = fow;
            locationReferencedPoint.FuntionalRoadClass = frc;
            locationReferencedPoint.Bearing = (int)BearingEncoder.EncodeBearing(coordinates, true).Value;

            return locationReferencedPoint;
        }

        /// <summary>
        /// Returns true if the given vertex is a valid candidate to use as a location reference point.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public virtual bool IsVertexValid(long vertex)
        {
            var arcs = this.Graph.GetEdges(vertex);

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
                    if (incomingOneway == null ||
                        incomingOneway.Value != incoming.Value.Forward)
                    { // ok, is not oneway or oneway is in incoming direction.
                        var outgoingCount = 0;
                        foreach (var outgoing in traversableArcs)
                        {
                            if (outgoing.Key != incoming.Key ||
                                !outgoing.Value.Equals(incoming.Value))
                            { // don't take the same edge.
                                var outgoingTags = this.Graph.TagsIndex.Get(outgoing.Value.Tags);
                                var oneway = this.Vehicle.IsOneWay(outgoingTags);
                                if (oneway == null ||
                                    oneway.Value == outgoing.Value.Forward)
                                { // ok, is not oneway or oneway is outgoing direction.
                                    outgoingCount++;
                                    if (outgoingCount > 1)
                                    { // it's not going down again, stop the search.
                                        break;
                                    }
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
        /// <param name="edge">The edge that leads to the target vertex.</param>
        /// <param name="targetVertex">The target vertex.</param>
        /// <param name="excludeSet">The set of vertices that should be excluded in the search.</param>
        /// <param name="searchForward">When true, the search is forward, otherwise backward.</param>
        public virtual PathSegment FindValidVertexFor(long vertex, LiveEdge edge, long targetVertex, HashSet<long> excludeSet, bool searchForward)
        {
            // GIST: execute a dykstra search to find a vertex that is valid.
            // this will return a vertex that is on the shortest path:
            // foundVertex -> vertex -> targetNeighbour.

            // initialize settled set.
            var settled = new HashSet<long>();
            settled.Add(targetVertex);

            // initialize heap.
            var heap = new BinaryHeap<PathSegment>(10);
            heap.Push(new PathSegment(vertex), 0);

            // find the path to the closest valid vertex.
            PathSegment pathTo = null;
            while (heap.Count > 0)
            {
                // get next.
                var current = heap.Pop();
                settled.Add(current.Vertex);

                // check if valid.
                if (current.Vertex != vertex &&
                    this.IsVertexValid(current.Vertex))
                { // ok! vertex is valid.
                    pathTo = current;
                }
                else
                { // continue search.
                    // add unsettled neighbours.
                    var arcs = this.Graph.GetEdges(current.Vertex);
                    foreach (var arc in arcs)
                    {
                        if (!excludeSet.Contains(arc.Key) &&
                            !settled.Contains(arc.Key) &&
                           !(current.Vertex == vertex && arc.Key == targetVertex && edge.Distance == arc.Value.Distance))
                        { // ok, new neighbour, and ok, not the edge and neighbour to ignore.
                            var tags = this.Graph.TagsIndex.Get(arc.Value.Tags);
                            if (this.Vehicle.CanTraverse(tags))
                            { // ok, we can traverse this edge.
                                var onway = this.Vehicle.IsOneWay(tags);
                                if (onway == null ||
                                  !(onway.Value == arc.Value.Forward ^ searchForward))
                                { // ok, no oneway or oneway reversed.
                                    var weight = this.Vehicle.Weight(this.Graph.TagsIndex.Get(arc.Value.Tags), arc.Value.Distance);
                                    var path = new PathSegment(arc.Key, current.Weight + weight, arc.Value, current);
                                    heap.Push(path, (float)path.Weight);
                                }
                            }
                        }
                    }
                }
            }

            // ok, is there a path found.
            if (pathTo == null)
            { // oeps, probably something wrong with network-topology.
                // just take the default option.
                //throw new Exception(
                //    string.Format("Could not find a valid vertex for invalid vertex [{0}].", vertex));
                return null;
            }

            // add the path to the given location.
            return pathTo;
        }

        /// <summary>
        /// Finds the shortest path between the given from->to.
        /// </summary>
        /// <param name="from">The source vertex.</param>
        /// <param name="to">The target vertex.</param>
        /// <param name="searchForward">Flag for search direction.</param>
        /// <returns></returns>
        public virtual PathSegment FindShortestPath(long from, long to, bool searchForward)
        {
            var router = this.GetRouter();
            var result = router.Calculate(this.Graph, this.Vehicle,
                from, to, searchForward);
            if (result == null)
            {
                result = router.Calculate(this.Graph, this.Vehicle,
                    from, to, searchForward, BasicRouter.MaxSettles);
            }
            return result;
        }

        /// <summary>
        /// Finds the shortest path between the given from->to.
        /// </summary>
        /// <param name="fromPaths">The paths to the source.</param>
        /// <param name="toPaths">The paths to the target.</param>
        /// <param name="searchForward">Flag for search direction.</param>
        /// <returns></returns>
        public virtual PathSegment FindShortestPath(List<PathSegment> fromPaths, List<PathSegment> toPaths, bool searchForward)
        {
            var router = this.GetRouter();
            var result = router.Calculate(this.Graph, this.Vehicle,
                fromPaths, toPaths, searchForward);
            //if (result == null)
            //{
            //    result = router.Calculate(this.Graph, this.Vehicle,
            //        fromPaths, toPaths, searchForward, BasicRouter.MAX_SETTLES);
            //}
            return result;
        }
    }
}