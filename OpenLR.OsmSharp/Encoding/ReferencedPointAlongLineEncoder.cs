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
        public ReferencedPointAlongLineEncoder(ReferencedEncoderBase<TEdge> mainEncoder, OpenLR.Encoding.LocationEncoder<PointAlongLineLocation> rawEncoder)
            : base(mainEncoder, rawEncoder)
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
                // initialize location.
                var location = new PointAlongLineLocation();

                // match fow/frc for first edge.
                FormOfWay fow;
                FunctionalRoadClass frc;
                var tags = this.GetTags(referencedLocation.Route.Edges[0].Tags);
                if(!this.TryMatching(tags, out frc, out fow))
                {
                    throw new ReferencedEncodingException(referencedLocation, "Could not find frc and/or fow for the given tags.");
                }
                location.First = new Model.LocationReferencePoint();
                location.First.Coordinate = this.GetVertexLocation(referencedLocation.Route.Vertices[0]);
                location.First.FormOfWay = fow;
                location.First.FuntionalRoadClass = frc;
                location.First.LowestFunctionalRoadClassToNext = location.First.FuntionalRoadClass;

                // match for last edge.
                tags = this.GetTags(referencedLocation.Route.Edges[referencedLocation.Route.Edges.Length - 1].Tags);
                if (!this.TryMatching(tags, out frc, out fow))
                {
                    throw new ReferencedEncodingException(referencedLocation, "Could not find frc and/or fow for the given tags.");
                }
                location.Last = new Model.LocationReferencePoint();
                location.Last.Coordinate = this.GetVertexLocation(referencedLocation.Route.Vertices[referencedLocation.Route.Vertices.Length - 1]);
                location.Last.FormOfWay = fow;
                location.Last.FuntionalRoadClass = frc;

                // initialize from point, to point and create the coordinate list.
                var from = new GeoCoordinate(location.First.Coordinate.Latitude, location.First.Coordinate.Longitude);
                var to = new GeoCoordinate(location.Last.Coordinate.Latitude, location.Last.Coordinate.Longitude);
                var coordinates = referencedLocation.GetCoordinates(this.MainEncoder);

                // calculate bearing.
                location.First.Bearing = (int)this.GetBearing(referencedLocation.Route.Vertices[0], referencedLocation.Route.Edges[0], referencedLocation.Route.Vertices[1], false).Value;
                location.Last.Bearing = (int)this.GetBearing(referencedLocation.Route.Vertices[referencedLocation.Route.Vertices.Length - 1], referencedLocation.Route.Edges[referencedLocation.Route.Edges.Length - 1], referencedLocation.Route.Vertices[referencedLocation.Route.Vertices.Length - 2], true).Value;

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