using OpenLR.Encoding;
using OpenLR.Model;
using OsmSharp.Collections.Tags;
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
    public class ReferencedNWBEncoder : ReferencedEncoderBase<LiveEdge>
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
        /// Returns the functional road class for the the given collections of tags.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public override FunctionalRoadClass GetFunctionalRoadClassFor(TagsCollectionBase tags)
        {
            string baansubsrt;
            if (tags.TryGetValue("BAANSUBSRT", out baansubsrt))
            {
                switch (baansubsrt)
                { // check there reference values against OSM: http://wiki.openstreetmap.org/wiki/Highway
                    case "HR": // main road.
                        return FunctionalRoadClass.Frc0;
                    case "AF":
                    case "OP":
                        return FunctionalRoadClass.Frc1;
                    case "VBD":
                        return FunctionalRoadClass.Frc2;

                    // all other road classes are unknown for now.
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
            string baansubsrt;
            if (tags.TryGetValue("BAANSUBSRT", out baansubsrt))
            {
                switch (baansubsrt)
                { // check there reference values against OSM: http://wiki.openstreetmap.org/wiki/Highway
                    case "HR": // main road.
                        return FormOfWay.Motorway;
                    case "AF":
                    case "OP":
                        return FormOfWay.MultipleCarriageWay;
                    case "VBD":
                        return FormOfWay.SingleCarriageWay;

                    // all other road classes are unknown for now.
                }
            }
            return FormOfWay.Undefined;
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

            return new ReferencedNWBEncoder(graph, rawLocationEncoder);
        }
    }
}
