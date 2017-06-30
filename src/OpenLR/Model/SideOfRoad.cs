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
    /// The side of road information (SOR) describes the relationship between the point of interest and a referenced line.
    /// </summary>
    public enum SideOfRoad
    {
        /// <summary>
        /// 0 - Point is directly on (or above) the road, or determination of right/left side is not applicable (default).
        /// </summary>
        OnOrAbove = 0,
        /// <summary>
        /// 1 - Point is on right side of the road.
        /// </summary>
        Right = 1,
        /// <summary>
        /// 2 - Point is on the left side of the road.
        /// </summary>
        Left = 2,
        /// <summary>
        /// 3 - Point is on both sides of the road.
        /// </summary>
        Both = 3
    }
}