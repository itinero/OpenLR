//// The MIT License (MIT)

//// Copyright (c) 2016 Ben Abelshausen

//// Permission is hereby granted, free of charge, to any person obtaining a copy
//// of this software and associated documentation files (the "Software"), to deal
//// in the Software without restriction, including without limitation the rights
//// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//// copies of the Software, and to permit persons to whom the Software is
//// furnished to do so, subject to the following conditions:

//// The above copyright notice and this permission notice shall be included in
//// all copies or substantial portions of the Software.

//// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//// THE SOFTWARE.

//using OpenLR.Model;
//using OpenLR.Referenced.Decoding.Candidates;
//using OsmSharp.Collections.PriorityQueues;
//using OsmSharp.Routing;
//using OsmSharp.Routing.Algorithms;
//using OsmSharp.Routing.Profiles;
//using System;
//using System.Collections.Generic;

//namespace OpenLR.Referenced.Router
//{
//    /// <summary>
//    /// A simple dykstra-based router.
//    /// </summary>
//    /// <remarks>Specific OpenLR-implementation that does only the bare necessities.</remarks>
//    public class BasicRouter
//    {
//        /// <summary>
//        /// Holds the maximum settles.
//        /// </summary>
//        public static uint MaxSettles = 100;

//        /// <summary>
//        /// Calculates a path between the two candidates using the information in the candidates.
//        /// </summary>
//        public Path Calculate(RouterDb db, Profile profile, CandidateVertexEdge from, CandidateVertexEdge to, 
//            FunctionalRoadClass minimum, bool ignoreFromEdge, bool ignoreToEdge)
//        {
//            return this.Calculate(db, profile, from, to, minimum, MaxSettles, ignoreFromEdge, ignoreToEdge);
//        }

//        /// <summary>
//        /// Calculates a path between the two candidates using the information in the candidates.
//        /// </summary>
//        /// <returns></returns>
//        public Path Calculate(RouterDb db, Profile profile, CandidateVertexEdge from, CandidateVertexEdge to, 
//            FunctionalRoadClass minimum, uint maxSettles, bool ignoreFromEdge, bool ignoreToEdge)
//        {
//            //// first check for the simple stuff.
//            //if (from.VertexId == to.VertexId)
//            //{ // route consists of one vertex.
//            //    return new Path(from.VertexId);
//            //}

//            //// check paths.
//            //var fromPath = new PathSegment<long>(from.TargetVertex, vehicle.Weight(graph.TagsIndex.Get(from.Edge.Tags), 
//            //    from.Edge.Distance), new PathSegment<long>(from.Vertex));
//            //if (!ignoreFromEdge || !ignoreToEdge)
//            //{ // do not check paths when one of the edges need to be ignored.
//            //    if (from.Vertex == to.TargetVertex &&
//            //        to.Vertex == from.TargetVertex)
//            //    { // edges are the same, 
//            //        return fromPath;
//            //    }
//            //}
//            //if(ignoreFromEdge)
//            //{ // ignore from edge, just use the from-vertex.
//            //    fromPath = new Path(from.VertexId);
//            //}

//            //// initialize the heap/visit list.
//            //var heap = new BinaryHeap<Path>(maxSettles / 4);
//            //var visited = new HashSet<long>();
//            //visited.Add(from.VertexId);

//            //// set target.
//            //var target = to.VertexId;
//            //var targetWeight = double.MaxValue;
//            //if(!ignoreToEdge)
//            //{ // also add the target to the visit list and actually search for the target candidate edge ending.
//            //    target = to.TargetVertex;
//            //    visited.Add(to.Vertex);
//            //}

//            //// create a path segment from the from-candidate.
//            //heap.Push(fromPath, (float)fromPath.Weight);

//            //// keep searching for the target.
//            //while (true)
//            //{
//            //    // get the next vertex.
//            //    var current = heap.Pop();
//            //    if (current == null)
//            //    { // there is nothing more in the queue, target will not be found.
//            //        break;
//            //    }
//            //    if (visited.Contains(current.VertexId))
//            //    { // move to the next neighbour.
//            //        continue;
//            //    }
//            //    visited.Add(current.VertexId);

//            //    // check for the target.
//            //    if(current.VertexId == target)
//            //    { // target was found.
//            //        if(ignoreToEdge)
//            //        {
//            //            return current;
//            //        }
//            //        return new PathSegment<long>(to.Vertex, current.Weight, current);
//            //    }

//            //    // check if the maximum settled vertex count has been reached.
//            //    if(visited.Count >= maxSettles)
//            //    { // stop search, target will not be found.
//            //        break;
//            //    }

//            //    // add the neighbours to queue.
//            //    var neighbours = graph.GetEdges(current.VertexId);
//            //    if (neighbours != null)
//            //    { // neighbours exist.
//            //        foreach (var neighbour in neighbours)
//            //        {
//            //            // check if the neighbour was settled before.
//            //            if (visited.Contains(neighbour.Key))
//            //            { // move to the next neighbour.
//            //                continue;
//            //            }

