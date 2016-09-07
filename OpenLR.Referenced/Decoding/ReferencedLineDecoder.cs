using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.Referenced.Decoding.Candidates;
using OpenLR.Referenced.Decoding.Scoring;
using OpenLR.Referenced.Locations;
using OpenLR.Referenced.Router;
using OpenLR.Referenced.Scoring;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Units.Distance;
using System.Collections.Generic;
using System.Linq;

namespace OpenLR.Referenced.Decoding
{
    /// <summary>
    /// Represents a referenced line location decoder.
    /// </summary>
    public class ReferencedLineDecoder : ReferencedDecoder<ReferencedLine, LineLocation>
    {
        /// <summary>
        /// Creates a line location graph decoder.
        /// </summary>
        /// <param name="mainDecoder"></param>
        /// <param name="rawDecoder"></param>
        public ReferencedLineDecoder(ReferencedDecoderBase mainDecoder, OpenLR.Decoding.LocationDecoder<LineLocation> rawDecoder)
            : base(mainDecoder, rawDecoder)
        {

        }

        /// <summary>
        /// Decodes an OpenLR-encoded unreferenced raw OpenLR location into a referenced Location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override ReferencedLine Decode(LineLocation location)
        {
            // get candidate vertices and edges.
            var candidates = new List<SortedSet<CandidateVertexEdge>>();
            var lrps = new List<LocationReferencePoint>();

            // loop over all lrps.
            lrps.Add(location.First);
            candidates.Add(this.MainDecoder.FindCandidatesFor(location.First, true));
            if (location.Intermediate != null)
            { // there are intermediates.
                for (int idx = 0; idx < location.Intermediate.Length; idx++)
                {
                    lrps.Add(location.Intermediate[idx]);
                    candidates.Add(this.MainDecoder.FindCandidatesFor(location.Intermediate[idx], true));
                }
            }
            lrps.Add(location.Last);
            candidates.Add(this.MainDecoder.FindCandidatesFor(location.Last, false));

            // find a route between each pair of sequential points.
            // start with the last two points and move backwards.
            var target = lrps[lrps.Count - 1];
            var targetCandidates = candidates[candidates.Count - 1];
            var lineLocationSegments = new List<ReferencedLine>();
            for (int idx = lrps.Count - 2; idx >= 0; idx--)
            {
                var source = lrps[idx];
                var sourceCandidates = candidates[idx];

                // build a list of combined scores.
                var combinedScoresSet = new SortedSet<CombinedScore>(new CombinedScoreComparer());
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
                CandidateVertexEdge bestSource = null;
                while (combinedScores.Count > 0)
                {
                    // get the first pair.
                    var combinedScore = combinedScores[0];
                    combinedScores.RemoveAt(0);

                    // find a route.
                    var candidate = this.MainDecoder.FindCandidateRoute(combinedScore.Source, combinedScore.Target,
                        source.LowestFunctionalRoadClassToNext.Value, false, idx < lrps.Count - 2);

                    // bring score of from/to also into the mix.
                    candidate.Score = candidate.Score + combinedScore.Score;

                    // verify bearing by adding it to the score.
                    if (candidate != null && candidate.Route != null)
                    { // calculate bearing and compare with reference bearing.
                        // calculate distance and compare with distancetonext.
                        var distance = candidate.Route.GetCoordinates(this.MainDecoder.Graph).Length().Value;
                        var expectedDistance = source.DistanceToNext;

                        // default a perfect score, only compare large distances.
                        Score deviation = Score.New(Score.DISTANCE_COMPARISON,
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

                        if ((candidate.Score.Value / candidate.Score.Reference) > this.MainDecoder.ScoreThreshold)
                        {
                            // check candidate.
                            if (best == null)
                            { // there was no previous candidate or candidate has no route.
                                best = candidate;
                                bestSource = combinedScore.Source;
                            }
                            else if (best.Score.Value < candidate.Score.Value)
                            { // the new candidate is better.
                                best = candidate;
                                bestSource = combinedScore.Source;
                            }
                            else if (best.Score.Value > candidate.Score.Value)
                            { // the current candidate is better.
                                break;
                            }

                            if (best.Score.Value == 1)
                            { // stop search on a perfect scrore!
                                break;
                            }
                        }                    }
                }

                // append the current best.
                if (best == null || best.Route == null)
                { // no location reference found between two points.
                    return null;
                }
                // keep the segment.
                lineLocationSegments.Insert(0, best.Route);

                // assign new next.
                target = source;
                targetCandidates = new SortedSet<CandidateVertexEdge>();
                targetCandidates.Add(bestSource); // only the best source can be re-used for the next segment.
            }

            // build the line location from the segments.
            var lineLocation = lineLocationSegments[0];
            for (var i = 1; i < lineLocationSegments.Count; i++)
            {
                lineLocation.Add(lineLocationSegments[i]);
            }

            lineLocation.PositiveOffsetPercentage = location.PositiveOffsetPercentage.Value;
            lineLocation.NegativeOffsetPercentage = location.NegativeOffsetPercentage.Value;

            return lineLocation;
        }
    }
}