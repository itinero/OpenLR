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
    /// Enumerates type of orientation.
    /// </summary>
    public enum Orientation
    {
        /// <summary>
        /// 0 - Point has no sense of orientation, or determination of orientation is not applicable (default).
        /// </summary>
        NoOrientation = 0,
        /// <summary>
        /// 1 - Point has orientation from first LRP towards second LRP.
        /// </summary>
        FirstToSecond = 1,
        /// <summary>
        /// 2 - Point has orientation from second LRP towards first LRP.
        /// </summary>
        SecondToFirst = 2,
        /// <summary>
        /// 3  - Point has orientation in both directions.
        /// </summary>
        BothDirections = 3
    }

    /// <summary>
    /// Extension methods to the orientation enumeration.
    /// </summary>
    public static class OrientationExtensions
    {
        /// <summary>
        /// Decodes the orientation into a oneway boolean.
        /// </summary>
        public static bool? DecodeOneway(this Orientation orientaton)
        {
            switch(orientaton)
            {
                case Orientation.FirstToSecond:
                    return true;
                case Orientation.SecondToFirst:
                    return false;
            }
            return null;
        }

        /// <summary>
        /// Encodes a oneway flag into an orientation.
        /// </summary>
        public static Orientation EncodeOneway(this bool? oneway)
        {
            if(oneway.HasValue)
            {
                if(oneway.Value)
                {
                    return Orientation.FirstToSecond;
                }
                return Orientation.SecondToFirst;
            }
            return Orientation.NoOrientation;
        }
    }
}