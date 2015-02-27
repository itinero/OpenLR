using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.Referenced.Locations;
using OsmSharp;
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
        /// Adjusts the given location to use valid points.
        /// </summary>
        /// <param name="referencedLine">The line to check.</param>
        public static void AdjustToValid(ReferencedEncoderBase encoder, ReferencedLine referencedLine)
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
                ReferencedLineEncoder.AdjustToValid(this.MainEncoder, referencedLocation);

                var coordinates = referencedLocation.GetCoordinates(this.MainEncoder);
                var length = coordinates.Length();

                // 3: The actual encoding now!
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

                // calculate bearing.
                location.First.Bearing = (int)this.GetBearing(referencedLocation.Vertices[0], referencedLocation.Edges[0],
                    referencedLocation.EdgeShapes[0], referencedLocation.Vertices[1], false).Value;
                location.Last.Bearing = (int)this.GetBearing(referencedLocation.Vertices[referencedLocation.Vertices.Length - 1],
                    referencedLocation.Edges[referencedLocation.Edges.Length - 1], referencedLocation.EdgeShapes[referencedLocation.Edges.Length - 1], 
                    referencedLocation.Vertices[referencedLocation.Vertices.Length - 2], true).Value;

                // calculate length.
                location.First.DistanceToNext = (int)length.Value;

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