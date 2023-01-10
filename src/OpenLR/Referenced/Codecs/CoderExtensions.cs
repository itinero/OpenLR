using System;
using Itinero;
using OpenLR.Model;
using OpenLR.Referenced.Locations;
using OpenLR.Scoring;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Itinero.Geo;
using Itinero.Network;
using Itinero.Network.Enumerators.Edges;
using Itinero.Routes.Paths;
using Itinero.Routing;
using Itinero.Snapping;
using OpenLR.Scoring.Candidates;

[assembly:InternalsVisibleTo("OpenLR.Test")]
namespace OpenLR.Referenced.Codecs;

/// <summary>
/// Helper functions to help with encoding/decoding referenced locations.
/// </summary>
internal static class CoderExtensions
{
    /// <summary>
    /// Finds all candidate vertex/edge pairs for a given location reference point.
    /// </summary>
    public static async Task<SortedSet<ScoredCandidate<(SnapPoint snapPoint, bool direction)>>> FindCandidatesFor(this Coder coder, LocationReferencePoint lrp, bool pointsForward, float maxVertexDistance = 40)
    {
        var vertexEdgeCandidates = new SortedSet<ScoredCandidate<(SnapPoint snapPoint, bool direction)>>();
        var vertexCandidates = coder.FindCandidateSnapPointsForAsync(lrp, maxVertexDistance);
        await foreach (var vertexCandidate in vertexCandidates)
        {
            var edgeCandidates = coder.FindCandidateSnapPointAndDirectionFor(vertexCandidate, pointsForward, lrp.FormOfWay.Value, lrp.FunctionalRoadClass.Value,
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
    public static async IAsyncEnumerable<ScoredCandidate<SnapPoint>> FindCandidateSnapPointsForAsync(this Coder coder,
        LocationReferencePoint lrp, double maxVertexDistanceInMeter = 40)
    {
        static bool ScoreOk(Coder coder, ScoredCandidate<SnapPoint> candidate)
        {
            return (candidate.Score.Value / candidate.Score.Reference >=
                    System.Math.Min(
                        coder.Settings.ScoreThreshold + coder.Settings.ScoreThreshold, 0.7));
        }

        // search all vertices in box determined by the max distance.
        var lrpLocation = lrp.Coordinate.ToLocation();
        var snapper = coder.Network.Snap(coder.Settings.RoutingSettings.Profile, settings =>
        {
            settings.MaxDistance = maxVertexDistanceInMeter;
        });

        var candidatesQualityOk = false;
        await foreach (var vertex in snapper.ToAllVerticesAsync(lrpLocation.longitude, lrpLocation.latitude))
        {
            var location = coder.Network.GetVertex(vertex);
            var distance = lrpLocation.DistanceEstimateInMeter(location);
            if (distance > maxVertexDistanceInMeter) continue; // too far.

            var snapPoint = snapper.To(vertex).First();
            var candidate = new ScoredCandidate<SnapPoint>(snapPoint, Score.New(Score.VertexDistance,
                $"The vertex score compare to max distance {maxVertexDistanceInMeter}",
                (float)System.Math.Max(0, (1.0 - (distance / maxVertexDistanceInMeter))),
                1));
            candidatesQualityOk |= ScoreOk(coder, candidate);

            yield return candidate;
        }

        if (candidatesQualityOk) yield break;

        // no candidates, create a virtual candidate.
        await foreach (var snapPoint in coder.Network.Snap(coder.Settings.RoutingSettings.Profile, s =>
                           {
                               s.MaxDistance = maxVertexDistanceInMeter;
                           })
                           .ToAllAsync(lrpLocation.longitude, lrpLocation.latitude))
        {
            var location = snapPoint.LocationOnNetwork(coder.Network);
            var distance = location.DistanceEstimateInMeter(lrpLocation);
            if (distance > maxVertexDistanceInMeter) continue; // too far.

            var candidate = new ScoredCandidate<SnapPoint>(snapPoint, Score.New(Score.VertexDistance,
                $"The vertex score compare to max distance {maxVertexDistanceInMeter}",
                (float)System.Math.Max(0, (1.0 - (distance / maxVertexDistanceInMeter))),
                1));
            yield return candidate;
        }
    }
        
    /// <summary>
    /// Finds candidate edges starting at a given vertex matching a given fow and frc.
    /// </summary>
    /// <param name="coder">The coder.</param>
    /// <param name="candidate">The candidate location.</param>
    /// <param name="pointsForward">When true the fow, bearing and frc point forward.</param>
    /// <param name="fow">The fow.</param>
    /// <param name="frc">The frc.</param>
    /// <param name="bearing">The bearing.</param>
    /// <returns></returns>
    public static IEnumerable<ScoredCandidate<(SnapPoint snapPoint, bool direction)>> FindCandidateSnapPointAndDirectionFor(this Coder coder, ScoredCandidate<SnapPoint> candidate, 
        bool pointsForward, FormOfWay fow, FunctionalRoadClass frc, double bearing)
    {
        var enumerator = coder.Network.GetEdgeEnumerator();
        var snapper = coder.Network.Snap(coder.Settings.RoutingSettings.Profile);
        if (candidate.Candidate.IsVertex)
        {
            var vertex = candidate.Candidate.GetVertex(coder.Network);
            enumerator.MoveTo(vertex);
            while (enumerator.MoveNext())
            {
                var snapPoint = snapper.ToTail(enumerator);
                if (snapPoint.IsError) continue;

                var match = coder.Interpreter.Match(enumerator, fow, frc);
                var matchScore = Score.New(Score.MatchArc, "Metric indicating a match with fow, frc etc...",
                    match, 2);
                if (!(match > 0)) continue;
                
                // consider forward direction.
                var shape = enumerator.GetCompleteShape();
                var localBearing = shape.Bearing(!pointsForward);
                var localBearingDiff = Math.Abs(Extensions.AngleSmallestDifference(localBearing, bearing));

                var bearingScore = Score.New(Score.BearingDiff, "Bearing difference score (0=1 & 180=0)",
                    (1f - (localBearingDiff / 180f)) * 2, 2);

                yield return new ScoredCandidate<(SnapPoint snapPoint, bool direction)>(
                    (snapPoint, enumerator.Forward),
                    candidate.Score * (matchScore + bearingScore));
                    
                // consider backward direction.
                localBearing = shape.Bearing(pointsForward);
                localBearingDiff = Math.Abs(Extensions.AngleSmallestDifference(localBearing, bearing));

                bearingScore = Score.New(Score.BearingDiff, "Bearing difference score (0=1 & 180=0)",
                    (1f - (localBearingDiff / 180f)) * 2, 2);

                yield return new ScoredCandidate<(SnapPoint snapPoint, bool direction)>(
                    (snapPoint, !enumerator.Forward),
                    candidate.Score * (matchScore + bearingScore));
            }
        }
        else
        {
            enumerator.MoveTo(candidate.Candidate.EdgeId);
            var result = snapper.To(candidate.Candidate.EdgeId, candidate.Candidate.Offset);
            if (!result.IsError)
            {
                var match = coder.Interpreter.Match(enumerator, fow, frc);
                var matchScore = Score.New(Score.MatchArc, "Metric indicating a match with fow, frc etc...",
                    match, 2);
                if (match > 0)
                {
                    var shape = enumerator.GetShapeBetween(candidate.Candidate.Offset);
                    var localBearing = shape.Bearing(!pointsForward);
                    var localBearingDiff =
                        Math.Abs(Extensions.AngleSmallestDifference(localBearing, bearing));

                    var bearingScore = Score.New(Score.BearingDiff, "Bearing difference score (0=1 & 180=0)",
                        (1f - (localBearingDiff / 180f)) * 2, 2);
                    yield return new ScoredCandidate<(SnapPoint snapPoint, bool direction)>((result.Value, pointsForward ? enumerator.Forward : !enumerator.Forward), candidate.Score * (matchScore + bearingScore));
                }
            }
                
            enumerator.MoveTo(candidate.Candidate.EdgeId, false);
            result = snapper.To(candidate.Candidate.EdgeId, candidate.Candidate.Offset, false);
            if (!result.IsError)
            {
                var match = coder.Interpreter.Match(enumerator, fow, frc);
                var matchScore = Score.New(Score.MatchArc, "Metric indicating a match with fow, frc etc...",
                    match, 2);
                if (match > 0)
                {
                    var shape = enumerator.GetShapeBetween((ushort)(ushort.MaxValue - candidate.Candidate.Offset));
                    var localBearing = shape.Bearing(!pointsForward);
                    var localBearingDiff =
                        Math.Abs(Extensions.AngleSmallestDifference(localBearing, bearing));

                    var bearingScore = Score.New(Score.BearingDiff, "Bearing difference score (0=1 & 180=0)",
                        (1f - (localBearingDiff / 180f)) * 2, 2);
                    yield return new ScoredCandidate<(SnapPoint snapPoint, bool direction)>((result.Value, pointsForward ? enumerator.Forward : !enumerator.Forward), candidate.Score * (matchScore + bearingScore));
                }
            }
        }
    }

    /// <summary>
    /// Calculates a route between the two given vertices.
    /// </summary>
    public static async Task<ScoredCandidate<Path?>> FindCandidateRoute(this Coder coder, (SnapPoint snapPoint, bool direction) from, (SnapPoint snapPoint, bool direction) to, 
        FunctionalRoadClass minimum, double distanceToNext)
    {
        var router = coder.Network.Route(new RoutingSettings()
        {
            MaxDistance = distanceToNext * 4, Profile = coder.Settings.RoutingSettings.Profile,
        });

        var path = await router.From((from.snapPoint, from.direction)).To((to.snapPoint, to.direction)).PathAsync();

        if (path.IsError)
        {
            return new ScoredCandidate<Path?>(null, Score.New(Score.CandidateRoute, "Candidate route quality.", 0, 1));
        }

        return new ScoredCandidate<Path?>(path, Score.New(Score.CandidateRoute, "Candidate route quality.", 1, 1));
    }
}
