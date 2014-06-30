using NUnit.Framework;
using OpenLR.Binary.Data;
using OpenLR.Model;
using System;

namespace OpenLR.Tests.Binary.Data
{
    /// <summary>
    /// Holds some bearing encoding/decoding tests.
    /// </summary>
    [TestFixture]
    public class BearingConvertorTests
    {
        /// <summary>
        /// Tests simple decoding.
        /// </summary>
        [Test]
        public void TestDecoding1()
        {
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                BearingConvertor.Decode(new byte[] { 0 }, 4);
            });

            Assert.AreEqual(0, BearingConvertor.Decode(new byte[] { 0 }, 0, 0));
            Assert.AreEqual(0, BearingConvertor.Decode(new byte[] { 0 }, 0));
            Assert.AreEqual(1, BearingConvertor.Decode(new byte[] { 1 }, 3));
            Assert.AreEqual(5, BearingConvertor.Decode(new byte[] { 5 }, 3));
            Assert.AreEqual(9, BearingConvertor.Decode(new byte[] { 9 }, 3));

            Assert.AreEqual(1, BearingConvertor.Decode(new byte[] { 4 }, 1));
            Assert.AreEqual(5, BearingConvertor.Decode(new byte[] { 20 }, 1));
            Assert.AreEqual(9, BearingConvertor.Decode(new byte[] { 36 }, 1));
        }

        /// <summary>
        /// Tests simple encoding.
        /// </summary>
        [Test]
        public void TestEncoding1()
        {
            var data = new byte[1];
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                BearingConvertor.Encode(0, data, 0, 10);
            });

            BearingConvertor.Encode(0, data, 0, 0);
            Assert.AreEqual(0, data[0]);
            BearingConvertor.Encode(1, data, 0, 3);
            Assert.AreEqual(1, data[0]);
            BearingConvertor.Encode(5, data, 0, 3);
            Assert.AreEqual(5, data[0]);
            BearingConvertor.Encode(9, data, 0, 3);
            Assert.AreEqual(9, data[0]);

            data[0] = 0;
            BearingConvertor.Encode(1, data, 0, 1);
            Assert.AreEqual(4, data[0]);
            data[0] = 0;
            BearingConvertor.Encode(5, data, 0, 1);
            Assert.AreEqual(20, data[0]);
            data[0] = 0;
            BearingConvertor.Encode(9, data, 0, 1);
            Assert.AreEqual(36, data[0]);
        }

        /// <summary>
        /// Tests encoding an angle into a bearing.
        /// </summary>
        [Test]
        public void TestBearingEncoding()
        {
            Assert.AreEqual(0, BearingConvertor.EncodeAngleToBearing(0));
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                BearingConvertor.EncodeAngleToBearing(-1);
            });
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                BearingConvertor.EncodeAngleToBearing(360);
            });
            Assert.AreEqual(8, BearingConvertor.EncodeAngleToBearing(90));
            Assert.AreEqual(16, BearingConvertor.EncodeAngleToBearing(180));
            Assert.AreEqual(24, BearingConvertor.EncodeAngleToBearing(270));
        }

        /// <summary>
        /// Tests decoding an angle from a bearing.
        /// </summary>
        [Test]
        public void TestBearingDecoding()
        {
            Assert.AreEqual(0, BearingConvertor.DecodeAngleFromBearing(0));
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                BearingConvertor.DecodeAngleFromBearing(-1);
            });
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                BearingConvertor.DecodeAngleFromBearing(32);
            });
            Assert.AreEqual(90, BearingConvertor.DecodeAngleFromBearing(8));
            Assert.AreEqual(180, BearingConvertor.DecodeAngleFromBearing(16));
            Assert.AreEqual(270, BearingConvertor.DecodeAngleFromBearing(24));
        }
    }
}