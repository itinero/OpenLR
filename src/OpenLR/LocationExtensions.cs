using System;
using System.Collections.Generic;
using System.Linq;
using Itinero;
using Itinero.Geo;
using OpenLR.Exceptions;
using OpenLR.Referenced;
using OpenLR.Referenced.Codecs;
using OpenLR.Referenced.Locations;

namespace OpenLR;

/// <summary>
/// Extension methods for locations.
/// </summary>
public static class LocationExtensions
{
    // /// <summary>
    // /// Converts the referenced line location to a list of sorted coordinates.
    // /// </summary>
    // /// <returns></returns>
    // public static IEnumerable<(double longitude, double latitude, float? e)> GetCoordinates(this ReferencedLine referencedLine, Coder coder, float offsetRatio,
    //     out int offsetEdgeIdx, out (double longitude, double latitude, float? e) offsetLocation, out float offsetLength, out float offsetEdgeLength, out float edgeLength)
    // {
    //     // calculate the total length first.
    //     var totalLength = referencedLine.GetCoordinates(coder.Router.Db).Length();
    //
    //     // calculate the lenght at the offset.
    //     offsetLength = (float)(totalLength * offsetRatio);
    //     offsetEdgeLength = -1;
    //     offsetEdgeIdx = -1;
    //     edgeLength = -1;
    //
    //     // loop over all coordinates and collect offsetLocation and offsetEdgeIdx.
    //     float currentOffsetLength = 0;
    //     float currentEdgeLength = 0;
    //     var coordinates = new List<(double longitude, double latitude, float? e)>();
    //     for (var i = 0; i < referencedLine.Edges.Length; i++)
    //     {
    //         List<(double longitude, double latitude, float? e)>? shape = null;
    //         currentEdgeLength = 0;
    //         if (i == 0 && referencedLine.Vertices[0] == Itinero.Constants.NO_VERTEX)
    //         { // shape from startlocation -> vertex1.
    //             if (referencedLine.Edges.Length == 1)
    //             { // only 1 edge, shape from startLocation -> endLocation.
    //                 shape = referencedLine.StartLocation.ShapePointsTo(coder.Router.Db, referencedLine.EndLocation);
    //                 shape.Insert(0, referencedLine.StartLocation.LocationOnNetwork(coder.Router.Db));
    //                 shape.Add(referencedLine.EndLocation.LocationOnNetwork(coder.Router.Db));
    //             }
    //             else
    //             { // just get shape to first vertex.
    //                 shape = referencedLine.StartLocation.ShapePointsTo(coder.Router.Db, referencedLine.Vertices[1]);
    //                 shape.Insert(0, referencedLine.StartLocation.LocationOnNetwork(coder.Router.Db));
    //                 shape.Add(coder.Router.Db.Network.GetVertex(referencedLine.Vertices[1]));
    //             }
    //         }
    //         else if (i == referencedLine.Edges.Length - 1 && referencedLine.Vertices[referencedLine.Vertices.Length - 1] == Itinero.Constants.NO_VERTEX)
    //         { // shape from second last vertex -> endlocation.
    //             shape = referencedLine.EndLocation.ShapePointsTo(coder.Router.Db, referencedLine.Vertices[referencedLine.Vertices.Length - 2]);
    //             shape.Reverse();
    //             shape.Insert(0, coder.Router.Db.Network.GetVertex(referencedLine.Vertices[referencedLine.Vertices.Length - 2]));
    //             shape.Add(referencedLine.EndLocation.LocationOnNetwork(coder.Router.Db));
    //         }
    //         else
    //         { // regular 2 vertices and edge.
    //             shape = coder.Router.Db.Network.GetShape(coder.Router.Db.Network.GetEdge(referencedLine.Edges[i]));
    //             if (referencedLine.Edges[i] < 0)
    //             {
    //                 shape.Reverse();
    //             }
    //         }
    //         if (shape != null)
    //         {
    //             currentEdgeLength = currentEdgeLength + shape.Length();
    //             if (coordinates.Count > 0)
    //             {
    //                 coordinates.RemoveAt(coordinates.Count - 1);
    //             }
    //             for (var j = 0; j < shape.Count; j++)
    //             {
    //                 coordinates.Add(shape[j]);
    //             }
    //         }
    //         
    //         // add current edge length to current offset.
    //         if ((currentOffsetLength + currentEdgeLength) >= offsetLength &&
    //             edgeLength < 0)
    //         { // it's this edge that has the valuable info.
    //             offsetEdgeIdx = i;
    //             offsetEdgeLength = offsetLength - currentOffsetLength;
    //             edgeLength = currentEdgeLength;
    //         }
    //         currentOffsetLength = currentOffsetLength + currentEdgeLength;
    //     }
    //
    //     // choose the last edge.
    //     if (edgeLength < 0)
    //     { // it's this edge that has the valuable info.
    //         offsetEdgeIdx = referencedLine.Edges.Length - 1;
    //         offsetEdgeLength = offsetLength - currentOffsetLength;
    //         edgeLength = currentEdgeLength;
    //     }
    //
    //     // calculate actual offset position.
    //     offsetLocation = coordinates.GetPositionLocation(offsetRatio);
    //     return coordinates;
    // }

