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
using Itinero.Algorithms.Collections;
using OpenLR.Model;
using OpenLR.Model.Locations;
using OpenLR.Referenced.Codecs.Candidates;
using OpenLR.Referenced.Codecs.Scoring;
using OpenLR.Referenced.Locations;
using OpenLR.Referenced.Scoring;
using System;
using System.Collections.Generic;
using Itinero.Algorithms;

namespace OpenLR.Referenced.Codecs
{
    /// <summary>
    /// The line codec.
    /// </summary>
    public static class ReferencedLineCodec
    {
        /// <summary>
        /// Decodes the given location.
        /// </summary>
        public static ReferencedLine Decode(LineLocation location, Coder coder)
        {
            // get candidate vertices and edges.
            var candidates = new List<Itinero.Algorithms.Collections.SortedSet<CandidatePathSegment>>();
            var lrps = new List<LocationReferencePoint>();

            // loop over all lrps.
            lrps.Add(location.First);
            candidates.Add(coder.FindCandidatesFor(location.First, true));
            if (location.Intermediate != null)
            {
                // there are intermediates.
                for (int idx = 0; idx < location.Intermediate.Length; idx++)
                {
                    lrps.Add(location.Intermediate[idx]);
                    candidates.Add(coder.FindCandidatesFor(location.Intermediate[idx], true));
                }
            }

            lrps.Add(location.Last);
            candidates.Add(coder.FindCandidatesFor(location.Last, false));

            // find a route between each pair of sequential points.
            // start with the last two points and move backwards.
            var target = lrps[lrps.Count - 1];
            var targetCandidates = candidates[candidates.Count - 1];
            var lineLocationSegments = new List<ReferencedLine>();
            for (var idx = lrps.Count - 2; idx >= 0; idx--)
            {
                var source = lrps[idx];
                var sourceCandidates = candidates[idx];

                // build a list of combined scores.
                var combinedScoresSet =
                    new Itinero.Algorithms.Collections.SortedSet<CombinedScore>(new CombinedScoreComparer());
                foreach (var targetCandidate in targetCandidates)
                {
                    foreach (var currentCandidate in sourceCandidates)
                    {
                        combinedScoresSet.Add(new CombinedScore()
                        {
                            Source = currentCandidate,
                            Target = targetCandidate
                        });
                    }
                }

                var combinedScores = new List<CombinedScore>(combinedScoresSet);

                // find the best candidate route.
                CandidateRoute best = null;
                CandidatePathSegment bestSource = null;
                var targetIsLast = idx == lrps.Count - 2;
                while (combinedScores.Count > 0)
                {
                    // get the first pair.
                    var combinedScore = combinedScores[0];
                    combinedScores.RemoveAt(0);

                    // find a route.
                    var candidate = coder.FindCandidateRoute(combinedScore.Source, combinedScore.Target,
                        source.LowestFunctionalRoadClassToNext.Value, targetIsLast);

                    // bring score of from/to also into the mix.
                    candidate.Score = candidate.Score + combinedScore.Score;

                    // verify bearing by adding it to the score.


                    if (candidate.Route == null)
                    {
                        continue;
                    }
                    // calculate bearing and compare with reference bearing.


                    // calculate distance and compare with distancetonext.
                    var distance = candidate.Route.GetCoordinates(coder.Router.Db).Length();
                    var expectedDistance = source.DistanceToNext;

                    // default a perfect score, only compare large distances.
                    var deviation = Score.New(Score.DISTANCE_COMPARISON,
                        "Compares expected location distance with decoded location distance (1=perfect, 0=difference bigger than total distance)",
                        1, 1);
                    if (expectedDistance > 200 || distance > 200)
                    {
                        // non-perfect score.
                        // don't care about difference smaller than 200m, the binary encoding only handles segments of about 50m.
                        var distanceDiff = Math.Max(Math.Abs(distance - expectedDistance) - 200, 0);
                        deviation = Score.New(Score.DISTANCE_COMPARISON,
                            "Compares expected location distance with decoded location distance (1=prefect, 0=difference bigger than total distance)",
                            1 - Math.Min(Math.Max(distanceDiff / expectedDistance, 0), 1), 1);
                    }

                    // add deviation-score.
                    candidate.Score = candidate.Score * deviation;

                    if (!((candidate.Score.Value / candidate.Score.Reference) > coder.Profile.ScoreThreshold))
                    {
                        continue;
                    }


                    // check candidate.
                    if (best == null)
                    {
                        // there was no previous candidate or candidate has no route.
                        best = candidate;
                        bestSource = combinedScore.Source;
                    }
                    else if (best.Score.Value < candidate.Score.Value)
                    {
                        // the new candidate is better.
                        best = candidate;
                        bestSource = combinedScore.Source;
                    }
                    else if (best.Score.Value > candidate.Score.Value)
                    {
                        // the current candidate is better.
                        break;
                    }

                    if (best.Score.Value == 1)
                    {
                        // stop search on a perfect scrore!
                        break;
                    }
                }

                // append the current best.
                if (best == null || best.Route == null)
                {
                    // no location reference found between two points.
                    return null;
                }

                if (!targetIsLast)
                {
                    // strip last edge and vertex, these will overlap with the previous.
                    best.Route.Vertices = best.Route.Vertices.Range(0, best.Route.Vertices.Length - 1);
                    best.Route.Edges = best.Route.Edges.Range(0, best.Route.Edges.Length - 1);
                }

                // keep the segment.
                lineLocationSegments.Insert(0, best.Route);

                // assign new next.
                target = source;
                targetCandidates = new Itinero.Algorithms.Collections.SortedSet<CandidatePathSegment>();
                targetCandidates.Add(bestSource); // only the best source can be re-used for the next segment.
            }

            // build the line location from the segments.
            var lineLocation = lineLocationSegments[0];
            for (var i = 1; i < lineLocationSegments.Count; i++)
            {
                lineLocation.Add(lineLocationSegments[i]);
            }

            lineLocation.PositiveOffsetPercentage = location.PositiveOffsetPercentage ?? 0;
            lineLocation.NegativeOffsetPercentage = location.NegativeOffsetPercentage ?? 0;

            return lineLocation;
        }


