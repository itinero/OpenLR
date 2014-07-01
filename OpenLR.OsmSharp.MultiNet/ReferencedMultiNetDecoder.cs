using OpenLR.Decoding;
using OpenLR.Model;
using OpenLR.OsmSharp.Decoding.Candidates;
using OpenLR.OsmSharp.Locations;
using OsmSharp.Collections.Tags;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Shape;
using OsmSharp.Routing.Shape.Readers;
using OsmSharp.Routing.Shape.Vehicles;
using OsmSharp.Units.Distance;
using System;
using System.Collections.Generic;

namespace OpenLR.OsmSharp.MultiNet
{
    /// <summary>
    /// An implementation of a referenced decoder based on TomTom MultiNet.
    /// </summary>
    public class ReferencedMultiNetDecoder : ReferencedDecoderBase<LiveEdge>
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
        public ReferencedMultiNetDecoder(IBasicRouterDataSource<LiveEdge> graph, Decoder locationDecoder)
            : base(graph, locationDecoder)
        {

        }

        /// <summary>
        /// Creates a new referenced live edge decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="locationDecoder"></param>
        /// <param name="maxVertexDistance"></param>
        public ReferencedMultiNetDecoder(IBasicRouterDataSource<LiveEdge> graph, Decoder locationDecoder, Meter maxVertexDistance)
            : base(graph, locationDecoder)
        {
            _maxVertexDistance = maxVertexDistance;
        }

        /// <summary>
        /// Gets a new router.
        /// </summary>
        /// <returns></returns>
        protected override IBasicRouter<LiveEdge> GetRouter()
        {
            return new DykstraRoutingLive();
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
            string frcString;
            if (!tags.TryGetValue("FRC", out frcString))
            { // not even an FRC column.
                return 0;
            }

            switch (frcString)
            {
                case "0": // main road.
                    if(frc == FunctionalRoadClass.Frc0)
                    {
                        return 1;
                    }
                    break;
                case "1": // first class road.
                    if (frc == FunctionalRoadClass.Frc1)
                    {
                        return 1;
                    }
                    break;
                case "2": // second class road.
                    if (frc == FunctionalRoadClass.Frc2)
                    {
                        return 1;
                    }
                    break;
                case "3":
                    if (frc == FunctionalRoadClass.Frc3)
                    {
                        return 1;
                    }
                    break;
                case "4":
                    if (frc == FunctionalRoadClass.Frc4)
                    {
                        return 1;
                    }
                    break;
                case "5":
                    if (frc == FunctionalRoadClass.Frc5)
                    {
                        return 1;
                    }
                    break;
                case "6":
                    if (frc == FunctionalRoadClass.Frc6)
                    {
                        return 1;
                    }
                    break;
                case "7":
                    if (frc == FunctionalRoadClass.Frc7)
                    {
                        return 1;
                    }
                    break;
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
        public override CandidateRoute<LiveEdge> FindCandiateRoute(uint from, uint to, FunctionalRoadClass minimum)
        {
            var fromList = new PathSegmentVisitList();
            fromList.UpdateVertex(new PathSegment<long>(from));
            var toList = new PathSegmentVisitList();
            toList.UpdateVertex(new PathSegment<long>(to));

            var vehicle = new global::OsmSharp.Routing.Shape.Vehicles.Car("ONEWAY", "FT", "TF", string.Empty); // define vehicle with the column and values that define the onway restrictions and (optional) the speed in KPH.
            var edgeInterpreter = new ShapefileEdgeInterpreter();
            var interpreter = new ShapefileRoutingInterpreter();
            var path = this.GetRouter().Calculate(this.Graph, interpreter, vehicle, fromList, toList, double.MaxValue, null);

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
                uint fromVertex = (uint)path.From.VertexId;
                uint toVertex = (uint)path.VertexId;

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
        public override Coordinate GetVertexLocation(long vertex)
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
        /// Creates a new referenced multinet decoder.
        /// </summary>
        /// <param name="folder">The folder containing the shapefile(s).</param>
        /// <param name="searchPattern">The search pattern to identify the relevant shapefiles.</param>
        /// <returns></returns>
        public static ReferencedMultiNetDecoder CreateBinary(string folder, string searchPattern)
        {
            return ReferencedMultiNetDecoder.Create(folder, searchPattern, new OpenLR.Binary.BinaryDecoder());
        }

        /// <summary>
        /// Creates a new referenced multinet decoder.
        /// </summary>
        /// <param name="graph">The graph containing the multinet network.</param>
        /// <returns></returns>
        public static ReferencedMultiNetDecoder CreateBinary(IBasicRouterDataSource<LiveEdge> graph)
        {
            return ReferencedMultiNetDecoder.Create(graph, new OpenLR.Binary.BinaryDecoder());
        }

        /// <summary>
        /// Creates a new referenced multinet decoder.
        /// </summary>
        /// <param name="folder">The folder containing the shapefile(s).</param>
        /// <param name="searchPattern">The search pattern to identify the relevant shapefiles.</param>
        /// <param name="rawLocationDecoder">The raw location decoder.</param>
        /// <returns></returns>
        public static ReferencedMultiNetDecoder Create(string folder, string searchPattern, Decoder rawLocationDecoder)
        {
            // create an instance of the graph reader and define the columns that contain the 'node-ids'.
            var graphReader = new ShapefileLiveGraphReader("F_JNCTID", "T_JNCTID");
            // read the graph from the folder where the shapefiles are placed.
            var graph = graphReader.Read(folder, searchPattern, new ShapefileRoutingInterpreter());

            return new ReferencedMultiNetDecoder(graph, rawLocationDecoder);
        }

        /// <summary>
        /// Creates a new referenced multinet decoder.
        /// </summary>
        /// <param name="graph">The graph containing the multinet network.</param>
        /// <param name="rawLocationDecoder">The raw location decoder.</param>
        /// <returns></returns>
        public static ReferencedMultiNetDecoder Create(IBasicRouterDataSource<LiveEdge> graph, Decoder rawLocationDecoder)
        {
            return new ReferencedMultiNetDecoder(graph, rawLocationDecoder);
        }

        /// <summary>
        /// Returns a value if a oneway restriction is found.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns>null: no restrictions, true: forward restriction, false: backward restriction.</returns>
        /// <returns></returns>
        public override bool? IsOneway(TagsCollectionBase tags)
        {
            var vehicle = new global::OsmSharp.Routing.Shape.Vehicles.Car("ONEWAY", "FT", "TF", string.Empty);
            return vehicle.IsOneWay(tags);
        }
    }
}