//            //            // get tags and check traversability and oneway.
//            //            var tags = graph.TagsIndex.Get(neighbour.Value.Tags);
//            //            if (vehicle.CanTraverse(tags))
//            //            { // yay! can traverse.
//            //                var oneway = vehicle.IsOneWay(tags);
//            //                if (oneway == null ||
//            //                    oneway.Value == neighbour.Value.Forward)
//            //                {
//            //                    // create path to neighbour and queue it.
//            //                    var weight = vehicle.Weight(graph.TagsIndex.Get(neighbour.Value.Tags), neighbour.Value.Distance);
//            //                    var path = new PathSegment<long>(neighbour.Key, current.Weight + weight, current);
//            //                    if (path.Weight < targetWeight)
//            //                    { // the weight of the neighbour is smaller than the first neighbour found.
//            //                        heap.Push(path, (float)path.Weight);

//            //                        // save distance.
//            //                        if (path.VertexId == target)
//            //                        { // the target is already found, no use of queuing neigbours that have a higher weight.
//            //                            if (targetWeight > path.Weight)
//            //                            { // set the weight.
//            //                                targetWeight = path.Weight;
//            //                            }
//            //                        }
//            //                    }
//            //                }
//            //            }
//            //        }
//            //    }
//            //}
//            return null;
//        }

//        /// <summary>
//        /// Calculates the shortest path between the two given 'virtual' vertices described by pre-calculate path segments.
//        /// </summary>
//        public PathSegment Calculate(RouterDb db, Profile profile, 
//            List<PathSegment> fromPaths, List<PathSegment> toPaths, bool searchForward)
//        {
//            return this.Calculate(db, profile, fromPaths, toPaths, searchForward, MaxSettles);
//        }

//        /// <summary>
//        /// Calculates the shortest path between the two given 'virtual' vertices described by pre-calculate path segments.
//        /// </summary>
//        public PathSegment Calculate(RouterDb db, Profile profile,
//            List<PathSegment> fromPaths, List<PathSegment> toPaths, bool searchForward, uint maxSettles)
//        {
//            //if (fromPaths.Count == 0) { return null; }
//            //if (toPaths.Count == 0) { return null; }

//            //// initialize the heap/visit list.
//            //var heap = new BinaryHeap<PathSegment>(maxSettles);
//            //var visited = new HashSet<long>();

//            //// queue the from-paths.
//            //heap.Push(fromPaths[0], (float)fromPaths[0].Weight);
//            //for (int i = 1; i < fromPaths.Count; i++)
//            //{
//            //    if (fromPaths[0].From.Vertex != fromPaths[i].From.Vertex)
//            //    {
//            //        throw new ArgumentException("When multiple from paths they should be one-hop away from a common vertex.");
//            //    }
//            //    heap.Push(fromPaths[i], (float)fromPaths[i].Weight);
//            //}

//            //// enumerate and store target-paths.
//            //var toPathDictonary = new Dictionary<long, PathSegment>();
//            //foreach (var toPath in toPaths)
//            //{
//            //    if(toPath.From != null)
//            //    {
//            //        toPathDictonary.Add(toPath.From.Vertex, toPath);
//            //    }
//            //    else
//            //    {
//            //        toPathDictonary.Add(toPath.Vertex, toPath);
//            //    }
//            //}

//            // keep searching for the target.
//            PathSegment bestToTarget = null;
//            //    while (true)
//            //    {
//            //        // get the next vertex.
//            //        var current = heap.Pop();

//            //        if (current == null)
//            //        { // there is nothing more in the queue, target will not be found.
//            //            break;
//            //        }
//            //        if (visited.Contains(current.Vertex))
//            //        { // move to the next neighbour.
//            //            continue;
//            //        }
//            //        visited.Add(current.Vertex);

//            //        // check for the target.
//            //        PathSegment foundToPath;
//            //        if (toPathDictonary.TryGetValue(current.Vertex, out foundToPath))
//            //        { // target was found.
//            //            toPathDictonary.Remove(current.Vertex);
//            //            if (bestToTarget == null ||
//            //                current.Weight + foundToPath.Weight < bestToTarget.Weight)
//            //            { // ok, this path is better!
//            //                if (foundToPath.From != null)
//            //                {
//            //                    bestToTarget = new PathSegment(foundToPath.Vertex, foundToPath.Weight + current.Weight, foundToPath.Edge, current);
//            //                }
//            //                else
//            //                {
//            //                    bestToTarget = current;
//            //                }
//            //            }
//            //            if (toPathDictonary.Count == 0)
//            //            { // no more targets let, this has to be it.
//            //                return bestToTarget;
//            //            }
//            //        }

//            //        // check if the maximum settled vertex count has been reached.
//            //        if (visited.Count >= maxSettles)
//            //        { // stop search, target will not be found.
//            //            break;
//            //        }

