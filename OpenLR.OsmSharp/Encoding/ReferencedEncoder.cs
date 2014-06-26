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
        /// Holds the main encoder.
        /// </summary>
        private ReferencedEncoderBase<TEdge> _mainEncoder;

        /// <summary>
        /// Creates a new dynamic graph encoder.
        /// </summary>
        /// <param name="mainEncoder"></param>
        /// <param name="rawEncoder"></param>
        /// <param name="graph"></param>
        /// <param name="router"></param>
        public ReferencedEncoder(ReferencedEncoderBase<TEdge> mainEncoder, OpenLR.Encoding.LocationEncoder<TLocation> rawEncoder, IBasicRouterDataSource<TEdge> graph, IBasicRouter<TEdge> router)
            : base(rawEncoder)
        {
            _mainEncoder = mainEncoder;
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
            return _mainEncoder.GetVertexLocation(vertex);
        }

        /// <summary>
        /// Returns the tags for the given tags id.
        /// </summary>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        protected TagsCollectionBase GetTags(uint tagsId)
        {
            return _mainEncoder.GetTags(tagsId);
        }

        /// <summary>
        /// Returns a corresponding functional road class for the given edge properties.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        protected FunctionalRoadClass GetFunctionalRoadClassFor(TagsCollectionBase tags)
        {
            return _mainEncoder.GetFunctionalRoadClassFor(tags);
        }

        /// <summary>
        /// Returns a corresponding form of way for the given edge properties.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        protected FormOfWay GetFormOfWayFor(TagsCollectionBase tags)
        {
            return _mainEncoder.GetFormOfWayFor(tags);
        }
    }
}
