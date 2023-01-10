using OpenLR.Model;
using OpenLR.Model.Locations;
using OpenLR.Referenced.Locations;
using OpenLR.Scoring;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Itinero;
using Itinero.IO.Json.GeoJson;
using Itinero.Routes.Paths;
using Itinero.Snapping;
using OpenLR.Exceptions;
using OpenLR.Scoring.Candidates;

namespace OpenLR.Referenced.Codecs;

/// <summary>
/// The line codec.
/// </summary>
public static class ReferencedLineCodec
{
    /// <summary>
    /// Decodes the given location.
    /// </summary>
    public static async Task<Result<ReferencedLine>> Decode(LineLocation location, Coder coder)
    {
        using var locationEnumerator = location.LocationReferencePoints().GetEnumerator();
        if (!locationEnumerator.MoveNext())
            return new Result<ReferencedLine>("Line location cannot be decoded, no LRPs found");

        // keep the total path.
        Path? decodedPath = null;

        // get first lrp and calculate candidates.
        var tail = locationEnumerator.Current;
        var tailCandidates = await coder.FindCandidatesFor(tail.lrp, !tail.isLast);
        while (locationEnumerator.MoveNext())
        {
            // get next lrp and calculate candidates.
            var head = locationEnumerator.Current;
            var headCandidates = await coder.FindCandidatesFor(head.lrp, !head.isLast);

            var geojson = coder.Network.ToGeoJson();

            // sort pairs and consider best scoring candidates first.
            var combinedScoresSet =
                new SortedSet<ScoredCandidate<(ScoredCandidate<(SnapPoint snapPoint, bool direction)> left,
                    ScoredCandidate<(SnapPoint snapPoint, bool direction)> right)>>();
            foreach (var tailCandidate in tailCandidates)
            foreach (var headCandidate in headCandidates)
            {
                combinedScoresSet.Add(tailCandidate.Add(headCandidate));
            }

            // calculate paths and build candidate routes.
            var best = new ScoredCandidate<Path?>(null, Score.Zero);
            foreach (var combinedCandidates in combinedScoresSet)
            {
                var deviation = Score.New(Score.DistanceComparison,
                    "Compares expected location distance with decoded location distance (1=perfect, 0=difference bigger than total distance)",
                    1, 1);
                
                // calculate perfect score, if it is worst than best, stop search.
                var perfectScore = combinedCandidates.Score + Score.New(Score.CandidateRoute, "Perfect route", 1, 1) +
                                   deviation;
                if (!(perfectScore.Value > best.Score.Value)) break;

                // calculate candidate route.
                var tailCandidate = combinedCandidates.Candidate.left;
                var headCandidate = combinedCandidates.Candidate.right;

                // find a route.
                var candidate = await coder.FindCandidateRoute(tailCandidate.Candidate, headCandidate.Candidate,
                    tail.lrp.LowestFunctionalRoadClassToNext.Value, tail.lrp.DistanceToNext);
                if (candidate.Candidate == null) continue;

                // verify bearing by adding it to the score.
                candidate = candidate.WithScore(combinedCandidates.Score);

                // calculate distance and compare with distance to next.
                var distance = candidate.Candidate.LengthInMeters();
                var expectedDistance = tail.lrp.DistanceToNext;

                // default a perfect score, only compare large distances.
                if (expectedDistance > 200 || distance > 200)
                {
                    // non-perfect score.
                    // don't care about difference smaller than 200m, the binary encoding only handles segments of about 50m.
                    var distanceDiff = Math.Max(Math.Abs(distance - expectedDistance) - 200, 0);
                    deviation = Score.New(Score.DistanceComparison,
                        "Compares expected location distance with decoded location distance (1=prefect, 0=difference bigger than total distance)",
                        1 - Math.Min(Math.Max(distanceDiff / expectedDistance, 0), 1), 1);
                }

                // add deviation-score.
                candidate = candidate.WithScore(deviation);

                // if score gets too bad, just stop.
                if (!((candidate.Score.Value / candidate.Score.Reference) > coder.Settings.ScoreThreshold)) continue;

                // if score is worse, move to next.
                if (best.Score.Value >= candidate.Score.Value) continue;
                best = candidate;
                
                // if score is perfect, stop.
                if (best.Score.IsPerfect) break;
            }

            // if best is null decoding fails.
            if (best.Candidate == null) return new Result<ReferencedLine>("Decoding failed, cannot find all paths");

            // append best path.
            decodedPath = decodedPath == null ? best.Candidate : decodedPath.Merge(best.Candidate);
        }

        // if decodedPath is null decoding fails.
        if (decodedPath == null) return new Result<ReferencedLine>("Decoding failed, cannot find all paths");

        // build the line location from the segments.
        return ReferencedLine.FromPath(decodedPath, location.PositiveOffsetPercentage,
            location.NegativeOffsetPercentage);
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
            //     // Step – 1: Check validity of the location and offsets to be encoded.
            //     // validate connected and traversal.
            //     if (referencedLocation == null)
            //     {
            //         throw new NullReferenceException(nameof(referencedLocation));
            //     }
            //         
            //     referencedLocation.ValidateConnected(coder);
            //     // validate offsets.
            //     referencedLocation.ValidateOffsets();
            //     // validate for binary.
            //     referencedLocation.ValidateBinary();
            //
            //     // Step – 2 Adjust start and end node of the location to represent valid map nodes.
            //     referencedLocation.AdjustToValidPoints(coder);
            //     // keep a list of LR-point.
            //     List<int> points;
            //
            //     if (settings.VerifyShortestPath)
            //     {
            //         // Step – 3     Determine coverage of the location by a shortest-path.
            //         // Step – 4     Check whether the calculated shortest-path covers the location completely. 
            //         //              Go to step 5 if the location is not covered completely, go to step 7 if the location is covered.
            //         // Step – 5     Determine the position of a new intermediate location reference point so that the part of the 
            //         //              location between the start of the shortest-path calculation and the new intermediate is covered 
            //         //              completely by a shortest-path.
            //         // Step – 6     Go to step 3 and restart shortest path calculation between the new intermediate location reference 
            //         //              point and the end of the location.
            //         points = VerifyIsOnShortestPath(referencedLocation, coder);
            //     }
            //     else
            //     {
            //         points = new List<int> {0, referencedLocation.Vertices.Length - 1};
            //     }
            //
            //     // Step – 7     Concatenate the calculated shortest-paths for a complete coverage of the location and form an 
            //     //              ordered list of location reference points (from the start to the end of the location).
            //
            //     // Step – 8     Check validity of the location reference path. If the location reference path is invalid then go 
            //     //              to step 9, if the location reference path is valid then go to step 10.
            //     // Step – 9     Add a sufficient number of additional intermediate location reference points if the distance 
            //     //              between two location reference points exceeds the maximum distance. Remove the start/end LR-point 
            //     //              if the positive/negative offset value exceeds the length of the corresponding path.
            //     referencedLocation.AdjustToValidDistance(coder, points);
            //
            //     // Step – 10    Create physical representation of the location reference.
            //     var coordinates = referencedLocation.GetCoordinates(coder.Router.Db);
            //     var length = coordinates.Length();
            //
            //     // 3: The actual encoding now!
            //     // initialize location.
            //     var location = new LineLocation();
            //
            //     // build lrp's.
            //     var locationReferencePoints = new List<LocationReferencePoint>();
            //     for (var idx = 0; idx < points.Count - 1; idx++)
            //     {
            //         locationReferencePoints.Add(referencedLocation.BuildLocationReferencePoint(coder,
            //             points[idx], points[idx + 1]));
            //     }
            //
            //     locationReferencePoints.Add(referencedLocation.BuildLocationReferencePointLast(
            //         coder, points[points.Count - 2]));
            //
            //     // build location.
            //     location.First = locationReferencePoints[0];
            //     location.Intermediate = new LocationReferencePoint[locationReferencePoints.Count - 2];
            //     for (var idx = 1; idx < locationReferencePoints.Count - 1; idx++)
            //     {
            //         location.Intermediate[idx - 1] = locationReferencePoints[idx];
            //     }
            //
            //     location.Last = locationReferencePoints[locationReferencePoints.Count - 1];
            //
            //     // set offsets.
            //     location.PositiveOffsetPercentage = referencedLocation.PositiveOffsetPercentage;
            //     location.NegativeOffsetPercentage = referencedLocation.NegativeOffsetPercentage;
            //
            //     return location;
            throw new NotImplementedException();
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
    //
    // /// <summary>
    // /// Creates a list of points on the ReferencedLine, until all the shortest paths between the points describe
    // /// exactly the referenced line.
    // /// (Actually, the list does not consist of the points, but of the indices of the points within the referenced line)
    // ///
    // /// Note that, if the endpoints of the referenced line already is the shortest path,
    // /// a list containing only the start- and endpoint of the RL is returned.
    // /// </summary>
    // /// <param name="referencedLine"></param>
    // /// <param name="coder"></param>
    // /// <returns></returns>
    // /// <exception cref="Exception"></exception>
    // private static List<int> VerifyIsOnShortestPath(ReferencedLine referencedLine, Coder coder)
    // {
    //     var points = new List<int> { 0, referencedLine.Vertices.Length - 1 };
    //
    //     var isOnShortestPath = false;
    //
    //
    //     // keep on adding intermediates until all paths between intermediates are on shortest paths.
    //     while (!isOnShortestPath)
    //     {
    //         isOnShortestPath = true;
    //
    //
    //         // loop over all LRP-pairs.
    //         for (var i = 0; i < points.Count - 1; i++)
    //         {
    //             var fromPoint = points[i];
    //             var toPoint = points[i + 1];
    //
    //             var fromEdge = referencedLine.Edges[fromPoint];
    //             var toEdge = referencedLine.Edges[toPoint - 1];
    //
    //             // calculate shortest path between their first and last edge.
    //             var path = SearchPath(coder, fromEdge, toEdge);
    //
    //             var edges = new List<long>();
    //             while (path.From != null)
    //             {
    //                 edges.Add(path.Edge);
    //                 path = path.From;
    //             }
    //
    //             edges.Reverse(); // Path is, by default, a linked list which is reversed. We reverse again to have normal ordering
    //
    //             // calculate coverage.
    //             var divergentPoint = -1;
    //             for (var j = 0; j < edges.Count; j++)
    //             {
    //                 // Walks from the start of the shortest path. If the shortest path diverges from the ReferencedLine,
    //                 // we know we have to insert an extra point. 
    //
    //                 var locationEdgeIdx = j + fromPoint;
    //                 // ReSharper disable once InvertIf
    //                 if (locationEdgeIdx >= referencedLine.Edges.Length ||
    //                     edges[j] != referencedLine.Edges[locationEdgeIdx])
    //                 {
    //                     divergentPoint = j + fromPoint;
    //                     break;
    //                 }
    //             }
    //
    //             // ReSharper disable once InvertIf
    //             if (divergentPoint != -1)
    //             {
    //                 if (points.Contains(divergentPoint + 1))
    //                 {
    //                     throw new ReferencedEncodingException(referencedLine,
    //                         $"Could not encode edge: can not normalize to shortest path. Point {divergentPoint} keeps diverging");
    //                 }
    //
    //                 // split if needed
    //                 // we add the point of the referenced line just _after_ the point where divergence happened
    //                 // Thus at index 'divergentPoint' + 1
    //                 points.Add(divergentPoint + 1);
    //                 points.Sort();
    //
    //
    //                 // This implies we were not on the shortest path and should check again
    //                 isOnShortestPath = false;
    //
    //                 break;
    //             }
    //         }
    //     }
    //
    //     return points;
    // }
    //
    // private static EdgePath<float> SearchPath(Coder coder, long fromEdge, long toEdge)
    // {
    //     var pathResult = coder.Router.TryCalculateRaw(coder.Settings.Profile,
    //         coder.Router.GetDefaultWeightHandler(coder.Settings.Profile),
    //         fromEdge, toEdge, coder.Settings.RoutingSettings);
    //     if (!pathResult.IsError) return pathResult.Value;
    //
    //     try
    //     {
    //         coder.Settings.RoutingSettings.SetMaxSearch(coder.Settings.Profile.FullName, float.MaxValue);
    //         pathResult = coder.Router.TryCalculateRaw(coder.Settings.Profile,
    //             coder.Router.GetDefaultWeightHandler(coder.Settings.Profile),
    //             fromEdge, toEdge, coder.Settings.RoutingSettings);
    //         if (pathResult.IsError)
    //         {
    //             throw new Exception("No path found between two edges of the line location.");
    //         }
    //         else
    //         {
    //             return pathResult.Value;
    //         }
    //     }
    //     finally
    //     {
    //         coder.Settings.RoutingSettings.SetMaxSearch(coder.Settings.Profile.FullName,
    //             coder.Settings.MaxSearch);
    //     }
    // }
}
