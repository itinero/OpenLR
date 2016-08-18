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

using Itinero.Algorithms.Search.Hilbert;
using Itinero.Data.Network;
using Itinero.Refactoring;
using OpenLR.Model;
using OpenLR.Referenced.Codecs.Candidates;
using OpenLR.Referenced.Decoding.Candidates;
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
        public static SortedSet<CandidateVertexEdge> FindCandidatesFor(this Coder coder, LocationReferencePoint lrp, bool forward, float maxVertexDistance = 40)
        {
            var vertexEdgeCandidates = new SortedSet<CandidateVertexEdge>(new CandidateVertexEdgeComparer());
            var vertexCandidates = coder.FindCandidateVerticesFor(lrp, maxVertexDistance);
            foreach (var vertexCandidate in vertexCandidates)
            {
                var edgeCandidates = coder.FindCandidateEdgesFor(vertexCandidate.Vertex, forward, lrp.FormOfWay.Value, lrp.FuntionalRoadClass.Value,
                    lrp.Bearing.Value);
                foreach (var edgeCandidate in edgeCandidates)
                {
                    vertexEdgeCandidates.Add(new CandidateVertexEdge()
                    {
                        EdgeId = edgeCandidate.EdgeId,
                        VertexId = vertexCandidate.Vertex,
                        Score = vertexCandidate.Score * edgeCandidate.Score
                    });
                }
            }
            return vertexEdgeCandidates;
        }

        /// <summary>
        /// Finds candidate vertices for a location reference point.
        /// </summary>
        public static IEnumerable<CandidateVertex> FindCandidateVerticesFor(this Coder coder, LocationReferencePoint lrp, float maxVertexDistanceInMeter = 40, 
            float candidateSearchBoxSize = 0.01f)
        {
            // build candidates list.
            var scoredCandidates = new List<CandidateVertex>();
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
                    scoredCandidates.Add(new CandidateVertex()
                    {
                        Score = Score.New(Score.VERTEX_DISTANCE, string.Format("The vertex score compare to max distance {0}", maxVertexDistanceInMeter),
                            (float)System.Math.Max(0, (1.0 - (distance / maxVertexDistanceInMeter))), 1), // calculate scoring compared to the fixed max distance.
                        Vertex = v
                    });
                }
            }

            if (scoredCandidates.Count == 0)
            { // no candidates, create a virtual candidate.
                throw new NotSupportedException("No candidates found for LRP: creating a virtual point is not supported.");
            }
            return scoredCandidates;
        }
        
        /// <summary>
        /// Finds candidate edges starting at a given vertex matching a given fow and frc.
        /// </summary>
        public static IEnumerable<CandidateEdge> FindCandidateEdgesFor(this Coder coder, long vertex, bool forward, FormOfWay fow, FunctionalRoadClass frc, 
            float bearing)
        {
            var profile = coder.Profile;
            var relevantEdges = new List<CandidateEdge>();
            var edgeEnumerator = coder.Router.Db.Network.GetEdgeEnumerator((uint)vertex);
            foreach (var edge in edgeEnumerator)
            {
                var edgeProfile = coder.Router.Db.EdgeProfiles.Get(edge.Data.Profile);
                var factor = profile.Profile.Factor(edgeProfile);
                
                if (factor.Value > 0 && (factor.Direction == 0 ||
                    (forward && (factor.Direction == 1) != edge.DataInverted) ||
                    (!forward && (factor.Direction == 1) == edge.DataInverted)))
                {
                    var score = Score.New(Score.MATCH_ARC, "Metric indicating a match with fow, frc etc...",
                        profile.Match(edgeProfile, fow, frc), 2);
                    if (score.Value > 0)
                    { // ok, there is a match.
                      // check bearing.
                        var shape = coder.Router.Db.Network.GetShape(edge);
                        var localBearing = BearingEncoder.EncodeBearing(shape, true);
                        var localBearingDiff = Extensions.AngleSmallestDifference(localBearing, bearing);

                        relevantEdges.Add(new CandidateEdge()
                        {
                            Score = score +
                                Score.New(Score.BEARING_DIFF, "Bearing difference (0=0 & 180=1)", ((180f - localBearingDiff) / 180f), 1),
                            EdgeId = edge.Id
                        });
                    }
                }
            }
            return relevantEdges;
        }

        /// <summary>
        /// Calculates a route between the two given vertices.
        /// </summary>
        public static CandidateRoute FindCandidateRoute(this Coder coder, CandidateVertexEdge from, CandidateVertexEdge to, FunctionalRoadClass minimum,
            bool ignoreFromEdge = false, bool ignoreToEdge = false)
        {
            throw new NotImplementedException();

            //var path = coder.Router.TryCalculateRaw(profile.Profile, from.ToRouterPoint(coder.Router.Db), to.ToRouterPoint(coder.Router.Db));

            //// if no route is found, score is 0.
            //if (path == null)
            //{
            //    return new CandidateRoute()
            //    {
            //        Route = null,
            //        Score = Score.New(Score.CANDIDATE_ROUTE, "Candidate route quality.", 0, 1)
            //    };
            //}

            //return new CandidateRoute()
            //{
            //    Route = new ReferencedLine(coder.Router.Db)
            //    {
            //        Edges = edges.ToArray(),
            //        Vertices = vertices.ToArray()
            //    },
            //    Score = Score.New(Score.CANDIDATE_ROUTE, "Candidate route quality.", 1, 1)
            //};
        }
    }
}