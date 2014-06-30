using OpenLR.Encoding;
using OpenLR.Model;
using OsmSharp.Collections.Tags;
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
    public class ReferencedMultiNetEncoder : ReferencedEncoderBase<LiveEdge>
    {
        /// <summary>
        /// Creates a new referenced live edge decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="locationEncoder"></param>
        public ReferencedMultiNetEncoder(IBasicRouterDataSource<LiveEdge> graph, Encoder locationEncoder)
            : base(graph, locationEncoder)
        {

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
        /// Returns the functional road class for the the given collections of tags.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public override FunctionalRoadClass GetFunctionalRoadClassFor(TagsCollectionBase tags)
        {
            string frc;
            if (tags.TryGetValue("FRC", out frc))
            {
                switch (frc)
                {
                    case "0": // main road.
                        return FunctionalRoadClass.Frc0;
                    case "1": // main road.
                        return FunctionalRoadClass.Frc1;
                    case "2": // main road.
                        return FunctionalRoadClass.Frc2;
                    case "3": // main road.
                        return FunctionalRoadClass.Frc3;
                    case "4": // main road.
                        return FunctionalRoadClass.Frc4;
                    case "5": // main road.
                        return FunctionalRoadClass.Frc5;
                    case "6": // main road.
                        return FunctionalRoadClass.Frc6;
                    case "7": // main road.
                        return FunctionalRoadClass.Frc7;
                }
            }
            return FunctionalRoadClass.Frc7;
        }

        /// <summary>
        /// Returns the form of way for the given collection of tags.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public override FormOfWay GetFormOfWayFor(TagsCollectionBase tags)
        {
            string fow;
            if (tags.TryGetValue("FOW", out fow))
            {
                switch (fow)
                {
                    case "1": // main road.
                        return FormOfWay.Motorway;
                    case "2":
                        return FormOfWay.MultipleCarriageWay;
                    case "3":
                        return FormOfWay.SingleCarriageWay;
                    case "4":
                        return FormOfWay.Roundabout;
                    case "5":
                        return FormOfWay.Undefined;
                    case "6":
                    case "7":
                    case "8":
                        return FormOfWay.Other;
                    case "9":
                        return FormOfWay.SlipRoad;
                    case "10":
                    case "11":
                    case "12":
                        return FormOfWay.Other;
                }
            }
            return FormOfWay.Other;
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
        public static ReferencedMultiNetEncoder CreateBinary(IBasicRouterDataSource<LiveEdge> graph)
        {
            return ReferencedMultiNetEncoder.Create(graph, new OpenLR.Binary.BinaryEncoder());
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

            return new ReferencedMultiNetEncoder(graph, rawLocationEncoder);
        }

        /// <summary>
        /// Creates a new referenced multinet decoder.
        /// </summary>
        /// <param name="graph">The graph containing the multinet network.</param>
        /// <param name="rawLocationEncoder">The raw location encoder.</param>
        /// <returns></returns>
        public static ReferencedMultiNetEncoder Create(IBasicRouterDataSource<LiveEdge> graph, Encoder rawLocationEncoder)
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
            var vehicle = new global::OsmSharp.Routing.Shape.Vehicles.Car("ONEWAY", "FT", "TF", string.Empty);
            return vehicle.IsOneWay(tags);
        }
    }
}
