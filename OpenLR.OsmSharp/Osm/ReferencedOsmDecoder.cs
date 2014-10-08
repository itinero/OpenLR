using OpenLR.Decoding;
using OpenLR.Model;
using OpenLR.OsmSharp.Decoding.Candidates;
using OpenLR.OsmSharp.Locations;
using OpenLR.OsmSharp.Router;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Units.Distance;
using System;
using System.Collections.Generic;

namespace OpenLR.OsmSharp.Osm
{
    /// <summary>
    /// An implementation of a referenced decoder based on OSM.
    /// </summary>
    public class ReferencedOsmDecoder : ReferencedDecoderBaseLiveEdge
    {
        /// <summary>
        /// Holds the maximum vertex distance.
        /// </summary>
        private Meter _maxVertexDistance = 40;

        /// <summary>
        /// Creates a new referenced live edge decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="locationDecoder"></param>
        public ReferencedOsmDecoder(BasicRouterDataSource<LiveEdge> graph, Decoder locationDecoder)
            : base(graph, Vehicle.Car, locationDecoder)
        {

        }

        /// <summary>
        /// Creates a new referenced live edge decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="locationDecoder"></param>
        /// <param name="maxVertexDistance"></param>
        public ReferencedOsmDecoder(BasicRouterDataSource<LiveEdge> graph, Decoder locationDecoder, Meter maxVertexDistance)
            : base(graph, Vehicle.Car, locationDecoder)
        {
            _maxVertexDistance = maxVertexDistance;
        }

        /// <summary>
        /// Returns the router.
        /// </summary>
        /// <returns></returns>
        protected override BasicRouter GetRouter()
        {
            return new BasicRouter();
        }

        /// <summary>
        /// Finds candidate vertices for a location reference point.
        /// </summary>
        /// <param name="lrp"></param>
        /// <returns></returns>
        public override IEnumerable<CandidateVertex> FindCandidateVerticesFor(LocationReferencePoint lrp)
        {
            return this.FindCandidateVerticesFor(lrp, _maxVertexDistance);
        }

        /// <summary>
        /// Calculates a match between the tags collection and the properties of the OpenLR location reference.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="fow"></param>
        /// <param name="frc"></param>
        /// <returns></returns>
        public override float MatchArc(TagsCollectionBase tags, FormOfWay fow, FunctionalRoadClass frc)
        {
            string highway;
            if (!tags.TryGetValue("highway", out highway))
            { // not even a highway tag!
                return 0;
            }

            // TODO: take into account form of way? Maybe not for OSM-data?
            switch (frc)
            { // check there reference values against OSM: http://wiki.openstreetmap.org/wiki/Highway
                case FunctionalRoadClass.Frc0: // main road.
                    if (highway == "motorway" || highway == "trunk")
                    {
                        return 1;
                    }
                    break;
                case FunctionalRoadClass.Frc1: // first class road.
                    if (highway == "primary" || highway == "primary_link")
                    {
                        return 1;
                    }
                    break;
                case FunctionalRoadClass.Frc2: // second class road.
                    if (highway == "secondary" || highway == "secondary_link")
                    {
                        return 1;
                    }
                    break;
                case FunctionalRoadClass.Frc3: // third class road.
                    if (highway == "tertiary" || highway == "tertiary_link")
                    {
                        return 1;
                    }
                    break;
                case FunctionalRoadClass.Frc4:
                    if (highway == "road" || highway == "road_link" ||
                        highway == "unclassified" || highway == "residential")
                    {
                        return 1;
                    }
                    break;
                case FunctionalRoadClass.Frc5:
                    if (highway == "road" || highway == "road_link" ||
                        highway == "unclassified" || highway == "residential" ||
                        highway == "living_street")
                    {
                        return 1;
                    }
                    break;
                case FunctionalRoadClass.Frc6:
                    if (highway == "road" || highway == "track" ||
                        highway == "unclassified" || highway == "residential" ||
                        highway == "living_street")
                    {
                        return 1;
                    }
                    break;
                case FunctionalRoadClass.Frc7: // other class road.
                    if (highway == "footway" || highway == "bridleway" ||
                        highway == "steps" || highway == "path" ||
                        highway == "living_street")
                    {
                        return 1;
                    }
                    break;
            }

            if (highway != null && highway.Length > 0)
            { // for any other highway return a low match.
                return 0.2f;
            }
            return 0;
        }

        /// <summary>
        /// Returns a value if a oneway restriction is found.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns>null: no restrictions, true: forward restriction, false: backward restriction.</returns>
        /// <returns></returns>
        public override bool? IsOneway(TagsCollectionBase tags)
        {
            return Vehicle.Car.IsOneWay(tags);
        }

        /// <summary>
        /// Calculates a route between the two given vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="minimum">The minimum FRC.</param>
        /// <returns></returns>
        public override CandidateRoute<LiveEdge> FindCandiateRoute(CandidateVertexEdge<LiveEdge> from, CandidateVertexEdge<LiveEdge> to, FunctionalRoadClass minimum)
        {
            var path = this.GetRouter().Calculate(this.Graph, new OsmRoutingInterpreter(), Vehicle.Car, from, to, minimum);

            // if no route is found, score is 0.
            if (path == null)
            {
                return new CandidateRoute<LiveEdge>()
                {
                    Route = null,
                    Score = 0
                };
            }

            var edges = new List<LiveEdge>();
            var vertices = new List<long>();
            vertices.Add(path.VertexId);
            while (path.From != null)
            {
                // add to vertices list.
                vertices.Add(path.From.VertexId);

                // get edge between current and from.
                var fromVertex = path.From.VertexId;
                var toVertex = path.VertexId;

                bool found = false;
                foreach (var arc in this.Graph.GetArcs(fromVertex))
                {
                    if (arc.Key == toVertex)
                    { // there is a candidate arc.
                        found = true;
                        edges.Add(arc.Value);
                    }
                }

                if (!found)
                { // this should be impossible.
                    throw new Exception("No edge found between two consequtive vertices on a route.");
                }

                // move to next segment.
                path = path.From;
            }

            // reverse lists.
            edges.Reverse();
            vertices.Reverse();

            return new CandidateRoute<LiveEdge>()
            {
                Route = new ReferencedLine<LiveEdge>(this.Graph)
                {
                    Edges = edges.ToArray(),
                    Vertices = vertices.ToArray()
                },
                Score = 1
            };
        }

        /// <summary>
        /// Returns the coordinate of the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public override Coordinate GetCoordinate(long vertex)
        {
            float latitude, longitude;
            if (!this.Graph.GetVertex(vertex, out latitude, out longitude))
            { // oeps, vertex does not exist!
                throw new ArgumentOutOfRangeException("vertex", string.Format("Vertex {0} not found!", vertex));
            }
            return new Coordinate()
            {
                Latitude = latitude,
                Longitude = longitude
            };
        }
    }
}