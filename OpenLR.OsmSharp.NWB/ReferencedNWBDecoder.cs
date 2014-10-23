using OpenLR.Decoding;
using OpenLR.Model;
using OpenLR.OsmSharp.Decoding.Candidates;
using OpenLR.OsmSharp.Locations;
using OpenLR.OsmSharp.Matching;
using OpenLR.OsmSharp.Router;
using OpenLR.OsmSharp.Scoring;
using OsmSharp.Collections.Tags;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Shape;
using OsmSharp.Routing.Shape.Readers;
using OsmSharp.Units.Distance;
using System;
using System.Collections.Generic;

namespace OpenLR.OsmSharp.NWB
{
    /// <summary>
    /// An implementation of a referenced decoder based on the Nationaal Wegenbestand (NWB) in the netherlands.
    /// </summary>
    public class ReferencedNWBDecoder : ReferencedDecoderBaseLiveEdge
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
        public ReferencedNWBDecoder(BasicRouterDataSource<LiveEdge> graph, Decoder locationDecoder)
            : base(graph, new global::OsmSharp.Routing.Shape.Vehicles.Car("RIJRICHTING", "H", "T", string.Empty), locationDecoder)
        {

        }

        /// <summary>
        /// Creates a new referenced live edge decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="locationDecoder"></param>
        /// <param name="maxVertexDistance"></param>
        public ReferencedNWBDecoder(BasicRouterDataSource<LiveEdge> graph, Decoder locationDecoder, Meter maxVertexDistance)
            : base(graph, new global::OsmSharp.Routing.Shape.Vehicles.Car("RIJRICHTING", "H", "T", string.Empty), locationDecoder)
        {
            _maxVertexDistance = maxVertexDistance;
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
            FormOfWay actualFow;
            FunctionalRoadClass actualFrc;
            if(NWBMapping.ToOpenLR(tags, out actualFow, out actualFrc))
            { // a mapping was found. match and score.
                return MatchScoring.MatchAndScore(frc, fow, actualFrc, actualFow);
            }
            return 0;
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
            var edgeInterpreter = new ShapefileEdgeInterpreter();
            var interpreter = new ShapefileRoutingInterpreter();
            var path = this.GetRouter().Calculate(this.Graph, interpreter, this.Vehicle, from, to, minimum);

            // if no route is found, score is 0.
            if (path == null)
            {
                return new CandidateRoute<LiveEdge>()
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
                uint fromVertex = (uint)path.From.VertexId;
                uint toVertex = (uint)path.VertexId;

                var edgeDistance = double.MaxValue;
                var arcs = this.Graph.GetArcs(fromVertex);
                LiveEdge? edge = null;
                foreach (var arc in arcs)
                {
                    if (arc.Key == toVertex &&
                        arc.Value.Distance < edgeDistance)
                    { // there is a candidate arc.
                        edgeDistance = arc.Value.Distance;
                        edge = arc.Value;
                    }
                }

                if (!edge.HasValue)
                { // this should be impossible.
                    throw new Exception("No edge found between two consequtive vertices on a route.");
                }
                edges.Add(edge.Value);


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
            if (!this.Graph.GetVertex((uint)vertex, out latitude, out longitude))
            { // oeps, vertex does not exist!
                throw new ArgumentOutOfRangeException("vertex", string.Format("Vertex {0} not found!", vertex));
            }
            return new Coordinate()
            {
                Latitude = latitude,
                Longitude = longitude
            };
        }

        /// <summary>
        /// Creates a new referenced NWB decoder.
        /// </summary>
        /// <param name="folder">The folder containing the shapefile(s).</param>
        /// <param name="searchPattern">The search pattern to identify the relevant shapefiles.</param>
        /// <returns></returns>
        public static ReferencedNWBDecoder CreateBinary(string folder, string searchPattern)
        {
            return ReferencedNWBDecoder.Create(folder, searchPattern, new OpenLR.Binary.BinaryDecoder());
        }

        /// <summary>
        /// Creates a new referenced NWB decoder.
        /// </summary>
        /// <param name="graph">The graph containing the NWB network.</param>
        /// <returns></returns>
        public static ReferencedNWBDecoder CreateBinary(BasicRouterDataSource<LiveEdge> graph)
        {
            return ReferencedNWBDecoder.Create(graph, new OpenLR.Binary.BinaryDecoder());
        }

        /// <summary>
        /// Creates a new referenced NWB decoder.
        /// </summary>
        /// <param name="graph">The graph containing the NWB network.</param>
        /// <returns></returns>
        public static ReferencedNWBDecoder CreateBinary(IBasicRouterDataSource<LiveEdge> graph)
        {
            return ReferencedNWBDecoder.Create(new BasicRouterDataSource<LiveEdge>(graph), new OpenLR.Binary.BinaryDecoder());
        }

        /// <summary>
        /// Creates a new referenced NWB decoder.
        /// </summary>
        /// <param name="graph">The graph containing the NWB network.</param>
        /// <param name="maxVertexDistance">The maximum vertex distance.</param>
        /// <returns></returns>
        public static ReferencedNWBDecoder CreateBinary(IBasicRouterDataSource<LiveEdge> graph, Meter maxVertexDistance)
        {
            return ReferencedNWBDecoder.Create(new BasicRouterDataSource<LiveEdge>(graph), new OpenLR.Binary.BinaryDecoder(), maxVertexDistance);
        }

        /// <summary>
        /// Creates a new referenced NWB decoder.
        /// </summary>
        /// <param name="graph">The graph containing the NWB network.</param>
        /// <param name="maxVertexDistance">The maximum vertex distance.</param>
        /// <returns></returns>
        public static ReferencedNWBDecoder CreateBinary(BasicRouterDataSource<LiveEdge> graph, Meter maxVertexDistance)
        {
            return ReferencedNWBDecoder.Create(graph, new OpenLR.Binary.BinaryDecoder(), maxVertexDistance);
        }

        /// <summary>
        /// Creates a new referenced NWB decoder.
        /// </summary>
        /// <param name="folder">The folder containing the shapefile(s).</param>
        /// <param name="searchPattern">The search pattern to identify the relevant shapefiles.</param>
        /// <param name="rawLocationDecoder">The raw location decoder.</param>
        /// <returns></returns>
        public static ReferencedNWBDecoder Create(string folder, string searchPattern, Decoder rawLocationDecoder)
        {
            // create an instance of the graph reader and define the columns that contain the 'node-ids'.
            var graphReader = new ShapefileLiveGraphReader("JTE_ID_BEG", "JTE_ID_END");
            // read the graph from the folder where the shapefiles are placed.
            var graph = graphReader.Read(folder, searchPattern, new ShapefileRoutingInterpreter());

            return ReferencedNWBDecoder.Create(new BasicRouterDataSource<LiveEdge>(graph), rawLocationDecoder);
        }

        /// <summary>
        /// Creates a new referenced NWB decoder.
        /// </summary>
        /// <param name="graph">The graph containing the NWB network.</param>
        /// <param name="rawLocationDecoder">The raw location decoder.</param>
        /// <returns></returns>
        public static ReferencedNWBDecoder Create(BasicRouterDataSource<LiveEdge> graph, Decoder rawLocationDecoder)
        {
            return new ReferencedNWBDecoder(graph, rawLocationDecoder);
        }

        /// <summary>
        /// Creates a new referenced NWB decoder.
        /// </summary>
        /// <param name="graph">The graph containing the NWB network.</param>
        /// <param name="rawLocationDecoder">The raw location decoder.</param>
        /// <param name="maxVertexDistance">The maximum vertex distance.</param>
        /// <returns></returns>
        public static ReferencedNWBDecoder Create(BasicRouterDataSource<LiveEdge> graph, Decoder rawLocationDecoder, Meter maxVertexDistance)
        {
            return new ReferencedNWBDecoder(graph, rawLocationDecoder, maxVertexDistance);
        }

        /// <summary>
        /// Returns a value if a oneway restriction is found.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns>null: no restrictions, true: forward restriction, false: backward restriction.</returns>
        /// <returns></returns>
        public override bool? IsOneway(TagsCollectionBase tags)
        {
            var vehicle = new global::OsmSharp.Routing.Shape.Vehicles.Car("RIJRICHTING", "H", "T", string.Empty);
            return vehicle.IsOneWay(tags);
        }
    }
}