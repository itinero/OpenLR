using NUnit.Framework;
using OpenLR.Binary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenLR.Tests.Binary.Data
{
    /// <summary>
    /// Holds some coordinate encoding/decoding tests.
    /// </summary>
    [TestFixture]
    public class CoordinateEncodingTests
    {
        /// <summary>
        /// Tests the simple decoding case.
        /// </summary>
        [Test]
        public void TestDecoding1()
        {
            double delta = 0.0001;

            // manually specify a binary coordinate.
            byte[] data = new byte[6];
            data[0] = 0;
            data[1] = 0;
            data[2] = 0;
            data[3] = 0;
            data[4] = 0;
            data[5] = 0;

            // decode the coordinate.
            var coordinate = CoordinateConverter.Decode(data);

            Assert.IsNotNull(coordinate);
            Assert.AreEqual(0, coordinate.Latitude);
            Assert.AreEqual(0, coordinate.Longitude);

            // manually specify a binary coordinate.
            data[0] = 0;
            data[1] = 0;
            data[2] = 255;
            data[3] = 0;
            data[4] = 0;
            data[5] = 255;

            // decode the coordinate.
            coordinate = CoordinateConverter.Decode(data);

            Assert.IsNotNull(coordinate);
            Assert.AreEqual(0.005460978, coordinate.Latitude, delta);
            Assert.AreEqual(0.005460978, coordinate.Longitude, delta);

            // manually specify a binary coordinate (see example in OpenLR whitepaper).
            data[0] = 4;
            data[1] = 91;
            data[2] = 91;
            data[3] = 35;
            data[4] = 70;
            data[5] = 245;

            // decode the coordinate.
            coordinate = CoordinateConverter.Decode(data);

            Assert.IsNotNull(coordinate);
            Assert.AreEqual(49.60851, coordinate.Latitude, delta);
            Assert.AreEqual(6.12683, coordinate.Longitude, delta);
        }
    }
}