    // /// <summary>
    // /// Validates if the location is connected.
    // /// </summary>
    // /// <returns></returns>
    // public static void ValidateConnected(this ReferencedLine line, Coder coder)
    // {
    //     var profile = coder.Settings;
    //     var getFactor = coder.Router.GetDefaultGetFactor(profile.Profile);
    //
    //     var edges = line.Edges;
    //     var vertices = line.Vertices;
    //
    //     // 1: Is the path connected?
    //     // 2: Is the path traversable?
    //     for (int edgeIdx = 0; edgeIdx < edges.Length; edgeIdx++)
    //     {
    //         var from = vertices[edgeIdx];
    //         var to = vertices[edgeIdx + 1];
    //
    //         // find edge.
    //         var found = false;
    //         RoutingEdge foundEdge = null;
    //         foreach (var edge in coder.Router.Db.Network.GetEdges(from))
    //         {
    //             if (edge.To == to &&
    //                 edge.IdDirected() == edges[edgeIdx])
    //             { // edge was found, is valid.
    //                 found = true;
    //                 foundEdge = edge;
    //                 break;
    //             }
    //         }
    //         if (!found)
    //         { // edge is not found, path not connected.
    //             throw new ArgumentOutOfRangeException(string.Format("Edge {0} cannot be found between vertex {1} and {2}. The given path is not connected.",
    //                 edges[edgeIdx].ToInvariantString(), from, to));
    //         }
    //
    //         // check whether the edge can traversed.
    //         var factor = getFactor(foundEdge.Data.Profile);
    //         if (factor.Value == 0)
    //         { // oeps, cannot be traversed.
    //             throw new ArgumentOutOfRangeException(string.Format("Edge at index {0} cannot be traversed by vehicle {1}.", edgeIdx, profile.Profile.Name));
    //         }
    //
    //         // check whether the edge can be traversed in the correct direction.
    //         var canMoveForward = (factor.Direction == 0) || 
    //                              (factor.Direction == 1 && !foundEdge.DataInverted) ||
    //                              (factor.Direction == 2 && foundEdge.DataInverted);
    //         if (!canMoveForward)
    //         { // path cannot be traversed in this direction.
    //             throw new ArgumentOutOfRangeException(string.Format("Edge at index {0} cannot be traversed by vehicle {1} in the direction given.", edgeIdx,
    //                 profile.Profile.Name));
    //         }
    //     }
    // }

    /// <summary>
    /// Validates the offsets.
    /// </summary>
    public static void ValidateOffsets(this ReferencedLine line)
    {

    }

    /// <summary>
    /// Validates the location for encoding in binary format.
    /// </summary>
    public static void ValidateBinary(this ReferencedLine line)
    {

    }

