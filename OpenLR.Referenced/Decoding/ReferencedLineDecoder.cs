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
            candidates.Add(this.MainDecoder.FindCandidatesFor(location.Last, true));

            // keep the total pathsegment.
            ReferencedLine lineLocation = null;
            Score lineLocationScore = null;

            // find a route between each pair of sequential points.
            var previous = lrps[0];
            var previousCandidates = candidates[0];
            for (int idx = 1; idx < lrps.Count; idx++)
            {
                CandidateRoute best = null;
                CombinedScore bestCombinedEdge = null;

                var current = lrps[idx];
                var currentCandidates = candidates[idx];

                // build a list of combined scores.
                var combinedScoresSet = new SortedSet<CombinedScore>(new CombinedScoreComparer());
                foreach (var previousCandidate in previousCandidates)
                {
                    foreach (var currentCandidate in currentCandidates)
                    {
                        if (previousCandidate.Vertex != currentCandidate.Vertex)
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
                    CandidateRoute candidate = null;
                    //if (idx < lrps.Count - 1)
                    //{ // from -> non-last location.
                    //    candidate = this.MainDecoder.FindCandidateRoute(combinedScore.Source, combinedScore.Target.Vertex,
                    //        lrps[idx].LowestFunctionalRoadClassToNext.Value);
                    //}
                    //else
                    //{ // non-last location -> last location.
                        candidate = this.MainDecoder.FindCandidateRoute(combinedScore.Source, combinedScore.Target,
                            lrps[idx].LowestFunctionalRoadClassToNext.Value);
                    //}

                    // bring score of from/to also into the mix.
                    candidate.Score = candidate.Score + combinedScore.Score;

                    // verify bearing by adding it to the score.
                    if (candidate != null && candidate.Route != null)
                    { // calculate bearing and compare with reference bearing.
                        // calculate distance and compare with distancetonext.
                        var distance = candidate.Route.GetCoordinates(this.MainDecoder).Length().Value;
                        var expectedDistance = location.First.DistanceToNext;
                        var distanceDiff = System.Math.Max(System.Math.Abs(distance - expectedDistance) - 200, 0); // don't care about difference smaller than 200m, the binary encoding only handles segments of about 50m.
                        var deviation = Score.New(Score.DISTANCE_COMPARISON, "Compares expected location distance with decoded location distance (1=prefect, 0=difference bigger than total distance)",
                            1 - System.Math.Min(System.Math.Max(distanceDiff / expectedDistance, 0), 1), 1);

                        // add deviation-score.
                        candidate.Score = candidate.Score * deviation;

                        if (candidate.Score.Value > this.MainDecoder.ScoreThreshold)
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

                // apend line location.
                if(lineLocation == null)
                { // no previous route.
                    lineLocation = best.Route;
                    lineLocationScore = best.Score;
                }
                else
                { // append.
                    lineLocation.Add(best.Route);
                    lineLocationScore = lineLocationScore + 
                        best.Score;
                }

                // assign new previous.
                previous = current;
                previousCandidates = currentCandidates;
            }

            // check if a location was found or not.
            if (lineLocation == null)
            { // no location could be found.
                throw new ReferencedDecodingException(location, "No valid location was found.");
            }

            // calculate offsets.
            var length = lineLocation.GetCoordinates(this.MainDecoder).Length().Value;
            lineLocation.NegativeOffset = 0;
            if(location.NegativeOffsetPercentage.HasValue)
            {
                lineLocation.NegativeOffset = (float)((location.NegativeOffsetPercentage.Value / 100) * length);
            }
            lineLocation.PositiveOffset = 0;
            if (location.PositiveOffsetPercentage.HasValue)
            {
                lineLocation.PositiveOffset = (float)((location.PositiveOffsetPercentage.Value / 100) * length);
            }
            return lineLocation;
        }
    }
}