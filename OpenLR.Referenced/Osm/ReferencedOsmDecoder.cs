using OpenLR.Decoding;
using OpenLR.Model;
using OpenLR.Referenced.Decoding.Candidates;
using OpenLR.Referenced.Locations;
using OpenLR.Referenced.Router;
using OpenLR.Referenced.Scoring;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Shape;
using OsmSharp.Routing.Shape.Readers;
using OsmSharp.Units.Distance;
using System;
using System.Collections.Generic;

namespace OpenLR.Referenced.Osm
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
        /// <returns></returns>
        public override CandidateRoute FindCandidateRoute(CandidateVertexEdge from, CandidateVertexEdge to, FunctionalRoadClass minimum,
            bool ignoreFromEdge = false, bool ignoreToEdge = false)
        {
            var path = this.GetRouter().Calculate(this.Graph, new OsmRoutingInterpreter(), Vehicle.Car, from, to, minimum,
                ignoreFromEdge, ignoreToEdge);

            // if no route is found, score is 0.
            if (path == null)
            {
                return new CandidateRoute()
                {
                    Route = null,
                    Score = Score.New(Score.CANDIDATE_ROUTE, "Candidate route quality.", 0, 1)
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
                foreach (var arc in this.Graph.GetEdges(fromVertex))
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

            // fill shapes.
            var edgeShapes = new GeoCoordinateSimple[edges.Count][];
            for (int i = 0; i < edges.Count; i++)
            {
                edgeShapes[i] = this.Graph.GetEdgeShape(vertices[i], vertices[i + 1]);
            }

            return new CandidateRoute()
            {
                Route = new ReferencedLine(this.Graph)
                {
                    Edges = edges.ToArray(),
                    EdgeShapes = edgeShapes,
                    Vertices = vertices.ToArray()
                },
                Score = Score.New(Score.CANDIDATE_ROUTE, "Candidate route quality.", 1, 1)
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

        #region Static Creation Helper Functions

        /// <summary>
        /// Creates a new referenced Osm decoder.
        /// </summary>
        /// <param name="folder">The folder containing the shapefile(s).</param>
        /// <param name="searchPattern">The search pattern to identify the relevant shapefiles.</param>
        /// <returns></returns>
        public static ReferencedOsmDecoder CreateBinary(string folder, string searchPattern)
        {
            return ReferencedOsmDecoder.Create(folder, searchPattern, new OpenLR.Binary.BinaryDecoder());
        }

        /// <summary>
        /// Creates a new referenced Osm decoder.
        /// </summary>
        /// <param name="graph">The graph containing the Osm network.</param>
        /// <returns></returns>
        public static ReferencedOsmDecoder CreateBinary(BasicRouterDataSource<LiveEdge> graph)
        {
            return ReferencedOsmDecoder.Create(graph, new OpenLR.Binary.BinaryDecoder());
        }

        /// <summary>
        /// Creates a new referenced Osm decoder.
        /// </summary>
        /// <param name="graph">The graph containing the Osm network.</param>
        /// <returns></returns>
        public static ReferencedOsmDecoder CreateBinary(IBasicRouterDataSource<LiveEdge> graph)
        {
            return ReferencedOsmDecoder.Create(new BasicRouterDataSource<LiveEdge>(graph), new OpenLR.Binary.BinaryDecoder());
        }

        /// <summary>
        /// Creates a new referenced Osm decoder.
        /// </summary>
        /// <param name="graph">The graph containing the Osm network.</param>
        /// <param name="maxVertexDistance">The maximum vertex distance.</param>
        /// <returns></returns>
        public static ReferencedOsmDecoder CreateBinary(IBasicRouterDataSource<LiveEdge> graph, Meter maxVertexDistance)
        {
            return ReferencedOsmDecoder.Create(new BasicRouterDataSource<LiveEdge>(graph), new OpenLR.Binary.BinaryDecoder(), maxVertexDistance);
        }

        /// <summary>
        /// Creates a new referenced Osm decoder.
        /// </summary>
        /// <param name="graph">The graph containing the Osm network.</param>
        /// <param name="maxVertexDistance">The maximum vertex distance.</param>
        /// <returns></returns>
        public static ReferencedOsmDecoder CreateBinary(BasicRouterDataSource<LiveEdge> graph, Meter maxVertexDistance)
        {
            return ReferencedOsmDecoder.Create(graph, new OpenLR.Binary.BinaryDecoder(), maxVertexDistance);
        }

        /// <summary>
        /// Creates a new referenced Osm decoder.
        /// </summary>
        /// <param name="folder">The folder containing the shapefile(s).</param>
        /// <param name="searchPattern">The search pattern to identify the relevant shapefiles.</param>
        /// <param name="rawLocationDecoder">The raw location decoder.</param>
        /// <returns></returns>
        public static ReferencedOsmDecoder Create(string folder, string searchPattern, Decoder rawLocationDecoder)
        {
            // create an instance of the graph reader and define the columns that contain the 'node-ids'.
            var graphReader = new ShapefileLiveGraphReader("JTE_ID_BEG", "JTE_ID_END");
            // read the graph from the folder where the shapefiles are placed.
            var graph = graphReader.Read(folder, searchPattern, new ShapefileRoutingInterpreter());

            return ReferencedOsmDecoder.Create(new BasicRouterDataSource<LiveEdge>(graph), rawLocationDecoder);
        }

        /// <summary>
        /// Creates a new referenced Osm decoder.
        /// </summary>
        /// <param name="graph">The graph containing the Osm network.</param>
        /// <param name="rawLocationDecoder">The raw location decoder.</param>
        /// <returns></returns>
        public static ReferencedOsmDecoder Create(BasicRouterDataSource<LiveEdge> graph, Decoder rawLocationDecoder)
        {
            return new ReferencedOsmDecoder(graph, rawLocationDecoder);
        }

        /// <summary>
        /// Creates a new referenced Osm decoder.
        /// </summary>
        /// <param name="graph">The graph containing the Osm network.</param>
        /// <param name="rawLocationDecoder">The raw location decoder.</param>
        /// <param name="maxVertexDistance">The maximum vertex distance.</param>
        /// <returns></returns>
        public static ReferencedOsmDecoder Create(BasicRouterDataSource<LiveEdge> graph, Decoder rawLocationDecoder, Meter maxVertexDistance)
        {
            return new ReferencedOsmDecoder(graph, rawLocationDecoder, maxVertexDistance);
        }

        #endregion
    }
}