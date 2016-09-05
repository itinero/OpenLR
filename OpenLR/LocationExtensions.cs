// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using Itinero;
using Itinero.Algorithms;
using Itinero.Data.Network;
using Itinero.LocalGeo;
using OpenLR.Referenced;
using OpenLR.Referenced.Codecs;
using OpenLR.Referenced.Locations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenLR
{
    /// <summary>
    /// Extension methods for locations.
    /// </summary>
    public static class LocationExtensions
    {
        /// <summary>
        /// Converts the referenced line location to a list of sorted coordinates.
        /// </summary>
        /// <returns></returns>
        public static List<Coordinate> GetCoordinates(this ReferencedLine route, Coder coder, float offsetRatio,
            out int offsetEdgeIdx, out Coordinate offsetLocation, out float offsetLength, out float offsetEdgeLength, out float edgeLength)
        {
            if (route == null) { throw new ArgumentNullException("route"); }
            if (route.Edges == null || route.Edges.Length == 0) { throw new ArgumentOutOfRangeException("route", "Route has no edges."); }
            if (route.Vertices == null || route.Vertices.Length == 0) { throw new ArgumentOutOfRangeException("route", "Route has no vertices."); }
            if (route.Vertices.Length != route.Edges.Length + 1) { throw new ArgumentOutOfRangeException("route", "Route is invalid: there should be n vertices and n-1 edges."); }

            // calculate the total length first.
            var totalLength = route.GetCoordinates(coder.Router.Db).Length();

            // calculate the lenght at the offst.
            offsetLength = (float)(totalLength * offsetRatio);
            offsetEdgeLength = -1;
            offsetEdgeIdx = -1;
            edgeLength = -1;

            // loop over all coordinates and collect offsetLocation and offsetEdgeIdx.
            float currentOffsetLength = 0;
            float currentEdgeLength = 0;
            var coordinates = new List<Coordinate>();
            coordinates.Add(coder.Router.Db.Network.GetVertex(route.Vertices[0]));
            for (int edgeIdx = 0; edgeIdx < route.Edges.Length; edgeIdx++)
            {
                currentEdgeLength = 0;
                var edge = coder.Router.Db.Network.GetEdge(route.Edges[edgeIdx]);
                var edgeShapes = edge.Shape;
                if (edgeShapes != null)
                { // there are intermediate coordinates.
                    foreach(var shape in edgeShapes)
                    {
                        coordinates.Add(shape);
                        currentEdgeLength = currentEdgeLength + Coordinate.DistanceEstimateInMeter(coordinates[coordinates.Count - 2],
                            coordinates[coordinates.Count - 1]);
                    }
                }
                coordinates.Add(coder.Router.Db.Network.GetVertex(route.Vertices[edgeIdx + 1]));
                currentEdgeLength = currentEdgeLength + Coordinate.DistanceEstimateInMeter(coordinates[coordinates.Count - 2],
                    coordinates[coordinates.Count - 1]);

                // add current edge length to current offset.
                if ((currentOffsetLength + currentEdgeLength) >= offsetLength &&
                    edgeLength < 0)
                { // it's this edge that has the valuable info.
                    offsetEdgeIdx = edgeIdx;
                    offsetEdgeLength = offsetLength - currentOffsetLength;
                    edgeLength = currentEdgeLength;
                }
                currentOffsetLength = currentOffsetLength + currentEdgeLength;
            }

            // choose the last edge.
            if (edgeLength < 0)
            { // it's this edge that has the valuable info.
                offsetEdgeIdx = route.Edges.Length - 1;
                offsetEdgeLength = offsetLength - currentOffsetLength;
                edgeLength = currentEdgeLength;
            }

            // calculate actual offset position.
            offsetLocation = coordinates.GetPositionLocation(offsetRatio);
            return coordinates;
        }

        /// <summary>
        /// Returns the location of the given position using the given coordinates as the polyline.
        /// </summary>
        /// <param name="coordinates">The polyline coordinates.</param>
        /// <param name="position">The position parameter [0-1].</param>
        /// <returns></returns>
        public static Coordinate GetPositionLocation(this List<Coordinate> coordinates, float position)
        {
            if (coordinates == null || coordinates.Count == 0) { throw new ArgumentOutOfRangeException("coordinates", "List of coordinates cannot be empty!"); }
            if (position < 0 || position > 1) { throw new ArgumentOutOfRangeException("position", "Position has to be in range [0-1]."); }

            if (coordinates.Count == 1)
            { // only one point, location is always the point itself.
                return coordinates[0];
            }

            var lengthAtPosition = coordinates.Length() * position;
            var lengthAtPrevious = 0.0f;
            var previous = coordinates[0];
            for (int idx = 1; idx < coordinates.Count; idx++)
            {
                var current = coordinates[idx];
                var segmentLength = Coordinate.DistanceEstimateInMeter(current, previous);
                if (lengthAtPrevious + segmentLength > lengthAtPosition)
                { // the point is in this segment.
                    var localPosition = (lengthAtPosition - lengthAtPrevious) / segmentLength;
                    return new Coordinate()
                    {
                        Latitude = previous.Latitude + (current.Latitude - previous.Latitude) * localPosition,
                        Longitude = previous.Longitude + (current.Longitude - previous.Longitude) * localPosition,
                    };
                }
                lengthAtPrevious = lengthAtPrevious + segmentLength;
                previous = current;
            }
            return coordinates[coordinates.Count - 1];
        }

        /// <summary>
        /// Gets the positive offset location.
        /// </summary>
        public static Coordinate GetPositiveOffsetLocation(this ReferencedLine referencedLine, RouterDb routerDb)
        {
            var coordinates = referencedLine.GetCoordinates(routerDb);

            return coordinates.GetPositionLocation(referencedLine.PositiveOffsetPercentage / 100.0f);
        }

        /// <summary>
        /// Gets the negative offset location.
        /// </summary>
        public static Coordinate GetNegativeOffsetLocation(this ReferencedLine referencedLine, RouterDb routerDb)
        {
            var coordinates = referencedLine.GetCoordinates(routerDb);

            return coordinates.GetPositionLocation(1f - (referencedLine.NegativeOffsetPercentage / 100.0f));
        }

        /// <summary>
        /// Converts the referenced line location to features.
        /// </summary>
        public static List<Coordinate> GetCoordinates(this ReferencedLine referencedLine, RouterDb routerDb)
        {
            return referencedLine.GetCoordinates(routerDb, 0, referencedLine.Vertices.Length);
        }

        /// <summary>
        /// Converts the referenced line location to features.
        /// </summary>
        public static List<Coordinate> GetCoordinates(this ReferencedLine referencedLine, RouterDb routerDb, int start, int count)
        {
            var coordinates = new List<Coordinate>();
            if (count <= 0)
            {
                return coordinates;
            }
            //coordinates.Add(routerDb.Network.GetVertex(referencedLine.Vertices[start]));
            for (var i = start; i < start + count - 1; i++)
            {
                var shape = routerDb.Network.GetShape(routerDb.Network.GetEdge(referencedLine.Edges[i]));
                if (shape != null)
                {
                    if (coordinates.Count > 0)
                    {
                        coordinates.RemoveAt(coordinates.Count - 1);
                    }
                    if (referencedLine.Edges[i] > 0)
                    {
                        for (var j = 0; j < shape.Count; j++)
                        {
                            coordinates.Add(shape[j]);
                        }
                    }
                    else
                    {
                        for (var j = shape.Count - 1; j >= 0; j--)
                        {
                            coordinates.Add(shape[j]);
                        }
                    }
                }
                //coordinates.Add(routerDb.Network.GetVertex(referencedLine.Vertices[i + 1]));
            }
            return coordinates;
        }

        /// <summary>
        /// Validates if the location is connected.
        /// </summary>
        /// <returns></returns>
        public static void ValidateConnected(this ReferencedLine line, Coder coder)
        {
            var profile = coder.Profile;

            var edges = line.Edges;
            var vertices = line.Vertices;

            // 1: Is the path connected?
            // 2: Is the path traversable?
            for (int edgeIdx = 0; edgeIdx < edges.Length; edgeIdx++)
            {
                var from = vertices[edgeIdx];
                var to = vertices[edgeIdx + 1];

                // find edge.
                var found = false;
                RoutingEdge foundEdge = null;
                foreach (var edge in coder.Router.Db.Network.GetEdges(from))
                {
                    if (edge.To == to &&
                        edge.IdDirected() == edges[edgeIdx])
                    { // edge was found, is valid.
                        found = true;
                        foundEdge = edge;
                        break;
                    }
                }
                if (!found)
                { // edge is not found, path not connected.
                    throw new ArgumentOutOfRangeException(string.Format("Edge {0} cannot be found between vertex {1} and {2}. The given path is not connected.",
                        edges[edgeIdx].ToInvariantString(), from, to));
                }

                // check whether the edge can traversed.
                var factor = profile.Profile.Factor(coder.Router.Db.EdgeProfiles.Get(foundEdge.Data.Profile));
                if (factor.Value == 0)
                { // oeps, cannot be traversed.
                    throw new ArgumentOutOfRangeException(string.Format("Edge at index {0} cannot be traversed by vehicle {1}.", edgeIdx, profile.Profile.Name));
                }

                // check whether the edge can be traversed in the correct direction.
                var canMoveForward = (factor.Direction == 0) || 
                    (factor.Direction == 1 && !foundEdge.DataInverted) ||
                    (factor.Direction == 2 && foundEdge.DataInverted);
                if (!canMoveForward)
                { // path cannot be traversed in this direction.
                    throw new ArgumentOutOfRangeException(string.Format("Edge at index {0} cannot be traversed by vehicle {1} in the direction given.", edgeIdx,
                        profile.Profile.Name));
                }
            }
        }

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

        /// <summary>
        /// Adjusts this location to use valid LR-points.
        /// </summary>
        public static void AdjustToValidPoints(this ReferencedLine line, Coder coder)
        {
            if (line.Vertices.Length <= 1) { throw new ArgumentException("Cannot adjust a line location with only one vertex."); }

            var vertex1Valid = coder.IsVertexValid(line.Vertices[0]);
            var vertex2Valid = coder.IsVertexValid(line.Vertices[line.Vertices.Length - 1]);
            if (vertex1Valid && vertex2Valid)
            { // already valid.
                return;
            }
            if (line.Vertices.Length > 2) { return; } // line was already adjusted.

            var vertex1 = line.Vertices[0];
            var vertex2 = line.Vertices[1];

            if (!coder.IsOnShortestPath(line.Vertices[0], line.Vertices[line.Vertices.Length - 1],
                vertex1, vertex2))
            { // impossible to expand edge.
                return;
            }

            // make sure the original sequence is still there on the shortest path.
            ReferencedLine validCopy = null;
            var backwardExcludeSet = line.GetVerticesSet();
            while (true)
            {
                // search backward.
                var workingCopy = line.Clone() as ReferencedLine;
                if (!workingCopy.TryAdjustToValidPointBackwards(coder, vertex1, vertex2, backwardExcludeSet))
                { // no more options exist, impossible to expand edge, just keep the edge itself.
                    return;
                }

                if (!vertex2Valid)
                { // search forward.
                    var forwardExcludeSet = workingCopy.GetVerticesSet();
                    do
                    {
                        var forwardWorkingCopy = workingCopy.Clone() as ReferencedLine;
                        if (!forwardWorkingCopy.TryAdjustToValidPointForwards(coder, vertex1, vertex2, forwardExcludeSet))
                        { // no more forward options for the current backward.
                            break;
                        }

                        // check valid.
                        if (coder.IsOnShortestPath(forwardWorkingCopy.Vertices[0], forwardWorkingCopy.Vertices[forwardWorkingCopy.Vertices.Length - 1],
                            vertex1, vertex2))
                        { // current location is valid.
                            validCopy = forwardWorkingCopy;
                            break;
                        }

                        // not valid here, exclude current forward.
                        forwardExcludeSet.Add(forwardWorkingCopy.Vertices[forwardWorkingCopy.Vertices.Length - 1]);
                    } while (true);
                }
                else
                { // check valid.
                    if (coder.IsOnShortestPath(workingCopy.Vertices[0], workingCopy.Vertices[workingCopy.Vertices.Length - 1],
                        vertex1, vertex2))
                    { // current location is valid.
                        validCopy = workingCopy;
                        break;
                    }
                }

                if (validCopy != null)
                { // current location is valid.
                    break;
                }

                if (vertex1Valid)
                { // vertex1 was already valid, no reason to continue searching.
                    return;
                }

                // exclude current backward and continue.
                backwardExcludeSet.Add(workingCopy.Vertices[0]);
            }

            // copy from working copy.
            line.Edges = validCopy.Edges;
            line.Vertices = validCopy.Vertices;
            line.NegativeOffsetPercentage = validCopy.NegativeOffsetPercentage;
            line.PositiveOffsetPercentage = validCopy.PositiveOffsetPercentage;
        }

        /// <summary>
        /// Tries to adjust this location forwards to a valid point.
        /// </summary>
        /// <returns></returns>
        public static bool TryAdjustToValidPointForwards(this ReferencedLine line, Coder coder, long vertex1, long vertex2,
            HashSet<uint> exclude)
        {
            var length = (float)line.Length(coder.Router.Db);
            var negativeOffsetLength = (line.NegativeOffsetPercentage / 100) * length;

            exclude = new HashSet<uint>(exclude);
            foreach (var vertex in line.Vertices)
            {
                exclude.Add(vertex);
            }

            if (!coder.IsVertexValid(line.Vertices[line.Vertices.Length - 1]))
            { // from is not valid, try to find a valid point.
                var vertexCount = line.Vertices.Length;
                var pathToValid = coder.FindValidVertexFor(line.Vertices[vertexCount - 1], -line.Edges[
                    line.Edges.Length - 1], line.Vertices[vertexCount - 2], exclude, true);

                // build edges list.
                if (pathToValid != null)
                { // no path found, just leave things as is.
                    var shortestRoute = coder.FindShortestPath(line.Vertices[vertexCount - 2], pathToValid.Vertex, true);
                    while (shortestRoute != null && !shortestRoute.HasVertex(line.Vertices[vertexCount - 1]))
                    { // the vertex that should be on this shortest route, isn't anymore.
                        // exclude the current target vertex, 
                        exclude.Add(pathToValid.Vertex);
                        // calulate a new path-to-valid.
                        pathToValid = coder.FindValidVertexFor(line.Vertices[vertexCount - 1], -line.Edges[
                            line.Edges.Length - 1], line.Vertices[vertexCount - 2], exclude, true);
                        if (pathToValid == null)
                        { // a new path was not found.
                            break;
                        }
                        shortestRoute = coder.FindShortestPath(line.Vertices[vertexCount - 2], pathToValid.Vertex, true);
                    }
                    if (pathToValid != null)
                    { // no path found, just leave things as is.
                        var pathToValidAsList = pathToValid.ToList();
                        var newVertices = new List<uint>();
                        var newEdges = new List<long>();
                        for (int idx = 0; idx < pathToValidAsList.Count; idx++)
                        { // loop over edges.
                            newVertices.Add(pathToValidAsList[idx].Vertex);
                            if (idx > 0)
                            {
                                newEdges.Add(pathToValidAsList[idx].Edge);
                            }
                        }

                        // create new location.
                        var edgesArray = new long[newEdges.Count + line.Edges.Length];
                        line.Edges.CopyTo(0, edgesArray, 0, line.Edges.Length);
                        newEdges.CopyTo(0, edgesArray, line.Edges.Length, newEdges.Count);
                        var vertexArray = new uint[newVertices.Count - 1 + line.Vertices.Length];
                        line.Vertices.CopyTo(0, vertexArray, 0, line.Vertices.Length);
                        newVertices.CopyTo(1, vertexArray, line.Vertices.Length, newVertices.Count - 1);

                        line.Edges = edgesArray;
                        line.Vertices = vertexArray;

                        // adjust offset length.
                        var newLength = (float)line.Length(coder.Router.Db);
                        negativeOffsetLength = negativeOffsetLength + (newLength - length);
                        length = newLength;
                    }
                    else
                    { // no valid path was found.
                        return false;
                    }
                }
                else
                { // no valid path was found.
                    return false;
                }
            }

            // update offset percentage
            line.NegativeOffsetPercentage = (float)((negativeOffsetLength / length) * 100.0);

            return true;
        }

        /// <summary>
        /// Tries to adjust this location backwards to a valid point.
        /// </summary>
        /// <returns></returns>
        public static bool TryAdjustToValidPointBackwards(this ReferencedLine line, Coder coder, uint vertex1, uint vertex2,
            HashSet<uint> exclude)
        {
            var length = line.Length(coder.Router.Db);
            var positiveOffsetLength = (line.PositiveOffsetPercentage / 100) * length;

            exclude = new HashSet<uint>(exclude);
            foreach (var vertex in line.Vertices)
            {
                exclude.Add(vertex);
            }

            if (!coder.IsVertexValid(line.Vertices[0]))
            { // from is not valid, try to find a valid point.
                var pathToValid = coder.FindValidVertexFor(line.Vertices[0], line.Edges[0], line.Vertices[1],
                    exclude, false);

                // build edges list.
                if (pathToValid != null)
                { // path found check if on shortest route.
                    var shortestRoute = coder.FindShortestPath(line.Vertices[1], pathToValid.Vertex, false);
                    while (shortestRoute != null && !shortestRoute.HasVertex(line.Vertices[0]))
                    { // the vertex that should be on this shortest route, isn't anymore.
                        // exclude the current target vertex, 
                        exclude.Add(pathToValid.Vertex);
                        // calulate a new path-to-valid.
                        pathToValid = coder.FindValidVertexFor(line.Vertices[0], line.Edges[0], line.Vertices[1],
                            exclude, false);
                        if (pathToValid == null)
                        { // a new path was not found.
                            break;
                        }
                        shortestRoute = coder.FindShortestPath(line.Vertices[1], pathToValid.Vertex, false);
                    }
                    if (pathToValid != null)
                    { // no path found, just leave things as is.
                        var pathToValidAsList = pathToValid.ToList();
                        var newVertices = new List<uint>();
                        var newEdges = new List<long>();
                        for (int idx = 0; idx < pathToValidAsList.Count; idx++)
                        { // loop over edges.
                            newVertices.Add(pathToValidAsList[idx].Vertex);
                            if (idx > 0)
                            {
                                newEdges.Add(-pathToValidAsList[idx].Edge); // need the reverse edges.
                            }
                        }
                        newEdges.Reverse();
                        newVertices.Reverse();

                        // create new location.
                        var edgesArray = new long[newEdges.Count + line.Edges.Length];
                        newEdges.CopyTo(0, edgesArray, 0, newEdges.Count);
                        line.Edges.CopyTo(0, edgesArray, newEdges.Count, line.Edges.Length);
                        var vertexArray = new uint[newVertices.Count - 1 + line.Vertices.Length];
                        newVertices.CopyTo(0, vertexArray, 0, newVertices.Count - 1);
                        line.Vertices.CopyTo(0, vertexArray, newVertices.Count - 1, line.Vertices.Length);

                        line.Edges = edgesArray;
                        line.Vertices = vertexArray;

                        // adjust offset length.
                        var newLength = (float)line.Length(coder.Router.Db);
                        positiveOffsetLength = positiveOffsetLength + (newLength - length);
                        length = newLength;
                    }
                    else
                    { // no valid path was found.
                        return false;
                    }
                }
                else
                { // no valid path was found.
                    return false;
                }
            }

            // update offset percentage.
            line.PositiveOffsetPercentage = (float)((positiveOffsetLength / length) * 100.0);

            return true;
        }

        /// <summary>
        /// Gets all vertices in one hashset.
        /// </summary>
        /// <returns></returns>
        public static HashSet<uint> GetVerticesSet(this ReferencedLine line)
        {
            var set = new HashSet<uint>();
            if (line.Vertices == null)
            { // empty set is ok.
                return set;
            }
            for (var i = 0; i < line.Vertices.Length; i++)
            {
                set.Add(line.Vertices[i]);
            }
            return set;
        }

        /// <summary>
        /// Adjusts this location by inserting intermediate LR-points if needed.
        /// </summary>
        /// 
        public static void AdjustToValidDistance(this ReferencedLine line, Coder coder, List<int> points, int start = 0)
        {
            // get start/end vertex.
            var vertexIdx1 = points[start];
            var vertexIdx2 = points[start + 1];
            var count = vertexIdx2 - vertexIdx1 + 1;

            // calculate length to begin with.
            var coordinates = line.GetCoordinates(coder.Router.Db, vertexIdx1, count);
            var length = coordinates.Length();
            if (length > 15000)
            { // too long.
                // find the best intermediate point.
                var intermediatePoints = new SortedDictionary<double, int>();
                for (int idx = vertexIdx1 + 1; idx < vertexIdx1 + count - 2; idx++)
                {
                    var score = 0.0;
                    if (coder.IsVertexValid(line.Vertices[idx]))
                    { // a valid vertex is obviously a better choice!
                        score = score + 4096;
                    }

                    // the length is good when close to 15000 but not over.
                    var lengthBefore = line.GetCoordinates(coder.Router.Db, vertexIdx1, idx - vertexIdx1 + 1).Length();
                    if (lengthBefore < 15000)
                    { // not over!
                        score = score + (1024 * (lengthBefore / 15000));
                    }
                    var lengthAfter = line.GetCoordinates(coder.Router.Db, idx, count - idx).Length();
                    if (lengthAfter < 15000)
                    { // not over!
                        score = score + (1024 * (lengthAfter / 15000));
                    }

                    // add to sorted dictionary.
                    intermediatePoints[8192 - score] = idx;
                }

                // select the best point and insert it in between.
                var bestPoint = intermediatePoints.First().Value;
                points.Insert(start + 1, bestPoint);

                // test the two distances.
                line.AdjustToValidDistance(coder, points, start + 1);
                line.AdjustToValidDistance(coder, points, start);
            }
        }



        /// <summary>
        /// Builds a location referenced point for the vertex at the given start-index.
        /// </summary>
        /// <returns></returns>
        public static Model.LocationReferencePoint BuildLocationReferencePoint(this ReferencedLine referencedLocation, Coder coder, int start, int end)
        {
            Model.FormOfWay fow;
            Model.FunctionalRoadClass frc;

            // get all coordinates along the sequence starting at 'start' and ending at 'end'.
            var coordinates = referencedLocation.GetCoordinates(coder.Router.Db, start, end - start + 1);

            // create location reference point.
            var locationReferencePoint = new Model.LocationReferencePoint();
            locationReferencePoint.Coordinate = coder.Router.Db.Network.GetVertex(referencedLocation.Vertices[start]).ToCoordinate();
            var edgeProfile = coder.Router.Db.EdgeProfiles.Get(coder.Router.Db.Network.GetEdge(referencedLocation.Edges[start]).Data.Profile);
            if (!coder.Profile.Extract(edgeProfile, out frc, out fow))
            {
                throw new ReferencedEncodingException(referencedLocation,
                    "Could not find frc and/or fow for the given tags.");
            }
            locationReferencePoint.FormOfWay = fow;
            locationReferencePoint.FuntionalRoadClass = frc;
            locationReferencePoint.Bearing = (int)BearingEncoder.EncodeBearing(coordinates);
            locationReferencePoint.DistanceToNext = (int)coordinates.Length();
            Model.FunctionalRoadClass? lowest = null;
            for (var edge = start; edge < end; edge++)
            {
                edgeProfile = coder.Router.Db.EdgeProfiles.Get(coder.Router.Db.Network.GetEdge(referencedLocation.Edges[edge]).Data.Profile);
                if (!coder.Profile.Extract(edgeProfile, out frc, out fow))
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
        public static Model.LocationReferencePoint BuildLocationReferencePointLast(this ReferencedLine referencedLocation, Coder coder, int before)
        {
            Model.FormOfWay fow;
            Model.FunctionalRoadClass frc;

            var end = referencedLocation.Vertices.Length - 1;

            // get all coordinates along the sequence starting at 'before' and ending at 'end'.
            var coordinates = referencedLocation.GetCoordinates(coder.Router.Db, before, end - before + 1);

            // create location reference point.
            var locationReferencedPoint = new Model.LocationReferencePoint();
            locationReferencedPoint.Coordinate = coder.Router.Db.Network.GetVertex(referencedLocation.Vertices[end]).ToCoordinate();
            var edgeProfile = coder.Router.Db.EdgeProfiles.Get(coder.Router.Db.Network.GetEdge(referencedLocation.Edges[end - 1]).Data.Profile);
            if (!coder.Profile.Extract(edgeProfile, out frc, out fow))
            {
                throw new ReferencedEncodingException(referencedLocation,
                    "Could not find frc and/or fow for the given tags.");
            }
            locationReferencedPoint.FormOfWay = fow;
            locationReferencedPoint.FuntionalRoadClass = frc;
            locationReferencedPoint.Bearing = (int)BearingEncoder.EncodeBearing(coordinates, true);

            return locationReferencedPoint;
        }
    }
}