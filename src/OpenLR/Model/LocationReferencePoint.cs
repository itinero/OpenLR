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

namespace OpenLR.Model
{
    /// <summary>
    /// Represents a point of the location which holds relevant information for a map-independent location reference.
    /// </summary>
    public class LocationReferencePoint
    {
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public Coordinate Coordinate { get; set; }

        /// <summary>
        /// Gets or sets the bearing.
        /// </summary>
        /// <remarks>Bearing is an angle in the range [0, 360[.</remarks>
        public int? Bearing { get; set; }

        /// <summary>
        /// Gets or sets the functional road class.
        /// </summary>
        public FunctionalRoadClass? FuntionalRoadClass { get; set; }

        /// <summary>
        /// Gets or sets the form of way.
        /// </summary>
        public FormOfWay? FormOfWay { get; set; }

        /// <summary>
        /// Gets or sets the lowest functional road class to the next point.
        /// </summary>
        public FunctionalRoadClass? LowestFunctionalRoadClassToNext { get; set; }

        /// <summary>
        /// Gets or sets the distance to the next point in meters.
        /// </summary>
        public int DistanceToNext { get; set; }
    }
}