//            //        // add the neighbours to queue.
//            //        var neighbours = graph.GetEdges(current.Vertex);
//            //        if (neighbours != null)
//            //        { // neighbours exist.
//            //            foreach (var neighbour in neighbours)
//            //            {
//            //                // check if the neighbour was settled before.
//            //                if (visited.Contains(neighbour.Key))
//            //                { // move to the next neighbour.
//            //                    continue;
//            //                }

//            //                // get tags and check traversability and oneway.
//            //                var tags = graph.TagsIndex.Get(neighbour.Value.Tags);
//            //                if (vehicle.CanTraverse(tags))
//            //                { // yay! can traverse.
//            //                    var onway = vehicle.IsOneWay(tags);
//            //                    if (onway == null ||
//            //                      !(onway.Value == neighbour.Value.Forward ^ searchForward))
//            //                    {
//            //                        // create path to neighbour and queue it.
//            //                        var weight = vehicle.Weight(graph.TagsIndex.Get(neighbour.Value.Tags), neighbour.Value.Distance);
//            //                        var path = new PathSegment(neighbour.Key, current.Weight + weight, neighbour.Value, current);
//            //                        if (bestToTarget == null ||
//            //                            path.Weight < bestToTarget.Weight)
//            //                        { // the weight of the neighbour is smaller than the first neighbour found.
//            //                            heap.Push(path, (float)path.Weight);
//            //                        }
//            //                    }
//            //                }
//            //            }
//            //        }
//            //    }
//            return bestToTarget;
//        }

//        /// <summary>
//        /// Calculates the shortest path between the two given vertices.
//        /// </summary>
//        public PathSegment Calculate(RouterDb db, Profile profile, 
//            long from, long to, bool searchForward)
//        {
//            return this.Calculate(db, profile, from, to, searchForward, MaxSettles);
//        }

//        /// <summary>
//        /// Calculates the shortest path between the two given vertices.
//        /// </summary>
//        public PathSegment Calculate(RouterDb db, Profile profile, 
//            long from, long to, bool searchForward, uint maxSettles)
//        {
//            //// first check for the simple stuff.
//            //if (from == to)
//            //{ // route consists of one vertex.
//            //    return new PathSegment(from);
//            //}

//            //// initialize the heap/visit list.
//            //var heap = new BinaryHeap<PathSegment>(100);
//            //var visited = new HashSet<long>();

//            //// set the target.
//            //var target = to;
//            //var targetWeight = double.MaxValue;

//            //// create a path segment from the from-candidate.
//            //heap.Push(new PathSegment(from), (float)0);

//            //// keep searching for the target.
//            //while (true)
//            //{
//            //    // get the next vertex.
//            //    var current = heap.Pop();
//            //    if (current == null)
//            //    { // there is nothing more in the queue, target will not be found.
//            //        break;
//            //    }
//            //    if (visited.Contains(current.Vertex))
//            //    { // move to the next neighbour.
//            //        continue;
//            //    }
//            //    visited.Add(current.Vertex);

//            //    // check for the target.
//            //    if (current.Vertex == target)
//            //    { // target was found.
//            //        return current;
//            //    }

//            //    // check if the maximum settled vertex count has been reached.
//            //    if (visited.Count >= maxSettles)
//            //    { // stop search, target will not be found.
//            //        break;
//            //    }

//            //    // add the neighbours to queue.
//            //    var neighbours = graph.GetEdges(current.Vertex);
//            //    if (neighbours != null)
//            //    { // neighbours exist.
//            //        foreach (var neighbour in neighbours)
//            //        {
//            //            // check if the neighbour was settled before.
//            //            if (visited.Contains(neighbour.Key))
//            //            { // move to the next neighbour.
//            //                continue;
//            //            }

//            //            // get tags and check traversability and oneway.
//            //            var tags = graph.TagsIndex.Get(neighbour.Value.Tags);
//            //            if (vehicle.CanTraverse(tags))
//            //            { // yay! can traverse.
//            //                var onway = vehicle.IsOneWay(tags);
//            //                if (onway == null ||
//            //                  !(onway.Value == neighbour.Value.Forward ^ searchForward))
//            //                {
//            //                    // create path to neighbour and queue it.
//            //                    var weight = vehicle.Weight(graph.TagsIndex.Get(neighbour.Value.Tags), neighbour.Value.Distance);
//            //                    var path = new PathSegment(neighbour.Key, current.Weight + weight, neighbour.Value, current);
//            //                    if (path.Weight < targetWeight)
//            //                    { // the weight of the neighbour is smaller than the first neighbour found.
//            //                        heap.Push(path, (float)path.Weight);

//            //                        // save distance.
//            //                        if (path.Vertex == target)
//            //                        { // the target is already found, no use of queuing neigbours that have a higher weight.
//            //                            if (targetWeight > path.Weight)
//            //                            { // set the weight.
//            //                                targetWeight = path.Weight;
//            //                            }
//            //                        }
//            //                    }
//            //                }
//            //            }
//            //        }
//            //    }
//            //}
//            return null;
//        }
//    }
//}