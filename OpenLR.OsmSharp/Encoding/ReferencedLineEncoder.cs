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
    /// Represents a referenced line location decoder.
    /// </summary>
    /// <typeparam name="TEdge"></typeparam>
    public class ReferencedLineEncoder<TEdge> : ReferencedEncoder<ReferencedLine<TEdge>, LineLocation, TEdge>
        where TEdge : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Creates a line referenced encoder.
        /// </summary>
        /// <param name="mainEncoder"></param>
        /// <param name="rawEncoder"></param>
        public ReferencedLineEncoder(ReferencedEncoderBase<TEdge> mainEncoder, OpenLR.Encoding.LocationEncoder<LineLocation> rawEncoder)
            : base(mainEncoder, rawEncoder)
        {

        }

        /// <summary>
        /// Encodes a line location.
        /// </summary>
        /// <param name="referencedLocation"></param>
        /// <returns></returns>
        public override LineLocation EncodeReferenced(ReferencedLine<TEdge> referencedLocation)
        {
            try
            {
                // initialize location.
                var location = new LineLocation();

                // match fow/frc for first edge.
                FormOfWay fow;
                FunctionalRoadClass frc;
                var tags = this.GetTags(referencedLocation.Edges[0].Tags);
                if(!this.TryMatching(tags, out frc, out fow))
                {
                    throw new ReferencedEncodingException(referencedLocation, "Could not find frc and/or fow for the given tags.");
                }
                location.First = new Model.LocationReferencePoint();
                location.First.Coordinate = this.GetVertexLocation(referencedLocation.Vertices[0]);
                location.First.FormOfWay = fow;
                location.First.FuntionalRoadClass = frc;
                location.First.LowestFunctionalRoadClassToNext = location.First.FuntionalRoadClass;

                // match for last edge.
                tags = this.GetTags(referencedLocation.Edges[referencedLocation.Edges.Length - 1].Tags);
                if (!this.TryMatching(tags, out frc, out fow))
                {
                    throw new ReferencedEncodingException(referencedLocation, "Could not find frc and/or fow for the given tags.");
                }
                location.Last = new Model.LocationReferencePoint();
                location.Last.Coordinate = this.GetVertexLocation(referencedLocation.Vertices[referencedLocation.Vertices.Length - 1]);
                location.Last.FormOfWay = fow;
                location.Last.FuntionalRoadClass = frc;

                // initialize from point, to point and create the coordinate list.
                var from = new GeoCoordinate(location.First.Coordinate.Latitude, location.First.Coordinate.Longitude);
                var to = new GeoCoordinate(location.Last.Coordinate.Latitude, location.Last.Coordinate.Longitude);
                var coordinates = referencedLocation.GetCoordinates(this.MainEncoder);

                // calculate bearing.
                location.First.Bearing = (int)this.GetBearing(referencedLocation.Vertices[0], referencedLocation.Edges[0], 
                    referencedLocation.Vertices[1], false).Value;
                location.Last.Bearing = (int)this.GetBearing(referencedLocation.Vertices[referencedLocation.Vertices.Length - 1], 
                    referencedLocation.Edges[referencedLocation.Edges.Length - 1], referencedLocation.Vertices[referencedLocation.Vertices.Length - 2], true).Value;

                // calculate length.
                var lengthInMeter = coordinates.Length();
                location.First.DistanceToNext = (int)lengthInMeter.Value;

                // set offsets.
                location.PositiveOffsetPercentage = referencedLocation.PositiveOffsetPercentage;
                location.NegativeOffsetPercentage = referencedLocation.NegativeOffsetPercentage;

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