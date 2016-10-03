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
using Itinero.Algorithms.Search.Hilbert;
using Itinero.Algorithms.Weights;
using Itinero.Data.Network;
using Itinero.Graphs;
using Itinero.Refactoring;
using OpenLR.Model;
using OpenLR.Referenced.Codecs.Candidates;
using OpenLR.Referenced.Locations;
using OpenLR.Referenced.Scoring;
using System;
using System.Collections.Generic;

namespace OpenLR.Referenced.Codecs
{
    /// <summary>
    /// Helper functions to help with encoding/decoding referenced locations.
    /// </summary>
    public static class CoderExtensions
    {
        /// <summary>
        /// Finds all candidate vertex/edge pairs for a given location reference point.
        /// </summary>
        public static SortedSet<CandidatePathSegment> FindCandidatesFor(this Coder coder, LocationReferencePoint lrp, bool forward, float maxVertexDistance = 40)
        {
            var vertexEdgeCandidates = new SortedSet<CandidatePathSegment>(new CandidateVertexEdgeComparer());
            var vertexCandidates = coder.FindCandidateLocationsFor(lrp, maxVertexDistance);
            foreach (var vertexCandidate in vertexCandidates)
            {
                var edgeCandidates = coder.FindCandidatePathSegmentsFor(vertexCandidate, forward, lrp.FormOfWay.Value, lrp.FuntionalRoadClass.Value,
                    lrp.Bearing.Value);
                foreach (var edgeCandidate in edgeCandidates)
                {
                    vertexEdgeCandidates.Add(edgeCandidate);
                }
            }
            return vertexEdgeCandidates;
        }

        /// <summary>
        /// Finds candidate vertices for a location reference point.
        /// </summary>
        public static IEnumerable<CandidateLocation> FindCandidateLocationsFor(this Coder coder, LocationReferencePoint lrp, float maxVertexDistanceInMeter = 40, 
            float candidateSearchBoxSize = 0.01f)
        {
            // build candidates list.
            var scoredCandidates = new List<CandidateLocation>();
            var lrpCoordinate = new Itinero.LocalGeo.Coordinate((float)lrp.Coordinate.Latitude, (float)lrp.Coordinate.Longitude);

            // get vertices and check their edges.
            var vertices = coder.Router.Db.Network.GeometricGraph.Search((float)lrp.Coordinate.Latitude, (float)lrp.Coordinate.Longitude, candidateSearchBoxSize);
            var candidates = new HashSet<long>();
            var edgeEnumerator = coder.Router.Db.Network.GetEdgeEnumerator();
            foreach (var v in vertices)
            {
                if (candidates.Contains(v))
                { // vertex was already considered.
                    continue;
                }

                var vertexLocation = coder.Router.Db.Network.GetVertex(v);
                var distance = Itinero.LocalGeo.Coordinate.DistanceEstimateInMeter(vertexLocation, lrpCoordinate);
                if (distance < maxVertexDistanceInMeter)
                {
                    candidates.Add(v);

                    // check if there are edges that can be used for the given profile.
                    edgeEnumerator.MoveTo(v);

                    RouterPoint location;
                    if (coder.Router.Db.TryCreateRouterPointForVertex(v, coder.Profile.Profile, out location))
                    {
                        scoredCandidates.Add(new CandidateLocation()
                        {
                            Score = Score.New(Score.VERTEX_DISTANCE, string.Format("The vertex score compare to max distance {0}", maxVertexDistanceInMeter),
                                (float)System.Math.Max(0, (1.0 - (distance / maxVertexDistanceInMeter))), 1), // calculate scoring compared to the fixed max distance.
                            Location = location
                        });
                    }
                }
            }

            if (scoredCandidates.Count == 0)
            { // no candidates, create a virtual candidate.
                var routerPoints = coder.Router.ResolveMultiple(new Itinero.Profiles.Profile[] { coder.Profile.Profile }, lrpCoordinate.Latitude, lrpCoordinate.Longitude, maxVertexDistanceInMeter);
                if (routerPoints.Count == 0)
                {
                    throw new Exception("No candidates found for LRP: Could not resolve a point at the location.");
                }
                foreach (var routerPoint in routerPoints)
                {
                    var distance = Itinero.LocalGeo.Coordinate.DistanceEstimateInMeter(routerPoint.LocationOnNetwork(coder.Router.Db), lrpCoordinate);
                    scoredCandidates.Add(new CandidateLocation()
                    {
                        Score = Score.New(Score.VERTEX_DISTANCE, string.Format("The vertex score compare to max distance {0}", maxVertexDistanceInMeter),
                            (float)System.Math.Max(0, (1.0 - (distance / maxVertexDistanceInMeter))), 1), // calculate scoring compared to the fixed max distance.
                        Location = routerPoint
                    });
                }
            }
            return scoredCandidates;
        }
        
