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
    /// The functional road class (FRC) of a line is a road classification based on the importance of the road.
    /// </summary>
    public enum FunctionalRoadClass
    {
        /// <summary>
        /// Main road
        /// </summary>
        Frc0 = 0,
        /// <summary>
        /// First class road
        /// </summary>
        Frc1 = 1,
        /// <summary>
        /// Second class road
        /// </summary>
        Frc2 = 2,
        /// <summary>
        /// Third class road
        /// </summary>
        Frc3 = 3,
        /// <summary>
        /// Fourth class road
        /// </summary>
        Frc4 = 4,
        /// <summary>
        /// Fifth class road
        /// </summary>
        Frc5 = 5,
        /// <summary>
        /// Sixth class road
        /// </summary>
        Frc6 = 6,
        /// <summary>
        /// Other class road
        /// </summary>
        Frc7 = 7
    }
}