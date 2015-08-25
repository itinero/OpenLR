using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.Referenced.Locations;
using OsmSharp;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.Osm.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenLR.Referenced.Encoding
{
    /// <summary>
    /// Represents a referenced line location decoder.
    /// </summary>
    public class ReferencedLineEncoder : ReferencedEncoder<ReferencedLine, LineLocation>
    {
        /// <summary>
        /// Creates a line referenced encoder.
        /// </summary>
        /// <param name="mainEncoder"></param>
        /// <param name="rawEncoder"></param>
        public ReferencedLineEncoder(ReferencedEncoderBase mainEncoder, OpenLR.Encoding.LocationEncoder<LineLocation> rawEncoder)
            : base(mainEncoder, rawEncoder)
        {

        }

        /// <summary>
        /// Encodes a line location.
        /// </summary>
        /// <param name="referencedLocation"></param>
        /// <returns></returns>
        public override LineLocation EncodeReferenced(ReferencedLine referencedLocation)
        {
            try
            {
                // Step – 1: Check validity of the location and offsets to be encoded.
                // validate connected and traversal.
                referencedLocation.ValidateConnected(this.MainEncoder);
                // validate offsets.
                referencedLocation.ValidateOffsets(this.MainEncoder);
                // validate for binary.
                referencedLocation.ValidateBinary(this.MainEncoder);

                // Step – 2 Adjust start and end node of the location to represent valid map nodes.
                referencedLocation.AdjustToValidPoints(this.MainEncoder);
                // keep a list of LR-point.
                var points = new List<int>(new int[] { 0, referencedLocation.Vertices.Length -1});

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
                referencedLocation.AdjustToValidDistances(this.MainEncoder, points);

                // Step – 10    Create physical representation of the location reference.
                var coordinates = referencedLocation.GetCoordinates(this.MainEncoder);
                var length = coordinates.Length();

                // 3: The actual encoding now!
                // initialize location.
                var location = new LineLocation();

                // build lrp's.
                var lrps = new List<LocationReferencePoint>();
                var lrpIdx = points[0];
                var lrp = this.BuildLrp(referencedLocation, lrpIdx);
                lrps.Add(lrp);
                for (int idx = 1; idx < points.Count; idx++)
                {
                    lrp.LowestFunctionalRoadClassToNext = this.BuildLowestFrcToNext(referencedLocation, lrpIdx, points[idx]);
                    lrp.DistanceToNext = this.BuildDistanceToNext(referencedLocation, lrpIdx, points[idx]);

                    lrpIdx = points[idx];
                    lrp = this.BuildLrp(referencedLocation, lrpIdx);
                    lrps.Add(lrp);
                }

                // build location.
                location.First = lrps[0];
                location.Intermediate = new LocationReferencePoint[lrps.Count - 2];
                for(int idx = 1; idx < lrps.Count - 1; idx++)
                {
                    location.Intermediate[idx - 1] = lrps[idx];
                }
                location.Last = lrps[lrps.Count - 1];

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
                throw new ReferencedEncodingException(referencedLocation, 
                    string.Format("Unhandled exception during ReferencedLineEncoder: {0}", ex.ToString()), ex);
            }
        }

        /// <summary>
        /// Builds a location referenced point for the vertex at the given index.
        /// </summary>
        /// <param name="referencedLocation">The referenced location.</param>
        /// <param name="idx">The index.</param>
        /// <returns></returns>
        private LocationReferencePoint BuildLrp(ReferencedLine referencedLocation, int idx)
        {
            // get all relevant info from tags.
            FormOfWay fow;
            FunctionalRoadClass frc;
            TagsCollectionBase tags;
            if(idx < referencedLocation.Edges.Length)
            { // not the last point.
                tags = this.GetTags(referencedLocation.Edges[idx].Tags);
            }
            else
            { // last point.
                tags = this.GetTags(referencedLocation.Edges[idx - 1].Tags);
            }
            if (!this.TryMatching(tags, out frc, out fow))
            {
                throw new ReferencedEncodingException(referencedLocation, "Could not find frc and/or fow for the given tags.");
            }

            // create location reference point.
            var lrp = new LocationReferencePoint();
            lrp.Coordinate = this.GetVertexLocation(referencedLocation.Vertices[idx]);
            lrp.FormOfWay = fow;
            lrp.FuntionalRoadClass = frc;
            if (idx + 1 < referencedLocation.Vertices.Length)
            { // not the last point.
                lrp.Bearing = (int)this.GetBearing(referencedLocation.Vertices[idx], referencedLocation.Edges[idx],
                    referencedLocation.EdgeShapes[idx], referencedLocation.Vertices[idx + 1], false).Value;
            }
            else
            { // last point.
                lrp.Bearing = (int)this.GetBearing(referencedLocation.Vertices[idx], referencedLocation.Edges[idx - 1],
                    referencedLocation.EdgeShapes[idx - 1], referencedLocation.Vertices[idx - 1], true).Value;
            }
            return lrp;
        }

        /// <summary>
        /// Builds the lowest frc to next from all edges between the two given verices indexes.
        /// </summary>
        /// <param name="referencedLocation">The referenced location.</param>
        /// <param name="vertex1">The first vertex.</param>
        /// <param name="vertex2">The last vertex.</param>
        /// <returns></returns>
        private FunctionalRoadClass BuildLowestFrcToNext(ReferencedLine referencedLocation, int vertex1, int vertex2)
        {
            FunctionalRoadClass? lowest = null;
            for (var edge = vertex1; edge < vertex2; edge++)
            {
                var tags = this.GetTags(referencedLocation.Edges[edge].Tags);
                FormOfWay fow;
                FunctionalRoadClass frc;
                if (!this.TryMatching(tags, out frc, out fow))
                {
                    throw new ReferencedEncodingException(referencedLocation, "Could not find frc and/or fow for the given tags.");
                }
                
                if(!lowest.HasValue || 
                    frc < lowest)
                { 
                    lowest = frc;
                }
            }
            return lowest.Value;
        }

        /// <summary>
        /// Builds the lowest frc to next from all edges between the two given verices indexes.
        /// </summary>
        /// <param name="referencedLocation">The referenced location.</param>
        /// <param name="vertexIdx1">The first vertex.</param>
        /// <param name="vertexIdx2">The last vertex.</param>
        /// <returns></returns>
        private int BuildDistanceToNext(ReferencedLine referencedLocation, int vertexIdx1, int vertexIdx2)
        {
            return (int)(referencedLocation.GetCoordinates(this.MainEncoder, vertexIdx1, vertexIdx2 - vertexIdx1 + 1).Length().Value);
        }
    }
}