        /// <summary>
        /// Encodes the given location.
        /// </summary>
        public static LineLocation Encode(ReferencedLine referencedLocation, Coder coder)
        {
            return Encode(referencedLocation, coder, EncodingSettings.Default);
        }

        /// <summary>
        /// Encodes the given location.
        /// </summary>
        public static LineLocation Encode(ReferencedLine referencedLocation, Coder coder, EncodingSettings settings)
        {
            try
            {
                // Step – 1: Check validity of the location and offsets to be encoded.
                // validate connected and traversal.
                referencedLocation.ValidateConnected(coder);
                // validate offsets.
                referencedLocation.ValidateOffsets();
                // validate for binary.
                referencedLocation.ValidateBinary();

                // Step – 2 Adjust start and end node of the location to represent valid map nodes.
                referencedLocation.AdjustToValidPoints(coder);
                // keep a list of LR-point.
                List<int> points;

                if (settings.VerifyShortestPath)
                {
                    // Step – 3     Determine coverage of the location by a shortest-path.
                    // Step – 4     Check whether the calculated shortest-path covers the location completely. 
                    //              Go to step 5 if the location is not covered completely, go to step 7 if the location is covered.
                    // Step – 5     Determine the position of a new intermediate location reference point so that the part of the 
                    //              location between the start of the shortest-path calculation and the new intermediate is covered 
                    //              completely by a shortest-path.
                    // Step – 6     Go to step 3 and restart shortest path calculation between the new intermediate location reference 
                    //              point and the end of the location.
                    points = VerifyIsOnShortestPath(referencedLocation, coder);
                }
                else
                {
                    points = new List<int> {0, referencedLocation.Vertices.Length - 1};
                }

                // Step – 7     Concatenate the calculated shortest-paths for a complete coverage of the location and form an 
                //              ordered list of location reference points (from the start to the end of the location).

                // Step – 8     Check validity of the location reference path. If the location reference path is invalid then go 
                //              to step 9, if the location reference path is valid then go to step 10.
                // Step – 9     Add a sufficient number of additional intermediate location reference points if the distance 
                //              between two location reference points exceeds the maximum distance. Remove the start/end LR-point 
                //              if the positive/negative offset value exceeds the length of the corresponding path.
                referencedLocation.AdjustToValidDistance(coder, points);

                // Step – 10    Create physical representation of the location reference.
                var coordinates = referencedLocation.GetCoordinates(coder.Router.Db);
                var length = coordinates.Length();

                // 3: The actual encoding now!
                // initialize location.
                var location = new LineLocation();

                // build lrp's.
                var locationReferencePoints = new List<LocationReferencePoint>();
                for (var idx = 0; idx < points.Count - 1; idx++)
                {
                    locationReferencePoints.Add(referencedLocation.BuildLocationReferencePoint(coder,
                        points[idx], points[idx + 1]));
                }

                locationReferencePoints.Add(referencedLocation.BuildLocationReferencePointLast(
                    coder, points[points.Count - 2]));

                // build location.
                location.First = locationReferencePoints[0];
                location.Intermediate = new LocationReferencePoint[locationReferencePoints.Count - 2];
                for (var idx = 1; idx < locationReferencePoints.Count - 1; idx++)
                {
                    location.Intermediate[idx - 1] = locationReferencePoints[idx];
                }

                location.Last = locationReferencePoints[locationReferencePoints.Count - 1];

                // set offsets.
                location.PositiveOffsetPercentage = referencedLocation.PositiveOffsetPercentage;
                location.NegativeOffsetPercentage = referencedLocation.NegativeOffsetPercentage;

                return location;
            }
            catch (ReferencedEncodingException)
            {
                // rethrow referenced encoding exception.
                throw;
            }
            catch (Exception ex)
            {
                // unhandled exception!
                throw new ReferencedEncodingException(referencedLocation,
                    $"Unhandled exception during ReferencedLineEncoder: {ex}", ex);
            }
        }

