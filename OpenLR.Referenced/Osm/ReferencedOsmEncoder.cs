using OpenLR.Encoding;
using OpenLR.Model;
using OpenLR.Referenced.Router;
using OsmSharp.Collections.PriorityQueues;
using OsmSharp.Collections.Tags;
using OsmSharp.Routing;
using System.Collections.Generic;

namespace OpenLR.Referenced.Osm
{
    /// <summary>
    /// An implementation of a referenced encoder based on OSM. 
    /// </summary>
    public class ReferencedOsmEncoder : ReferencedEncoderBase
    {
        /// <summary>
        /// Creates a new referenced live edge decoder.
        /// </summary>
        public ReferencedOsmEncoder(RouterDb routerDb, Encoder locationEncoder)
            : base(routerDb, locationEncoder)
        {

        }

        /// <summary>
        /// Tries to match the given tags and figure out a corresponding frc and fow.
        /// </summary>
        public override bool TryMatching(TagsCollectionBase tags, out FunctionalRoadClass frc, out FormOfWay fow)
        {
            frc = FunctionalRoadClass.Frc7;
            fow = FormOfWay.Undefined;
            string highway;
            if (tags.TryGetValue("highway", out highway))
            {
                switch (highway)
                { // check there reference values against OSM: http://wiki.openstreetmap.org/wiki/Highway
                    case "motorway":
                    case "trunk":
                        frc = FunctionalRoadClass.Frc0;
                        break;
                    case "primary":
                    case "primary_link":
                        frc = FunctionalRoadClass.Frc1;
                        break;
                    case "secondary":
                    case "secondary_link":
                        frc = FunctionalRoadClass.Frc2;
                        break;
                    case "tertiary":
                    case "tertiary_link":
                        frc = FunctionalRoadClass.Frc3;
                        break;
                    case "road":
                    case "road_link":
                    case "unclassified":
                    case "residential":
                        frc = FunctionalRoadClass.Frc4;
                        break;
                    case "living_street":
                        frc = FunctionalRoadClass.Frc5;
                        break;
                    default:
                        frc = FunctionalRoadClass.Frc7;
                        break;
                }
                switch (highway)
                { // check there reference values against OSM: http://wiki.openstreetmap.org/wiki/Highway
                    case "motorway":
                    case "trunk":
                        fow = FormOfWay.Motorway;
                        break;
                    case "primary":
                    case "primary_link":
                        fow = FormOfWay.MultipleCarriageWay;
                        break;
                    case "secondary":
                    case "secondary_link":
                    case "tertiary":
                    case "tertiary_link":
                        fow = FormOfWay.SingleCarriageWay;
                        break;
                    default:
                        fow = FormOfWay.SingleCarriageWay;
                        break;
                }
                return true; // should never fail on a highway tag.
            }
            return false;
        }

        #region Static Creation Helper Functions

        /// <summary>
        /// Creates a new referenced Osm encoder.
        /// </summary>
        /// <param name="folder">The folder containing the shapefile(s).</param>
        /// <param name="searchPattern">The search pattern to identify the relevant shapefiles.</param>
        /// <returns></returns>
        public static ReferencedOsmEncoder CreateBinary(string folder, string searchPattern)
        {
            return ReferencedOsmEncoder.Create(folder, searchPattern, new OpenLR.Binary.BinaryEncoder());
        }

        /// <summary>
        /// Creates a new referenced Osm encoder.
        /// </summary>
        public static ReferencedOsmEncoder CreateBinary(RouterDb routerDb)
        {
            return ReferencedOsmEncoder.Create(routerDb, new OpenLR.Binary.BinaryEncoder());
        }

        /// <summary>
        /// Creates a new referenced Osm encoder.
        /// </summary>
        /// <param name="folder">The folder containing the shapefile(s).</param>
        /// <param name="searchPattern">The search pattern to identify the relevant shapefiles.</param>
        /// <param name="rawLocationEncoder">The raw location encoder.</param>
        /// <returns></returns>
        public static ReferencedOsmEncoder Create(string folder, string searchPattern, Encoder rawLocationEncoder)
        {
            // create an instance of the graph reader and define the columns that contain the 'node-ids'.
            var graphReader = new ShapefileLiveGraphReader("JTE_ID_BEG", "JTE_ID_END");
            // read the graph from the folder where the shapefiles are placed.
            var graph = graphReader.Read(folder, searchPattern, new ShapefileRoutingInterpreter());

            return ReferencedOsmEncoder.Create(new BasicRouterDataSource<LiveEdge>(graph), rawLocationEncoder);
        }

        /// <summary>
        /// Creates a new referenced Osm encoder.
        /// </summary>
        /// <param name="routerDb">The router db containing the OSM network.</param>
        /// <param name="rawLocationEncoder">The raw location encoder.</param>
        /// <returns></returns>
        public static ReferencedOsmEncoder Create(RouterDb routerDb, Encoder rawLocationEncoder)
        {
            return new ReferencedOsmEncoder(routerDb, rawLocationEncoder);
        }

        #endregion
    }
}