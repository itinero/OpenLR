using OpenLR.Decoding;
using OpenLR.Model;
using OpenLR.Referenced.Decoding.Candidates;
using OpenLR.Referenced.Locations;
using OpenLR.Referenced.Router;
using OpenLR.Referenced.Scoring;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Shape;
using OsmSharp.Routing.Shape.Readers;
using OsmSharp.Units.Distance;
using System;
using System.Collections.Generic;

namespace OpenLR.Referenced.MultiNet
{
    /// <summary>
    /// An implementation of a referenced decoder based on TomTom MultiNet.
    /// </summary>
    public class ReferencedMultiNetDecoder : ReferencedDecoderBaseLiveEdge
    {
        /// <summary>
        /// Creates a new referenced live edge decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="locationDecoder"></param>
        public ReferencedMultiNetDecoder(BasicRouterDataSource<LiveEdge> graph, Decoder locationDecoder)
            : base(graph, new global::OsmSharp.Routing.Shape.Vehicles.Car("ONEWAY", "FT", "TF", string.Empty), locationDecoder)
        {

        }

        /// <summary>
        /// Creates a new referenced live edge decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="locationDecoder"></param>
        /// <param name="maxVertexDistance"></param>
        public ReferencedMultiNetDecoder(BasicRouterDataSource<LiveEdge> graph, Decoder locationDecoder, Meter maxVertexDistance)
            : base(graph, new global::OsmSharp.Routing.Shape.Vehicles.Car("ONEWAY", "FT", "TF", string.Empty), locationDecoder, maxVertexDistance)
        {

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
            string frcString;
            if (!tags.TryGetValue("FRC", out frcString))
            { // not even an FRC column.
                return 0;
            }

            float frcOneDifferent = 0.8f;
            float frcTwoDifferent = 0.6f;
            float frcMoreDifferent = 0.4f;

            var frcScore = 0.0f;
            switch (frcString)
            {
                case "0": // main road.
                    if(frc == FunctionalRoadClass.Frc0)
                    {
                        frcScore = 1;
                    }
                    else if(frc == FunctionalRoadClass.Frc1)
                    {
                        frcScore = frcOneDifferent;
                    }
                    else if(frc == FunctionalRoadClass.Frc2)
                    {
                        frcScore = frcTwoDifferent;
                    }
                    else
                    {
                        frcScore = frcMoreDifferent;
                    }
                    break;
                case "1": // first class road.
                    if (frc == FunctionalRoadClass.Frc1)
                    {
                        frcScore = 1;
                    }
                    else if(frc == FunctionalRoadClass.Frc0 || 
                        frc == FunctionalRoadClass.Frc2)
                    {
                        frcScore = frcOneDifferent;
                    }
                    else if(frc == FunctionalRoadClass.Frc3)
                    {
                        frcScore = frcTwoDifferent;
                    }
                    else
                    {
                        frcScore = frcMoreDifferent;
                    }
                    break;
                case "2": // second class road.
                    if (frc == FunctionalRoadClass.Frc2)
                    {
                        frcScore = 1;
                    }
                    else if (frc == FunctionalRoadClass.Frc1 ||
                        frc == FunctionalRoadClass.Frc3)
                    {
                        frcScore = frcOneDifferent;
                    }
                    else if (frc == FunctionalRoadClass.Frc4 ||
                        frc == FunctionalRoadClass.Frc0)
                    {
                        frcScore = frcTwoDifferent;
                    }
                    else
                    {
                        frcScore = frcMoreDifferent;
                    }
                    break;
                case "3":
                    if (frc == FunctionalRoadClass.Frc3)
                    {
                        frcScore = 1;
                    }
                    else if (frc == FunctionalRoadClass.Frc2 ||
                        frc == FunctionalRoadClass.Frc4)
                    {
                        frcScore = frcOneDifferent;
                    }
                    else if (frc == FunctionalRoadClass.Frc1 ||
                        frc == FunctionalRoadClass.Frc5)
                    {
                        frcScore = frcTwoDifferent;
                    }
                    else
                    {
                        frcScore = frcMoreDifferent;
                    }
                    break;
                case "4":
                    if (frc == FunctionalRoadClass.Frc4)
                    {
                        frcScore = 1;
                    }
                    else if (frc == FunctionalRoadClass.Frc3 ||
                        frc == FunctionalRoadClass.Frc5)
                    {
                        frcScore = frcOneDifferent;
                    }
                    else if (frc == FunctionalRoadClass.Frc2 ||
                        frc == FunctionalRoadClass.Frc6)
                    {
                        frcScore = frcTwoDifferent;
                    }
                    else
                    {
                        frcScore = frcMoreDifferent;
                    }
                    break;
                case "5":
                    if (frc == FunctionalRoadClass.Frc5)
                    {
                        frcScore = 1;
                    }
                    else if (frc == FunctionalRoadClass.Frc4 ||
                        frc == FunctionalRoadClass.Frc6)
                    {
                        frcScore = frcOneDifferent;
                    }
                    else if (frc == FunctionalRoadClass.Frc3 ||
                        frc == FunctionalRoadClass.Frc7)
                    {
                        frcScore = frcTwoDifferent;
                    }
                    else
                    {
                        frcScore = frcMoreDifferent;
                    }
                    break;
                case "6":
                    if (frc == FunctionalRoadClass.Frc6)
                    {
                        frcScore = 1;
                    }
                    else if (frc == FunctionalRoadClass.Frc5 ||
                        frc == FunctionalRoadClass.Frc7)
                    {
                        frcScore = frcOneDifferent;
                    }
                    else if (frc == FunctionalRoadClass.Frc4)
                    {
                        frcScore = frcTwoDifferent;
                    }
                    else
                    {
                        frcScore = frcMoreDifferent;
                    }
                    break;
                case "7":
                    if (frc == FunctionalRoadClass.Frc7)
                    {
                        frcScore = 1;
                    }
                    else if (frc == FunctionalRoadClass.Frc6)
                    {
                        frcScore = frcOneDifferent;
                    }
                    else if (frc == FunctionalRoadClass.Frc5)
                    {
                        frcScore = frcTwoDifferent;
                    }
                    else
                    {
                        frcScore = frcMoreDifferent;
                    }
                    break;
                case "8":
                    if (frc == FunctionalRoadClass.Frc7)
                    {
                        frcScore = frcOneDifferent;
                    }
                    else if (frc == FunctionalRoadClass.Frc6)
                    {
                        frcScore = frcTwoDifferent;
                    }
                    else
                    {
                        frcScore = frcMoreDifferent;
                    }
                    break;
            }
            
            float fowOneDifferent = 0.8f;
            float fowMoreDifferent = 0.4f;

            var fowScore = 0.0f;
            string fowString;
            if (tags.TryGetValue("FOW", out fowString))
            {
                fowScore = 0.2f;
                switch (fowString)
                {
                    case "1":
                        if(fow == FormOfWay.Motorway)
                        {
                            fowScore = 1;
                        }
                        else if(fow == FormOfWay.MultipleCarriageWay)
                        {
                            fowScore = fowOneDifferent;
                        }
                        break;
                    case "2":
                        if (fow == FormOfWay.MultipleCarriageWay)
                        {
                            fowScore = 1;
                        }
                        else if (fow == FormOfWay.Motorway ||
                            fow == FormOfWay.SingleCarriageWay)
                        {
                            fowScore = fowOneDifferent;
                        }
                        break;
                    case "3":
                        if (fow == FormOfWay.SingleCarriageWay)
                        {
                            fowScore = 1;
                        }
                        else if (fow == FormOfWay.MultipleCarriageWay)
                        {
                            fowScore = fowOneDifferent;
                        }
                        break;
                    case "4":
                        if (fow == FormOfWay.Roundabout)
                        {
                            fowScore = 1;
                        }
                        fowScore = fowMoreDifferent;
                        break;
                    case "8":
                        if (fow == FormOfWay.TrafficSquare)
                        {
                            fowScore = 1;
                        }
                        fowScore = fowMoreDifferent;
                        break;
                    case "10":
                        if (fow == FormOfWay.SlipRoad)
                        {
                            fowScore = 1;
                        }
                        fowScore = fowMoreDifferent;
                        break;
                    case "6":
                    case "7":
                    case "9":
                    case "11":
                    case "12":
                    case "14":
                    case "15":
                        if(fow == FormOfWay.Other)
                        {
                            fowScore = fowOneDifferent;
                        }
                        break;
                    default:
                        if(fow == FormOfWay.Undefined)
                        {
                            fowScore = fowMoreDifferent;
                        }
                        break;
                }
            }
            return frcScore + fowScore;
        }

