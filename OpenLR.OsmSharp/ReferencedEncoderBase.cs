using OpenLR.Encoding;
using OpenLR.Model;
using OpenLR.OsmSharp.Encoding;
using OpenLR.OsmSharp.Locations;
using OpenLR.Referenced;
using OsmSharp.Collections.Tags;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using System;

namespace OpenLR.OsmSharp
{
    /// <summary>
    /// A referenced encoder implementation.
    /// </summary>
    public abstract class ReferencedEncoderBase<TEdge> : OpenLR.Referenced.Encoding.ReferencedEncoder
        where TEdge : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Holds the basic router datasource.
        /// </summary>
        private readonly IBasicRouterDataSource<TEdge> _graph;

        /// <summary>
        /// Creates a new referenced encoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="locationEncoder"></param>
        public ReferencedEncoderBase(IBasicRouterDataSource<TEdge> graph, Encoder locationEncoder)
            : base(locationEncoder)
        {
            _graph = graph;
        }

        /// <summary>
        /// Returns the router.
        /// </summary>
        /// <returns></returns>
        protected abstract IBasicRouter<TEdge> GetRouter();

        /// <summary>
        /// Returns the reference graph.
        /// </summary>
        protected IBasicRouterDataSource<TEdge> Graph
        {
            get
            {
                return _graph;
            }
        }

        /// <summary>
        /// Gets the referenced point along line encoder.
        /// </summary>
        protected virtual ReferencedPointAlongLineEncoder<TEdge> GetReferencedPointAlongLineEncoder()
        {
            return new ReferencedPointAlongLineEncoder<TEdge>(this, this.LocationEncoder.CreatePointAlongLineLocationEncoder(), _graph, this.GetRouter());
        }

        /// <summary>
        /// Encodes a point along line location.
        /// </summary>
        /// <param name="pointAlongLineLocation"></param>
        /// <returns></returns>
        public virtual string Encode(ReferencedPointAlongLine<TEdge> pointAlongLineLocation)
        {
            return this.GetReferencedPointAlongLineEncoder().Encode(pointAlongLineLocation);
        }

        /// <summary>
        /// Encodes the given location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override string Encode(ReferencedLocation location)
        {
            if (location == null) { throw new ArgumentNullException("location"); }

            if (location is ReferencedPointAlongLine<TEdge>)
            {
                return this.Encode(location as ReferencedPointAlongLine<TEdge>);
            }

            throw new ArgumentOutOfRangeException("location",
                string.Format("Location cannot be encoded by any of the encoders: {0}", location.ToString()));
        }

        /// <summary>
        /// Returns the location of the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public abstract Coordinate GetVertexLocation(long vertex);

        /// <summary>
        /// Returns the tags associated with the given tags id.
        /// </summary>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        public abstract TagsCollectionBase GetTags(uint tagsId);

        /// <summary>
        /// Returns the functional road class for the the given collections of tags.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public abstract FunctionalRoadClass GetFunctionalRoadClassFor(TagsCollectionBase tags);

        /// <summary>
        /// Returns the form of way for the given collection of tags.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public abstract FormOfWay GetFormOfWayFor(TagsCollectionBase tags);
    }
}