        /// <summary>
        /// Finds candidate edges starting at a given vertex matching a given fow and frc.
        /// </summary>
        public static IEnumerable<CandidatePathSegment> FindCandidatePathSegmentsFor(this Coder coder, CandidateLocation location, bool forward, FormOfWay fow, FunctionalRoadClass frc, 
            float bearing)
        {
            var profile = coder.Profile;
            var relevantEdges = new List<CandidatePathSegment>();
            if (location.Location.IsVertex())
            { // location is a vertex, probably 99% of the time.
                var vertex = location.Location.VertexId(coder.Router.Db);
                var edgeEnumerator = coder.Router.Db.Network.GetEdgeEnumerator(vertex);
                foreach (var edge in edgeEnumerator)
                {
                    var edgeProfile = coder.Router.Db.EdgeProfiles.Get(edge.Data.Profile);
                    var factor = profile.Profile.Factor(edgeProfile);

                    if (factor.Value > 0 && (factor.Direction == 0 ||
                        (forward && (factor.Direction == 1) != edge.DataInverted) ||
                        (!forward && (factor.Direction == 1) == edge.DataInverted)))
                    {
                        var matchScore = Score.New(Score.MATCH_ARC, "Metric indicating a match with fow, frc etc...",
                            profile.Match(edgeProfile, fow, frc), 2);
                        if (matchScore.Value > 0)
                        { // ok, there is a match.
                          // check bearing.
                            var shape = coder.Router.Db.Network.GetShape(edge);
                            var localBearing = BearingEncoder.EncodeBearing(shape, false);
                            var localBearingDiff = System.Math.Abs(Extensions.AngleSmallestDifference(localBearing, bearing));

                            var bearingScore = Score.New(Score.BEARING_DIFF, "Bearing difference (0=0 & 180=1)", ((180f - localBearingDiff) / 180f), 1);
                            relevantEdges.Add(new CandidatePathSegment()
                            {
                                Score = location.Score * (matchScore + bearingScore),
                                Location = location.Location,
                                Path = new EdgePath<float>(edge.To, factor.Value * edge.Data.Distance, edge.IdDirected(), new EdgePath<float>(vertex))
                            });
                        }
                    }
                }
            }
            else
            { // location is not a vertex but a virtual point, try available directions.
                var paths = location.Location.ToEdgePaths(coder.Router.Db, coder.Profile.Profile.DefaultWeightHandlerCached(coder.Router.Db), forward);
                var edgeEnumerator = coder.Router.Db.Network.GetEdgeEnumerator();
                var locationOnNetwork = location.Location.LocationOnNetwork(coder.Router.Db);
                foreach (var path in paths)
                {
                    if (path.From == null)
                    {
                        throw new Exception("An LRP was resolved while at the same time it resolved to an exact vertex.");
                    }
                    edgeEnumerator.MoveToEdge(path.Edge);
                    var edge = edgeEnumerator.Current;
                    var edgeProfile = coder.Router.Db.EdgeProfiles.Get(edge.Data.Profile);
                    var factor = profile.Profile.Factor(edgeProfile);

                    if (factor.Value > 0 && (factor.Direction == 0 ||
                        (forward && (factor.Direction == 1) != edge.DataInverted) ||
                        (!forward && (factor.Direction == 1) == edge.DataInverted)))
                    {
                        var matchScore = Score.New(Score.MATCH_ARC, "Metric indicating a match with fow, frc etc...",
                            profile.Match(edgeProfile, fow, frc), 2);
                        if (matchScore.Value > 0)
                        { // ok, there is a match.
                          // check bearing.

                            // get shape from location -> path.
                            var shape = location.Location.ShapePointsTo(coder.Router.Db, path.Vertex);
                            shape.Insert(0, locationOnNetwork);
                            shape.Add(coder.Router.Db.Network.GetVertex(path.Vertex));

                            var localBearing = BearingEncoder.EncodeBearing(shape, false);
                            var localBearingDiff = System.Math.Abs(Extensions.AngleSmallestDifference(localBearing, bearing));

                            var bearingScore = Score.New(Score.BEARING_DIFF, "Bearing difference (0=0 & 180=1)", ((180f - localBearingDiff) / 180f), 1);
                            relevantEdges.Add(new CandidatePathSegment()
                            {
                                Score = location.Score * (matchScore + bearingScore),
                                Location = location.Location,
                                Path = path
                            });
                        }
                    }
                }
            }
            return relevantEdges;
        }

