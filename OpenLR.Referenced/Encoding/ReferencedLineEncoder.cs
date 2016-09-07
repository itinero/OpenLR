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
                var coordinates = referencedLocation.GetCoordinates(this.MainEncoder.Graph);
                var length = coordinates.Length();

                // 3: The actual encoding now!
                // initialize location.
                var location = new LineLocation();

                // build lrp's.
                var locationReferencePoints = new List<LocationReferencePoint>();
                for(var idx = 0; idx < points.Count - 1; idx++)
                {
                    locationReferencePoints.Add(this.MainEncoder.BuildLocationReferencePoint(
                        referencedLocation, points[idx], points[idx + 1]));
                }
                locationReferencePoints.Add(this.MainEncoder.BuildLocationReferencePointLast(
                    referencedLocation, points[points.Count - 2]));

                // build location.
                location.First = locationReferencePoints[0];
                location.Intermediate = new LocationReferencePoint[locationReferencePoints.Count - 2];
                for(var idx = 1; idx < locationReferencePoints.Count - 1; idx++)
                {
                    location.Intermediate[idx - 1] = locationReferencePoints[idx];
                }
                location.Last = locationReferencePoints[locationReferencePoints.Count - 1];

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
    }
}