    // /// <summary>
    // /// Gets a proper router point for a vertex in the referenced line for the vertex at the given index. 
    // /// </summary>
    // /// <param name="line">The line location.</param>
    // /// <param name="i">The index of the vertex.</param>
    // /// <returns>A router point using an edge in the line location.</returns>
    // public static RouterPoint RouterPointForVertex(this ReferencedLine line, int i)
    // {
    //     if (i >= line.Vertices.Length) throw new IndexOutOfRangeException();
    //     
    //     if (i == line.Vertices.Length - 1)
    //     { // in the last vertex use the previous edge.
    //         var edge = line.Edges[i - 1];
    //         var directedEdge = new DirectedEdgeId(edge);
    //
    //         if (directedEdge.Forward)
    //         {
    //             return new RouterPoint(0, 0, directedEdge.EdgeId, ushort.MaxValue);
    //         }
    //         return new RouterPoint(0, 0, directedEdge.EdgeId, 0);
    //     }
    //     else
    //     { // use the next edge for the router point.
    //         var edge = line.Edges[i];
    //         var directedEdge = new DirectedEdgeId(edge);
    //
    //         if (directedEdge.Forward)
    //         {
    //             return new RouterPoint(0, 0, directedEdge.EdgeId, 0);
    //         }
    //
    //         return new RouterPoint(0, 0, directedEdge.EdgeId, ushort.MaxValue);
    //     }
    // }
    //
    // /// <summary>
    // /// Adjusts the given referenced line by expanding it to use valid LRP locations as start and end locations.
    // /// </summary>
    // /// <param name="line">The referenced line.</param>
    // /// <param name="coder">The coder.</param>
    // /// <returns>A new adjusted referenced line.</returns>
    // /// <exception cref="ArgumentException"></exception>
    // public static ReferencedLine AdjustToValidPoints(this ReferencedLine line, Coder coder)
    // {
    //     if (line.Vertices.Length <= 1) { throw new ArgumentException("Cannot adjust a line location with only one vertex."); }
    //
    //     var vertex1Valid = coder.IsVertexValid(line.Vertices[0]);
    //     var vertex2Valid = coder.IsVertexValid(line.Vertices[line.Vertices.Length - 1]);
    //     if (vertex1Valid && vertex2Valid)
    //     { // already valid.
    //         return;
    //     }
    //     if (line.Vertices.Length > 2) { return; } // line was already adjusted.
    //
    //     var vertex1 = line.Vertices[0];
    //     var vertex2 = line.Vertices[1];
    //
    //     if (!coder.IsOnShortestPath(line.RouterPointForVertex(0), line.RouterPointForVertex(line.Vertices.Length - 1),
    //             vertex1, vertex2))
    //     { // impossible to expand edge.
    //         return;
    //     }
    //
    //     // make sure the original sequence is still there on the shortest path.
    //     ReferencedLine validCopy = null;
    //     var backwardExcludeSet = line.GetVerticesSet();
    //     while (true)
    //     {
    //         // search backward.
    //         var workingCopy = line.Clone() as ReferencedLine;
    //         if (!workingCopy.TryAdjustToValidPointBackwards(coder, vertex1, vertex2, backwardExcludeSet))
    //         { // no more options exist, impossible to expand edge, just keep the edge itself.
    //             return;
    //         }
    //
    //         if (!vertex2Valid)
    //         { // search forward.
    //             var forwardExcludeSet = workingCopy.GetVerticesSet();
    //             do
    //             {
    //                 var forwardWorkingCopy = workingCopy.Clone() as ReferencedLine;
    //                 if (!forwardWorkingCopy.TryAdjustToValidPointForwards(coder, vertex1, vertex2, forwardExcludeSet))
    //                 { // no more forward options for the current backward.
    //                     break;
    //                 }
    //
    //                 // check valid.
    //                 if (coder.IsOnShortestPath(forwardWorkingCopy.Vertices[0], forwardWorkingCopy.Vertices[forwardWorkingCopy.Vertices.Length - 1],
    //                         vertex1, vertex2))
    //                 { // current location is valid.
    //                     validCopy = forwardWorkingCopy;
    //                     break;
    //                 }
    //
    //                 // not valid here, exclude current forward.
    //                 forwardExcludeSet.Add(forwardWorkingCopy.Vertices[forwardWorkingCopy.Vertices.Length - 1]);
    //             } while (true);
    //         }
    //         else
    //         { // check valid.
    //             if (coder.IsOnShortestPath(workingCopy.Vertices[0], workingCopy.Vertices[workingCopy.Vertices.Length - 1],
    //                     vertex1, vertex2))
    //             { // current location is valid.
    //                 validCopy = workingCopy;
    //                 break;
    //             }
    //         }
    //
    //         if (validCopy != null)
    //         { // current location is valid.
    //             break;
    //         }
    //
    //         if (vertex1Valid)
    //         { // vertex1 was already valid, no reason to continue searching.
    //             return;
    //         }
    //
    //         // exclude current backward and continue.
    //         backwardExcludeSet.Add(workingCopy.Vertices[0]);
    //     }
    //
    //     // copy from working copy.
    //     line.Edges = validCopy.Edges;
    //     line.Vertices = validCopy.Vertices;
    //     line.NegativeOffsetPercentage = validCopy.NegativeOffsetPercentage;
    //     line.PositiveOffsetPercentage = validCopy.PositiveOffsetPercentage;
    // }

