﻿// using System;
// using System.Collections.Generic;
//
// using Itinero.Profiles;
// using Itinero.Routes.Paths;
// using Itinero.Snapping;
// using OpenLR.Model;
// using OpenLR.Referenced;
// using OpenLR.Referenced.Codecs;
// using OpenLR.Referenced.Locations;
//
// namespace OpenLR
// {
//     /// <summary>
//     /// Contains extension methods related to the coder.
//     /// </summary>
//     public static class CoderExtensions
//     {
//         /// <summary>
//         /// Returns true if the given vertex is a valid candidate to use as a location reference point.
//         /// </summary>
//         public static bool IsVertexValid(this Coder coder, uint vertex)
//         {
//             var profile = coder.Settings;
//             var getFactor = coder.Router.GetDefaultGetFactor(profile.Profile);
//             var edges = coder.Router.Db.Network.GetEdges(vertex);
//             var db = coder.Router.Db;
//             var restrictionsFunc = db.GetGetRestrictions(profile.Profile, null);
//
//
//             // go over each arc and count the traversable arcs.
//             var traversCount = 0;
//             foreach (var edge in edges)
//             {
//                 var factor = getFactor(edge.Data.Profile);
//                 if (factor.Value != 0)
//                 {
//                     traversCount++;
//                 }
//             }
//
//             if (traversCount == 1)
//             {
//                 // This is a dead end, and thus always a valid reference point
//                 return true;
//             }
//
//             if (traversCount == 2)
//             {
//                 // This is probably just a helper point to indicate the road shape
//                 // Normally, this should _not_ be a valid point
//
//
//                 // There are exceptions however!
//
//                 // If this vertex happens to be a bollard, gate or some other roadblock, the vertex just happens to connect two dead ends
//                 // Then, the vertex is valid! That is what we are checking below 
//                 var restrictions = restrictionsFunc.Invoke(vertex);
//                 foreach (var restriction in restrictions)
//                 {
//                     if (restriction.Length == 1)
//                     {
//                         // This is a simple restriction
//                         return true;
//                         // Other cases are probably turn restrictions and do not imply validity of this point
//                     }
//                 }
//
//
//                 return false;
//             }
//
//             if (traversCount >= 4)
//             {
//                 // Always a valid refence point
//                 return true;
//             }
//
//             // At this point, traversCount == 3
//
//             // special cases are possible here, but we need more info:
//             // how many roads go in this vertex? How many roads go out? How many are bidirectional?
//
//             var incoming = new List<Tuple<long, IEnumerable<(string key, string value)>, uint>>();
//             var outgoing = new List<Tuple<long, IEnumerable<(string key, string value)>, uint>>();
//             var bidirectional = new List<Tuple<long, IEnumerable<(string key, string value)>, uint>>();
//             foreach (var edge in edges)
//             {
//                 var edgeProfile = coder.Router.Db.EdgeProfiles.Get(edge.Data.Profile);
//                 var factor = getFactor(edge.Data.Profile);
//
//
//                 if (factor.Value == 0) continue;
//
//
//                 if (factor.Direction == 0)
//                 {
//                     // bidirectional, can be used as incoming.
//                     bidirectional.Add(new Tuple<long, IEnumerable<(string key, string value)>, uint>(edge.From, edgeProfile, edge.Id));
//                 }
//                 else if ((factor.Direction == 2 && !edge.DataInverted) ||
//                          (factor.Direction == 1 && edge.DataInverted))
//                 {
//                     // oneway is forward but arc is backward, arc is incoming.
//                     // oneway is backward and arc is forward, arc is incoming.
//                     incoming.Add(new Tuple<long, IEnumerable<(string key, string value)>, uint>(edge.From, edgeProfile, edge.Id));
//                 }
//                 else if ((factor.Direction == 2 && edge.DataInverted) ||
//                          (factor.Direction == 1 && !edge.DataInverted))
//                 {
//                     // oneway is forward and arc is forward, arc is outgoing.
//                     // oneway is backward and arc is backward, arc is outgoing.
//                     outgoing.Add(new Tuple<long, IEnumerable<(string key, string value)>, uint>(edge.From, edgeProfile, edge.Id));
//                 }
//             }
//
//             if (bidirectional.Count != 1 || incoming.Count != 1 || outgoing.Count != 1)
//             {
//                 // It's NOT a special case:
//                 // There are (for example): two outgoing vertices, meaning that this is a junction which can be referenced
//                 return true;
//             }
//
//             // all special cases are found below here: there is exactly one incoming, one outgoing and one bidirectional arc
//             // In other words, this _could_ be simple a change in representation (a bidirectional street where a part of it is represented as two roads)
//
//             // get incoming's frc and fow.
//             FormOfWay incomingFow, outgoingFow;
//             FunctionalRoadClass incomingFrc, outgoingFrc, bidirectionalFrc;
//
//             if (!profile.Extract(incoming[0].Item2, out incomingFrc, out incomingFow))
//             {
//                 // We can not extract more information: assume invalid
//                 return false;
//             }
//
//
//             if (incomingFow == FormOfWay.Roundabout)
//             {
//                 // is this a roundabout, always a valid reference point.
//                 return true;
//             }
//
//             if (profile.Extract(outgoing[0].Item2, out outgoingFrc, out outgoingFow))
//             {
//                 if (outgoingFow == FormOfWay.Roundabout)
//                 {
//                     // is this a roundabout, always valid.
//                     return true;
//                 }
//
//                 if (incomingFrc != outgoingFrc)
//                 {
//                     // is there a difference in frc.
//                     return true;
//                 }
//
//                 if (profile.Extract(bidirectional[0].Item2, out bidirectionalFrc, out _))
//                 {
//                     if (incomingFrc != bidirectionalFrc)
//                     {
//                         // is there a difference in frc.
//                         return true;
//                     }
//                 }
//             }
//
//             // at this stage we have:
//             // - two oneways, in opposite direction
//             // - one bidirectional
//             // - all same frc.
//
//             // the only thing left to check is if the oneway edges go in the same general direction or not.
//             // compare bearings but only if distance is large enough.
//             var incomingShape = coder.Router.Db.Network.GetShape(coder.Router.Db.Network.GetEdge(incoming[0].Item3));
//             var outgoingShape = coder.Router.Db.Network.GetShape(coder.Router.Db.Network.GetEdge(outgoing[0].Item3));
//
//             if (incomingShape.Length() < 25 &&
//                 outgoingShape.Length() < 25)
//             {
//                 // edges are too short to compare bearing in a way meaningful for determining this.
//                 // assume not valid.
//                 return false;
//             }
//
//             var incomingBearing = BearingEncoder.EncodeBearing(incomingShape);
//             var outgoingBearing = BearingEncoder.EncodeBearing(outgoingShape);
//
//             // ReSharper disable once ConvertIfStatementToReturnStatement
//             if (Extensions.AngleSmallestDifference(incomingBearing, outgoingBearing) > 30)
//             {
//                 // edges are clearly not going in the same direction -> this is a valid junction.
//                 return true;
//             }
//
//             /*
//              * This vertex could happen to be restricted (e.g. a roadblock could be standing here)
//              * In that case, the vertex would happen to be three dead ends coming together.
//              * However, then we'd logically have three bidirectional arcs, so that case could never reach here
//              * (Or the data might be wrong)
//              */
//
//             // We checked all special cases that we know of and that should be valid - we assume the point is invalid;
//             // Found a special case that is forgotten here? Open an issue
//             return false;
//         }
//
//         /// <summary>
//         /// Finds the shortest path between the given from->to.
//         /// </summary>
//         public static Path FindShortestPath(this Coder coder, uint from, uint to, bool searchForward)
//         {
//             var fromRouterPoint = coder.Router.Db.Network.CreateRouterPointForVertex(from);
//             var toRouterPoint = coder.Router.Db.Network.CreateRouterPointForVertex(to);
//
//             return FindShortestPath(coder, fromRouterPoint, toRouterPoint, searchForward);
//         }
//
//         /// <summary>
//         /// Finds the shortest path between the given from->to.
//         /// </summary>
//         public static Path FindShortestPath(this Coder coder, SnapPoint fromRouterPoint, SnapPoint toRouterPoint, bool searchForward)
//         {
//             var weightHandler = coder.Router.GetDefaultWeightHandler(coder.Settings.Profile);
//             if (searchForward)
//             {
//                 var result = coder.Router.TryCalculateRaw(coder.Settings.Profile, weightHandler,
//                     fromRouterPoint, toRouterPoint, coder.Settings.RoutingSettings);
//                 if (result.IsError && coder.Settings.HasAggressiveSettings)
//                 {
//                     result = coder.Router.TryCalculateRaw(coder.Settings.Profile, weightHandler,
//                         fromRouterPoint, toRouterPoint, coder.Settings.AggressiveRoutingSettings);
//                 }
//
//                 if (result.IsError) return null;
//                 return result.Value;
//             }
//             else
//             {
//                 var result = coder.Router.TryCalculateRaw(coder.Settings.Profile, weightHandler,
//                     toRouterPoint, fromRouterPoint, coder.Settings.RoutingSettings);
//                 if (result.IsError && coder.Settings.HasAggressiveSettings)
//                 {
//                     result = coder.Router.TryCalculateRaw(coder.Settings.Profile, weightHandler,
//                         toRouterPoint, fromRouterPoint, coder.Settings.AggressiveRoutingSettings);
//                 }
//
//                 if (result.IsError) return null;
//                 return result.Value;
//             }
//         }
//
//         /// <summary>
//         /// Returns true if the sequence vertex1->vertex2 occurs on the shortest path between from and to.
//         /// </summary>
//         /// <returns></returns>
//         public static bool IsOnShortestPath(this Coder coder, uint from, uint to, uint vertex1, uint vertex2)
//         {
//             var path = coder.FindShortestPath(from, to, true).ToListAsVertices();
//             for (var i = 1; i < path.Count; i++)
//             {
//                 if (path[i - 1] == vertex1 &&
//                     path[i] == vertex2)
//                 {
//                     return true;
//                 }
//             }
//
//             return false;
//         }
//
//         /// <summary>
//         /// Returns true if the sequence vertex1->vertex2 occurs on the shortest path between from and to.
//         /// </summary>
//         /// <returns></returns>
//         public static bool IsOnShortestPath(this Coder coder, SnapPoint fromRouterPoint, SnapPoint toRouterPoint, uint vertex1, uint vertex2)
//         {
//             var path = coder.FindShortestPath(fromRouterPoint, toRouterPoint, true).ToListAsVertices();
//             for (var i = 1; i < path.Count; i++)
//             {
//                 if (path[i - 1] == vertex1 &&
//                     path[i] == vertex2)
//                 {
//                     return true;
//                 }
//             }
//
//             return false;
//         }
//
//         /// <summary>
//         /// Finds a valid vertex for the given vertex but does not search in the direction of the target neighbour.
//         /// </summary>
//         public static Path FindValidVertexFor(this Coder coder, uint vertex, long targetDirectedEdgeId,
//             uint targetVertex, HashSet<uint> excludeSet, bool searchForward)
//         {
//             var profile = coder.Settings.Profile;
//
//             // GIST: execute a dykstra search to find a vertex that is valid.
//             // this will return a vertex that is on the shortest path:
//             // foundVertex -> vertex -> targetNeighbour.
//
//             var targetEdge = Constants.NO_EDGE;
//             if (targetDirectedEdgeId > 0)
//             {
//                 targetEdge = (uint) (targetDirectedEdgeId - 1);
//             }
//             else
//             {
//                 targetEdge = (uint) ((-targetDirectedEdgeId) - 1);
//             }
//
//             // initialize settled set.
//             var settled = new HashSet<long>();
//             settled.Add(targetVertex);
//
//             // initialize heap.
//             var heap = new BinaryHeap<EdgePath<float>>(10);
//             heap.Push(new EdgePath<float>((uint) vertex), 0);
//
//             // find the path to the closest valid vertex.
//             EdgePath<float> pathTo = null;
//             var edgeEnumerator = coder.Router.Db.Network.GetEdgeEnumerator();
//             while (heap.Count > 0)
//             {
//                 // get next.
//                 var current = heap.Pop();
//                 if (settled.Contains(current.Vertex))
//                 {
//                     // don't consider vertices twice.
//                     continue;
//                 }
//
//                 settled.Add(current.Vertex);
//
//                 // limit search.
//                 if (settled.Count > coder.Settings.MaxSettles)
//                 {
//                     // not valid vertex found.
//                     return null;
//                 }
//
//                 // check if valid.
//                 if (current.Vertex != vertex &&
//                     coder.IsVertexValid(current.Vertex))
//                 {
//                     // ok! vertex is valid.
//                     pathTo = current;
//                 }
//                 else
//                 {
//                     // continue search.
//                     // add unsettled neighbours.
//                     edgeEnumerator.MoveTo(current.Vertex);
//                     foreach (var edge in edgeEnumerator)
//                     {
//                         if (!excludeSet.Contains(edge.To) &&
//                             !settled.Contains(edge.To) &&
//                             !(edge.Id == targetEdge))
//                         {
//                             // ok, new neighbour, and ok, not the edge and neighbour to ignore.
//                             var edgeProfile = coder.Router.Db.EdgeProfiles.Get(edge.Data.Profile);
//                             var factor = profile.Factor(edgeProfile);
//
//                             if (factor.Value > 0 && (factor.Direction == 0 ||
//                                                      (searchForward && (factor.Direction == 1) != edge.DataInverted) ||
//                                                      (!searchForward && (factor.Direction == 1) == edge.DataInverted)))
//                             {
//                                 // ok, we can traverse this edge and no oneway or oneway reversed.
//                                 var weight = current.Weight + factor.Value * edge.Data.Distance;
//                                 var path = new EdgePath<float>(edge.To, weight, edge.IdDirected(), current);
//                                 heap.Push(path, (float) path.Weight);
//                             }
//                         }
//                     }
//                 }
//             }
//
//             // ok, is there a path found.
//             if (pathTo == null)
//             {
//                 // oeps, probably something wrong with network-topology.
//                 // just take the default option.
//                 //throw new Exception(
//                 //    string.Format("Could not find a valid vertex for invalid vertex [{0}].", vertex));
//                 return null;
//             }
//
//             // add the path to the given location.
//             return pathTo;
//         }
//
//         /// <summary>
//         /// Builds a point along line location.
//         /// </summary>
//         public static ReferencedPointAlongLine BuildPointAlongLine(this Coder coder, Coordinate coordinate)
//         {
//             return coder.BuildPointAlongLine(coordinate, out _);
//         }
//
//         /// <summary>
//         /// Builds a point along line location.
//         /// </summary>
//         public static ReferencedPointAlongLine BuildPointAlongLine(this Coder coder, Coordinate coordinate,
//             out RouterPoint resolvedPoint)
//         {
//             return coder.BuildPointAlongLine(coordinate.Latitude, coordinate.Longitude, out resolvedPoint);
//         }
//
//         /// <summary>
//         /// Builds a point along line location.
//         /// </summary>
//         public static ReferencedPointAlongLine BuildPointAlongLine(this Coder coder, float latitude, float longitude)
//         {
//             return coder.BuildPointAlongLine(latitude, longitude, out _);
//         }
//
//         /// <summary>
//         /// Builds a point along line location.
//         /// </summary>
//         public static ReferencedPointAlongLine BuildPointAlongLine(this Coder coder, float latitude, float longitude,
//             out RouterPoint resolvedPoint)
//         {
//             var routerPoint = coder.Router.TryResolve(coder.Settings.Profile, latitude, longitude, coder.Settings.MaxResolveDistance);
//             if (routerPoint.IsError)
//             {
//                 throw new Exception(
//                     "Could not build point along line: Could not find an edge close to the given location.");
//             }
//
//             resolvedPoint = routerPoint.Value;
//             var locationOnNetwork = resolvedPoint.LocationOnNetwork(coder.Router.Db);
//
//             // get edge info.
//             var edge = coder.Router.Db.Network.GetEdge(routerPoint.Value.EdgeId);
//
//             // check direction.
//             var getFactor = coder.Router.GetDefaultGetFactor(coder.Settings.Profile);
//             var forward = true;
//             var factor = getFactor(edge.Data.Profile);
//             if (factor.Direction == 2)
//             {
//                 forward = false;
//             }
//
//             // build the location with one edge.
//             ReferencedPointAlongLine referencedPointAlongLine = null;
//             if (forward)
//             {
//                 referencedPointAlongLine = new ReferencedPointAlongLine()
//                 {
//                     Route = new ReferencedLine()
//                     {
//                         Edges = new long[] {edge.IdDirected()},
//                         Vertices = new uint[] {edge.From, edge.To},
//                         StartLocation = coder.Router.Db.CreateRouterPointForEdgeAndVertex(edge.IdDirected(), edge.From),
//                         EndLocation = coder.Router.Db.CreateRouterPointForEdgeAndVertex(edge.IdDirected(), edge.To)
//                     },
//                     Latitude = locationOnNetwork.Latitude,
//                     Longitude = locationOnNetwork.Longitude,
//                     Orientation = Orientation.NoOrientation
//                 };
//             }
//             else
//             {
//                 referencedPointAlongLine = new ReferencedPointAlongLine()
//                 {
//                     Route = new ReferencedLine()
//                     {
//                         Edges = new long[] {-edge.IdDirected()},
//                         Vertices = new uint[] {edge.To, edge.From},
//                         StartLocation = coder.Router.Db.CreateRouterPointForEdgeAndVertex(edge.IdDirected(), edge.To),
//                         EndLocation = coder.Router.Db.CreateRouterPointForEdgeAndVertex(edge.IdDirected(), edge.From)
//                     },
//                     Latitude = locationOnNetwork.Latitude,
//                     Longitude = locationOnNetwork.Longitude,
//                     Orientation = Orientation.NoOrientation
//                 };
//             }
//
//             // expand to valid location.
//             referencedPointAlongLine.Route.AdjustToValidPoints(coder);
//             referencedPointAlongLine.Route.StartLocation = coder.Router.Db.CreateRouterPointForEdgeAndVertex(
//                 referencedPointAlongLine.Route.Edges[0], referencedPointAlongLine.Route.Vertices[0]);
//             referencedPointAlongLine.Route.EndLocation = coder.Router.Db.CreateRouterPointForEdgeAndVertex(
//                 referencedPointAlongLine.Route.Edges[referencedPointAlongLine.Route.Edges.Length - 1],
//                 referencedPointAlongLine.Route.Vertices[referencedPointAlongLine.Route.Vertices.Length - 1]);
//
//             return referencedPointAlongLine;
//         }
//
//         /// <summary>
//         /// Encodes a set of coordinates as a point along line.
//         /// </summary>
//         public static string EncodeAsPointAlongLine(this Coder coder, float latitude, float longitude)
//         {
//             return coder.EncodeAsPointAlongLine(latitude, longitude, out _);
//         }
//
//         /// <summary>
//         /// Encodes a set of coordinates as a point along line.
//         /// </summary>
//         public static string EncodeAsPointAlongLine(this Coder coder, float latitude, float longitude,
//             out RouterPoint resolvedPoint)
//         {
//             return coder.Encode(coder.BuildPointAlongLine(latitude, longitude, out resolvedPoint));
//         }
//
//         /// <summary>
//         /// Builds the shortest path between the two coordinates as a referenced line.
//         /// </summary>
//         public static ReferencedLine BuildLine(this Coder coder, Coordinate coordinate1, Coordinate coordinate2)
//         {
//             return coder.BuildLine(coordinate1, coordinate2, out _);
//         }
//
//         /// <summary>
//         /// Builds a referenced line representing a single edge.
//         /// </summary>
//         public static ReferencedLine BuildEdge(this Coder coder, long directedEdgeId)
//         {
//             var edge = coder.Router.Db.Network.GetEdge(directedEdgeId);
//
//             return new ReferencedLine()
//             {
//                 Edges = new long[]
//                 {
//                     directedEdgeId
//                 },
//                 Vertices = new uint[]
//                 {
//                     edge.From,
//                     edge.To
//                 },
//                 NegativeOffsetPercentage = 0,
//                 PositiveOffsetPercentage = 0,
//                 StartLocation = coder.Router.Db.CreateRouterPointForEdgeAndVertex(directedEdgeId, edge.From),
//                 EndLocation = coder.Router.Db.CreateRouterPointForEdgeAndVertex(directedEdgeId, edge.To)
//             };
//         }
//
//         /// <summary>
//         /// Builds the shortest path between the two coordinates as a referenced line.
//         /// </summary>
//         public static ReferencedLine BuildLine(this Coder coder, Coordinate coordinate1, Coordinate coordinate2,
//             out Route route)
//         {
//             // calculate raw path.
//             var weightHandler = coder.Router.GetDefaultWeightHandler(coder.Settings.Profile);
//             var source = coder.Router.Resolve(coder.Settings.Profile, coordinate1, 100);
//             var target = coder.Router.Resolve(coder.Settings.Profile, coordinate2, 100);
//             var path = coder.Router.TryCalculateRaw(coder.Settings.Profile, weightHandler,
//                 source, target, coder.Settings.RoutingSettings);
//             if (path.IsError)
//             {
//                 throw new InvalidOperationException("No route found.");
//             }
//
//             var pathDistance = path.Value.Weight;
//
//             // build route.
//             route = coder.Router.BuildRoute(coder.Settings.Profile, weightHandler, source, target, path.Value).Value;
//
//             // build referenced line by building vertices and edge list.
//             var pathAsList = path.Value.ToList();
//             var edges = new List<long>();
//             var vertices = new List<uint>();
//             for (var i = 0; i < pathAsList.Count; i++)
//             {
//                 vertices.Add(pathAsList[i].Vertex);
//                 if (i > 0)
//                 {
//                     if (pathAsList[i].Edge != Constants.NO_EDGE &&
//                         pathAsList[i].Edge != -Constants.NO_EDGE)
//                     {
//                         edges.Add(pathAsList[i].Edge);
//                     }
//                     else
//                     {
//                         var edgeEnumerator = coder.Router.Db.Network.GeometricGraph.Graph.GetEdgeEnumerator();
//                         var edge = edgeEnumerator.FindBestEdge(weightHandler, vertices[vertices.Count - 2],
//                             vertices[vertices.Count - 1], out _);
//                         edges.Add(edge);
//                     }
//                 }
//             }
//
//             // make sure that 'first' and 'last' are real vertices.
//             var sourceOffset = 0f;
//             if (vertices[0] == Constants.NO_VERTEX)
//             {
//                 var edge = coder.Router.Db.Network.GetEdge(edges[0]);
//                 if (edge.From == vertices[1])
//                 {
//                     sourceOffset = Coordinate.DistanceEstimateInMeter(coordinate1,
//                         coder.Router.Db.Network.GetVertex(edge.To));
//                     vertices[0] = edge.To;
//                 }
//                 else if (edge.To == vertices[1])
//                 {
//                     sourceOffset = Coordinate.DistanceEstimateInMeter(coordinate1,
//                         coder.Router.Db.Network.GetVertex(edge.From));
//                     vertices[0] = edge.From;
//                 }
//                 else
//                 {
//                     throw new Exception("First edge does not match first vertex.");
//                 }
//             }
//
//             var targetOffset = 0f;
//             if (vertices[vertices.Count - 1] == Constants.NO_VERTEX)
//             {
//                 var edge = coder.Router.Db.Network.GetEdge(edges[edges.Count - 1]);
//                 if (edge.From == vertices[vertices.Count - 2])
//                 {
//                     targetOffset = Coordinate.DistanceEstimateInMeter(coordinate2,
//                         coder.Router.Db.Network.GetVertex(edge.To));
//                     vertices[vertices.Count - 1] = edge.To;
//                 }
//                 else if (edge.To == vertices[vertices.Count - 2])
//                 {
//                     targetOffset = Coordinate.DistanceEstimateInMeter(coordinate2,
//                         coder.Router.Db.Network.GetVertex(edge.From));
//                     vertices[vertices.Count - 1] = edge.From;
//                 }
//                 else
//                 {
//                     throw new Exception("Last edge does not match last vertex.");
//                 }
//             }
//
//             var totalDistance = pathDistance + sourceOffset + targetOffset;
//
//             return new ReferencedLine()
//             {
//                 Edges = edges.ToArray(),
//                 Vertices = vertices.ToArray(),
//                 NegativeOffsetPercentage = 100.0f * (targetOffset / totalDistance),
//                 PositiveOffsetPercentage = 100.0f * (sourceOffset / totalDistance),
//                 StartLocation = coder.Router.Db.CreateRouterPointForEdgeAndVertex(edges[0], vertices[0]),
//                 EndLocation =
//                     coder.Router.Db.CreateRouterPointForEdgeAndVertex(edges[edges.Count - 1],
//                         vertices[vertices.Count - 1])
//             };
//         }
//
//         /// <summary>
//         /// Builds the shortest path along all the given coordinates as a referenced line.
//         /// </summary>
//         public static ReferencedLine BuildLine(this Coder coder, Coordinate[] coordinates)
//         {
//             var edgesTotal = new List<long>();
//             var verticesTotal = new List<uint>();
//             float pathDistance = 0;
//             var sourceOffset = 0f;
//             var targetOffset = 0f;
//             var totalDistance = 0f;
//             for (int w = 1; w < coordinates.Length; w++)
//             {
//                 var coordinate1 = coordinates[w - 1];
//                 var coordinate2 = coordinates[w];
//
//                 // calculate raw path.
//                 var weightHandler = coder.Router.GetDefaultWeightHandler(coder.Settings.Profile);
//                 var source = coder.Router.Resolve(coder.Settings.Profile, coordinate1, 100);
//                 var target = coder.Router.Resolve(coder.Settings.Profile, coordinate2, 100);
//                 var path = coder.Router.TryCalculateRaw(coder.Settings.Profile, weightHandler,
//                     source, target, coder.Settings.RoutingSettings);
//                 if (path.IsError)
//                 {
//                     throw new InvalidOperationException("No route found.");
//                 }
//
//                 // build route.
//                 var route = coder.Router.BuildRoute(coder.Settings.Profile, weightHandler, source, target, path.Value)
//                     .Value;
//                 //pathDistance += path.Value.Weight;
//                 pathDistance += route.TotalDistance;
//
//                 // build referenced line by building vertices and edge list.
//                 var pathAsList = path.Value.ToList();
//                 var edges = new List<long>();
//                 var vertices = new List<uint>();
//                 for (var i = 0; i < pathAsList.Count; i++)
//                 {
//                     vertices.Add(pathAsList[i].Vertex);
//                     if (i > 0)
//                     {
//                         if (pathAsList[i].Edge != Constants.NO_EDGE &&
//                             pathAsList[i].Edge != -Constants.NO_EDGE)
//                         {
//                             edges.Add(pathAsList[i].Edge);
//                         }
//                         else
//                         {
//                             var edgeEnumerator = coder.Router.Db.Network.GeometricGraph.Graph.GetEdgeEnumerator();
//                             var edge = edgeEnumerator.FindBestEdge(weightHandler, vertices[vertices.Count - 2],
//                                 vertices[vertices.Count - 1], out _);
//                             edges.Add(edge);
//                         }
//                     }
//                 }
//
//                 if (vertices[0] == Constants.NO_VERTEX)
//                 {
//                     var edge = coder.Router.Db.Network.GetEdge(edges[0]);
//                     if (edge.From == vertices[1])
//                     {
//                         vertices[0] = edge.To;
//                     }
//                     else if (edge.To == vertices[1])
//                     {
//                         vertices[0] = edge.From;
//                     }
//                     else
//                     {
//                         throw new Exception("First edge does not match first vertex.");
//                     }
//                 }
//
//                 if (vertices[vertices.Count - 1] == Constants.NO_VERTEX)
//                 {
//                     var edge = coder.Router.Db.Network.GetEdge(edges[edges.Count - 1]);
//                     if (edge.From == vertices[vertices.Count - 2])
//                     {
//                         vertices[vertices.Count - 1] = edge.To;
//                     }
//                     else if (edge.To == vertices[vertices.Count - 2])
//                     {
//                         vertices[vertices.Count - 1] = edge.From;
//                     }
//                     else
//                     {
//                         throw new Exception("Last edge does not match last vertex.");
//                     }
//                 }
//
//                 if (edgesTotal.Count > 0)
//                     edges.RemoveAt(0);
//                 edgesTotal.AddRange(edges);
//
//                 if (verticesTotal.Count > 0)
//                 {
//                     vertices.RemoveAt(0);
//                     vertices.RemoveAt(0);
//                 }
//
//                 verticesTotal.AddRange(vertices);
//             }
//
//             // ReSharper disable once InconsistentNaming
//             var edge_ = coder.Router.Db.Network.GetEdge(edgesTotal[0]);
//             if (edge_.From == verticesTotal[1])
//             {
//                 sourceOffset = Coordinate.DistanceEstimateInMeter(coordinates[0],
//                     coder.Router.Db.Network.GetVertex(edge_.To));
//             }
//             else if (edge_.To == verticesTotal[1])
//             {
//                 sourceOffset = Coordinate.DistanceEstimateInMeter(coordinates[0],
//                     coder.Router.Db.Network.GetVertex(edge_.From));
//             }
//             else
//             {
//                 throw new Exception("First edge does not match first vertex.");
//             }
//
//             edge_ = coder.Router.Db.Network.GetEdge(edgesTotal[edgesTotal.Count - 1]);
//             if (edge_.From == verticesTotal[verticesTotal.Count - 2])
//             {
//                 targetOffset = Coordinate.DistanceEstimateInMeter(coordinates[coordinates.Length - 1],
//                     coder.Router.Db.Network.GetVertex(edge_.To));
//             }
//             else if (edge_.To == verticesTotal[verticesTotal.Count - 2])
//             {
//                 targetOffset = Coordinate.DistanceEstimateInMeter(coordinates[coordinates.Length - 1],
//                     coder.Router.Db.Network.GetVertex(edge_.From));
//             }
//             else
//             {
//                 throw new Exception("Last edge does not match last vertex.");
//             }
//
//             totalDistance = pathDistance + sourceOffset + targetOffset;
//
//             return new ReferencedLine()
//             {
//                 Edges = edgesTotal.ToArray(),
//                 Vertices = verticesTotal.ToArray(),
//                 NegativeOffsetPercentage = 100.0f * (targetOffset / totalDistance),
//                 PositiveOffsetPercentage = 100.0f * (sourceOffset / totalDistance),
//                 StartLocation = coder.Router.Db.CreateRouterPointForEdgeAndVertex(edgesTotal[0], verticesTotal[0]),
//                 EndLocation = coder.Router.Db.CreateRouterPointForEdgeAndVertex(edgesTotal[edgesTotal.Count - 1],
//                     verticesTotal[verticesTotal.Count - 1])
//             };
//         }
//     }
// }
