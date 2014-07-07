using NetTopologySuite.LinearReferencing;
using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.OsmSharp.Locations;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Units.Distance;
using System;
using System.Collections.Generic;

namespace OpenLR.OsmSharp.Encoding
{
    /// <summary>
    /// Represents a referenced point along line location decoder.
    /// </summary>
    /// <typeparam name="TEdge"></typeparam>
    public class ReferencedPointAlongLineEncoder<TEdge> : ReferencedEncoder<ReferencedPointAlongLine<TEdge>, PointAlongLineLocation, TEdge>
        where TEdge : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Creates a point along line referenced encoder.
        /// </summary>
        /// <param name="mainEncoder"></param>
        /// <param name="rawEncoder"></param>
        /// <param name="graph"></param>
        /// <param name="router"></param>
        public ReferencedPointAlongLineEncoder(ReferencedEncoderBase<TEdge> mainEncoder, OpenLR.Encoding.LocationEncoder<PointAlongLineLocation> rawEncoder, IBasicRouterDataSource<TEdge> graph, IBasicRouter<TEdge> router)
            : base(mainEncoder, rawEncoder, graph, router)
        {

        }

        /// <summary>
        /// Encodes a point along location.
        /// </summary>
        /// <param name="referencedLocation"></param>
        /// <returns></returns>
        public override PointAlongLineLocation EncodeReferenced(ReferencedPointAlongLine<TEdge> referencedLocation)
        {
            try
            {
                // get the tags collection.
                var tags = this.GetTags(referencedLocation.Edge.Tags);

                // initialize location.
                var location = new PointAlongLineLocation();
                location.First = new Model.LocationReferencePoint();
                location.First.Coordinate = this.GetVertexLocation(referencedLocation.VertexFrom);
                location.First.FormOfWay = this.GetFormOfWayFor(tags);
                location.First.FuntionalRoadClass = this.GetFunctionalRoadClassFor(tags);
                location.First.LowestFunctionalRoadClassToNext = location.First.FuntionalRoadClass;
                location.Last = new Model.LocationReferencePoint();
                location.Last.Coordinate = this.GetVertexLocation(referencedLocation.VertexTo);
                location.Last.FormOfWay = this.GetFormOfWayFor(tags);
                location.Last.FuntionalRoadClass = this.GetFunctionalRoadClassFor(tags);

                // initialize from point, to point and create the coordinate list.
                var from = new GeoCoordinate(location.First.Coordinate.Latitude, location.First.Coordinate.Longitude);
                var to = new GeoCoordinate(location.Last.Coordinate.Latitude, location.Last.Coordinate.Longitude);
                var coordinates = referencedLocation.Edge.GetCoordinates(from, to);

                // calculate bearing.
                location.First.Bearing = (int)this.GetBearing(referencedLocation.VertexFrom, referencedLocation.Edge, referencedLocation.VertexTo, false).Value;
                location.Last.Bearing = (int)this.GetBearing(referencedLocation.VertexTo, referencedLocation.Edge, referencedLocation.VertexFrom, true).Value;

                // calculate length.
                var lengthInMeter = coordinates.Length();
                location.First.DistanceToNext = (int)lengthInMeter.Value;

                // calculate orientation and side of road.
                PointF2D bestProjected;
                LinePointPosition bestPosition;
                Meter bestOffset;
                if (!coordinates.ProjectOn(new PointF2D(referencedLocation.Longitude, referencedLocation.Latitude), out bestProjected, out bestPosition, out bestOffset))
                { // the projection on the edge failed.
                    throw new ReferencedEncodingException(referencedLocation, "The point in the ReferencedPointAlongLine could not be projected on the referenced edge.");
                }

                location.Orientation = referencedLocation.Orientation;
                switch (bestPosition)
                {
                    case global::OsmSharp.Math.Primitives.LinePointPosition.Left:
                        location.SideOfRoad = SideOfRoad.Left;
                        break;
                    case global::OsmSharp.Math.Primitives.LinePointPosition.On:
                        location.SideOfRoad = SideOfRoad.OnOrAbove;
                        break;
                    case global::OsmSharp.Math.Primitives.LinePointPosition.Right:
                        location.SideOfRoad = SideOfRoad.Right;
                        break;
                }

                // calculate offset.
                location.PositiveOffsetPercentage = (float)(bestOffset.Value / lengthInMeter.Value) * 100.0f;

                return location;
            }
            catch (ReferencedEncodingException)
            { // rethrow referenced encoding exception.
                throw;
            }
            catch (Exception ex)
            { // unhandled exception!
                throw new ReferencedEncodingException(referencedLocation, "Unhandled exception during ReferencedPointAlongLineEncoder", ex);
            }
        }
    }
}