using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.Referenced.Locations;
using OsmSharp;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
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
            this.ValidateForBinary = true;
        }

        /// <summary>
        /// Gets or sets the validate for binary flag.
        /// </summary>
        public bool ValidateForBinary { get; set; }

        /// <summary>
        /// Validates if the location is connected.
        /// </summary>
        /// <param name="line">The line to check.</param>
        /// <returns></returns>
        private void ValidateConnected(ReferencedLine line)
        {
            var edges = line.Edges;
            var vertices = line.Vertices;

            // 1: Is the path connected?
            // 2: Is the path traversable?
            for (int edgeIdx = 0; edgeIdx < edges.Length; edgeIdx++)
            {
                var from = vertices[edgeIdx];
                var to = vertices[edgeIdx + 1];

                bool found = false;
                foreach (var edge in this.MainEncoder.Graph.GetEdges(from))
                {
                    if (edge.Key == to &&
                        edge.Value.Equals(edges[edgeIdx]))
                    { // edge was found, is valid.
                        found = true;
                        break;
                    }
                }
                if (!found)
                { // edge is not found, path not connected.
                    throw new ArgumentOutOfRangeException(string.Format("Edge {0} cannot be found between vertex {1} and {2}. The given path is not connected.",
                        edges[edgeIdx].ToInvariantString(), from, to));
                }
                // check whether the edge can traversed.
                var tags = this.MainEncoder.Graph.TagsIndex.Get(edges[edgeIdx].Tags);
                if (!this.MainEncoder.Vehicle.CanTraverse(tags))
                { // oeps, cannot be traversed.
                    throw new ArgumentOutOfRangeException(string.Format("Edge at index {0} cannot be traversed by vehicle {1}.", edgeIdx, this.MainEncoder.Vehicle.UniqueName));
                }
                // check whether the edge can be traversed in the correct direction.
                var oneway = this.MainEncoder.Vehicle.IsOneWay(tags);
                var canMoveForward = (oneway == null) || (oneway.Value == edges[edgeIdx].Forward);
                if (!canMoveForward)
                { // path cannot be traversed in this direction.
                    throw new ArgumentOutOfRangeException(string.Format("Edge at index {0} cannot be traversed by vehicle {1} in the direction given.", edgeIdx, this.MainEncoder.Vehicle.UniqueName));
                }
            }
        }

        /// <summary>
        /// Validates the offsets.
        /// </summary>
        /// <param name="line">The line to check.</param>
        private void ValidateOffsets(ReferencedLine line)
        {

        }

        /// <summary>
        /// Validates the location for encoding in binary format.
        /// </summary>
        /// <param name="line">The line to check.</param>
        private void ValidateBinary(ReferencedLine line)
        {

        }

        /// <summary>
        /// Adjusts the given location to use valid LR-points.
        /// </summary>
        /// <param name="encoder">The encoder.</param>
        /// <param name="referencedLine">The line to check.</param>
        public static void AdjustToValidPoints(ReferencedEncoderBase encoder, ReferencedLine referencedLine)
        {
            var length = (float)referencedLine.Length(encoder).Value;
            var positiveOffsetLength = (referencedLine.PositiveOffsetPercentage / 100) * length;
            var negativeOffsetLength = (referencedLine.NegativeOffsetPercentage / 100) * length;
            var excludeSet = new HashSet<long>();
            if (!encoder.IsVertexValid(referencedLine.Vertices[0]))
            { // from is not valid, try to find a valid point.
                var pathToValid = encoder.FindValidVertexFor(referencedLine.Vertices[0], referencedLine.Edges[0], referencedLine.Vertices[1],
                    excludeSet, false);

                // build edges list.
                if (pathToValid != null)
                { // no path found, just leave things as is.
                    var shortestRoute = encoder.FindShortestPath(referencedLine.Vertices[1], pathToValid.Vertex, false);
                    while (shortestRoute != null && !shortestRoute.Contains(referencedLine.Vertices[0]))
                    { // the vertex that should be on this shortest route, isn't anymore.
                        // exclude the current target vertex, 
                        excludeSet.Add(pathToValid.Vertex);
                        // calulate a new path-to-valid.
                        pathToValid = encoder.FindValidVertexFor(referencedLine.Vertices[0], referencedLine.Edges[0], referencedLine.Vertices[1],
                            excludeSet, false);
                        if (pathToValid == null)
                        { // a new path was not found.
                            break;
                        }
                        shortestRoute = encoder.FindShortestPath(referencedLine.Vertices[1], pathToValid.Vertex, false);
                    }
                    if (pathToValid != null)
                    { // no path found, just leave things as is.
                        var newVertices = pathToValid.ToArray().Reverse().ToList();
                        var newEdges = new List<LiveEdge>();
                        for (int idx = 0; idx < newVertices.Count - 1; idx++)
                        { // loop over edges.
                            var edge = newVertices[idx].Edge;
                            // Next OsmSharp version: use closest.Value.Value.Reverse()?
                            var reverseEdge = new LiveEdge();
                            reverseEdge.Tags = edge.Tags;
                            reverseEdge.Forward = !edge.Forward;
                            reverseEdge.Distance = edge.Distance;

                            edge = reverseEdge;
                            newEdges.Add(edge);
                        }

                        // create new location.
                        var edgesArray = new LiveEdge[newEdges.Count + referencedLine.Edges.Length];
                        newEdges.CopyTo(0, edgesArray, 0, newEdges.Count);
                        referencedLine.Edges.CopyTo(0, edgesArray, newEdges.Count, referencedLine.Edges.Length);
                        var vertexArray = new long[newVertices.Count - 1 + referencedLine.Vertices.Length];
                        newVertices.ConvertAll(x => (long)x.Vertex).CopyTo(0, vertexArray, 0, newVertices.Count - 1);
                        referencedLine.Vertices.CopyTo(0, vertexArray, newVertices.Count - 1, referencedLine.Vertices.Length);

                        referencedLine.Edges = edgesArray;
                        referencedLine.Vertices = vertexArray;

                        // adjust offset length.
                        var newLength = (float)referencedLine.Length(encoder).Value;
                        positiveOffsetLength = positiveOffsetLength + (newLength - length);
                        length = newLength;
                    }
                }
            }
            excludeSet.Clear();
            if (!encoder.IsVertexValid(referencedLine.Vertices[referencedLine.Vertices.Length - 1]))
            { // from is not valid, try to find a valid point.
                var vertexCount = referencedLine.Vertices.Length;
                var pathToValid = encoder.FindValidVertexFor(referencedLine.Vertices[vertexCount - 1], referencedLine.Edges[
                    referencedLine.Edges.Length - 1].ToReverse(), referencedLine.Vertices[vertexCount - 2], excludeSet, true);

                // build edges list.
                if (pathToValid != null)
                { // no path found, just leave things as is.
                    var shortestRoute = encoder.FindShortestPath(referencedLine.Vertices[vertexCount - 2], pathToValid.Vertex, true);
                    while (shortestRoute != null && !shortestRoute.Contains(referencedLine.Vertices[vertexCount - 1]))
                    { // the vertex that should be on this shortest route, isn't anymore.
                        // exclude the current target vertex, 
                        excludeSet.Add(pathToValid.Vertex);
                        // calulate a new path-to-valid.
                        pathToValid = encoder.FindValidVertexFor(referencedLine.Vertices[vertexCount - 1], referencedLine.Edges[
                            referencedLine.Edges.Length - 1].ToReverse(), referencedLine.Vertices[vertexCount - 2], excludeSet, true);
                        if (pathToValid == null)
                        { // a new path was not found.
                            break;
                        }
                        shortestRoute = encoder.FindShortestPath(referencedLine.Vertices[vertexCount - 2], pathToValid.Vertex, true);
                    }
                    if (pathToValid != null)
                    { // no path found, just leave things as is.
                        var newVertices = pathToValid.ToArray().ToList();
                        var newEdges = new List<LiveEdge>();
                        for (int idx = 1; idx < newVertices.Count; idx++)
                        { // loop over edges.
                            var edge = newVertices[idx].Edge;
                            newEdges.Add(edge);
                        }

                        // create new location.
                        var edgesArray = new LiveEdge[newEdges.Count + referencedLine.Edges.Length];
                        referencedLine.Edges.CopyTo(0, edgesArray, 0, referencedLine.Edges.Length);
                        newEdges.CopyTo(0, edgesArray, referencedLine.Edges.Length, newEdges.Count);
                        var vertexArray = new long[newVertices.Count - 1 + referencedLine.Vertices.Length];
                        referencedLine.Vertices.CopyTo(0, vertexArray, 0, referencedLine.Vertices.Length);
                        newVertices.ConvertAll(x => (long)x.Vertex).CopyTo(1, vertexArray, referencedLine.Vertices.Length, newVertices.Count - 1);

                        referencedLine.Edges = edgesArray;
                        referencedLine.Vertices = vertexArray;

                        // adjust offset length.
                        var newLength = (float)referencedLine.Length(encoder).Value;
                        negativeOffsetLength = negativeOffsetLength + (newLength - length);
                        length = newLength;
                    }
                }
            }

            // update offset percentags.
            referencedLine.PositiveOffsetPercentage = (float)((positiveOffsetLength / length) * 100.0);
            referencedLine.NegativeOffsetPercentage = (float)((negativeOffsetLength / length) * 100.0);

            // fill shapes.
            referencedLine.EdgeShapes = new GeoCoordinateSimple[referencedLine.Edges.Length][];
            for (int i = 0; i < referencedLine.Edges.Length; i++)
            {
                referencedLine.EdgeShapes[i] = encoder.Graph.GetEdgeShape(
                    referencedLine.Vertices[i], referencedLine.Vertices[i + 1]);
            }
        }

        /// <summary>
        /// Adjusts the given location by inserting intermediate LR-points if needed.
        /// </summary>
        /// <param name="encoder">The encoder.</param>
        /// <param name="referencedLine">The line to check.</param>
        /// <param name="points">The indexes of the LR-points.</param>
        /// 
        public static void AdjustToValidDistances(ReferencedEncoderBase encoder, ReferencedLine referencedLine, List<int> points)
        {
            ReferencedLineEncoder.AdjustToValidDistance(encoder, referencedLine, points, 0);
        }

        /// <summary>
        /// Adjusts the given location by inserting an intermediate LR-point at the point representing pointIdx.
        /// </summary>
        /// <param name="encoder">The encoder.</param>
        /// <param name="referencedLine">The line to check.</param>
        /// <param name="points">The indexes of the LR-points.</param>
        /// <param name="start">The starting vertex.</param>
        public static void AdjustToValidDistance(ReferencedEncoderBase encoder, ReferencedLine referencedLine, List<int> points, int start)
        {
            // get start/end vertex.
            var vertexIdx1 = points[start];
            var vertexIdx2 = points[start + 1];
            var count = vertexIdx2 - vertexIdx1;

            // calculate length to begin with.
            var coordinates = referencedLine.GetCoordinates(encoder, vertexIdx1, count);
            var length = coordinates.Length().Value;
            if (length > 15000)
            { // too long.
                // find the best intermediate point.
                var intermediatePoints = new SortedDictionary<double, int>();
                for (int idx = vertexIdx1 + 1; idx < vertexIdx1 + count - 2; idx++)
                {
                    var score = 0.0;
                    if(encoder.IsVertexValid(referencedLine.Vertices[idx]))
                    { // a valid vertex is obviously a better choice!
                        score = score + 4096;
                    }

                    // the length is good when close to 15000 but not over.
                    var lengthBefore = referencedLine.GetCoordinates(encoder, vertexIdx1, idx - vertexIdx1 + 1).Length();
                    if (lengthBefore.Value < 15000)
                    { // not over!
                        score = score + (1024 * (lengthBefore.Value / 15000));
                    }
                    var lengthAfter = referencedLine.GetCoordinates(encoder, idx, count - idx).Length();
                    if (lengthAfter.Value < 15000)
                    { // not over!
                        score = score + (1024 * (lengthAfter.Value / 15000));
                    }

                    // add to sorted dictionary.
                    intermediatePoints[8192 - score] = idx;
                }

                // select the best point and insert it in between.
                var bestPoint = intermediatePoints.First().Value;
                points.Insert(start + 1, bestPoint);

                // test the two distances.
                ReferencedLineEncoder.AdjustToValidDistance(encoder, referencedLine, points, start + 1);
                ReferencedLineEncoder.AdjustToValidDistance(encoder, referencedLine, points, start);
            }
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
                this.ValidateConnected(referencedLocation);
                // validate offsets.
                this.ValidateOffsets(referencedLocation);
                // validate for binary.
                if (this.ValidateForBinary) { this.ValidateBinary(referencedLocation); }

                // Step – 2 Adjust start and end node of the location to represent valid map nodes.
                ReferencedLineEncoder.AdjustToValidPoints(this.MainEncoder, referencedLocation);
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
                ReferencedLineEncoder.AdjustToValidDistances(this.MainEncoder, referencedLocation, points);

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
                lrp.Bearing = (int)this.GetBearing(referencedLocation.Vertices[idx - 1], referencedLocation.Edges[idx - 2],
                    referencedLocation.EdgeShapes[idx - 2], referencedLocation.Vertices[idx - 2], true).Value;
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
            return FunctionalRoadClass.Frc7;
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
            return (int)(referencedLocation.GetCoordinates(this.MainEncoder, vertexIdx1, vertexIdx2 - vertexIdx1).Length().Value);
        }
    }
}