    // /// <summary>
    // /// Tries to adjust this location forwards to a valid point.
    // /// </summary>
    // /// <returns></returns>
    // public static bool TryAdjustToValidPointForwards(this ReferencedLine line, Coder coder, long vertex1, long vertex2,
    //     HashSet<uint> exclude)
    // {
    //     var length = (float)line.Length(coder.Router.Db);
    //     var negativeOffsetLength = (line.NegativeOffsetPercentage / 100) * length;
    //     var positiveOffsetLength = (line.PositiveOffsetPercentage / 100) * length;
    //
    //     exclude = new HashSet<uint>(exclude);
    //     foreach (var vertex in line.Vertices)
    //     {
    //         exclude.Add(vertex);
    //     }
    //
    //     if (!coder.IsVertexValid(line.Vertices[line.Vertices.Length - 1]))
    //     { // from is not valid, try to find a valid point.
    //         var vertexCount = line.Vertices.Length;
    //         var pathToValid = coder.FindValidVertexFor(line.Vertices[vertexCount - 1], -line.Edges[
    //             line.Edges.Length - 1], line.Vertices[vertexCount - 2], exclude, true);
    //
    //         // build edges list.
    //         if (pathToValid != null)
    //         { // no path found, just leave things as is.
    //             var shortestRoute = coder.FindShortestPath(line.Vertices[vertexCount - 2], pathToValid.Vertex, true);
    //             while (shortestRoute != null && !shortestRoute.HasVertex(line.Vertices[vertexCount - 1]))
    //             { // the vertex that should be on this shortest route, isn't anymore.
    //                 // exclude the current target vertex, 
    //                 exclude.Add(pathToValid.Vertex);
    //                 // calulate a new path-to-valid.
    //                 pathToValid = coder.FindValidVertexFor(line.Vertices[vertexCount - 1], -line.Edges[
    //                     line.Edges.Length - 1], line.Vertices[vertexCount - 2], exclude, true);
    //                 if (pathToValid == null)
    //                 { // a new path was not found.
    //                     break;
    //                 }
    //                 shortestRoute = coder.FindShortestPath(line.Vertices[vertexCount - 2], pathToValid.Vertex, true);
    //             }
    //             if (pathToValid != null)
    //             { // no path found, just leave things as is.
    //                 var pathToValidAsList = pathToValid.ToList();
    //                 var newVertices = new List<uint>();
    //                 var newEdges = new List<long>();
    //                 for (int idx = 0; idx < pathToValidAsList.Count; idx++)
    //                 { // loop over edges.
    //                     newVertices.Add(pathToValidAsList[idx].Vertex);
    //                     if (idx > 0)
    //                     {
    //                         newEdges.Add(pathToValidAsList[idx].Edge);
    //                     }
    //                 }
    //
    //                 // create new location.
    //                 var edgesArray = new long[newEdges.Count + line.Edges.Length];
    //                 line.Edges.CopyTo(0, edgesArray, 0, line.Edges.Length);
    //                 newEdges.CopyTo(0, edgesArray, line.Edges.Length, newEdges.Count);
    //                 var vertexArray = new uint[newVertices.Count - 1 + line.Vertices.Length];
    //                 line.Vertices.CopyTo(0, vertexArray, 0, line.Vertices.Length);
    //                 newVertices.CopyTo(1, vertexArray, line.Vertices.Length, newVertices.Count - 1);
    //
    //                 line.Edges = edgesArray;
    //                 line.Vertices = vertexArray;
    //
    //                 // adjust offset length.
    //                 var newLength = (float)line.Length(coder.Router.Db);
    //                 negativeOffsetLength = negativeOffsetLength + (newLength - length);
    //                 length = newLength;
    //             }
    //             else
    //             { // no valid path was found.
    //                 return false;
    //             }
    //         }
    //         else
    //         { // no valid path was found.
    //             return false;
    //         }
    //     }
    //
    //     // update offset percentage
    //     line.NegativeOffsetPercentage = (float)((negativeOffsetLength / length) * 100.0);
    //     line.PositiveOffsetPercentage = (float)((positiveOffsetLength / length) * 100.0);
    //
    //     return true;
    // }

