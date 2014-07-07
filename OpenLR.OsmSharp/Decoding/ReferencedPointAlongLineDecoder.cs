using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.OsmSharp.Decoding.Candidates;
using OpenLR.OsmSharp.Decoding.Scoring;
using OpenLR.OsmSharp.Locations;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Units.Angle;
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
            var fromBearingReference = (Degree)location.First.Bearing;
            var toBearingReference = (Degree)location.Last.Bearing;

            // loop over all lrps.
            lrps.Add(location.First);
            candidates.Add(this.FindCandidatesFor(location.First, true));
            lrps.Add(location.Last);
            candidates.Add(this.FindCandidatesFor(location.Last, true));

            // build a list of combined scores.
            var combinedScoresSet = new SortedSet<CombinedScore<TEdge>>(new CombinedScoreComparer<TEdge>());
            foreach (var previousCandidate in candidates[0])
            {
                foreach (var currentCandidate in candidates[1])
                {
                    combinedScoresSet.Add(new CombinedScore<TEdge>()
                    {
                        Source = previousCandidate,
                        Target = currentCandidate
                    });
                }
            }

            // find the best candidate route.
            CandidateRoute<TEdge> best = null;
            var combinedScores = new List<CombinedScore<TEdge>>(combinedScoresSet);
            while (combinedScores.Count > 0)
            {
                // get the first pair.
                var combinedScore = combinedScores.First();
                combinedScores.Remove(combinedScore);

                // find a route.
                var candidate = this.FindCandidateRoute(combinedScore.Source.Vertex, combinedScore.Target.Vertex,
                    lrps[0].LowestFunctionalRoadClassToNext.Value);

                // verify bearing by adding it to the score.
                if (candidate != null && candidate.Route != null)
                { // calculate bearing and compare with reference bearing.
                    //var fromBearing = this.GetBearing(candidate.Route.Vertices[0],
                    //    candidate.Route.Edges[0],
                    //    candidate.Route.Vertices[1],
                    //    true);
                    //var fromBearingDiff = System.Math.Abs(fromBearing.SmallestDifference(fromBearingReference));
                    //var toBearing = this.GetBearing(candidate.Route.Vertices[candidate.Route.Vertices.Length - 1],
                    //    candidate.Route.Edges[candidate.Route.Edges.Length - 1],
                    //    candidate.Route.Vertices[candidate.Route.Vertices.Length - 2],
                    //    false);
                    //var toBearingDiff = System.Math.Abs(toBearing.SmallestDifference(toBearingReference));
                    //var diffScore = 1.0 - (fromBearingDiff / 360.0) - (toBearingDiff / 360.0);
                    //candidate.Score = candidate.Score * (float)diffScore;
                }

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

            // check if a location was found or not.
            if(best == null || best.Route == null)
            { // no location could be found.
                throw new ReferencedDecodingException(location, "No valid location was found.");
            }

            // calculate the percentage value.
            var offsetRatio = 0.0;
            if(location.PositiveOffsetPercentage.HasValue)
            { // there is a percentage set.
                offsetRatio = location.PositiveOffsetPercentage.Value / 100.0;
            }

            // calculate the actual location and take into account the shape.
            var coordinates = best.Route.Edges[0].GetCoordinates(location.First.Coordinate.ToGeoCoordinate(), location.Last.Coordinate.ToGeoCoordinate());
            var referenceLocation = coordinates.GetPositionLocation(offsetRatio);

            var longitudeReference = referenceLocation.Longitude;
            var latitudeReference = referenceLocation.Latitude;

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