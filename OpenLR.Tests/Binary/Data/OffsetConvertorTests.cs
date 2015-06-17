using NUnit.Framework;
using OpenLR.Binary.Data;
using System;

namespace OpenLR.Tests.Binary.Data
{
    /// <summary>
    /// Holds some bearing encoding/decoding tests.
    /// </summary>
    [TestFixture]
    public class OffsetConvertorTests
    {
        /// <summary>
        /// Tests decoding of offset flags.
        /// </summary>
        [Test]
        public void TestDecodingFlag1()
        {
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                OffsetConvertor.DecodeFlag(new byte[] { 0 }, 0, 8);
            });

            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 1 }, 0, 0));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 1 }, 0, 1));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 1 }, 0, 2));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 1 }, 0, 3));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 1 }, 0, 4));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 1 }, 0, 5));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 1 }, 0, 6));
            Assert.True(OffsetConvertor.DecodeFlag(new byte[] { 1 }, 0, 7));

            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 2 }, 0, 0));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 2 }, 0, 1));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 2 }, 0, 2));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 2 }, 0, 3));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 2 }, 0, 4));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 2 }, 0, 5));
            Assert.IsTrue(OffsetConvertor.DecodeFlag(new byte[] { 2 }, 0, 6));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 2 }, 0, 7));

            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 4 }, 0, 0));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 4 }, 0, 1));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 4 }, 0, 2));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 4 }, 0, 3));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 4 }, 0, 4));
            Assert.IsTrue(OffsetConvertor.DecodeFlag(new byte[] { 4 }, 0, 5));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 4 }, 0, 6));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 4 }, 0, 7));

            Assert.IsTrue(OffsetConvertor.DecodeFlag(new byte[] { 128 }, 0, 0));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 128 }, 0, 1));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 128 }, 0, 2));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 128 }, 0, 3));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 128 }, 0, 4));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 128 }, 0, 5));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 128 }, 0, 6));
            Assert.IsFalse(OffsetConvertor.DecodeFlag(new byte[] { 128 }, 0, 7));
        }

        /// <summary>
        /// Tests encoding of offset flags.
        /// </summary>
        [Test]
        public void TestEncodingFlag1()
        {
            var data = new byte[1];
            data[0] = 0;
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                OffsetConvertor.EncodeFlag(false, data, 0, 8);
            });
            
            data[0] = 0;
            OffsetConvertor.EncodeFlag(true, data, 0, 0);
            Assert.AreEqual(128, data[0]);

            data[0] = 0;
            OffsetConvertor.EncodeFlag(true, data, 0, 1);
            Assert.AreEqual(64, data[0]);

            data[0] = 0;
            OffsetConvertor.EncodeFlag(true, data, 0, 2);
            Assert.AreEqual(32, data[0]);

            data[0] = 0;
            OffsetConvertor.EncodeFlag(true, data, 0, 3);
            Assert.AreEqual(16, data[0]);

            data[0] = 0;
            OffsetConvertor.EncodeFlag(true, data, 0, 4);
            Assert.AreEqual(8, data[0]);

            data[0] = 0;
            OffsetConvertor.EncodeFlag(true, data, 0, 5);
            Assert.AreEqual(4, data[0]);

            data[0] = 0;
            OffsetConvertor.EncodeFlag(true, data, 0, 6);
            Assert.AreEqual(2, data[0]);

            data[0] = 0;
            OffsetConvertor.EncodeFlag(true, data, 0, 7);
            Assert.AreEqual(1, data[0]);
        }

        /// <summary>
        /// Tests encoding of offset.
        /// </summary>
        [Test]
        public void TestEncoding1()
        {
            var data = new byte[1];

            data[0] = 0;
            OffsetConvertor.Encode(25, data, 0);
            Assert.AreEqual(64, data[0]);

            data[0] = 0;
            OffsetConvertor.Encode(50, data, 0);
            Assert.AreEqual(128, data[0]);

            data[0] = 0;
            OffsetConvertor.Encode(75, data, 0);
            Assert.AreEqual(192, data[0]);

            // regression-tests from actual data.
            // 60 [23.44% - 23.83%]
            data[0] = 0;
            OffsetConvertor.Encode(23.8f, data, 0);
            Assert.AreEqual(60, data[0]);

            data[0] = 0;
            OffsetConvertor.Encode(23.5f, data, 0);
            Assert.AreEqual(60, data[0]);

            // 185 [72.27% - 72.66%]
            data[0] = 0;
            OffsetConvertor.Encode(72.2f, data, 0);
            Assert.AreEqual(184, data[0]);

            data[0] = 0;
            OffsetConvertor.Encode(72.3f, data, 0);
            Assert.AreEqual(185, data[0]);

            data[0] = 0;
            OffsetConvertor.Encode(72.6f, data, 0);
            Assert.AreEqual(185, data[0]);

            data[0] = 0;
            OffsetConvertor.Encode(72.7f, data, 0);
            Assert.AreEqual(186, data[0]);

            data[0] = 0;
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                OffsetConvertor.Encode(-1, data, 0);
            });
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                OffsetConvertor.Encode(101, data, 0);
            });
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                OffsetConvertor.Encode(100, data, 0);
            });
        }
    }
}