    // /// <summary>
    // /// Tries to adjust this location backwards to a valid point.
    // /// </summary>
    // /// <returns></returns>
    // public static bool TryAdjustToValidPointBackwards(this ReferencedLine line, Coder coder, uint vertex1, uint vertex2,
    //     HashSet<uint> exclude)
    // {
    //     var length = line.Length(coder.Router.Db);
    //     var positiveOffsetLength = (line.PositiveOffsetPercentage / 100) * length;
    //
    //     exclude = new HashSet<uint>(exclude);
    //     foreach (var vertex in line.Vertices)
    //     {
    //         exclude.Add(vertex);
    //     }
    //
    //     if (!coder.IsVertexValid(line.Vertices[0]))
    //     { // from is not valid, try to find a valid point.
    //         var pathToValid = coder.FindValidVertexFor(line.Vertices[0], line.Edges[0], line.Vertices[1],
    //             exclude, false);
    //
    //         // build edges list.
    //         if (pathToValid != null)
    //         { // path found check if on shortest route.
    //             var shortestRoute = coder.FindShortestPath(line.Vertices[1], pathToValid.Vertex, false);
    //             while (shortestRoute != null && !shortestRoute.HasVertex(line.Vertices[0]))
    //             { // the vertex that should be on this shortest route, isn't anymore.
    //                 // exclude the current target vertex, 
    //                 exclude.Add(pathToValid.Vertex);
    //                 // calulate a new path-to-valid.
    //                 pathToValid = coder.FindValidVertexFor(line.Vertices[0], line.Edges[0], line.Vertices[1],
    //                     exclude, false);
    //                 if (pathToValid == null)
    //                 { // a new path was not found.
    //                     break;
    //                 }
    //                 shortestRoute = coder.FindShortestPath(line.Vertices[1], pathToValid.Vertex, false);
    //             }
    //             if (pathToValid != null)
    //             { // no path found, just leave things as is.
    //                 var pathToValidAsList = pathToValid.ToList();
    //                 var newVertices = new List<uint>();
    //                 var newEdges = new List<long>();
    //                 for (int idx = 0; idx < pathToValidAsList.Count; idx++)
    //                 { // loop over edges.
    //                     newVertices.Add(pathToValidAsList[idx].Vertex);
    //                     if (idx > 0)
    //                     {
    //                         newEdges.Add(-pathToValidAsList[idx].Edge); // need the reverse edges.
    //                     }
    //                 }
    //                 newEdges.Reverse();
    //                 newVertices.Reverse();
    //
    //                 // create new location.
    //                 var edgesArray = new long[newEdges.Count + line.Edges.Length];
    //                 newEdges.CopyTo(0, edgesArray, 0, newEdges.Count);
    //                 line.Edges.CopyTo(0, edgesArray, newEdges.Count, line.Edges.Length);
    //                 var vertexArray = new uint[newVertices.Count - 1 + line.Vertices.Length];
    //                 newVertices.CopyTo(0, vertexArray, 0, newVertices.Count - 1);
    //                 line.Vertices.CopyTo(0, vertexArray, newVertices.Count - 1, line.Vertices.Length);
    //
    //                 line.Edges = edgesArray;
    //                 line.Vertices = vertexArray;
    //
    //                 // adjust offset length.
    //                 var newLength = (float)line.Length(coder.Router.Db);
    //                 positiveOffsetLength = positiveOffsetLength + (newLength - length);
    //                 length = newLength;
    //             }
    //             else
    //             { // no valid path was found.
    //                 return false;
    //             }
    //         }
    //         else
    //         { // no valid path was found.
    //             return false;
    //         }
    //     }
    //
    //     // update offset percentage.
    //     line.PositiveOffsetPercentage = (float)((positiveOffsetLength / length) * 100.0);
    //
    //     return true;
    // }

    // /// <summary>
    // /// Gets all vertices in one hashset.
    // /// </summary>
    // /// <returns></returns>
    // public static HashSet<uint> GetVerticesSet(this ReferencedLine line)
    // {
    //     var set = new HashSet<uint>();
    //     if (line.Vertices == null)
    //     { // empty set is ok.
    //         return set;
    //     }
    //     for (var i = 0; i < line.Vertices.Length; i++)
    //     {
    //         set.Add(line.Vertices[i]);
    //     }
    //     return set;
    // }
    //
    // /// <summary>
    // /// Adjusts this location by inserting intermediate LR-points if needed.
    // /// </summary>
    // /// 
    // public static void AdjustToValidDistance(this ReferencedLine line, Coder coder, List<int> points, int start = 0)
    // {
    //     // get start/end vertex.
    //     var vertexIdx1 = points[start];
    //     var vertexIdx2 = points[start + 1];
    //     var count = vertexIdx2 - vertexIdx1 + 1;
    //
    //     // calculate length to begin with.
    //     var coordinates = line.GetCoordinates(coder.Router.Db, vertexIdx1, count);
    //     var length = coordinates.Length();
    //     if (length > 15000)
    //     { // too long.
    //         // find the best intermediate point.
    //         var intermediatePoints = new SortedDictionary<double, int>();
    //         for (int idx = vertexIdx1 + 1; idx < vertexIdx1 + count - 2; idx++)
    //         {
    //             var score = 0.0;
    //             if (coder.IsVertexValid(line.Vertices[idx]))
    //             { // a valid vertex is obviously a better choice!
    //                 score = score + 4096;
    //             }
    //
    //             // the length is good when close to 15000 but not over.
    //             var lengthBefore = line.GetCoordinates(coder.Router.Db, vertexIdx1, idx - vertexIdx1 + 1).Length();
    //             if (lengthBefore < 15000)
    //             { // not over!
    //                 score = score + (1024 * (lengthBefore / 15000));
    //             }
    //             var lengthAfter = line.GetCoordinates(coder.Router.Db, idx, count - idx).Length();
    //             if (lengthAfter < 15000)
    //             { // not over!
    //                 score = score + (1024 * (lengthAfter / 15000));
    //             }
    //
    //             // add to sorted dictionary.
    //             intermediatePoints[8192 - score] = idx;
    //         }
    //
    //         // select the best point and insert it in between.
    //         var bestPoint = intermediatePoints.First().Value;
    //         points.Insert(start + 1, bestPoint);
    //
    //         // test the two distances.
    //         line.AdjustToValidDistance(coder, points, start + 1);
    //         line.AdjustToValidDistance(coder, points, start);
    //     }
    // }



