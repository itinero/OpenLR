using OpenLR.Encoding;
using OpenLR.Model;
using OsmSharp.Collections.Tags;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Shape;
using OsmSharp.Routing.Shape.Readers;
using System;

namespace OpenLR.OsmSharp.NWB
{
    /// <summary>
    /// An implementation of a referenced encoder based on the Nationaal Wegenbestand (NWB) in the netherlands.
    /// </summary>
    public class ReferencedNWBEncoder : ReferencedEncoderBaseLiveEdge
    {
        /// <summary>
        /// Creates a new referenced live edge decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="locationEncoder"></param>
        public ReferencedNWBEncoder(IBasicRouterDataSource<LiveEdge> graph, Encoder locationEncoder)
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
        /// Tries to match the given tags and figure out a corresponding frc and fow.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="frc"></param>
        /// <param name="fow"></param>
        /// <returns>False if no matching was found.</returns>
        public override bool TryMatching(TagsCollectionBase tags, out FunctionalRoadClass frc, out FormOfWay fow)
        {
            return NWBMapping.ToOpenLR(tags, out fow, out frc);
        }

        /// <summary>
        /// Creates a new referenced NWB encoder.
        /// </summary>
        /// <param name="folder">The folder containing the shapefile(s).</param>
        /// <param name="searchPattern">The search pattern to identify the relevant shapefiles.</param>
        /// <returns></returns>
        public static ReferencedNWBEncoder CreateBinary(string folder, string searchPattern)
        {
            return ReferencedNWBEncoder.Create(folder, searchPattern, new OpenLR.Binary.BinaryEncoder());
        }

        /// <summary>
        /// Creates a new referenced NWB encoder.
        /// </summary>
        /// <param name="graph">The graph containing the NWB network.</param>
        /// <returns></returns>
        public static ReferencedNWBEncoder CreateBinary(IBasicRouterDataSource<LiveEdge> graph)
        {
            return ReferencedNWBEncoder.Create(graph, new OpenLR.Binary.BinaryEncoder());
        }

        /// <summary>
        /// Creates a new referenced NWB encoder.
        /// </summary>
        /// <param name="folder">The folder containing the shapefile(s).</param>
        /// <param name="searchPattern">The search pattern to identify the relevant shapefiles.</param>
        /// <param name="rawLocationEncoder">The raw location encoder.</param>
        /// <returns></returns>
        public static ReferencedNWBEncoder Create(string folder, string searchPattern, Encoder rawLocationEncoder)
        {
            // create an instance of the graph reader and define the columns that contain the 'node-ids'.
            var graphReader = new ShapefileLiveGraphReader("JTE_ID_BEG", "JTE_ID_END");
            // read the graph from the folder where the shapefiles are placed.
            var graph = graphReader.Read(folder, searchPattern, new ShapefileRoutingInterpreter());

            return ReferencedNWBEncoder.Create(graph, rawLocationEncoder);
        }

        /// <summary>
        /// Creates a new referenced NWB encoder.
        /// </summary>
        /// <param name="graph">The graph containing the NWB network.</param>
        /// <param name="rawLocationEncoder">The raw location encoder.</param>
        /// <returns></returns>
        public static ReferencedNWBEncoder Create(IBasicRouterDataSource<LiveEdge> graph, Encoder rawLocationEncoder)
        {
            return new ReferencedNWBEncoder(graph, rawLocationEncoder);
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
        private Vehicle _vehicle = new global::OsmSharp.Routing.Shape.Vehicles.Car("RIJRICHTNG", "H", "T", string.Empty);

        /// <summary>
        /// Returns the encoder vehicle profile.
        /// </summary>
        public override global::OsmSharp.Routing.Vehicle Vehicle
        {
            get { return _vehicle; }
        }
    }
}