using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.OsmSharp.Decoding.Candidates;
using OpenLR.OsmSharp.Decoding.Scoring;
using OpenLR.OsmSharp.Locations;
using OpenLR.OsmSharp.Router;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Units.Distance;
using System.Collections.Generic;
using System.Linq;

namespace OpenLR.OsmSharp.Decoding
{
    /// <summary>
    /// Represents a referenced line location decoder.
    /// </summary>
    /// <typeparam name="TEdge"></typeparam>
    public class ReferencedLineDecoder<TEdge> : ReferencedDecoder<ReferencedLine<TEdge>, LineLocation, TEdge>
        where TEdge : IGraphEdgeData
    {
        /// <summary>
        /// Creates a line location graph decoder.
        /// </summary>
        /// <param name="mainDecoder"></param>
        /// <param name="rawDecoder"></param>
        public ReferencedLineDecoder(ReferencedDecoderBase<TEdge> mainDecoder, OpenLR.Decoding.LocationDecoder<LineLocation> rawDecoder)
            : base(mainDecoder, rawDecoder)
        {

        }

        /// <summary>
        /// Decodes an OpenLR-encoded unreferenced raw OpenLR location into a referenced Location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override ReferencedLine<TEdge> Decode(LineLocation location)
        {
            // get candidate vertices and edges.
            var candidates = new List<SortedSet<CandidateVertexEdge<TEdge>>>();
            var lrps = new List<LocationReferencePoint>();

            // loop over all lrps.
            lrps.Add(location.First);
            candidates.Add(this.FindCandidatesFor(location.First, true));
            if (location.Intermediate != null)
            { // there are intermediates.
                for (int idx = 0; idx < location.Intermediate.Length; idx++)
                {
                    lrps.Add(location.Intermediate[idx]);
                    candidates.Add(this.FindCandidatesFor(location.Intermediate[idx], true));
                }
            }
            lrps.Add(location.Last);
            candidates.Add(this.FindCandidatesFor(location.Last, true));

            // keep the total pathsegment.
            ReferencedLine<TEdge> lineLocation = null;

            // find a route between each pair of sequential points.
            var previous = lrps[0];
            var previousCandidates = candidates[0];
            for (int idx = 1; idx < lrps.Count; idx++)
            {
                var current = lrps[idx];
                var currentCandidates = candidates[idx];

                // build a list of combined scores.
                var combinedScoresSet = new SortedSet<CombinedScore<TEdge>>(new CombinedScoreComparer<TEdge>());
                foreach (var previousCandidate in previousCandidates)
                {
                    foreach (var currentCandidate in currentCandidates)
                    {
                        combinedScoresSet.Add(new CombinedScore<TEdge>()
                            {
                                Source = previousCandidate,
                                Target = currentCandidate
                            });
                    }
                }
                var combinedScores = new List<CombinedScore<TEdge>>(combinedScoresSet);

                // find the best candidate route.
                CandidateRoute<TEdge> best = null;
                while (combinedScores.Count > 0)
                {
                    // get the first pair.
                    var combinedScore = combinedScores[0];
                    combinedScores.RemoveAt(0);

                    // find a route.
                    var candidate = this.FindCandidateRoute(combinedScore.Source, combinedScore.Target,
                        previous.LowestFunctionalRoadClassToNext.Value);

                    // confirm first/last edge.
                    // TODO: this part.

                    // check candidate.
                    if (best == null)
                    { // there was no previous candidate or candidate has no route.
                        best = candidate;
                    }
                    else if (best.Score.Value < candidate.Score.Value)
                    { // the new candidate is better.
                        best = candidate;
                    }
                    else if (best.Score.Value > candidate.Score.Value)
                    { // the current candidate is better.
                        break;
                    }

                    if (best.Score.Value == 1)
                    { // stop search on a perfect scrore!
                        break;
                    }
                }

                // append the current best.
                if (best == null || best.Route == null)
                { // no location reference found between two points.
                    return null;
                }
                if(lineLocation == null)
                { // no previous route.
                    lineLocation = best.Route;
                }
                else
                { // append.
                    lineLocation.Add(best.Route);
                }

                // assign new previous.
                previous = current;
                previousCandidates = currentCandidates;
            }

            return lineLocation;
        }
    }
}