using NUnit.Framework;
using OpenLR.Codecs.Binary.Data;
using OpenLR.Model;

namespace OpenLR.Test.Binary.Data
{
    /// <summary>
    /// Holds some coordinate encoding/decoding tests.
    /// </summary>
    [TestFixture]
    public class CoordinateConvertorTests
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

            // decode the coordinate (ensure full code coverage).
            coordinate = CoordinateConverter.Decode(data, 0);

            Assert.IsNotNull(coordinate);
            Assert.AreEqual(49.60851, coordinate.Latitude, delta);
            Assert.AreEqual(6.12683, coordinate.Longitude, delta);
        }

        /// <summary>
        /// Tests the simple simple relative decoding case.
        /// </summary>
        [Test]
        public void TestDecodingRelative1()
        {
            double delta = 0.0001;

            // manually specify a binary coordinate (see example in OpenLR whitepaper).
            byte[] data = new byte[4];
            data[0] = 0;
            data[1] = 155;
            data[2] = 254;
            data[3] = 59;

            // decode the coordinate relative to another coordinate.
            var reference = new Coordinate()
            {
                Latitude = 49.60851,
                Longitude = 6.12683
            };
            var coordinate = CoordinateConverter.DecodeRelative(reference, data);

            Assert.IsNotNull(coordinate);
            Assert.AreEqual(6.12838, coordinate.Longitude, delta);
            Assert.AreEqual(49.60398, coordinate.Latitude, delta);

            // (ensure full code coverage).
            coordinate = CoordinateConverter.DecodeRelative(reference, data, 0);

            Assert.IsNotNull(coordinate);
            Assert.AreEqual(6.12838, coordinate.Longitude, delta);
            Assert.AreEqual(49.60398, coordinate.Latitude, delta);
        }

        /// <summary>
        /// Tests the simple encoding case.
        /// </summary>
        [Test]
        public void TestEncoding1()
        {
            // specify coordinate.
            var coordinate = new Coordinate()
            {
                Latitude = 0,
                Longitude = 0
            };

            // encode.
            var data = new byte[6];
            CoordinateConverter.Encode(coordinate, data, 0);
            Assert.AreEqual(data[0], 0);
            Assert.AreEqual(data[1], 0);
            Assert.AreEqual(data[2], 0);
            Assert.AreEqual(data[3], 0);
            Assert.AreEqual(data[4], 0);
            Assert.AreEqual(data[5], 0);

            // specify coordinate.
            coordinate = new Coordinate()
            {
                Latitude = 0.005460978,
                Longitude = 0.005460978
            };

            // encode.
            CoordinateConverter.Encode(coordinate, data, 0);
            Assert.AreEqual(data[0], 0);
            Assert.AreEqual(data[1], 0);
            Assert.AreEqual(data[2], 255);
            Assert.AreEqual(data[3], 0);
            Assert.AreEqual(data[4], 0);
            Assert.AreEqual(data[5], 255);

            // specify coordinate.
            coordinate = new Coordinate()
            {
                Latitude = 49.60851,
                Longitude = 6.12683
            };

            // encode.
            CoordinateConverter.Encode(coordinate, data, 0);
            Assert.AreEqual(data[0], 4);
            Assert.AreEqual(data[1], 91);
            Assert.AreEqual(data[2], 91);
            Assert.AreEqual(data[3], 35);
            Assert.AreEqual(data[4], 70);
            Assert.AreEqual(data[5], 244);
        }

        /// <summary>
        /// Tests the simple simple relative decoding case.
        /// </summary>
        [Test]
        public void TestEncodeRelative1()
        {
            // decode the coordinate relative to another coordinate.
            var reference = new Coordinate()
            {
                Latitude = 49.60851,
                Longitude = 6.12683
            };
            var coordinate = new Coordinate()
            {
                Latitude = 49.60398,
                Longitude = 6.12838
            };

            // encode.
            byte[] data = new byte[4];
            CoordinateConverter.EncodeRelative(reference, coordinate, data, 0);
            Assert.AreEqual(data[0], 0);
            Assert.AreEqual(data[1], 154);
            Assert.AreEqual(data[2], 254);
            Assert.AreEqual(data[3], 59);
        }

        /// <summary>
        /// Tests encoding decoding negative lat/lons, regression test for issue:
        /// https://github.com/itinero/OpenLR/issues/76
        /// </summary>
        [Test]
        public void RegressionTestEncodeDecodeNegative()
        {
            var delta = 0.0001;

            // specify coordinate.
            var coordinate = new Coordinate()
            {
                Latitude = -52.932136535644531,
                Longitude = -1.5213972330093384
            };

            // encode.
            var data = new byte[1024];
            CoordinateConverter.Encode(coordinate, data, 0);
            var decoded = CoordinateConverter.Decode(data);

            Assert.AreEqual(coordinate.Latitude, decoded.Latitude, delta);
            Assert.AreEqual(coordinate.Longitude, decoded.Longitude, delta);
        }

        /// <summary>
        /// Encodes and decodes negative integers, regression test for issue:
        /// https://github.com/itinero/OpenLR/issues/76
        /// </summary>
        [Test]
        public void RegressionEncodeDecodeNegativeInt()
        {
            var i = -10284;
            var data = new byte[1024];
            CoordinateConverter.EncodeInt24(i, data, 0);

            Assert.AreEqual(i, CoordinateConverter.DecodeInt24(data, 0));

            i = -10284;
            CoordinateConverter.EncodeInt24(i, data, 0);
            Assert.AreEqual(i, CoordinateConverter.DecodeInt24(data, 0));

            i = -184;
            CoordinateConverter.EncodeInt24(i, data, 0);
            Assert.AreEqual(i, CoordinateConverter.DecodeInt24(data, 0));

            i = -78124;
            CoordinateConverter.EncodeInt24(i, data, 0);
            Assert.AreEqual(i, CoordinateConverter.DecodeInt24(data, 0));

            i = 10284;
            CoordinateConverter.EncodeInt24(i, data, 0);

            Assert.AreEqual(i, CoordinateConverter.DecodeInt24(data, 0));

            i = 10284;
            CoordinateConverter.EncodeInt24(i, data, 0);
            Assert.AreEqual(i, CoordinateConverter.DecodeInt24(data, 0));

            i = 184;
            CoordinateConverter.EncodeInt24(i, data, 0);
            Assert.AreEqual(i, CoordinateConverter.DecodeInt24(data, 0));

            i = 78124;
            CoordinateConverter.EncodeInt24(i, data, 0);
            Assert.AreEqual(i, CoordinateConverter.DecodeInt24(data, 0));

            // try the 16-bit code.
            i = -10000;
            CoordinateConverter.EncodeInt16(i, data, 0);
            Assert.AreEqual(i, CoordinateConverter.DecodeInt16(data, 0));

            i = -1024;
            CoordinateConverter.EncodeInt16(i, data, 0);
            Assert.AreEqual(i, CoordinateConverter.DecodeInt16(data, 0));

            i = -184;
            CoordinateConverter.EncodeInt16(i, data, 0);
            Assert.AreEqual(i, CoordinateConverter.DecodeInt16(data, 0));

            i = -781;
            CoordinateConverter.EncodeInt16(i, data, 0);
            Assert.AreEqual(i, CoordinateConverter.DecodeInt16(data, 0));

            i = 14;
            CoordinateConverter.EncodeInt16(i, data, 0);

            Assert.AreEqual(i, CoordinateConverter.DecodeInt16(data, 0));

            i = 104;
            CoordinateConverter.EncodeInt16(i, data, 0);
            Assert.AreEqual(i, CoordinateConverter.DecodeInt16(data, 0));

            i = 184;
            CoordinateConverter.EncodeInt16(i, data, 0);
            Assert.AreEqual(i, CoordinateConverter.DecodeInt16(data, 0));

            i = 724;
            CoordinateConverter.EncodeInt16(i, data, 0);
            Assert.AreEqual(i, CoordinateConverter.DecodeInt16(data, 0));
        }
    }
}