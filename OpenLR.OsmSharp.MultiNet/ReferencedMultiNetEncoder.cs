using OpenLR.Encoding;
using OpenLR.Model;
using OsmSharp.Collections.Tags;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;
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
    }
}
