using OpenLR.Encoding;
using OpenLR.Model;
using OpenLR.OsmSharp.Router;
using OsmSharp.Collections.Tags;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Shape;
using OsmSharp.Routing.Shape.Readers;
using System;

namespace OpenLR.OsmSharp.MultiNet
{
    /// <summary>
    /// An implementation of a referenced encoder based on TomTom MultiNet.
    /// </summary>
    public class ReferencedMultiNetEncoder : ReferencedEncoderBaseLiveEdge
    {
        /// <summary>
        /// Creates a new referenced live edge decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="locationEncoder"></param>
        public ReferencedMultiNetEncoder(BasicRouterDataSource<LiveEdge> graph, Encoder locationEncoder)
            : base(graph, locationEncoder)
        {

        }

        /// <summary>
        /// Returns the location of the given vertex.
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
        /// Returns the tags associated with the given tags id.
        /// </summary>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        public override TagsCollectionBase GetTags(uint tagsId)
        {
            return this.Graph.TagsIndex.Get(tagsId);
        }

        /// <summary>
        /// Tries to match the given tags and figure out a corresponding frc and fow.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="frc"></param>
        /// <param name="fow"></param>
        /// <returns>False if no matching was found.</returns>
        public override bool TryMatching(TagsCollectionBase tags, out FunctionalRoadClass frc, out FormOfWay fow)
        {
            frc = FunctionalRoadClass.Frc7;
            fow = FormOfWay.Undefined;
            string frcValue;
            if (tags.TryGetValue("FRC", out frcValue))
            {
                switch (frcValue)
                {
                    case "0": // main road.
                        frc = FunctionalRoadClass.Frc0;
                        break;
                    case "1": // main road.
                        frc = FunctionalRoadClass.Frc1;
                        break;
                    case "2": // main road.
                        frc = FunctionalRoadClass.Frc2;
                        break;
                    case "3": // main road.
                        frc = FunctionalRoadClass.Frc3;
                        break;
                    case "4": // main road.
                        frc = FunctionalRoadClass.Frc4;
                        break;
                    case "5": // main road.
                        frc = FunctionalRoadClass.Frc5;
                        break;
                    case "6": // main road.
                        frc = FunctionalRoadClass.Frc6;
                        break;
                    case "7": // main road.
                        frc = FunctionalRoadClass.Frc7;
                        break;
                }
            }
            string fowValue;
            if (tags.TryGetValue("FOW", out fowValue))
            {
                switch (fowValue)
                {
                    case "1": // main road.
                        fow = FormOfWay.Motorway;
                        break;
                    case "2":
                        fow = FormOfWay.MultipleCarriageWay;
                        break;
                    case "3":
                        fow = FormOfWay.SingleCarriageWay;
                        break;
                    case "4":
                        fow = FormOfWay.Roundabout;
                        break;
                    case "5":
                        fow = FormOfWay.Undefined;
                        break;
                    case "6":
                    case "7":
                    case "8":
                        fow = FormOfWay.Other;
                        break;
                    case "9":
                        fow = FormOfWay.SlipRoad;
                        break;
                    case "10":
                    case "11":
                    case "12":
                        fow = FormOfWay.Other;
                        break;
                }
            }
            return true;
        }

        /// <summary>
        /// Creates a new referenced multinet encoder.
        /// </summary>
        /// <param name="folder">The folder containing the shapefile(s).</param>
        /// <param name="searchPattern">The search pattern to identify the relevant shapefiles.</param>
        /// <returns></returns>
        public static ReferencedMultiNetEncoder CreateBinary(string folder, string searchPattern)
        {
            return ReferencedMultiNetEncoder.Create(folder, searchPattern, new OpenLR.Binary.BinaryEncoder());
        }

        /// <summary>
        /// Creates a new referenced multinet encoder.
        /// </summary>
        /// <param name="graph">The graph containing the multinet network.</param>
        /// <returns></returns>
        public static ReferencedMultiNetEncoder CreateBinary(BasicRouterDataSource<LiveEdge> graph)
        {
            return ReferencedMultiNetEncoder.Create(graph, new OpenLR.Binary.BinaryEncoder());
        }

        /// <summary>
        /// Creates a new referenced multinet encoder.
        /// </summary>
        /// <param name="graph">The graph containing the multinet network.</param>
        /// <returns></returns>
        public static ReferencedMultiNetEncoder CreateBinary(IBasicRouterDataSource<LiveEdge> graph)
        {
            return ReferencedMultiNetEncoder.CreateBinary(new BasicRouterDataSource<LiveEdge>(graph));
        }

        /// <summary>
        /// Creates a new referenced multinet decoder.
        /// </summary>
        /// <param name="folder">The folder containing the shapefile(s).</param>
        /// <param name="searchPattern">The search pattern to identify the relevant shapefiles.</param>
        /// <param name="rawLocationEncoder">The raw location encoder.</param>
        /// <returns></returns>
        public static ReferencedMultiNetEncoder Create(string folder, string searchPattern, Encoder rawLocationEncoder)
        {
            // create an instance of the graph reader and define the columns that contain the 'node-ids'.
            var graphReader = new ShapefileLiveGraphReader("F_JNCTID", "T_JNCTID");
            // read the graph from the folder where the shapefiles are placed.
            var graph = graphReader.Read(folder, searchPattern, new ShapefileRoutingInterpreter());

            return new ReferencedMultiNetEncoder(new BasicRouterDataSource<LiveEdge>(graph), rawLocationEncoder);
        }

        /// <summary>
        /// Creates a new referenced multinet decoder.
        /// </summary>
        /// <param name="graph">The graph containing the multinet network.</param>
        /// <param name="rawLocationEncoder">The raw location encoder.</param>
        /// <returns></returns>
        public static ReferencedMultiNetEncoder Create(BasicRouterDataSource<LiveEdge> graph, Encoder rawLocationEncoder)
        {
            return new ReferencedMultiNetEncoder(graph, rawLocationEncoder);
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

        /// <summary>
        /// Holds the encoder vehicle.
        /// </summary>
        private Vehicle _vehicle = new global::OsmSharp.Routing.Shape.Vehicles.Car("ONEWAY", "FT", "TF", string.Empty);

        /// <summary>
        /// Returns the encoder vehicle profile.
        /// </summary>
        public override global::OsmSharp.Routing.Vehicle Vehicle
        {
            get { return _vehicle; }
        }
    }
}