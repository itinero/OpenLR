using NUnit.Framework;
using OpenLR.Binary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void TestDecoding1()
        {
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                OffsetConvertor.Decode(new byte[] { 0 }, 0, 8);
            });

            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 1 }, 0, 0));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 1 }, 0, 1));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 1 }, 0, 2));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 1 }, 0, 3));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 1 }, 0, 4));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 1 }, 0, 5));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 1 }, 0, 6));
            Assert.True(OffsetConvertor.Decode(new byte[] { 1 }, 0, 7));

            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 2 }, 0, 0));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 2 }, 0, 1));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 2 }, 0, 2));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 2 }, 0, 3));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 2 }, 0, 4));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 2 }, 0, 5));
            Assert.IsTrue(OffsetConvertor.Decode(new byte[] { 2 }, 0, 6));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 2 }, 0, 7));

            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 4 }, 0, 0));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 4 }, 0, 1));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 4 }, 0, 2));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 4 }, 0, 3));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 4 }, 0, 4));
            Assert.IsTrue(OffsetConvertor.Decode(new byte[] { 4 }, 0, 5));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 4 }, 0, 6));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 4 }, 0, 7));

            Assert.IsTrue(OffsetConvertor.Decode(new byte[] { 128 }, 0, 0));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 128 }, 0, 1));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 128 }, 0, 2));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 128 }, 0, 3));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 128 }, 0, 4));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 128 }, 0, 5));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 128 }, 0, 6));
            Assert.IsFalse(OffsetConvertor.Decode(new byte[] { 128 }, 0, 7));
        }

        /// <summary>
        /// Tests encoding of offset flags.
        /// </summary>
        [Test]
        public void TestEncoding1()
        {
            var data = new byte[1];
            data[0] = 0;
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                OffsetConvertor.Encode(false, data, 0, 8);
            });
            
            data[0] = 0;
            OffsetConvertor.Encode(true, data, 0, 0);
            Assert.AreEqual(128, data[0]);

            data[0] = 0;
            OffsetConvertor.Encode(true, data, 0, 1);
            Assert.AreEqual(64, data[0]);

            data[0] = 0;
            OffsetConvertor.Encode(true, data, 0, 2);
            Assert.AreEqual(32, data[0]);

            data[0] = 0;
            OffsetConvertor.Encode(true, data, 0, 3);
            Assert.AreEqual(16, data[0]);

            data[0] = 0;
            OffsetConvertor.Encode(true, data, 0, 4);
            Assert.AreEqual(8, data[0]);

            data[0] = 0;
            OffsetConvertor.Encode(true, data, 0, 5);
            Assert.AreEqual(4, data[0]);

            data[0] = 0;
            OffsetConvertor.Encode(true, data, 0, 6);
            Assert.AreEqual(2, data[0]);

            data[0] = 0;
            OffsetConvertor.Encode(true, data, 0, 7);
            Assert.AreEqual(1, data[0]);
        }
    }
}