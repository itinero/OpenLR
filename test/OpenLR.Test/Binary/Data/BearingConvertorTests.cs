using NUnit.Framework;
using OpenLR.Codecs.Binary.Data;
using System;

namespace OpenLR.Test.Binary.Data
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
    }
}