    // /// <summary>
    // /// Builds a location referenced point for the vertex at the given start-index.
    // /// </summary>
    // /// <returns></returns>
    // public static Model.LocationReferencePoint BuildLocationReferencePoint(this ReferencedLine referencedLocation, Coder coder, int start, int end)
    // {
    //     Model.FormOfWay fow;
    //     Model.FunctionalRoadClass frc;
    //
    //     // get all coordinates along the sequence starting at 'start' and ending at 'end'.
    //     var coordinates = referencedLocation.GetCoordinates(coder.Router.Db, start, end - start + 1);
    //
    //     // create location reference point.
    //     var locationReferencePoint = new Model.LocationReferencePoint();
    //     locationReferencePoint.Coordinate = coder.Router.Db.Network.GetVertex(referencedLocation.Vertices[start]).ToCoordinate();
    //     var edgeProfile = coder.Router.Db.EdgeProfiles.Get(coder.Router.Db.Network.GetEdge(referencedLocation.Edges[start]).Data.Profile);
    //     if (!coder.Settings.Extract(edgeProfile, out frc, out fow))
    //     {
    //         throw new ReferencedEncodingException(referencedLocation,
    //             "Could not find frc and/or fow for the given tags.");
    //     }
    //     locationReferencePoint.FormOfWay = fow;
    //     locationReferencePoint.FuntionalRoadClass = frc;
    //     locationReferencePoint.Bearing = (int)BearingEncoder.EncodeBearing(coordinates);
    //     locationReferencePoint.DistanceToNext = (int)coordinates.Length();
    //     Model.FunctionalRoadClass? lowest = null;
    //     for (var edge = start; edge < end; edge++)
    //     {
    //         edgeProfile = coder.Router.Db.EdgeProfiles.Get(coder.Router.Db.Network.GetEdge(referencedLocation.Edges[edge]).Data.Profile);
    //         if (!coder.Settings.Extract(edgeProfile, out frc, out fow))
    //         {
    //             throw new ReferencedEncodingException(referencedLocation,
    //                 "Could not find frc and/or fow for the given tags.");
    //         }
    //
    //         if (!lowest.HasValue ||
    //             frc > lowest)
    //         {
    //             lowest = frc;
    //         }
    //     }
    //     locationReferencePoint.LowestFunctionalRoadClassToNext = lowest;
    //
    //     return locationReferencePoint;
    // }
    //
    // /// <summary>
    // /// Builds a location referenced point for the last vertex.
    // /// </summary>
    // /// <returns></returns>
    // public static Model.LocationReferencePoint BuildLocationReferencePointLast(this ReferencedLine referencedLocation, Coder coder, int before)
    // {
    //     Model.FormOfWay fow;
    //     Model.FunctionalRoadClass frc;
    //
    //     var end = referencedLocation.Vertices.Length - 1;
    //
    //     // get all coordinates along the sequence starting at 'before' and ending at 'end'.
    //     var coordinates = referencedLocation.GetCoordinates(coder.Router.Db, before, end - before + 1);
    //
    //     // create location reference point.
    //     var locationReferencedPoint = new Model.LocationReferencePoint();
    //     locationReferencedPoint.Coordinate = coder.Router.Db.Network.GetVertex(referencedLocation.Vertices[end]).ToCoordinate();
    //     var edgeProfile = coder.Router.Db.EdgeProfiles.Get(coder.Router.Db.Network.GetEdge(referencedLocation.Edges[end - 1]).Data.Profile);
    //     if (!coder.Settings.Extract(edgeProfile, out frc, out fow))
    //     {
    //         throw new ReferencedEncodingException(referencedLocation,
    //             "Could not find frc and/or fow for the given tags.");
    //     }
    //     locationReferencedPoint.FormOfWay = fow;
    //     locationReferencedPoint.FuntionalRoadClass = frc;
    //     locationReferencedPoint.Bearing = (int)BearingEncoder.EncodeBearing(coordinates, true);
    //
    //     return locationReferencedPoint;
    // }
    //
    // /// <summary>
    // /// Gets the edge closest to the location in the point along line.
    // /// </summary>
    // public static long GetLocationEdge(this ReferencedPointAlongLine pointAlongLine, RouterDb routerDb)
    // {
    //     var offsetInMeter = float.MaxValue;
    //     return pointAlongLine.GetLocationEdge(routerDb, out offsetInMeter);
    // }

