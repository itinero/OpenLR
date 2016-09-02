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

using Itinero.Refactoring;
using OpenLR.Exceptions;
using OpenLR.Model;
using OpenLR.Model.Locations;
using OpenLR.Referenced.Codecs.Candidates;
using OpenLR.Referenced.Codecs.Scoring;
using OpenLR.Referenced.Locations;
using OpenLR.Referenced.Scoring;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenLR.Referenced.Codecs
{
    /// <summary>
    /// The point along line codec.
    /// </summary>
    public static class ReferencedPointAlongLineCodec
    {
        /// <summary>
        /// Holds the maximum distance of the to-encode lat/lon to the project lat/lon.
        /// </summary>
        public static float MaxDistanceFromProjected = 10;

        /// <summary>
        /// Decodes the given location.
        /// </summary>
        public static ReferencedPointAlongLine Decode(PointAlongLineLocation location, Coder coder)
        {
            CandidateRoute best = null;
            CombinedScore bestCombinedEdge = null;

            // get candidate vertices and edges.
            var candidates = new List<SortedSet<CandidateVertexEdge>>();
            var lrps = new List<LocationReferencePoint>();
            var fromBearingReference = location.First.Bearing;
            var toBearingReference = location.Last.Bearing;

            // loop over all lrps.
            lrps.Add(location.First);
            var firstCandidates = coder.FindCandidatesFor(location.First, true);
            candidates.Add(firstCandidates);
            var lastCandidates = coder.FindCandidatesFor(location.Last, false);
            candidates.Add(lastCandidates);

            // build a list of combined scores.
            var combinedScoresSet = new SortedSet<CombinedScore>(new CombinedScoreComparer());
            foreach (var previousCandidate in candidates[0])
            {
                foreach (var currentCandidate in candidates[1])
                {
                    if (previousCandidate.VertexId != currentCandidate.VertexId)
                    { // make sure vertices are different.
                        combinedScoresSet.Add(new CombinedScore()
                        {
                            Source = previousCandidate,
                            Target = currentCandidate
                        });
                    }
                }
            }

            // find the best candidate route.
            var combinedScores = new List<CombinedScore>(combinedScoresSet);
            while (combinedScores.Count > 0)
            {
                // get the first pair.
                var combinedScore = combinedScores.First();
                combinedScores.Remove(combinedScore);

                // find a route.
                var candidate = coder.FindCandidateRoute(combinedScore.Source, combinedScore.Target,
                    lrps[0].LowestFunctionalRoadClassToNext.Value);

                // bring score of from/to also into the mix.
                candidate.Score = candidate.Score + combinedScore.Score;

                // verify bearing by adding it to the score.
                if (candidate != null && candidate.Route != null)
                { // calculate bearing and compare with reference bearing.
                    // calculate distance and compare with distancetonext.
                    var distance = candidate.Route.GetCoordinates(coder.Router.Db).Length();
                    var expectedDistance = location.First.DistanceToNext;

                    // default a perfect score, only compare large distances.
                    var deviation = Score.New(Score.DISTANCE_COMPARISON,
                        "Compares expected location distance with decoded location distance (1=perfect, 0=difference bigger than total distance)", 1, 1);
                    if (expectedDistance > 200 || distance > 200)
                    { // non-perfect score.
                        // don't care about difference smaller than 200m, the binary encoding only handles segments of about 50m.
                        var distanceDiff = System.Math.Max(System.Math.Abs(distance - expectedDistance) - 200, 0);
                        deviation = Score.New(Score.DISTANCE_COMPARISON, "Compares expected location distance with decoded location distance (1=prefect, 0=difference bigger than total distance)",
                            1 - System.Math.Min(System.Math.Max(distanceDiff / expectedDistance, 0), 1), 1);
                    }

                    // add deviation-score.
                    candidate.Score = candidate.Score * deviation;

                    if ((candidate.Score.Value / candidate.Score.Reference) > coder.Profile.ScoreThreshold)
                    { // ok, candidate is good enough.
                        // check candidate.
                        if (best == null)
                        { // there was no previous candidate.
                            best = candidate;
                            bestCombinedEdge = combinedScore;
                        }
                        else if (best.Score.Value < candidate.Score.Value)
                        { // the new candidate is better.
                            best = candidate;
                            bestCombinedEdge = combinedScore;
                        }
                        else if (best.Score.Value > candidate.Score.Value)
                        { // the current candidate is better.
                            break;
                        }
                    }
                }
            }

            // check if a location was found or not.
            if (best == null || best.Route == null)
            { // no location could be found.
                throw new ReferencedDecodingException(location, "No valid location was found.");
            }

            // calculate total score.
            var totalScore = bestCombinedEdge.Score + best.Score;

            // calculate the percentage value.
            var offsetRatio = 0.0f;
            if (location.PositiveOffsetPercentage.HasValue)
            { // there is a percentage set.
                offsetRatio = location.PositiveOffsetPercentage.Value / 100.0f;
            }

            // calculate the actual location and take into account the shape.
            int offsetEdgeIdx;
            Itinero.LocalGeo.Coordinate offsetLocation;
            float offsetLength;
            float offsetEdgeLength;
            float edgeLength;
            var coordinates = best.Route.GetCoordinates(coder, offsetRatio,
                out offsetEdgeIdx, out offsetLocation, out offsetLength, out offsetEdgeLength, out edgeLength);

            var longitudeReference = offsetLocation.Longitude;
            var latitudeReference = offsetLocation.Latitude;

            // create the referenced location.
            var pointAlongLineLocation = new ReferencedPointAlongLine();
            pointAlongLineLocation.Score = totalScore;
            pointAlongLineLocation.Route = best.Route;
            pointAlongLineLocation.Latitude = latitudeReference;
            pointAlongLineLocation.Longitude = longitudeReference;
            if (location.Orientation.HasValue)
            {
                pointAlongLineLocation.Orientation = location.Orientation.Value;
            }
            else
            {
                pointAlongLineLocation.Orientation = Orientation.NoOrientation;
            }

            // add the edge meta data.
            pointAlongLineLocation.EdgeMeta = new EdgeMeta()
            {
                Idx = offsetEdgeIdx,
                Length = edgeLength,
                Offset = offsetEdgeLength
            };

            return pointAlongLineLocation;
        }

        /// <summary>
        /// Encodes the given location.
        /// </summary>
        public static PointAlongLineLocation Encode(ReferencedPointAlongLine referencedLocation, Coder coder)
        {
            try
            {
                // Step – 1: Check validity of the location and offsets to be encoded.
                // validate connected and traversal.
                referencedLocation.Route.ValidateConnected(coder);
                // validate offsets.
                referencedLocation.Route.ValidateOffsets();
                // validate for binary.
                referencedLocation.Route.ValidateBinary();

                // Step – 2 Adjust start and end node of the location to represent valid map nodes.
                referencedLocation.Route.AdjustToValidPoints(coder);

                // keep a list of LR-point.
                var points = new List<int>(new int[] { 0, referencedLocation.Route.Vertices.Length - 1 });

                // Step – 3     Determine coverage of the location by a shortest-path.
                // Step – 4     Check whether the calculated shortest-path covers the location completely. 
                //              Go to step 5 if the location is not covered completely, go to step 7 if the location is covered.
                // Step – 5     Determine the position of a new intermediate location reference point so that the part of the 
                //              location between the start of the shortest-path calculation and the new intermediate is covered 
                //              completely by a shortest-path.
                // Step – 6     Go to step 3 and restart shortest path calculation between the new intermediate location reference 
                //              point and the end of the location.
                // Step – 7     Concatenate the calculated shortest-paths for a complete coverage of the location and form an 
                //              ordered list of location reference points (from the start to the end of the location).

                // Step – 8     Check validity of the location reference path. If the location reference path is invalid then go 
                //              to step 9, if the location reference path is valid then go to step 10.
                // Step – 9     Add a sufficient number of additional intermediate location reference points if the distance 
                //              between two location reference points exceeds the maximum distance. Remove the start/end LR-point 
                //              if the positive/negative offset value exceeds the length of the corresponding path.

                // WARNING: the OpenLR-spec says that there cannot be intermediate points on an PointAlongLineLocation.
                //              this means that if the route found here is > 15.000m it cannot be encoding.
                //              assumed is that the OpenLR-spec assumes that this will never happen (?)
                // referencedLocation.Route.AdjustToValidDistances(this.MainEncoder, points);

                // Step – 10    Create physical representation of the location reference.
                var coordinates = referencedLocation.Route.GetCoordinates(coder.Router.Db);
                var lengthInMeter = coordinates.Length();

                var location = new PointAlongLineLocation();
                location.First = referencedLocation.Route.BuildLocationReferencePoint(
                    coder, 0, referencedLocation.Route.Vertices.Length - 1);
                location.Last = referencedLocation.Route.BuildLocationReferencePointLast(
                    coder, 0);

                // calculate orientation and side of road.
                float projectedLatitude;
                float projectedLongitude;
                float projectedDistanceFromFirst;
                int projectedShapeIndex;
                float distanceToProjected;
                float totalLength;
                LinePointPosition position;
                if (!coordinates.ProjectOn(referencedLocation.Latitude, referencedLocation.Longitude, out projectedLatitude, out projectedLongitude,
                    out projectedDistanceFromFirst, out projectedShapeIndex, out distanceToProjected, out totalLength, out position))
                { // the projection on the edge failed.
                    // try to find the closest point.
                    distanceToProjected = float.MaxValue;
                    totalLength = 0;
                    for (var i = 0; i < coordinates.Count; i++)
                    {
                        var distance = Itinero.LocalGeo.Coordinate.DistanceEstimateInMeter(coordinates[i].Latitude, coordinates[i].Longitude,
                            referencedLocation.Latitude, referencedLocation.Longitude);
                        if (i > 0)
                        {
                            totalLength += Itinero.LocalGeo.Coordinate.DistanceEstimateInMeter(coordinates[i-1].Latitude, coordinates[i-1].Longitude,
                                coordinates[i].Latitude, coordinates[i].Longitude);
                        }
                        if (distance < distanceToProjected)
                        {
                            projectedDistanceFromFirst = totalLength;
                            distanceToProjected = distance;
                            projectedShapeIndex = i;
                            position = LinePointPosition.On;
                            projectedLatitude = coordinates[i].Latitude;
                            projectedLongitude = coordinates[i].Longitude;
                        }
                    }
                }
                if (distanceToProjected > MaxDistanceFromProjected)
                {
                    throw new ReferencedEncodingException(referencedLocation, string.Format("The point in the ReferencedPointAlongLine is too far from the referenced edge: {0}m with a max allowed of {1}m.",
                        distanceToProjected, MaxDistanceFromProjected));
                }

                location.Orientation = referencedLocation.Orientation;
                switch (position)
                {
                    case LinePointPosition.Left:
                        location.SideOfRoad = SideOfRoad.Left;
                        break;
                    case LinePointPosition.On:
                        location.SideOfRoad = SideOfRoad.OnOrAbove;
                        break;
                    case LinePointPosition.Right:
                        location.SideOfRoad = SideOfRoad.Right;
                        break;
                }

                // calculate offset.
                location.PositiveOffsetPercentage = (float)(projectedDistanceFromFirst / lengthInMeter) * 100.0f;
                if (location.PositiveOffsetPercentage >= 100)
                { // should be in the range of [0-100[.
                    // encoding should always work even if not 100% accurate in this case.
                    location.PositiveOffsetPercentage = 99;
                }

                return location;
            }
            catch (ReferencedEncodingException)
            { // rethrow referenced encoding exception.
                throw;
            }
            catch (Exception ex)
            { // unhandled exception!
                throw new ReferencedEncodingException(referencedLocation,
                    string.Format("Unhandled exception during ReferencedPointAlongLineEncoder: {0}", ex.ToString()), ex);
            }
        }
    }
}