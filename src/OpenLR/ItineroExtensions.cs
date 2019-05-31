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

using Itinero.Graphs.Geometric;
using Itinero.LocalGeo;
using Itinero.Algorithms.Search.Hilbert;
using Itinero;
using OpenLR.Referenced.Codecs.Candidates;
using System.Collections.Generic;
using Itinero.Profiles;
using Itinero.Algorithms.Search;
using Itinero.Data.Network;
using System;

namespace OpenLR.Referenced
{
    /// <summary>
    /// Contains extension methods for itinero.
    /// </summary>
    public static class ItineroExtensions
    {
        /// <summary>
        /// Searches for an edge that has the exact start- and endpoints given within the given tolerance.
        /// </summary>
        public static uint SearchEdgeExact(this GeometricGraph graph, Coordinate location1, Coordinate location2, float tolerance)
        {
            var vertex1 = graph.SearchClosest(location1.Latitude, location1.Longitude, 0.01f, 0.01f);
            if (vertex1 == Constants.NO_VERTEX)
            {
                return Constants.NO_EDGE;
            }
            var vertex1Location = graph.GetVertex(vertex1);
            if (Coordinate.DistanceEstimateInMeter(location1, vertex1Location) > tolerance)
            {
                return Constants.NO_EDGE;
            }
            var edgeEnumerator = graph.GetEdgeEnumerator();
            var best = float.MaxValue;
            var bestEdge = Constants.NO_EDGE;
            while(edgeEnumerator.MoveNext())
            {
                var vertex2Location = graph.GetVertex(edgeEnumerator.To);
                var dist = Coordinate.DistanceEstimateInMeter(location2, vertex2Location);
                if (dist < tolerance &&
                    dist < best)
                {
                    best = dist;
                    bestEdge = edgeEnumerator.Id;
                }
            }
            return bestEdge;
        }

        /// <summary>
        /// Projects on to a given shape and returns data about projection point.
        /// </summary>
        public static bool ProjectOn(this IEnumerable<Coordinate> shape, float latitude, float longitude,
            out float projectedLatitude, out float projectedLongitude, out float projectedDistanceFromFirst,
            out int projectedShapeIndex, out float distanceToProjected, out float totalLength, out LinePointPosition position)
        {
            distanceToProjected = float.MaxValue;
            projectedDistanceFromFirst = 0;
            projectedLatitude = float.MaxValue;
            projectedLongitude = float.MaxValue;
            projectedShapeIndex = -1;
            position = LinePointPosition.On;

            var previousShapeDistance = 0.0f;
            var previousShapeIndex = -1;
            var shapeEnumerator = shape.GetEnumerator();
            Coordinate? previous = null;
            var coordinate = new Coordinate(latitude, longitude);
            
            while (true)
            {
                // make sure there is a previous.
                if (previous == null)
                {
                    if (!shapeEnumerator.MoveNext())
                    {
                        break;
                    }
                    previous = shapeEnumerator.Current;
                    
                    // check previous for candidacy.
                    var distance = Coordinate.DistanceEstimateInMeter(
                        previous.Value.Latitude, previous.Value.Longitude,
                        latitude, longitude);
                    distanceToProjected = distance;
                    projectedDistanceFromFirst = 0;
                    projectedLatitude = previous.Value.Latitude;
                    projectedLongitude = previous.Value.Longitude;
                    projectedShapeIndex = 0;
                    position = LinePointPosition.On;
                }

                // get next point.
                Coordinate? current = null;
                if (!shapeEnumerator.MoveNext())
                {
                    break;
                }
                current = shapeEnumerator.Current;
                
                // project on segment.
                var line = new Line(previous.Value, current.Value);
                var projectedPoint = line.ProjectOn(coordinate);
                if (projectedPoint != null)
                { // ok, projection succeeded.
                    var distance = Coordinate.DistanceEstimateInMeter(
                        projectedPoint.Value.Latitude, projectedPoint.Value.Longitude,
                        latitude, longitude);
                    if (distance < distanceToProjected)
                    { // ok, new best edge yay!
                        distanceToProjected = distance;
                        projectedLatitude = projectedPoint.Value.Latitude;
                        projectedLongitude = projectedPoint.Value.Longitude;
                        projectedDistanceFromFirst = (previousShapeDistance +
                            Coordinate.DistanceEstimateInMeter(
                                projectedLatitude, projectedLongitude,
                                previous.Value.Latitude, previous.Value.Longitude));
                        projectedShapeIndex = previousShapeIndex + 1;
                        position = line.PositionOfPoint(projectedPoint.Value);
                    }
                }

                // add up the current shape distance.
                previousShapeDistance += Coordinate.DistanceEstimateInMeter(
                        previous.Value.Latitude, previous.Value.Longitude,
                        current.Value.Latitude, current.Value.Longitude);
                
                // check current for candidacy.
                var currentDistance = Coordinate.DistanceEstimateInMeter(
                    current.Value.Latitude, current.Value.Longitude,
                    latitude, longitude);
                if (currentDistance < distanceToProjected)
                {
                    distanceToProjected = currentDistance;
                    projectedDistanceFromFirst = previousShapeDistance;
                    projectedLatitude = current.Value.Latitude;
                    projectedLongitude = current.Value.Longitude;
                    projectedShapeIndex = previousShapeIndex + 1;
                    position = LinePointPosition.On;
                }
                
                previousShapeIndex++;

                previous = current.Value;
            }
            totalLength = previousShapeDistance;

            return distanceToProjected != float.MaxValue;
        }