        /// <summary>
        /// Calculates a route between the two given vertices.
        /// </summary>
        public static CandidateRoute FindCandidateRoute(this Coder coder, CandidatePathSegment from, CandidatePathSegment to, FunctionalRoadClass minimum,
            bool ignoreFromEdge = false, bool ignoreToEdge = false)
        {
            var weightHandler = coder.Profile.Profile.DefaultWeightHandler(coder.Router);

            var directedEdgeFrom = from.Path.Edge;
            var directedEdgeTo = -to.Path.Edge;

            var path = coder.Router.TryCalculateRaw(coder.Profile.Profile, weightHandler,
                directedEdgeFrom, directedEdgeTo, coder.Profile.RoutingSettings);

            // if no route is found, score is 0.
            if (path.IsError)
            {
                return new CandidateRoute()
                {
                    Route = null,
                    Score = Score.New(Score.CANDIDATE_ROUTE, "Candidate route quality.", 0, 1)
                };
            }

            var pathAsList = path.Value.ToList();
            var edges = new List<long>();
            var vertices = new List<uint>();
            for(var i = 0; i < pathAsList.Count; i++)
            {
                vertices.Add(pathAsList[i].Vertex);
                if (i > 0)
                {
                    if (pathAsList[i].Edge != Itinero.Constants.NO_EDGE &&
                        pathAsList[i].Edge != -Itinero.Constants.NO_EDGE)
                    {
                        edges.Add(pathAsList[i].Edge);
                    }
                    else
                    {
                        var edgeEnumerator = coder.Router.Db.Network.GeometricGraph.Graph.GetEdgeEnumerator();
                        float best;
                        var edge = edgeEnumerator.FindBestEdge(weightHandler, vertices[vertices.Count - 2],
                            vertices[vertices.Count - 1], out best);
                        edges.Add(edge);
                    }
                }
            }

            var startLocation = from.Location;
            if (!from.Location.IsVertex())
            { // first vertex is virtual.
                vertices[0] = Itinero.Constants.NO_VERTEX;
            }
            else
            { // make sure routerpoint has same edge.
                startLocation = coder.Router.Db.CreateRouterPointForEdgeAndVertex(edges[0], vertices[0]);
            }

            var endLocation = to.Location;
            if (!to.Location.IsVertex())
            { // last vertex is virtual.
                vertices[vertices.Count - 1] = Itinero.Constants.NO_VERTEX;
            }
            else
            { // make sure routerpoint has the same edge.
                endLocation = coder.Router.Db.CreateRouterPointForEdgeAndVertex(edges[edges.Count - 1], vertices[vertices.Count - 1]);
            }            

            return new CandidateRoute()
            {
                Route = new ReferencedLine()
                {
                    Edges = edges.ToArray(),
                    Vertices = vertices.ToArray(),
                    StartLocation = startLocation,
                    EndLocation = endLocation
                },
                Score = Score.New(Score.CANDIDATE_ROUTE, "Candidate route quality.", 1, 1)
            };
        }
    }
}