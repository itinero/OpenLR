using OpenLR.Encoding;
using OpenLR.Model;
using OsmSharp.Collections.Tags;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;
using System;

namespace OpenLR.OsmSharp.Osm
{
    /// <summary>
    /// An implementation of a referenced encoder based on OSM. 
    /// </summary>
    public class ReferencedOsmEncoder : ReferencedEncoderBase<LiveEdge>
    {
        /// <summary>
        /// Creates a new referenced live edge decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="locationEncoder"></param>
        public ReferencedOsmEncoder(IBasicRouterDataSource<LiveEdge> graph, Encoder locationEncoder)
            : base(graph, locationEncoder)
        {

        }

        /// <summary>
        /// Gets a new router.
        /// </summary>
        /// <returns></returns>
        protected override global::OsmSharp.Routing.Graph.Router.IBasicRouter<LiveEdge> GetRouter()
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
            string highway;
            if (tags.TryGetValue("highway", out highway))
            {
                switch (highway)
                { // check there reference values against OSM: http://wiki.openstreetmap.org/wiki/Highway
                    case "motorway":
                    case "trunk":
                        return FunctionalRoadClass.Frc0;
                    case "primary":
                    case "primary_link":
                        return FunctionalRoadClass.Frc1;
                    case "secondary":
                    case "secondary_link":
                        return FunctionalRoadClass.Frc2;
                    case "tertiary":
                    case "tertiary_link":
                        return FunctionalRoadClass.Frc3;
                    case "road":
                    case "road_link":
                    case "unclassified":
                    case "residential":
                        return FunctionalRoadClass.Frc4;
                    case "living_street":
                        return FunctionalRoadClass.Frc5;
                    case "footway":
                    case "bridleway":
                    case "steps":
                    case "path":
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
            string highway;
            if (tags.TryGetValue("highway", out highway))
            {
                switch (highway)
                { // check there reference values against OSM: http://wiki.openstreetmap.org/wiki/Highway
                    case "motorway":
                    case "trunk":
                        return FormOfWay.Motorway;
                    case "primary":
                    case "primary_link":
                        return FormOfWay.MultipleCarriageWay;
                    case "secondary":
                    case "secondary_link":
                    case "tertiary":
                    case "tertiary_link":
                        return FormOfWay.SingleCarriageWay;
                }
            }
            return FormOfWay.SingleCarriageWay;
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
    }
}