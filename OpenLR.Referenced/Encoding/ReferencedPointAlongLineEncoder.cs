using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.Referenced.Locations;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Primitives;
using OsmSharp.Units.Distance;
using System;
using System.Collections.Generic;

namespace OpenLR.Referenced.Encoding
{
    /// <summary>
    /// Represents a referenced point along line location decoder.
    /// </summary>
    public class ReferencedPointAlongLineEncoder : ReferencedEncoder<ReferencedPointAlongLine, PointAlongLineLocation>
    {
        /// <summary>
        /// Creates a point along line referenced encoder.
        /// </summary>
        /// <param name="mainEncoder"></param>
        /// <param name="rawEncoder"></param>
        public ReferencedPointAlongLineEncoder(ReferencedEncoderBase mainEncoder, OpenLR.Encoding.LocationEncoder<PointAlongLineLocation> rawEncoder)
            : base(mainEncoder, rawEncoder)
        {

        }

        /// <summary>
        /// Encodes a point along location.
        /// </summary>
        /// <param name="referencedLocation"></param>
        /// <returns></returns>
        public override PointAlongLineLocation EncodeReferenced(ReferencedPointAlongLine referencedLocation)
        {
            try
            {
                // Step – 1: Check validity of the location and offsets to be encoded.
                // validate connected and traversal.
                referencedLocation.Route.ValidateConnected(this.MainEncoder);
                // validate offsets.
                referencedLocation.Route.ValidateOffsets(this.MainEncoder);
                // validate for binary.
                referencedLocation.Route.ValidateBinary(this.MainEncoder);

                // Step – 2 Adjust start and end node of the location to represent valid map nodes.
                referencedLocation.Route.AdjustToValidPoints(this.MainEncoder);
                // keep a list of LR-point.
                var points = new List<int>(new int[] { 0, referencedLocation.Route.Vertices.Length - 1 });

                // Step – 3     Determine coverage of the location by a shortest-path.
                // Step – 4     Check whether the calculated shortest-path covers the location completely. 
                //              Go to step 5 if the location is not covered completely, go to step 7 if the location is covered.
                // Step – 5     Determine the position of a new intermediate location reference point so that the part of the 
                //              location between the start of the shortest-path calculation and the new intermediate is covered 
                //              completely by a shortest-path.
                // Step – 6     Go to step 3 and restart shortest path calculation between the new intermediate location reference 
                //              point and the end of the location.
                // Step – 7     Concatenate the calculated shortest-paths for a complete coverage of the location and form an 
                //              ordered list of location reference points (from the start to the end of the location).

                // Step – 8     Check validity of the location reference path. If the location reference path is invalid then go 
                //              to step 9, if the location reference path is valid then go to step 10.
                // Step – 9     Add a sufficient number of additional intermediate location reference points if the distance 
                //              between two location reference points exceeds the maximum distance. Remove the start/end LR-point 
                //              if the positive/negative offset value exceeds the length of the corresponding path.

                // WARNING: the OpenLR-spec says that there cannot be intermediate points on an PointAlongLineLocation.
                //              this means that if the route found here is > 15.000m it cannot be encoding.
                //              assumed is that the OpenLR-spec assumes that this will never happen (?)
                // referencedLocation.Route.AdjustToValidDistances(this.MainEncoder, points);

                // Step – 10    Create physical representation of the location reference.
                var coordinates = referencedLocation.Route.GetCoordinates(this.MainEncoder);
                var lengthInMeter = coordinates.Length();

                var location = new PointAlongLineLocation();
                location.First = this.MainEncoder.BuildLocationReferencePoint(
                    referencedLocation.Route, 0, referencedLocation.Route.Vertices.Length - 1);
                location.Last = this.MainEncoder.BuildLocationReferencePointLast(
                    referencedLocation.Route, 0);
                
                // calculate orientation and side of road.
                PointF2D bestProjected;
                LinePointPosition bestPosition;
                Meter bestOffset;
                int bestIndex;
                if (!coordinates.ProjectOn(new PointF2D(referencedLocation.Longitude, referencedLocation.Latitude),
                    out bestProjected, out bestPosition, out bestOffset, out bestIndex))
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
                if(location.PositiveOffsetPercentage >= 100)
                { // should be in the range of [0-100[.
                    // encoding should always work even if not 100% accurate in this case.
                    location.PositiveOffsetPercentage = 99;
                }

                return location;
            }
            catch (ReferencedEncodingException)
            { // rethrow referenced encoding exception.
                throw;
            }
            catch (Exception ex)
            { // unhandled exception!
                throw new ReferencedEncodingException(referencedLocation,
                    string.Format("Unhandled exception during ReferencedPointAlongLineEncoder: {0}", ex.ToString()), ex);
            }
        }
    }
}