        /// <summary>
        /// Calculates the position of this point relative to this line.
        /// 
        /// Left/Right is viewed from point1 in the direction of point2.
        /// </summary>
        public static LinePointPosition PositionOfPoint(this Line line, Coordinate coordinate)
        {
            return line.PositionOfPoint(coordinate.Latitude, coordinate.Longitude);
        }

        /// <summary>
        /// Calculates the position of this point relative to this line.
        /// 
        /// Left/Right is viewed from point1 in the direction of point2.
        /// </summary>
        public static LinePointPosition PositionOfPoint(this Line line, float latitude, float longitude)
        {
            var latDiff1 = latitude - line.Coordinate1.Latitude;
            var lonDiff1 = longitude - line.Coordinate1.Longitude;
            var latDirection = line.Coordinate2.Latitude - line.Coordinate1.Latitude;
            var lonDirection = line.Coordinate2.Longitude - line.Coordinate1.Longitude;

            var cross = lonDirection * latDiff1 - latDirection * lonDiff1;
            if (cross > 0)
            {
                return LinePointPosition.Left;
            }
            else if (cross < 0)
            {
                return LinePointPosition.Right;
            }
            else
            {
                return LinePointPosition.On;
            }
        }

        /// <summary>
        /// Resolves multiple routerpoints.
        /// </summary>
        public static List<RouterPoint> ResolveMultiple(this Router router, Profile[] profiles, float latitude, float longitude, float maxOffsetDistance)
        {
            var algorithm = new ResolveMultipleAlgorithm(router.Db.Network.GeometricGraph, latitude, longitude, 0.01f, maxOffsetDistance,
                router.GetIsAcceptable(profiles));
            algorithm.Run();
            if (!algorithm.HasSucceeded)
            {
                return new List<RouterPoint>();
            }
            return algorithm.Results;
        }
        
        /// <summary>
        /// Creates a router point for the given edge.
        /// </summary>
        public static RouterPoint CreateRouterPointForEdgeAndVertex(this RouterDb routerDb, long directedEdgeId, uint vertex)
        {
            var edge = routerDb.Network.GetEdge(directedEdgeId);
            var location = routerDb.Network.GetVertex(vertex);

            if (edge.From == vertex)
            {
                return new RouterPoint(location.Latitude, location.Longitude, edge.Id, 0);
            }
            else if (edge.To == vertex)
            {
                return new RouterPoint(location.Latitude, location.Longitude, edge.Id, ushort.MaxValue);
            }
            throw new System.Exception("Cannot create router point: vertex not on given edge.");
        }

        /// <summary>
        /// Creates a router point for the given vertex.
        /// </summary>
        public static bool TryCreateRouterPointForVertex(this Router router, uint vertex, Profile profile, out RouterPoint routerPoint)
        {
            if (!router.Db.Network.GeometricGraph.GetVertex(vertex, out var latitude, out var longitude))
            {
                throw new ArgumentException("Vertex doesn't exist, cannot create routerpoint.");
            }

            var getFactor = router.GetDefaultGetFactor(profile);
            var edges = router.Db.Network.GetEdgeEnumerator(vertex);
            while (edges.MoveNext())
            {
                var factor = getFactor(edges.Data.Profile);
                if (factor.Value > 0)
                {
                    routerPoint = router.Db.CreateRouterPointForEdgeAndVertex(edges.IdDirected(), vertex);
                    return true;
                }
            }
            routerPoint = null;
            return false;
        }

        /// <summary>
        /// Creates a router point for the given vertex.
        /// </summary>
        [Obsolete]
        public static bool TryCreateRouterPointForVertex(this RouterDb routerDb, uint vertex, Profile profile, out RouterPoint routerPoint)
        {
            float latitude, longitude;
            if (!routerDb.Network.GeometricGraph.GetVertex(vertex, out latitude, out longitude))
            {
                throw new ArgumentException("Vertex doesn't exist, cannot create routerpoint.");
            }
            var edges = routerDb.Network.GetEdgeEnumerator(vertex);
            while (edges.MoveNext())
            {
                var edgeProfile = routerDb.EdgeProfiles.Get(edges.Data.Profile);
                var factor = profile.Factor(edgeProfile);
                if (factor.Value != 0)
                {
                    routerPoint = routerDb.CreateRouterPointForEdgeAndVertex(edges.IdDirected(), vertex);
                    return true;
                }
            }
            routerPoint = null;
            return false;
        }
    }

    /// <summary>
    /// The line-point positions.
    /// </summary>
    public enum LinePointPosition
    {
        /// <summary>
        /// Left.
        /// </summary>
        Left,
        /// <summary>
        /// Right.
        /// </summary>
        Right,
        /// <summary>
        /// On.
        /// </summary>
        On
    }
}