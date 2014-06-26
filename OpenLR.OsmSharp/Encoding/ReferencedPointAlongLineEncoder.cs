using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.OsmSharp.Locations;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;

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
            // get the tags collection.
            var tags = this.GetTags(referencedLocation.Edge.Tags);

            var location = new PointAlongLineLocation();
            location.First = new Model.LocationReferencePoint();
            location.First.Coordinate = this.GetVertexLocation(referencedLocation.VertexFrom);
            location.First.FormOfWay = this.GetFormOfWayFor(tags);
            location.First.FuntionalRoadClass = this.GetFunctionalRoadClassFor(tags);
            location.First.LowestFunctionalRoadClassToNext = location.First.FuntionalRoadClass;
            location.First.BearingDistance = 0;

            location.Last = new Model.LocationReferencePoint();
            location.Last.Coordinate = this.GetVertexLocation(referencedLocation.VertexTo);
            location.Last.FormOfWay = this.GetFormOfWayFor(tags);
            location.Last.FuntionalRoadClass = this.GetFunctionalRoadClassFor(tags);
            location.Last.BearingDistance = 0;

            // calculate side of road, orientation, ...
            // create line between two poins.
            var line = new GeoCoordinateLine(
                new GeoCoordinate(location.First.Coordinate.Latitude, location.First.Coordinate.Longitude),
                new GeoCoordinate(location.Last.Coordinate.Latitude, location.Last.Coordinate.Longitude),
                true, true);
            location.First.BearingDistance = 0;
            location.First.DistanceToNext = (int)line.LengthReal.Value;
            // location.Last.BearingDistance = 0;
            var projectedPoint = line.ProjectOn(new global::OsmSharp.Math.Primitives.PointF2D(referencedLocation.Longitude, referencedLocation.Latitude));
            location.PositiveOffset = (int)new GeoCoordinate(projectedPoint).DistanceReal(new GeoCoordinate(location.First.Coordinate.Latitude, location.First.Coordinate.Longitude)).Value;
            location.Orientation = Orientation.NoOrientation;
            switch(line.PositionOfPoint(new global::OsmSharp.Math.Primitives.PointF2D(referencedLocation.Longitude, referencedLocation.Latitude)))
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
            return location;
        }
    }
}