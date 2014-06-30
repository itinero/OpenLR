using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.OsmSharp.Decoding.Candidates;
using OpenLR.OsmSharp.Decoding.Scoring;
using OpenLR.OsmSharp.Locations;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.OsmSharp.Decoding
{
    /// <summary>
    /// Represents a referenced point along line location decoder.
    /// </summary>
    /// <typeparam name="TEdge"></typeparam>
    public class ReferencedPointAlongLineDecoder<TEdge> : ReferencedDecoder<ReferencedPointAlongLine<TEdge>, PointAlongLineLocation, TEdge>
        where TEdge : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Creates a point along line location graph decoder.
        /// </summary>
        /// <param name="mainDecoder"></param>
        /// <param name="rawDecoder"></param>
        /// <param name="graph"></param>
        /// <param name="router"></param>
        public ReferencedPointAlongLineDecoder(ReferencedDecoderBase<TEdge> mainDecoder, OpenLR.Decoding.LocationDecoder<PointAlongLineLocation> rawDecoder, IBasicRouterDataSource<TEdge> graph, IBasicRouter<TEdge> router)
            : base(mainDecoder, rawDecoder, graph, router)
        {

        }

        /// <summary>
        /// Decodes an OpenLR-encoded unreferenced raw OpenLR location into a referenced Location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override ReferencedPointAlongLine<TEdge> Decode(PointAlongLineLocation location)
        {
            // get candidate vertices and edges.
            var candidates = new List<SortedSet<CandidateVertexEdge<TEdge>>>();
            var lrps = new List<LocationReferencePoint>();

            // loop over all lrps.
            lrps.Add(location.First);
            candidates.Add(this.FindCandidatesFor(location.First, true));
            lrps.Add(location.Last);
            candidates.Add(this.FindCandidatesFor(location.Last, true));

            // build a list of combined scores.
            var combinedScores = new SortedSet<CombinedScore<TEdge>>(new CombinedScoreComparer<TEdge>());
            foreach (var previousCandidate in candidates[0])
            {
                foreach (var currentCandidate in candidates[1])
                {
                    combinedScores.Add(new CombinedScore<TEdge>()
                    {
                        Source = previousCandidate,
                        Target = currentCandidate
                    });
                }
            }

            // find the best candidate route.
            CandidateRoute<TEdge> best = null;
            while (combinedScores.Count > 0)
            {
                // get the first pair.
                var combinedScore = combinedScores.First();
                combinedScores.Remove(combinedScore);

                // find a route.
                var candidate = this.FindCandiateRoute(combinedScore.Source.Vertex, combinedScore.Target.Vertex,
                    lrps[0].LowestFunctionalRoadClassToNext.Value);

                // confirm first/last edge.
                // TODO: this part.

                // check candidate.
                if (best == null)
                { // there was no previous candidate.
                    best = candidate;
                }
                else if (best.Score < candidate.Score)
                { // the new candidate is better.
                    best = candidate;
                }
                else if (best.Score > candidate.Score)
                { // the current candidate is better.
                    break;
                }
            }

            // calculate the percentage value.
            var percentage = (double)location.PositiveOffset.Value / (double)location.First.DistanceToNext;

            // calculate the actual location.
            var longitudeReference = (location.First.Coordinate.Longitude - location.Last.Coordinate.Longitude) * percentage + location.Last.Coordinate.Longitude;
            var latitudeReference = (location.First.Coordinate.Latitude - location.Last.Coordinate.Latitude) * percentage + location.Last.Coordinate.Latitude;

            // create the referenced location.
            var pointAlongLineLocation = new ReferencedPointAlongLine<TEdge>();
            pointAlongLineLocation.Edge = best.Route.Edges[0];
            pointAlongLineLocation.VertexFrom = best.Route.Vertices[0];
            pointAlongLineLocation.VertexTo = best.Route.Vertices[1];
            pointAlongLineLocation.Latitude = latitudeReference;
            pointAlongLineLocation.Longitude = longitudeReference;
            if(location.Orientation.HasValue)
            {
                pointAlongLineLocation.Orientation = location.Orientation.Value;
            }
            else
            {
                pointAlongLineLocation.Orientation = Orientation.NoOrientation;
            }
            return pointAlongLineLocation;
        }
    }
}