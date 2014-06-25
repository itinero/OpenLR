using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.Referenced;
using OpenLR.Referenced.Encoding;
using OsmSharp.Collections.Tags;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using System;

namespace OpenLR.OsmSharp.Encoding
{
    /// <summary>
    /// Represents a dynamic graph encoder: Encodes a reference OpenLR location into an unreferenced location.
    /// </summary>
    public abstract class ReferencedEncoder<TReferencedLocation, TLocation, TEdge> : ReferencedLocationEncoder<TReferencedLocation, TLocation>
        where TEdge : IDynamicGraphEdgeData
        where TReferencedLocation : ReferencedLocation
        where TLocation : ILocation
    {
        /// <summary>
        /// Holds the graph.
        /// </summary>
        private IBasicRouterDataSource<TEdge> _graph;

        /// <summary>
        /// Creates a new dynamic graph encoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="router"></param>
        public ReferencedEncoder(OpenLR.Encoding.LocationEncoder<TLocation> rawEncoder, IBasicRouterDataSource<TEdge> graph, IBasicRouter<TEdge> router)
            : base(rawEncoder)
        {
            _graph = graph;
            //_router = router;
        }

        /// <summary>
        /// Returns the coordinate of the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        protected Coordinate GetVertexLocation(long vertex)
        {
            float latitude, longitude;
            if (!_graph.GetVertex((uint)vertex, out latitude, out longitude))
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
        /// Returns the tags for the given tags id.
        /// </summary>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        protected TagsCollectionBase GetTags(uint tagsId)
        {
            return _graph.TagsIndex.Get(tagsId);
        }

        /// <summary>
        /// Returns a corresponding functional road class for the given edge properties.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        protected FunctionalRoadClass GetFunctionalRoadClassFor(TagsCollectionBase tags)
        {
            // TODO: move this stuff to a more general class that can be changed for other networks!
            // use osm-schema for now.
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
        /// Returns a corresponding form of way for the given edge properties.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        protected FormOfWay GetFormOfWayFor(TagsCollectionBase tags)
        {
            // TODO: move this stuff to a more general class that can be changed for other networks!
            // use osm-schema for now.
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
    }
}
