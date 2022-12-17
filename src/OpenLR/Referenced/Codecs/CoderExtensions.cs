using Itinero;
using OpenLR.Model;
using OpenLR.Referenced.Codecs.Candidates;
using OpenLR.Referenced.Locations;
using OpenLR.Referenced.Scoring;
using System;
using System.Collections.Generic;
using System.Linq;
using Itinero.Geo;
using Itinero.Network;
using Itinero.Snapping;

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
        public static Itinero.Algorithms.Collections.SortedSet<CandidateSnapPoint> FindCandidatesFor(this Coder coder, LocationReferencePoint lrp, bool forward, float maxVertexDistance = 40)
        {
            var vertexEdgeCandidates = new Itinero.Algorithms.Collections.SortedSet<CandidateSnapPoint>(new CandidateVertexEdgeComparer());
            var vertexCandidates = coder.FindCandidateLocationsFor(lrp, maxVertexDistance);
            foreach (var vertexCandidate in vertexCandidates)
            {
                var edgeCandidates = coder.FindCandidatePathSegmentsFor(vertexCandidate, forward, lrp.FormOfWay.Value, lrp.FuntionalRoadClass.Value,
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
        public static async IAsyncEnumerable<CandidateLocation> FindCandidateLocationsFor(this Coder coder,
            LocationReferencePoint lrp, double maxVertexDistanceInMeter = 40)
        {
            static bool ScoreOk(Coder coder, CandidateLocation candidate)
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
                var candidate = new CandidateLocation
                {
                    Score = Score.New(Score.VertexDistance,
                        $"The vertex score compare to max distance {maxVertexDistanceInMeter}",
                        (float)System.Math.Max(0, (1.0 - (distance / maxVertexDistanceInMeter))),
                        1), // calculate scoring compared to the fixed max distance.
                    Location = snapPoint
                };
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

                var candidate = new CandidateLocation
                {
                    Score = Score.New(Score.VertexDistance,
                        $"The vertex score compare to max distance {maxVertexDistanceInMeter}",
                        (float)System.Math.Max(0, (1.0 - (distance / maxVertexDistanceInMeter))),
                        1), // calculate scoring compared to the fixed max distance.
                    Location = snapPoint
                };
                yield return candidate;
            }
        }

        /// <summary>
        /// Finds candidate edges starting at a given vertex matching a given fow and frc.
        /// </summary>
        public static IEnumerable<CandidateSnapPoint> FindCandidatePathSegmentsFor(this Coder coder, CandidateLocation location, bool forward, FormOfWay fow, FunctionalRoadClass frc, 
            float bearing)
        {
            var costFunction = coder.Network.GetCostFunctionFor(coder.Settings.RoutingSettings.Profile);
            if (location.Location.IsVertex)
            {
                var vertex = location.Location.GetVertex(coder.Network);

                var enumerator = coder.Network.GetEdgeEnumerator();
                enumerator.MoveTo(vertex);
                while (enumerator.MoveNext())
                {
                    var factor = costFunction.Get(enumerator);
                    if (factor.cost <= 0) continue; // edge cannot be access by profile.

                    var match = coder.Interpreter.Match(enumerator, fow, frc);
                    if (match <= 0) continue; // match is really really bad.
                    
                    
                }
            }
            
            var getFactor = coder.Router.GetDefaultGetFactor(coder.Settings.Profile);
            var profile = coder.Settings;
            var relevantEdges = new List<CandidateSnapPoint>();
            if (location.Location.IsVertex())
            { // location is a vertex, probably 99% of the time.
                var vertex = location.Location.VertexId(coder.Router.Db);
                var edgeEnumerator = coder.Router.Db.Network.GetEdgeEnumerator(vertex);
                foreach (var edge in edgeEnumerator)
                {
                    var factor = getFactor(edge.Data.Profile);

                    if (factor.Value > 0 && (factor.Direction == 0 ||
                        (forward && (factor.Direction == 1) != edge.DataInverted) ||
                        (!forward && (factor.Direction == 1) == edge.DataInverted)))
                    {
                        var edgeProfile = coder.Router.Db.EdgeProfiles.Get(edge.Data.Profile);
                        var matchScore = Score.New(Score.MatchArc, "Metric indicating a match with fow, frc etc...",
                            profile.Match(edgeProfile, fow, frc), 2);
                        if (matchScore.Value > 0)
                        { // ok, there is a match.
                          // check bearing.
                            var shape = coder.Router.Db.Network.GetShape(edge);
                            var localBearing = BearingEncoder.EncodeBearing(shape, false);
                            var localBearingDiff = System.Math.Abs(Extensions.AngleSmallestDifference(localBearing, bearing));

                            var bearingScore = Score.New(Score.BearingDiff, "Bearing difference score (0=1 & 180=0)", (1f - (localBearingDiff / 180f)) * 2, 2);
                            relevantEdges.Add(new CandidateSnapPoint()
                            {
                                Score = location.Score * (matchScore + bearingScore),
                                Location = location.Location,
                                Path = new EdgePath<float>(edge.To, factor.Value * edge.Data.Distance, edge.IdDirected(), new EdgePath<float>(vertex))
                            });
                        }
                    }
                }
            }
            else
            { // location is not a vertex but a virtual point, try available directions.
                var paths = location.Location.ToEdgePaths(coder.Router.Db, coder.Settings.Profile.DefaultWeightHandlerCached(coder.Router.Db), forward);
                var edgeEnumerator = coder.Router.Db.Network.GetEdgeEnumerator();
                var locationOnNetwork = location.Location.LocationOnNetwork(coder.Router.Db);
                foreach (var path in paths)
                {
                    if (path.From == null)
                    {
                        throw new Exception("An LRP was resolved while at the same time it resolved to an exact vertex.");
                    }
                    edgeEnumerator.MoveToEdge(path.Edge);
                    var edge = edgeEnumerator.Current;
                    var edgeProfile = coder.Router.Db.EdgeProfiles.Get(edge.Data.Profile);

                    var matchScore = Score.New(Score.MatchArc, "Metric indicating a match with fow, frc etc...",
                        profile.Match(edgeProfile, fow, frc), 2);
                    if (matchScore.Value > 0)
                    { // ok, there is a match.
                      // check bearing.

                        // get shape from location -> path.
                        var shape = location.Location.ShapePointsTo(coder.Router.Db, path.Vertex);
                        shape.Insert(0, locationOnNetwork);
                        shape.Add(coder.Router.Db.Network.GetVertex(path.Vertex));

                        var localBearing = BearingEncoder.EncodeBearing(shape, false);
                        var localBearingDiff = System.Math.Abs(Extensions.AngleSmallestDifference(localBearing, bearing));

                        var bearingScore = Score.New(Score.BearingDiff, "Bearing difference score (0=1 & 180=0)", (1f - (localBearingDiff / 180f)) * 2, 2);
                        relevantEdges.Add(new CandidateSnapPoint()
                        {
                            Score = location.Score * (matchScore + bearingScore),
                            Location = location.Location,
                            Path = path
                        });
                    }
                }
            }
            return relevantEdges;
        }

        /// <summary>
        /// Calculates a route between the two given vertices.
        /// </summary>
        public static CandidateRoute FindCandidateRoute(this Coder coder, CandidateSnapPoint from, CandidateSnapPoint to, FunctionalRoadClass minimum,
            bool invertTargetEdge = true)
        {
            var weightHandler = coder.Settings.Profile.DefaultWeightHandler(coder.Router);

            var directedEdgeFrom = from.Path.Edge;
            var directedEdgeTo = -to.Path.Edge;
            if (!invertTargetEdge)
            {
                directedEdgeTo = -directedEdgeTo;
            }

            // TODO: probably the bug is one-edge routes.
            var path = coder.Router.TryCalculateRaw(coder.Settings.Profile, weightHandler,
                directedEdgeFrom, directedEdgeTo, coder.Settings.RoutingSettings);
            if (Itinero.LocalGeo.Coordinate.DistanceEstimateInMeter(from.Location.Location(), to.Location.Location()) > coder.Settings.MaxSearch / 2)
            {
                try
                {
                    coder.Settings.RoutingSettings.SetMaxSearch(coder.Settings.Profile.FullName, coder.Settings.MaxSearch * 4);
                    path = coder.Router.TryCalculateRaw(coder.Settings.Profile, weightHandler,
                        directedEdgeFrom, directedEdgeTo, coder.Settings.RoutingSettings);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    coder.Settings.RoutingSettings.SetMaxSearch(coder.Settings.Profile.FullName, coder.Settings.MaxSearch);
                }
            }

            // if no route is found, score is 0.
            if (path.IsError)
            {
                return new CandidateRoute()
                {
                    Route = null,
                    Score = Score.New(Score.CandidateRoute, "Candidate route quality.", 0, 1)
                };
            }

            var pathAsList = path.Value.ToList();
            var edges = new List<long>();
            var vertices = new List<uint>();
            for(var i = 0; i < pathAsList.Count; i++)
            {
                vertices.Add(pathAsList[i].Vertex);
                if (i > 0)
                {
                    if (pathAsList[i].Edge != Itinero.Constants.NO_EDGE &&
                        pathAsList[i].Edge != -Itinero.Constants.NO_EDGE)
                    {
                        edges.Add(pathAsList[i].Edge);
                    }
                    else
                    {
                        var edgeEnumerator = coder.Router.Db.Network.GeometricGraph.Graph.GetEdgeEnumerator();
                        float best;
                        var edge = edgeEnumerator.FindBestEdge(weightHandler, vertices[vertices.Count - 2],
                            vertices[vertices.Count - 1], out best);
                        edges.Add(edge);
                    }
                }
            }

            var startLocation = from.Location;
            if (!from.Location.IsVertex())
            { // first vertex is virtual.
                vertices[0] = Itinero.Constants.NO_VERTEX;
            }
            else
            { // make sure routerpoint has same edge.
                startLocation = coder.Router.Db.CreateRouterPointForEdgeAndVertex(edges[0], vertices[0]);
            }

            var endLocation = to.Location;
            if (!to.Location.IsVertex())
            { // last vertex is virtual.
                vertices[vertices.Count - 1] = Itinero.Constants.NO_VERTEX;
            }
            else
            { // make sure routerpoint has the same edge.
                endLocation = coder.Router.Db.CreateRouterPointForEdgeAndVertex(edges[edges.Count - 1], vertices[vertices.Count - 1]);
            }            

            return new CandidateRoute()
            {
                Route = new ReferencedLine()
                {
                    Edges = edges.ToArray(),
                    Vertices = vertices.ToArray(),
                    StartLocation = startLocation,
                    EndLocation = endLocation
                },
                Score = Score.New(Score.CandidateRoute, "Candidate route quality.", 1, 1)
            };
        }
    }
}
