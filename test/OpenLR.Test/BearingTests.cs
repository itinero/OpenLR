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

using Itinero.LocalGeo;
using NUnit.Framework;
using OpenLR.Referenced.Codecs;
using System.Collections.Generic;

namespace OpenLR.Tests
{
    /// <summary>
    /// Holds tests for converting bearings to/from angles.
    /// </summary>
    [TestFixture]
    public class BearingTests
    {

        /// <summary>
        /// Tests bearing decoding.
        /// </summary>
        [Test]
        public void TestBearingEncoding()
        {
            // (topLeft) ---- 12.5m ---- (topMiddle) ---- 12.5m ---- (topRight)
            //  |                             |                             |
            //  |                             |                             |
            //  |                             |                             |
            // 10m                           10m                           10m
            //  |                             |                             |
            //  |                             |                             |
            //  |                             |                             |
            // (left)    ---- 12.5m ----   (middle)  ---- 12.5m ----    (right)

            var left = new Coordinate(50.84431698343915f, 3.144611937981996f);
            var right = new Coordinate(50.84431699551239f, 3.1449685962923635f);
            var topLeft = new Coordinate(50.84440717501314f, 3.1446117265095705f);
            var topRight = new Coordinate(50.84440717501314f, 3.1449675435791398f);
            var middle = new Coordinate(50.8443169894425f, 3.144789284159275f);
            var topMiddle = new Coordinate(50.84440717501314f, 3.1447892841399474f);

            // encode left-right.
            var coordinates = new List<Coordinate>();
            coordinates.Add(left);
            coordinates.Add(right);
            Assert.AreEqual(90, BearingEncoder.EncodeBearing(coordinates), 1);

            // encode right-left.
            coordinates = new List<Coordinate>();
            coordinates.Add(right);
            coordinates.Add(left);
            Assert.AreEqual(270, BearingEncoder.EncodeBearing(coordinates), 1);

            // encode left-topLeft-topMiddle.
            coordinates = new List<Coordinate>();
            coordinates.Add(left);
            coordinates.Add(topLeft);
            coordinates.Add(topMiddle);
            Assert.AreEqual(58.98, BearingEncoder.EncodeBearing(coordinates), 1);

            // encode middle-topMiddle-topLeft.
            coordinates = new List<Coordinate>();
            coordinates.Add(middle);
            coordinates.Add(topMiddle);
            coordinates.Add(topLeft);
            Assert.AreEqual(301.01, BearingEncoder.EncodeBearing(coordinates), 1);
        }
    }
}