    // /// <summary>
    // /// Gets the edge closest to the location in the point along line.
    // /// </summary>
    // public static long GetLocationEdge(this ReferencedPointAlongLine pointAlongLine, RouterDb routerDb, out float offsetInMeter)
    // {
    //     return pointAlongLine.Route.ProjectOn(routerDb, pointAlongLine.Latitude, pointAlongLine.Longitude, out offsetInMeter);
    // }
    //
    // /// <summary>
    // /// Projects the given coordinates on the referenced line and returns the edge.
    // /// </summary>
    // public static long ProjectOn(this ReferencedLine line, RouterDb routerDb, float latitude, float longitude)
    // {
    //     var offsetInMeter = float.MaxValue;
    //     return line.ProjectOn(routerDb, latitude, longitude, out offsetInMeter);
    // }
    //
    // /// <summary>
    // /// Projects the given coordinates on the referenced line and returns the edge.
    // /// </summary>
    // public static long ProjectOn(this ReferencedLine line, RouterDb routerDb, float latitude, float longitude, out float offsetInMeter)
    // {
    //     long edge = Itinero.Constants.NO_EDGE;
    //     var bestDistance = float.MaxValue;
    //     offsetInMeter = float.MaxValue;
    //
    //     for(var j = 0; j < line.Edges.Length; j++)
    //     {
    //         var shape = routerDb.Network.GetShape(routerDb.Network.GetEdge(line.Edges[j]));
    //         if (line.Edges[j] < 0)
    //         {
    //             shape.Reverse();
    //         }
    //             
    //         float projectedLatitude;
    //         float projectedLongitude;
    //         int projectedShapeIndex;
    //         float distanceToProjected;
    //         float totalLength;
    //         float projectedOffsetInMeter;
    //         LinePointPosition position;
    //         if (!shape.ProjectOn(latitude, longitude, out projectedLatitude, out projectedLongitude,
    //                 out projectedOffsetInMeter, out projectedShapeIndex, out distanceToProjected, out totalLength, out position))
    //         {
    //             // try to find the closest point.
    //             distanceToProjected = float.MaxValue;
    //             totalLength = 0;
    //             for (var i = 0; i < shape.Count; i++)
    //             {
    //                 var distance = Itinero.LocalGeo.Coordinate.DistanceEstimateInMeter(shape[i].Latitude, shape[i].Longitude,
    //                     latitude, longitude);
    //                 if (i > 0)
    //                 {
    //                     totalLength += Itinero.LocalGeo.Coordinate.DistanceEstimateInMeter(shape[i - 1].Latitude, shape[i - 1].Longitude,
    //                         shape[i].Latitude, shape[i].Longitude);
    //                 }
    //                 if (distance < distanceToProjected)
    //                 {
    //                     projectedOffsetInMeter = totalLength;
    //                     distanceToProjected = distance;
    //                     projectedShapeIndex = i;
    //                     position = LinePointPosition.On;
    //                     projectedLatitude = shape[i].Latitude;
    //                     projectedLongitude = shape[i].Longitude;
    //                 }
    //             }
    //         }
    //
    //         if (distanceToProjected < bestDistance)
    //         {
    //             edge = line.Edges[j];
    //             offsetInMeter = projectedOffsetInMeter;
    //             bestDistance = distanceToProjected;
    //         }
    //     }
    //     return edge;
    // }
    //
    // /// <summary>
    // /// Builds a path from the given referenced line start and the first referenced point and ending at the last.
    // /// </summary>
    // public static EdgePath<float> BuildPathFromLine(this ReferencedLine referencedLine, RouterDb routerDb, out RouterPoint source, out RouterPoint target)
    // {
    //     float postiveOffsetInMeters, negativeOffsetInMeters;
    //     return referencedLine.BuildPathFromLine(routerDb, out source, out postiveOffsetInMeters, out target, out negativeOffsetInMeters);
    // }
    //
    // /// <summary>
    // /// Builds a path from the given referenced line start and the first referenced point and ending at the last.
    // /// </summary>
    // public static EdgePath<float> BuildPathFromLine(this ReferencedLine referencedLine, RouterDb routerDb, out float postiveOffsetInMeters, out float negativeOffsetInMeters)
    // {
    //     RouterPoint source;
    //     RouterPoint target;
    //     return referencedLine.BuildPathFromLine(routerDb, out source, out postiveOffsetInMeters, out target, out negativeOffsetInMeters);
    // }
    //
    // /// <summary>
    // /// Builds a path from the given referenced line start and the first referenced point and ending at the last.
    // /// </summary>
    // public static EdgePath<float> BuildPathFromLine(this ReferencedLine referencedLine, RouterDb routerDb, out RouterPoint source, out float postiveOffsetInMeters, 
    //     out RouterPoint target, out float negativeOffsetInMeters)
    // {
    //     source = referencedLine.GetPositiveOffsetRouterPoint(routerDb);
    //     target = referencedLine.GetNegativeOffsetRouterPoint(routerDb);
    //     negativeOffsetInMeters = 0;
    //     postiveOffsetInMeters = 0;
    //
    //     var started = false;
    //     var path = new EdgePath<float>();
    //     for(var e = 0; e < referencedLine.Edges.Length; e++)
    //     {
    //         var directedEdgeId = referencedLine.Edges[e];
    //         var edge = routerDb.Network.GetEdge(directedEdgeId);
    //
    //         var to = edge.To;
    //         if (directedEdgeId < 0)
    //         {
    //             to = edge.From;
    //         }
    //
    //         if (!started)
    //         {
    //             if (edge.Id != source.EdgeId)
    //             {
    //                 continue;
    //             }
    //             negativeOffsetInMeters = (source.Offset / (float)ushort.MaxValue) * edge.Data.Distance;
    //             if (directedEdgeId < 0)
    //             {
    //                 negativeOffsetInMeters = edge.Data.Distance - negativeOffsetInMeters;
    //             }
    //             path = new EdgePath<float>(to, edge.Data.Distance - negativeOffsetInMeters, directedEdgeId, path);
    //             started = true;
    //             continue;
    //         }
    //
    //         if (edge.Id  == target.EdgeId)
    //         {
    //             postiveOffsetInMeters = (target.Offset / (float)ushort.MaxValue) * edge.Data.Distance;
    //             if (directedEdgeId > 0)
    //             {
    //                 postiveOffsetInMeters = edge.Data.Distance - postiveOffsetInMeters;
    //             }
    //             path = new EdgePath<float>(Constants.NO_VERTEX, path.Weight + edge.Data.Distance - postiveOffsetInMeters, directedEdgeId, path);
    //             break;
    //         }
    //         else
    //         {
    //             path = new EdgePath<float>(to, path.Weight + edge.Data.Distance, directedEdgeId, path);
    //         }
    //     }
    //         
    //     return path;
    // }
    //
    // /// <summary>
    // /// Gets the positive offset routerpoint.
    // /// </summary>
    // public static RouterPoint GetPositiveOffsetRouterPoint(this ReferencedLine referencedLine, RouterDb routerDb)
    // {
    //     return referencedLine.GetOffsetRouterPoint(routerDb, referencedLine.PositiveOffsetPercentage);
    // }
    //
    // /// <summary>
    // /// Gets the negative offset routerpoint.
    // /// </summary>
    // public static RouterPoint GetNegativeOffsetRouterPoint(this ReferencedLine referencedLine, RouterDb routerDb)
    // {
    //     return referencedLine.GetOffsetRouterPoint(routerDb, 100 - referencedLine.NegativeOffsetPercentage);
    // }
    //     
    // /// <summary>
    // /// Gets a positive offset routerpoint.
    // /// </summary>
    // public static RouterPoint GetOffsetRouterPoint(this ReferencedLine referencedLine, RouterDb routerDb, float positiveOffsetPercentage)
    // {
    //     var length = referencedLine.Length(routerDb);
    //     var lengthOffset = (positiveOffsetPercentage / 100.0f) * length;
    //
    //     var totalLength = 0f;
    //     for (var e = 0; e < referencedLine.Edges.Length; e++)
    //     {
    //         var directedEdgeId = referencedLine.Edges[e];
    //         var edge = routerDb.Network.GetEdge(directedEdgeId);
    //         var shape = routerDb.Network.GetShape(edge);
    //         if (directedEdgeId < 0)
    //         {
    //             shape.Reverse();
    //         }
    //
    //         var shapeLength = shape.Length();
    //         if (lengthOffset < shapeLength + totalLength)
    //         { // offset is in this edge.
    //             var relativeOffset = (lengthOffset - totalLength) / shapeLength;
    //             var offset = (ushort)(ushort.MaxValue * relativeOffset);
    //             if (directedEdgeId < 0)
    //             {
    //                 offset = (ushort)(ushort.MaxValue - offset);
    //             }
    //             var routerPoint = new RouterPoint(0, 0, edge.Id, offset);
    //             var location = routerPoint.LocationOnNetwork(routerDb);
    //             return new RouterPoint(location.Latitude, location.Longitude, edge.Id, offset);
    //         }
    //         totalLength += shapeLength;
    //     }
    //     return routerDb.CreateRouterPointForEdge(referencedLine.Edges[referencedLine.Edges.Length - 1], false);
    // }
}
