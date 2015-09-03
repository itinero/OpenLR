using OpenLR.Referenced.Exceptions;
using OpenLR.Referenced.Locations;
using OpenLR.Referenced.Router;
using OsmSharp;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Units.Distance;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenLR.Referenced
{
    /// <summary>
    /// Contains encoder extensions.
    /// </summary>
    public static class ReferencedEncoderBaseExtensions
    {
        /// <summary>
        /// Builds a point along line location.
        /// </summary>
        /// <returns></returns>
        public static ReferencedPointAlongLine BuildPointAlongLine(this ReferencedEncoderBase encoder, GeoCoordinate location, double boxSize = 0.1)
        {
            return encoder.BuildPointAlongLine(location, 10, boxSize);
        }

        /// <summary>
        /// Builds a point along line location.
        /// </summary>
        /// <returns></returns>
        public static ReferencedPointAlongLine BuildPointAlongLine(this ReferencedEncoderBase encoder, GeoCoordinate location, Meter maxDistance, double boxSize = 0.1)
        {
            if (location == null) { throw new ArgumentNullException("location"); }

            // get the closest edge that can be traversed to the given location.
            var closest = encoder.Graph.GetClosestEdge(location, maxDistance, boxSize);
            if (closest == null)
            { // no location could be found. 
                throw new BuildLocationFailedException("No network features found near the given location. Make sure the network covers the given location.");
            }
            var oneway = encoder.Vehicle.IsOneWay(encoder.Graph.TagsIndex.Get(closest.Item3.Tags));
            if(oneway.HasValue && oneway.Value != closest.Item3.Forward)
            { // when the edge is not traversible in the direct that it's given in, reverse it.
                var reverseEdge = new LiveEdge();
                reverseEdge.Tags = closest.Item3.Tags;
                reverseEdge.Forward = !closest.Item3.Forward;
                reverseEdge.Distance = closest.Item3.Distance;

                closest = new Tuple<long, long, LiveEdge>(closest.Item2, closest.Item1,
                    reverseEdge);
            }

            // get locations of edge.
            var startLocation = encoder.GetVertexLocation(closest.Item1).ToGeoCoordinate();
            var endLocation = encoder.GetVertexLocation(closest.Item2).ToGeoCoordinate();

            // build a proper referenced line.
            var referencedLine = new ReferencedLine(encoder.Graph);
            referencedLine.Vertices = new long[] { closest.Item1, closest.Item2 };
            referencedLine.Edges = new LiveEdge[] { closest.Item3 };
            referencedLine.PositiveOffsetPercentage = 0;
            referencedLine.NegativeOffsetPercentage = 0;
            referencedLine.EdgeShapes = new GeoCoordinateSimple[][] 
            {
                encoder.Graph.GetEdgeShape(
                    referencedLine.Vertices[0], referencedLine.Vertices[1])
            };

            // build the point-along-line location.
            return new ReferencedPointAlongLine()
            {
                Route = referencedLine,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Orientation = Model.Orientation.NoOrientation
            };
        }

        ///// <summary>
        ///// Builds a line location along the shortest path between start and end location.
        ///// </summary>
        ///// <param name="encoder">The encoder.</param>
        ///// <param name="startLocation">The start location.</param>
        ///// <param name="endLocation">The end location.</param>
        ///// <returns></returns>
        ///// <remarks>This should only be used when sure the start and endlocation or very close to the network use for encoding.</remarks>
        //public static ReferencedLine BuildLineLocation(this ReferencedEncoderBase encoder, GeoCoordinate startLocation, GeoCoordinate endLocation)
        //{
        //    return encoder.BuildLineLocation(startLocation, endLocation, 1);
        //}

        ///// <summary>
        ///// Builds a line location along the shortest path between start and end location.
        ///// </summary>
        ///// <param name="encoder">The encoder.</param>
        ///// <param name="startLocation">The start location.</param>
        ///// <param name="endLocation">The end location.</param>
        ///// <param name="tolerance">The tolerance value, the minimum distance between a given start or endlocation and the network used for encoding.</param>
        ///// <returns></returns>
        ///// <remarks>This should only be used when sure the start and endlocation or very close to the network use for encoding.</remarks>
        //public static ReferencedLine BuildLineLocation(this ReferencedEncoderBase encoder, GeoCoordinate startLocation, GeoCoordinate endLocation, Meter tolerance)
        //{
        //    PointF2D bestProjected;
        //    LinePointPosition bestPosition;
        //    Meter bestStartOffset;
        //    Meter bestEndOffset;
        //    double epsilon = 0.1;

        //    if (startLocation == null) { throw new ArgumentNullException("startLocation"); }
        //    if (endLocation == null) { throw new ArgumentNullException("endLocation"); }

        //    // search start and end location hooks.
        //    var startEdge = encoder.Graph.GetClosestEdge(startLocation, tolerance);
        //    if (startEdge == null)
        //    { // no closest edge found within tolerance, encoding has failed!
        //        throw new BuildLocationFailedException("Location {0} is too far from the network used for encoding with used tolerance {1}",
        //            startLocation, tolerance);
        //    }
        //    // project the startlocation on the edge.
        //    var coordinates = encoder.Graph.GetCoordinates(startEdge);
        //    var startEdgeLength = coordinates.Length();
        //    if (!coordinates.ProjectOn(startLocation, out bestProjected, out bestPosition, out bestStartOffset))
        //    { // projection failed,.
        //        throw new BuildLocationFailedException("Projection of location {0} on the closest edge failed.",
        //            startLocation);
        //    }
        //    // construct from pathsegments.
        //    var startPaths = new List<PathSegment>();
        //    if (bestStartOffset.Value < epsilon)
        //    { // use the first vertex as start location.
        //        startPaths.Add(new PathSegment(startEdge.Item1));
        //    }
        //    else if ((startEdgeLength.Value - bestStartOffset.Value) < epsilon)
        //    { // use the last vertex as end start location.
        //        startPaths.Add(new PathSegment(startEdge.Item2));
        //    }
        //    else
        //    { // point is somewhere in between.
        //        var tags = encoder.Graph.TagsIndex.Get(startEdge.Item3.Tags);
        //        var oneway = encoder.Vehicle.IsOneWay(tags);

        //        // weightBefore: vertex1->{x}
        //        var weightBefore = encoder.Vehicle.Weight(tags, (float)bestStartOffset.Value);
        //        // weightAfter: {x}->vertex2.
        //        var weightAfter = encoder.Vehicle.Weight(tags, (float)(startEdgeLength.Value - bestStartOffset.Value));

        //        if (startEdge.Item3.Forward)
        //        { // edge is forward.
        //            // vertex1->{x}->vertex2

        //            // consider the part {x}->vertex1 x being the source.
        //            if (oneway == null || !oneway.Value)
        //            {  // edge cannot be oneway forward.
        //                startPaths.Add(new PathSegment(startEdge.Item1, weightBefore, startEdge.Item3.ToReverse(),
        //                    new PathSegment(-1)));
        //            }

        //            // consider the part {x}->vertex2 x being the source.
        //            if (oneway == null || oneway.Value)
        //            { // edge cannot be oneway backward.
        //                startPaths.Add(new PathSegment(startEdge.Item2, weightAfter, startEdge.Item3,
        //                    new PathSegment(-1)));
        //            }
        //        }
        //        else
        //        { // edge is backward.
        //            // vertex1->{x}->vertex2

        //            // consider the part {x}->vertex1 x being the source.
        //            if (oneway == null || oneway.Value)
        //            {  // edge cannot be oneway forward but edge is backward.
        //                startPaths.Add(new PathSegment(startEdge.Item1, weightBefore, startEdge.Item3.ToReverse(),
        //                    new PathSegment(-1)));
        //            }

        //            // consider the part {x}->vertex2 x being the source.
        //            if (oneway == null || !oneway.Value)
        //            { // edge cannot be oneway backward but edge is backward.
        //                startPaths.Add(new PathSegment(startEdge.Item2, weightAfter, startEdge.Item3,
        //                    new PathSegment(-1)));
        //            }
        //        }
        //    }

        //    var endEdge = encoder.Graph.GetClosestEdge(endLocation, tolerance);
        //    if (endEdge == null)
        //    { // no closest edge found within tolerance, encoding has failed!
        //        throw new BuildLocationFailedException("Location {0} is too far from the network used for encoding with used tolerance {1}",
        //            endLocation, tolerance);
        //    }
        //    // project the endlocation on the edge.
        //    coordinates = encoder.Graph.GetCoordinates(endEdge);
        //    var endEdgeLength = coordinates.Length();
        //    if (!coordinates.ProjectOn(endLocation, out bestProjected, out bestPosition, out bestEndOffset))
        //    { // projection failed.
        //        throw new BuildLocationFailedException("Projection of location {0} on the closest edge failed.",
        //            endLocation);
        //    }
        //    // construct from pathsegments.
        //    var endPaths = new List<PathSegment>();
        //    if (bestEndOffset.Value < epsilon)
        //    { // use the first vertex as end location.
        //        endPaths.Add(new PathSegment(endEdge.Item1));
        //    }
        //    else if ((endEdgeLength.Value - bestEndOffset.Value) < epsilon)
        //    { // use the last vertex as end end location.
        //        endPaths.Add(new PathSegment(endEdge.Item2));
        //    }
        //    else
        //    { // point is somewhere in between.
        //        var tags = encoder.Graph.TagsIndex.Get(endEdge.Item3.Tags);
        //        var oneway = encoder.Vehicle.IsOneWay(tags);
        //        // weightBefore: vertex1->{x}
        //        var weightBefore = encoder.Vehicle.Weight(tags, (float)bestEndOffset.Value);
        //        // weightAfter: {x}->vertex2.
        //        var weightAfter = encoder.Vehicle.Weight(tags, (float)(endEdgeLength.Value - bestEndOffset.Value));

        //        if (endEdge.Item3.Forward)
        //        { // edge is forward.
        //            // vertex1->{x}->vertex2

        //            // consider vertex1->{x} x being the target.
        //            if (oneway == null || oneway.Value)
        //            {  // edge cannot be oneway backward.
        //                endPaths.Add(new PathSegment(-1, weightBefore, endEdge.Item3,
        //                    new PathSegment(endEdge.Item1)));
        //            }

        //            // consider vertex2->{x} x being the target.
        //            if (oneway == null || !oneway.Value)
        //            { // edge cannot be onway forward.
        //                endPaths.Add(new PathSegment(-1, weightAfter, endEdge.Item3.ToReverse(),
        //                    new PathSegment(endEdge.Item2)));
        //            }
        //        }
        //        else
        //        { // edge is backward.
        //            // vertex1->{x}->vertex2

        //            // consider vertex1->{x} x being the target.
        //            if (oneway == null || !oneway.Value)
        //            {  // edge cannot be oneway backward.
        //                endPaths.Add(new PathSegment(-1, weightBefore, endEdge.Item3,
        //                    new PathSegment(endEdge.Item1)));
        //            }

        //            // consider vertex2->{x} x being the target.
        //            if (oneway == null || oneway.Value)
        //            { // edge cannot be onway forward.
        //                endPaths.Add(new PathSegment(-1, weightAfter, endEdge.Item3.ToReverse(),
        //                    new PathSegment(endEdge.Item2)));
        //            }
        //        }
        //    }

        //    // build a route.
        //    var vertices = new List<long>();
        //    var edges = new List<LiveEdge>();

        //    if(startEdge.Item3.Equals(endEdge.Item3))
        //    { // same identical edge.
        //        if (bestEndOffset.Value > bestStartOffset.Value)
        //        { // path from->to.
        //            vertices.Add(startEdge.Item1);
        //            vertices.Add(startEdge.Item2);
        //            edges.Add(startEdge.Item3);
        //        }
        //        else
        //        { // path to->from.
        //            vertices.Add(startEdge.Item2);
        //            vertices.Add(startEdge.Item1);
        //            edges.Add((LiveEdge)startEdge.Item3.Reverse());
        //        }
        //    }
        //    else if (startEdge.Item3.Equals(endEdge.Item3.Reverse()))
        //    { // same edge but reversed.
        //        var bestEndOffsetReversed = endEdgeLength.Value - bestEndOffset.Value;
        //        if (bestEndOffsetReversed > bestStartOffset.Value)
        //        { // path from->to.
        //            vertices.Add(startEdge.Item1);
        //            vertices.Add(startEdge.Item2);
        //            edges.Add(startEdge.Item3);
        //        }
        //        else
        //        { // path to->from.
        //            vertices.Add(startEdge.Item2);
        //            vertices.Add(startEdge.Item1);
        //            edges.Add((LiveEdge)startEdge.Item3.Reverse());
        //        }
        //    }
        //    else
        //    { // route as usual.
        //        // calculate shortest path.
        //        var shortestPath = encoder.FindShortestPath(startPaths, endPaths, true);
        //        if (shortestPath == null)
        //        { // routing failed,.
        //            throw new BuildLocationFailedException("A route between start {0} and end point {1} was not found.",
        //                startLocation, endLocation);
        //        }

        //        // convert to edge and vertex-array.
        //        vertices.Add(shortestPath.Vertex);
        //        edges.Add(shortestPath.Edge);
        //        while (shortestPath.From != null)
        //        {
        //            shortestPath = shortestPath.From;
        //            vertices.Add(shortestPath.Vertex);
        //            if (shortestPath.From != null)
        //            {
        //                edges.Add(shortestPath.Edge);
        //            }
        //        }
        //        vertices.Reverse();
        //        edges.Reverse();
        //    }

        //    // extract vertices, edges and offsets.
        //    if (vertices[0] < 0)
        //    { // replace the first virtual vertex with the real vertex.
        //        if (vertices[1] == startEdge.Item1)
        //        { // the virtual vertex should be item2.
        //            vertices[0] = startEdge.Item2;
        //        }
        //        else
        //        { // the virtual vertex should be item1.
        //            vertices[0] = startEdge.Item1;
        //        }
        //    }
        //    if (vertices[vertices.Count - 1] < 0)
        //    { // replace the last virtual vertex with the real vertex.
        //        if (vertices[vertices.Count - 2] == endEdge.Item1)
        //        { // the virtual vertex should be item2.
        //            vertices[vertices.Count - 1] = endEdge.Item2;
        //        }
        //        else
        //        { // the virtual vertex should be item1.
        //            vertices[vertices.Count - 1] = endEdge.Item1;
        //        }
        //    }

        //    // calculate offset.
        //    var referencedLine = new OpenLR.Referenced.Locations.ReferencedLine(encoder.Graph)
        //    {
        //        Edges = edges.ToArray(),
        //        Vertices = vertices.ToArray()
        //    };
        //    var length = referencedLine.Length(encoder).Value;

        //    // project again on the first edge.
        //    startEdge = new Tuple<long, long, LiveEdge>(referencedLine.Vertices[0], referencedLine.Vertices[1], referencedLine.Edges[0]);
        //    coordinates = encoder.Graph.GetCoordinates(startEdge);
        //    if (!coordinates.ProjectOn(startLocation, out bestProjected, out bestPosition, out bestStartOffset))
        //    { // projection did not succeed.
        //        throw new BuildLocationFailedException("Projection of location {0} on the first edge of shortest path failed.",
        //            endLocation);
        //    }
        //    var positivePercentageOffset = (float)System.Math.Max(System.Math.Min((bestStartOffset.Value / length) * 100.0, 100), 0);

        //    // project again on the last edge.
        //    endEdge = new Tuple<long, long, LiveEdge>(referencedLine.Vertices[referencedLine.Vertices.Length - 2],
        //        referencedLine.Vertices[referencedLine.Vertices.Length - 1], referencedLine.Edges[referencedLine.Edges.Length - 1]);
        //    coordinates = encoder.Graph.GetCoordinates(endEdge);
        //    endEdgeLength = coordinates.Length();
        //    if (!coordinates.ProjectOn(endLocation, out bestProjected, out bestPosition, out bestEndOffset))
        //    { // projection did not succeed.
        //        throw new BuildLocationFailedException("Projection of location {0} on the first edge of shortest path failed.",
        //            endLocation);
        //    }
        //    var negativePercentageOffset = (float)System.Math.Max((System.Math.Min(((endEdgeLength.Value - bestEndOffset.Value) / length) * 100.0, 100)), 0);

        //    return encoder.BuildLineLocation(vertices.ToArray(), edges.ToArray(), positivePercentageOffset, negativePercentageOffset);
        //}

        /// <summary>
        /// Builds a line location along the shortest path between start and end location while the start and end location are the exact location of vertices from the netwerk being encoded on.
        /// </summary>
        /// <param name="encoder">The encoder.</param>
        /// <param name="startLocation1">The first point of the edge containing the start location.</param>
        /// <param name="startLocation2">The second point of the edge containing the start location.</param>
        /// <param name="startOffset">The offset of the start location in meters.</param>
        /// <param name="endLocation1">The first point of the edge containing the end location.</param>
        /// <param name="endLocation2">The second point of the edge containing the end location.</param>
        /// <param name="endOffset">The offset of the end location in meters.</param>
        /// <param name="tolerance">The tolerance value, the minimum distance between a given start or endlocation and the network used for encoding.</param>
        /// <returns></returns>
        /// <remarks>The edges need to be traversible from first to second point.</remarks>
        public static ReferencedLine BuildLineLocationVertexExact(this ReferencedEncoderBase encoder, 
            GeoCoordinate startLocation1, GeoCoordinate startLocation2, Meter startOffset,
            GeoCoordinate endLocation1, GeoCoordinate endLocation2, Meter endOffset, Meter tolerance)
        {
            var epsilon = tolerance.Value;

            if (startLocation1 == null) { throw new ArgumentNullException("startLocation1"); }
            if (startLocation2 == null) { throw new ArgumentNullException("startLocation2"); }
            if (endLocation1 == null) { throw new ArgumentNullException("endLocation1"); }
            if (endLocation2 == null) { throw new ArgumentNullException("endLocation2"); }

            // search start and end location hooks.
            var startEdge = encoder.Graph.GetClosestEdge(startLocation1, startLocation2, tolerance);
            if (startEdge == null)
            { // no closest edge found within tolerance, encoding has failed!
                throw new BuildLocationFailedException("Location {0}->{1} is too far from the network used for encoding with used tolerance {2}",
                    startLocation1, startLocation2, tolerance);
            }
            // project the startlocation on the edge.
            var coordinates = encoder.Graph.GetCoordinates(startEdge);
            var startEdgeLength = coordinates.Length();
            // construct from pathsegments.
            var startPaths = new List<PathSegment>();
            if (startOffset.Value < epsilon)
            { // use the first vertex as start location.
                startPaths.Add(new PathSegment(startEdge.Item1));
            }
            else if ((startEdgeLength.Value - startOffset.Value) < epsilon)
            { // use the last vertex as end start location.
                startPaths.Add(new PathSegment(startEdge.Item2));
            }
            else
            { // point is somewhere in between.
                var tags = encoder.Graph.TagsIndex.Get(startEdge.Item3.Tags);
                var oneway = encoder.Vehicle.IsOneWay(tags);

                // weightBefore: vertex1->{x}
                var weightBefore = encoder.Vehicle.Weight(tags, (float)startOffset.Value);
                // weightAfter: {x}->vertex2.
                var weightAfter = encoder.Vehicle.Weight(tags, (float)(startEdgeLength.Value - startOffset.Value));

                if (startEdge.Item3.Forward)
                { // edge is forward.
                    // vertex1->{x}->vertex2

                    // consider the part {x}->vertex1 x being the source.
                    if (oneway == null || !oneway.Value)
                    {  // edge cannot be oneway forward.
                        startPaths.Add(new PathSegment(startEdge.Item1, weightBefore, startEdge.Item3.ToReverse(),
                            new PathSegment(-1)));
                    }

                    // consider the part {x}->vertex2 x being the source.
                    if (oneway == null || oneway.Value)
                    { // edge cannot be oneway backward.
                        startPaths.Add(new PathSegment(startEdge.Item2, weightAfter, startEdge.Item3,
                            new PathSegment(-1)));
                    }
                }
                else
                { // edge is backward.
                    // vertex1->{x}->vertex2

                    // consider the part {x}->vertex1 x being the source.
                    if (oneway == null || oneway.Value)
                    {  // edge cannot be oneway forward but edge is backward.
                        startPaths.Add(new PathSegment(startEdge.Item1, weightBefore, startEdge.Item3.ToReverse(),
                            new PathSegment(-1)));
                    }

                    // consider the part {x}->vertex2 x being the source.
                    if (oneway == null || !oneway.Value)
                    { // edge cannot be oneway backward but edge is backward.
                        startPaths.Add(new PathSegment(startEdge.Item2, weightAfter, startEdge.Item3,
                            new PathSegment(-1)));
                    }
                }
            }

            var endEdge = encoder.Graph.GetClosestEdge(endLocation1, endLocation2, tolerance);
            if (endEdge == null)
            { // no closest edge found within tolerance, encoding has failed!
                throw new BuildLocationFailedException("Location {0}->{1} is too far from the network used for encoding with used tolerance {2}",
                    endLocation1, endLocation2, tolerance);
            }
            // project the endlocation on the edge.
            coordinates = encoder.Graph.GetCoordinates(endEdge);
            var endEdgeLength = coordinates.Length();
            // construct from pathsegments.
            var endPaths = new List<PathSegment>();
            if (endOffset.Value < epsilon)
            { // use the first vertex as end location.
                endPaths.Add(new PathSegment(endEdge.Item1));
            }
            else if ((endEdgeLength.Value - endOffset.Value) < epsilon)
            { // use the last vertex as end end location.
                endPaths.Add(new PathSegment(endEdge.Item2));
            }
            else
            { // point is somewhere in between.
                var tags = encoder.Graph.TagsIndex.Get(endEdge.Item3.Tags);
                var oneway = encoder.Vehicle.IsOneWay(tags);
                // weightBefore: vertex1->{x}
                var weightBefore = encoder.Vehicle.Weight(tags, (float)endOffset.Value);
                // weightAfter: {x}->vertex2.
                var weightAfter = encoder.Vehicle.Weight(tags, (float)(endEdgeLength.Value - endOffset.Value));

                if (endEdge.Item3.Forward)
                { // edge is forward.
                    // vertex1->{x}->vertex2

                    // consider vertex1->{x} x being the target.
                    if (oneway == null || oneway.Value)
                    {  // edge cannot be oneway backward.
                        endPaths.Add(new PathSegment(-1, weightBefore, endEdge.Item3,
                            new PathSegment(endEdge.Item1)));
                    }

                    // consider vertex2->{x} x being the target.
                    if (oneway == null || !oneway.Value)
                    { // edge cannot be onway forward.
                        endPaths.Add(new PathSegment(-1, weightAfter, endEdge.Item3.ToReverse(),
                            new PathSegment(endEdge.Item2)));
                    }
                }
                else
                { // edge is backward.
                    // vertex1->{x}->vertex2

                    // consider vertex1->{x} x being the target.
                    if (oneway == null || !oneway.Value)
                    {  // edge cannot be oneway backward.
                        endPaths.Add(new PathSegment(-1, weightBefore, endEdge.Item3,
                            new PathSegment(endEdge.Item1)));
                    }

                    // consider vertex2->{x} x being the target.
                    if (oneway == null || oneway.Value)
                    { // edge cannot be onway forward.
                        endPaths.Add(new PathSegment(-1, weightAfter, endEdge.Item3.ToReverse(),
                            new PathSegment(endEdge.Item2)));
                    }
                }
            }

            // build a route.
            var vertices = new List<long>();
            var edges = new List<LiveEdge>();

            if (startEdge.Item3.Equals(endEdge.Item3.Reverse()))
            { // same edge but reversed.
                // invert end offset.
                endOffset = encoder.Graph.GetCoordinates(startEdge).Length().Value - endOffset.Value;
                
                // use exactly the same edge.
                endEdge = startEdge;
            } 
           
            if (startEdge.Item3.Equals(endEdge.Item3))
            { // same identical edge.
                var endOffsetFromStart = encoder.Graph.GetCoordinates(startEdge).Length().Value - endOffset.Value;
                if (endOffsetFromStart > startOffset.Value)
                { // path from->to.
                    vertices.Add(startEdge.Item1);
                    vertices.Add(startEdge.Item2);
                    edges.Add(startEdge.Item3);
                }
                else
                { // path to->from.
                    var reverseEdge = (LiveEdge)startEdge.Item3.Reverse();
                    vertices.Add(startEdge.Item2);
                    vertices.Add(startEdge.Item1);
                    edges.Add(reverseEdge);

                    // we need to reverse some stuff.
                    startOffset = encoder.Graph.GetCoordinates(startEdge).Length().Value - startOffset.Value;
                    startEdge = new Tuple<long, long, LiveEdge>(
                        startEdge.Item2, startEdge.Item1, reverseEdge);
                    endOffset = encoder.Graph.GetCoordinates(startEdge).Length().Value - endOffset.Value;
                    endEdge = startEdge;
                }
            }
            else
            { // route as usual.
                // calculate shortest path.
                var shortestPath = encoder.FindShortestPath(startPaths, endPaths, true);
                if (shortestPath == null)
                { // routing failed,.
                    throw new BuildLocationFailedException("A route between start {0}->{1} [@{2}] and end point {3}->{4} [@{5}] was not found.",
                        startLocation1, startLocation2, startOffset, endLocation1, endLocation2, endOffset);
                }

                // convert to edge and vertex-array.
                vertices.Add(shortestPath.Vertex);
                edges.Add(shortestPath.Edge);
                while (shortestPath.From != null)
                {
                    shortestPath = shortestPath.From;
                    vertices.Add(shortestPath.Vertex);
                    if (shortestPath.From != null)
                    {
                        edges.Add(shortestPath.Edge);
                    }
                }
                vertices.Reverse();
                edges.Reverse();
            }

            // extract vertices, edges and offsets.
            if (vertices[0] < 0)
            { // replace the first virtual vertex with the real vertex.
                if (vertices[1] == startEdge.Item1)
                { // the virtual vertex should be item2.
                    vertices[0] = startEdge.Item2;
                }
                else
                { // the virtual vertex should be item1.
                    vertices[0] = startEdge.Item1;
                }
            }
            if (vertices[vertices.Count - 1] < 0)
            { // replace the last virtual vertex with the real vertex.
                if (vertices[vertices.Count - 2] == endEdge.Item1)
                { // the virtual vertex should be item2.
                    vertices[vertices.Count - 1] = endEdge.Item2;
                }
                else
                { // the virtual vertex should be item1.
                    vertices[vertices.Count - 1] = endEdge.Item1;
                }
            }

            // calculate offset.
            var referencedLine = new OpenLR.Referenced.Locations.ReferencedLine(encoder.Graph)
            {
                Edges = edges.ToArray(),
                Vertices = vertices.ToArray()
            };
            var length = referencedLine.Length(encoder).Value;

            if(length < epsilon)
            { // the total length of the route is smaller than tolerance value, in this case the result can be anything.
                // exception is the best option here, decrease tolerance value or expand too short locations.
                throw new BuildLocationFailedException(
                    "Cannot build location: Total length of perliminary location only {1}m, smaller than tolerance value {0}m.", epsilon, length);
            }

            // project again on the start edge.
            var positivePercentageOffset = 0f;
            var edgeLength = encoder.Graph.GetCoordinates(startEdge).Length();
            if (startOffset.Value < epsilon && startEdge.Item1 == referencedLine.Vertices[0])
            { 
                positivePercentageOffset = 0f;
            }
            else if (Math.Abs(startOffset.Value - length) < epsilon && startEdge.Item2 == referencedLine.Vertices[0])
            {
                positivePercentageOffset = (float)((edgeLength.Value / length) * 100.0);
            }
            else if(startEdge.Item1 == referencedLine.Vertices[0] && startEdge.Item2 == referencedLine.Vertices[1])
            { // forward edge.
                positivePercentageOffset = (float)System.Math.Max(System.Math.Min((startOffset.Value / length) * 100.0, 100), 0);
            }
            else if (startEdge.Item2 == referencedLine.Vertices[0] && startEdge.Item1 == referencedLine.Vertices[1])
            { // backward edge.
                positivePercentageOffset = (float)System.Math.Max(System.Math.Min(((edgeLength.Value - startOffset.Value) / length) * 100.0, 100), 0);
            }
            else 
            {
                throw new BuildLocationFailedException("Routing failed: first edge in route is not edge that was started from.");
            }

            // project again on the end edge.
            var negativePercentageOffset = 0f;
            edgeLength = encoder.Graph.GetCoordinates(endEdge).Length();
            if (endOffset.Value < epsilon && endEdge.Item1 == referencedLine.Vertices[referencedLine.Vertices.Length - 1])
            {
                negativePercentageOffset = 0f;
            }
            else if (Math.Abs(endOffset.Value - length) < epsilon && endEdge.Item1 == referencedLine.Vertices[referencedLine.Vertices.Length - 2])
            {
                negativePercentageOffset = (float)((edgeLength.Value / length) * 100.0);
            }
            else if (endEdge.Item1 == referencedLine.Vertices[referencedLine.Vertices.Length - 2] && 
                     endEdge.Item2 == referencedLine.Vertices[referencedLine.Vertices.Length - 1])
            { // forward edge.
                negativePercentageOffset = (float)System.Math.Max(System.Math.Min((endOffset.Value / length) * 100.0, 100), 0);
            }
            else if (endEdge.Item1 == referencedLine.Vertices[referencedLine.Vertices.Length - 1] && 
                     endEdge.Item2 == referencedLine.Vertices[referencedLine.Vertices.Length - 2])
            { // backward edge.
                negativePercentageOffset = (float)System.Math.Max(System.Math.Min(((edgeLength.Value - endOffset.Value) / length) * 100.0, 100), 0);
            }
            else 
            {
                throw new BuildLocationFailedException("Routing failed: last edge in route is not edge that was ended with.");
            }



            return encoder.BuildLineLocation(vertices.ToArray(), edges.ToArray(), positivePercentageOffset, negativePercentageOffset);
        }

        /// <summary>
        /// Builds a line location given a sequence of vertex->edge->vertex...edge->vertex.
        /// </summary>
        /// <param name="encoder">The encoder.</param>
        /// <param name="vertices">The vertices along the path to create the location for. Contains at least two vertices (#vertices = #edges + 1).</param>
        /// <param name="edges">The edge along the path to create the location for. Contains at least one edge (#vertices = #edges + 1). Edges need to be traversible by the vehicle profile used by the encoder in the direction of the path</param>
        /// <param name="positivePercentageOffset">The offset in percentage relative to the distance of the total path and it's start. [0-100[</param>
        /// <param name="negativePercentageOffset">The offset in percentage relative to the distance of the total path and it's end. [0-100[</param>
        /// <returns></returns>
        public static ReferencedLine BuildLineLocation(this ReferencedEncoderBase encoder, long[] vertices, LiveEdge[] edges,
            float positivePercentageOffset, float negativePercentageOffset)
        {
            // validate parameters.
            if (encoder == null) { throw new ArgumentNullException("encoder"); }
            if (vertices == null) { throw new ArgumentNullException("vertices"); }
            if (edges == null) { throw new ArgumentNullException("edges"); }
            if (vertices.Length < 2) { throw new ArgumentOutOfRangeException("vertices", "A referenced line location can only be created with a valid path consisting of at least two vertices and one edge."); }
            if (edges.Length < 1) { throw new ArgumentOutOfRangeException("edges", "A referenced line location can only be created with a valid path consisting of at least two vertices and one edge."); }
            if (edges.Length + 1 != vertices.Length) { throw new ArgumentException("The #vertices need to equal #edges + 1 to have a valid path."); }

            if (positivePercentageOffset < 0 || positivePercentageOffset >= 100) { throw new ArgumentOutOfRangeException("positivePercentageOffset", "The positive percentage offset should be in the range [0-100[."); }
            if (negativePercentageOffset < 0 || negativePercentageOffset >= 100) { throw new ArgumentOutOfRangeException("negativePercentageOffset", "The negative percentage offset should be in the range [0-100[."); }
            if ((negativePercentageOffset + positivePercentageOffset) > 100) { throw new ArgumentException("The negative and positive percentage offsets together should be in the range [0-100[."); }

            // OK, now we have a naive location, we need to check if it's valid.
            // see: §F section 11.1 @ http://www.tomtom.com/lib/OpenLR/OpenLR-whitepaper.pdf
            var referencedLine = new OpenLR.Referenced.Locations.ReferencedLine(encoder.Graph)
            {
                Edges = edges.Clone() as LiveEdge[],
                Vertices = vertices.Clone() as long[]
            };
            referencedLine.NegativeOffsetPercentage = negativePercentageOffset;
            referencedLine.PositiveOffsetPercentage = positivePercentageOffset;

            // fill shapes.
            referencedLine.EdgeShapes = new GeoCoordinateSimple[referencedLine.Edges.Length][];
            for (int i = 0; i < referencedLine.Edges.Length; i++)
            {
                referencedLine.EdgeShapes[i] = encoder.Graph.GetEdgeShape(
                    referencedLine.Vertices[i], referencedLine.Vertices[i + 1]);
            }

            return referencedLine;
        }
    }
}
