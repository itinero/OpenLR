// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using Itinero;
using System;

namespace OpenLR.Referenced.Locations
{
    /// <summary>
    /// Represents a referenced line location with a graph as a reference.
    /// </summary>
    public class ReferencedLine : ReferencedLocation
    {
        /// <summary>
        /// Creates a new referenced line.
        /// </summary>
        public ReferencedLine()
        {

        }

        /// <summary>
        /// Gets or sets the vertices.
        /// </summary>
        public uint[] Vertices { get; set; }

        /// <summary>
        /// Gets or sets the edges in the form of directed edge id's.
        /// </summary>
        public long[] Edges { get; set; }

        /// <summary>
        /// Gets or sets the start location.
        /// </summary>
        /// <remarks>
        /// When the first vertex in the vertices array equals Itinero.Constants.NO_VERTEX this location is not a vertex but a location on the first edge.
        /// </remarks>
        public RouterPoint StartLocation { get; set; }

        /// <summary>
        /// Gets or sets the end location.
        /// </summary>
        /// <remarks>
        /// When the end vertex in the vertices array equals Itinero.Constants.NO_VERTEX this location is not a vertex but a location on the last edge.
        /// </remarks>
        public RouterPoint EndLocation { get; set; }

        /// <summary>
        /// Gets or sets the offset at the beginning of the path representing this location.
        /// </summary>
        public float PositiveOffsetPercentage { get; set; }

        /// <summary>
        /// Gets or sets the offset at the end of the path representing this location.
        /// </summary>
        public float NegativeOffsetPercentage { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new ReferencedLine()
            {
                Edges = this.Edges == null ? null : this.Edges.Clone() as long[],
                Vertices = this.Vertices == null ? null : this.Vertices.Clone() as uint[],
                NegativeOffsetPercentage = this.NegativeOffsetPercentage,
                PositiveOffsetPercentage = this.PositiveOffsetPercentage,
                StartLocation = new RouterPoint(
                    this.StartLocation.Latitude,
                    this.StartLocation.Longitude,
                    this.StartLocation.EdgeId,
                    this.StartLocation.Offset),
                EndLocation = new RouterPoint(
                    this.EndLocation.Latitude,
                    this.EndLocation.Longitude,
                    this.EndLocation.EdgeId,
                    this.EndLocation.Offset)
            };
        }

        /// <summary>
        /// Adds another line location to this one.
        /// </summary>
        public void Add(ReferencedLine location)
        {
            if(this.Vertices[this.Vertices.Length - 1] == location.Vertices[0])
            { // there is a match.
                // merge vertices.
                var vertices = new uint[this.Vertices.Length + location.Vertices.Length - 1];
                this.Vertices.CopyTo(vertices, 0);
                for(int idx = 1; idx < location.Vertices.Length; idx++)
                {
                    vertices[this.Vertices.Length + idx - 1] = location.Vertices[idx];
                }
                this.Vertices = vertices;

                // merge edges.
                var edges = new long[this.Edges.Length + location.Edges.Length];
                this.Edges.CopyTo(edges, 0);
                location.Edges.CopyTo(edges, this.Edges.Length);
                this.Edges = edges;
                // Update EndLocation and NegativeOffset
                this.EndLocation = location.EndLocation;
                this.NegativeOffsetPercentage = location.NegativeOffsetPercentage;
                return;
            }
            throw new Exception("Cannot add a location without them having one vertex incommon.");
        }
    }
}
