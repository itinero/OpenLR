using OpenLR.Encoding;
using OpenLR.Model;
using OpenLR.OsmSharp.Encoding;
using OpenLR.OsmSharp.Locations;
using OpenLR.Referenced;
using OsmSharp.Collections.Tags;
using OsmSharp.Math;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Units.Angle;
using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Returns a value if a oneway restriction is found.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns>null: no restrictions, true: forward restriction, false: backward restriction.</returns>
        /// <returns></returns>
        public abstract bool? IsOneway(TagsCollectionBase tags);

        /// <summary>
        /// Returns the bearing calculate between two given vertices along the given edge.
        /// </summary>
        /// <param name="vertexFrom"></param>
        /// <param name="edge"></param>
        /// <param name="vertexTo"></param>
        /// <param name="forward">When true the edge is forward relative to the vertices, false the edge is backward.</param>
        /// <returns></returns>
        public virtual Degree GetBearing(long vertexFrom, TEdge edge, long vertexTo, bool forward)
        {
            var coordinates = new List<GeoCoordinate>();
            float latitude, longitude;
            this.Graph.GetVertex((uint)vertexFrom, out latitude, out longitude);
            coordinates.Add(new GeoCoordinate(latitude, longitude));

            if (edge.Coordinates != null)
            { // there are intermediates, add them in the correct order.
                if (forward)
                {
                    coordinates.AddRange(edge.Coordinates.Select<GeoCoordinateSimple, GeoCoordinate>(x => {return new GeoCoordinate(x.Latitude, x.Longitude);}));
                }
                else
                {
                    coordinates.AddRange(edge.Coordinates.Reverse().Select<GeoCoordinateSimple, GeoCoordinate>(x => { return new GeoCoordinate(x.Latitude, x.Longitude); }));
                }
            }

            this.Graph.GetVertex((uint)vertexTo, out latitude, out longitude);
            coordinates.Add(new GeoCoordinate(latitude, longitude));

            return BearingEncoder.EncodeBearing(coordinates);
        }
    }
}