        /// <summary>
        /// Calculates a route between the two given vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="minimum">The minimum FRC.</param>
        /// <returns></returns>
        public override CandidateRoute FindCandidateRoute(CandidateVertexEdge from, CandidateVertexEdge to, FunctionalRoadClass minimum)
        {
            var edgeInterpreter = new ShapefileEdgeInterpreter();
            var interpreter = new ShapefileRoutingInterpreter();
            var path = this.GetRouter().Calculate(this.Graph, interpreter, this.Vehicle, from, to, minimum);

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
                        if (arc.Value.Forward)
                        {
                            edges.Add(arc.Value);
                        }
                        else
                        {
                            edges.Add(arc.Value.ToReverse());
                        }
                        break;
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

        /// <summary>
        /// Returns a value if a oneway restriction is found.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns>null: no restrictions, true: forward restriction, false: backward restriction.</returns>
        /// <returns></returns>
        public override bool? IsOneway(TagsCollectionBase tags)
        {
            return this.Vehicle.IsOneWay(tags);
        }

        #region Static Creation Helper Functions

        /// <summary>
        /// Creates a new referenced MultiNet decoder.
        /// </summary>
        /// <param name="folder">The folder containing the shapefile(s).</param>
        /// <param name="searchPattern">The search pattern to identify the relevant shapefiles.</param>
        /// <returns></returns>
        public static ReferencedMultiNetDecoder CreateBinary(string folder, string searchPattern)
        {
            return ReferencedMultiNetDecoder.Create(folder, searchPattern, new OpenLR.Binary.BinaryDecoder());
        }

        /// <summary>
        /// Creates a new referenced MultiNet decoder.
        /// </summary>
        /// <param name="graph">The graph containing the MultiNet network.</param>
        /// <returns></returns>
        public static ReferencedMultiNetDecoder CreateBinary(BasicRouterDataSource<LiveEdge> graph)
        {
            return ReferencedMultiNetDecoder.Create(graph, new OpenLR.Binary.BinaryDecoder());
        }

        /// <summary>
        /// Creates a new referenced MultiNet decoder.
        /// </summary>
        /// <param name="graph">The graph containing the MultiNet network.</param>
        /// <returns></returns>
        public static ReferencedMultiNetDecoder CreateBinary(IBasicRouterDataSource<LiveEdge> graph)
        {
            return ReferencedMultiNetDecoder.Create(new BasicRouterDataSource<LiveEdge>(graph), new OpenLR.Binary.BinaryDecoder());
        }

        /// <summary>
        /// Creates a new referenced MultiNet decoder.
        /// </summary>
        /// <param name="graph">The graph containing the MultiNet network.</param>
        /// <param name="maxVertexDistance">The maximum vertex distance.</param>
        /// <returns></returns>
        public static ReferencedMultiNetDecoder CreateBinary(IBasicRouterDataSource<LiveEdge> graph, Meter maxVertexDistance)
        {
            return ReferencedMultiNetDecoder.Create(new BasicRouterDataSource<LiveEdge>(graph), new OpenLR.Binary.BinaryDecoder(), maxVertexDistance);
        }

        /// <summary>
        /// Creates a new referenced MultiNet decoder.
        /// </summary>
        /// <param name="graph">The graph containing the MultiNet network.</param>
        /// <param name="maxVertexDistance">The maximum vertex distance.</param>
        /// <returns></returns>
        public static ReferencedMultiNetDecoder CreateBinary(BasicRouterDataSource<LiveEdge> graph, Meter maxVertexDistance)
        {
            return ReferencedMultiNetDecoder.Create(graph, new OpenLR.Binary.BinaryDecoder(), maxVertexDistance);
        }

        /// <summary>
        /// Creates a new referenced MultiNet decoder.
        /// </summary>
        /// <param name="folder">The folder containing the shapefile(s).</param>
        /// <param name="searchPattern">The search pattern to identify the relevant shapefiles.</param>
        /// <param name="rawLocationDecoder">The raw location decoder.</param>
        /// <returns></returns>
        public static ReferencedMultiNetDecoder Create(string folder, string searchPattern, Decoder rawLocationDecoder)
        {
            // create an instance of the graph reader and define the columns that contain the 'node-ids'.
            var graphReader = new ShapefileLiveGraphReader("JTE_ID_BEG", "JTE_ID_END");
            // read the graph from the folder where the shapefiles are placed.
            var graph = graphReader.Read(folder, searchPattern, new ShapefileRoutingInterpreter());

            return ReferencedMultiNetDecoder.Create(new BasicRouterDataSource<LiveEdge>(graph), rawLocationDecoder);
        }

        /// <summary>
        /// Creates a new referenced MultiNet decoder.
        /// </summary>
        /// <param name="graph">The graph containing the MultiNet network.</param>
        /// <param name="rawLocationDecoder">The raw location decoder.</param>
        /// <returns></returns>
        public static ReferencedMultiNetDecoder Create(BasicRouterDataSource<LiveEdge> graph, Decoder rawLocationDecoder)
        {
            return new ReferencedMultiNetDecoder(graph, rawLocationDecoder);
        }

        /// <summary>
        /// Creates a new referenced MultiNet decoder.
        /// </summary>
        /// <param name="graph">The graph containing the MultiNet network.</param>
        /// <param name="rawLocationDecoder">The raw location decoder.</param>
        /// <param name="maxVertexDistance">The maximum vertex distance.</param>
        /// <returns></returns>
        public static ReferencedMultiNetDecoder Create(BasicRouterDataSource<LiveEdge> graph, Decoder rawLocationDecoder, Meter maxVertexDistance)
        {
            return new ReferencedMultiNetDecoder(graph, rawLocationDecoder, maxVertexDistance);
        }

        #endregion
    }
}