        /// <summary>
        /// Creates a list of points on the ReferencedLine, until all the shortest paths between the points describe
        /// exactly the referenced line.
        /// (Actually, the list does not consist of the points, but of the indices of the points within the referenced line)
        ///
        /// Note that, if the endpoints of the referenced line already is the shortest path,
        /// a list containing only the start- and endpoint of the RL is returned.
        /// </summary>
        /// <param name="referencedLine"></param>
        /// <param name="coder"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static List<int> VerifyIsOnShortestPath(ReferencedLine referencedLine, Coder coder)
        {
            var points = new List<int> {0, referencedLine.Vertices.Length - 1};

            var isOnShortestPath = false;


            // keep on adding intermediates until all paths between intermediates are on shortest paths.
            while (!isOnShortestPath)
            {
                isOnShortestPath = true;


                // loop over all LRP-pairs.
                for (var i = 0; i < points.Count - 1; i++)
                {
                    var fromPoint = points[i];
                    var toPoint = points[i + 1];

                    var fromEdge = referencedLine.Edges[fromPoint];
                    var toEdge = referencedLine.Edges[toPoint - 1];

                    // calculate shortest path between their first and last edge.
                    var path = SearchPath(coder, fromEdge, toEdge);

                    var edges = new List<long>();
                    while (path.From != null)
                    {
                        edges.Add(path.Edge);
                        path = path.From;
                    }

                    edges.Reverse(); // Path is, by default, a linked list which is reversed. We reverse again to have normal ordering

                    // calculate coverage.
                    var divergentPoint = -1;
                    for (var j = 0; j < edges.Count; j++)
                    {
                        // Walks from the start of the shortest path. If the shortest path diverges from the ReferencedLine,
                        // we know we have to insert an extra point. 
                        
                        var locationEdgeIdx = j + fromPoint;
                        // ReSharper disable once InvertIf
                        if (locationEdgeIdx >= referencedLine.Edges.Length ||
                            edges[j] != referencedLine.Edges[locationEdgeIdx])
                        {
                            divergentPoint = j + fromPoint;
                            break;
                        }
                    }

                    // ReSharper disable once InvertIf
                    if (divergentPoint != -1)
                    {
                        if (points.Contains(divergentPoint))
                        {
                            throw new ReferencedEncodingException(referencedLine, $"Could not encode edge: can not normalize to shortest path. Point {divergentPoint} keeps diverging");   
                        }
                        // split if needed
                        // we add the point of the referenced line just _after_ the point where divergence happened
                        // Thus at index 'divergentPoint' + 1
                        points.Add(divergentPoint + 1);
                        points.Sort();


                        // This implies we were not on the shortest path and should check again
                        isOnShortestPath = false;

                        break;
                    }
                }
            }

            return points;
        }

        private static EdgePath<float> SearchPath(Coder coder, long fromEdge, long toEdge)
        {
            var pathResult = coder.Router.TryCalculateRaw(coder.Profile.Profile,
                coder.Router.GetDefaultWeightHandler(coder.Profile.Profile),
                fromEdge, toEdge, coder.Profile.RoutingSettings);
            if (!pathResult.IsError) return pathResult.Value;

            try
            {
                coder.Profile.RoutingSettings.SetMaxSearch(coder.Profile.Profile.FullName, float.MaxValue);
                pathResult = coder.Router.TryCalculateRaw(coder.Profile.Profile,
                    coder.Router.GetDefaultWeightHandler(coder.Profile.Profile),
                    fromEdge, toEdge, coder.Profile.RoutingSettings);
                if (pathResult.IsError)
                {
                    throw new Exception("No path found between two edges of the line location.");
                }
                else
                {
                    return pathResult.Value;
                }
            }
            finally
            {
                coder.Profile.RoutingSettings.SetMaxSearch(coder.Profile.Profile.FullName,
                    coder.Profile.MaxSearch);
            }
        }
    }
}