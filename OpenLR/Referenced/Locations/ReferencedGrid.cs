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

namespace OpenLR.Referenced.Locations
{
    /// <summary>
    /// Represents a referenced grid location with a graph as a reference.
    /// </summary>
    /// <remarks>The reference graph play no part here, a grid is just a grid.</remarks>
    public class ReferencedGrid : ReferencedLocation
    {
        /// <summary>
        /// Gets or sets the lower left latitude.
        /// </summary>
        public double LowerLeftLatitude { get; set; }

        /// <summary>
        /// Gets or sets the lower left longitude.
        /// </summary>
        public double LowerLeftLongitude { get; set; }

        /// <summary>
        /// Gets or sets the upper right latitude.
        /// </summary>
        public double UpperRightLatitude { get; set; }

        /// <summary>
        /// Gets or sets the upper right longitude.
        /// </summary>
        public double UpperRightLongitude { get; set; }

        /// <summary>
        /// Gets or sets the number of rows.
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// Gets or sets the number of columns.
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        public override object Clone()
        {
            return new ReferencedGrid()
            {
                LowerLeftLatitude = this.LowerLeftLatitude,
                LowerLeftLongitude = this.LowerLeftLongitude,
                UpperRightLatitude = this.UpperRightLatitude,
                UpperRightLongitude = this.UpperRightLongitude,
                Rows = this.Rows,
                Columns = this.Columns
            };
        }
    }
}