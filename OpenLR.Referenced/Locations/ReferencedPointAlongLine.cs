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

using OpenLR.Model;
using OpenLR.Referenced.Scoring;
using OpenLR.Referenced;
using OsmSharp.Units.Distance;

namespace OpenLR.Referenced.Locations
{
    /// <summary>
    /// Represents a referenced point along line location with a graph as a reference.
    /// </summary>
    public class ReferencedPointAlongLine : ReferencedLocation
    {
        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the route (vertex->edge->vertex->edge->vertex) associated with this location.
        /// </summary>
        public ReferencedLine Route { get; set; }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        public Orientation Orientation { get; set; }

        /// <summary>
        /// Holds an indication of accuracy.
        /// </summary>
        public Score Score { get; set; }

        /// <summary>
        /// Gets or sets the edge meta.
        /// </summary>
        public EdgeMeta EdgeMeta { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            if(this.Route == null)
            {
                return new ReferencedPointAlongLine()
                {
                    EdgeMeta = this.EdgeMeta,
                    Latitude = this.Latitude,
                    Longitude = this.Longitude,
                    Orientation = this.Orientation,
                    Score = this.Score
                };
            }
            return new ReferencedPointAlongLine()
            {
                EdgeMeta = this.EdgeMeta,
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                Orientation = this.Orientation,
                Route = this.Route.Clone() as ReferencedLine,
                Score = this.Score
            };
        }
    }

    /// <summary>
    /// Represents edge meta data.
    /// </summary>
    public struct EdgeMeta
    {
        /// <summary>
        /// Gets or sets the edge idx.
        /// </summary>
        public int Idx { get; set; }

        /// <summary>
        /// Gets or sets the edge idx.
        /// </summary>
        public Meter Offset { get; set; }

        /// <summary>
        /// Gets or sets the edge length.
        /// </summary>
        public Meter Length { get; set; }
    }
}