using NUnit.Framework;
using OpenLR.Binary.Data;
using OpenLR.Model;
using System;

namespace OpenLR.Tests.Binary.Data
{
    /// <summary>
    /// Holds some header encoding/decoding tests.
    /// </summary>
    [TestFixture]
    public class HeaderConvertorTests
    {
        /// <summary>
        /// Tests simple decoding.
        /// </summary>
        [Test]
        public void TestDecoding1()
        {
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                HeaderConvertor.Decode(new byte[] { 0 }, 4);
            });

            // decode a test header.
            var header = HeaderConvertor.Decode(new byte[] { 11 }, 0);
            Assert.AreEqual(3, header.Version);
            Assert.AreEqual(false, header.ArF0);
            Assert.AreEqual(false, header.IsPoint);
            Assert.AreEqual(false, header.ArF1);
            Assert.AreEqual(true, header.HasAttributes);

            // decode another test header.
            header = HeaderConvertor.Decode(new byte[] { 35 }, 0);
            Assert.AreEqual(3, header.Version);
            Assert.AreEqual(false, header.ArF0);
            Assert.AreEqual(true, header.IsPoint);
            Assert.AreEqual(false, header.ArF1);
            Assert.AreEqual(false, header.HasAttributes);

            // decode another test header.
            header = HeaderConvertor.Decode(new byte[] { 43 }, 0);
            Assert.AreEqual(3, header.Version);
            Assert.AreEqual(false, header.ArF0);
            Assert.AreEqual(true, header.IsPoint);
            Assert.AreEqual(false, header.ArF1);
            Assert.AreEqual(true, header.HasAttributes);

            // decode another test header.
            header = HeaderConvertor.Decode(new byte[] { 3 }, 0);
            Assert.AreEqual(3, header.Version);
            Assert.AreEqual(false, header.ArF0);
            Assert.AreEqual(false, header.IsPoint);
            Assert.AreEqual(false, header.ArF1);
            Assert.AreEqual(false, header.HasAttributes);

            // decode another test header.
            header = HeaderConvertor.Decode(new byte[] { 67 }, 0);
            Assert.AreEqual(3, header.Version);
            Assert.AreEqual(false, header.ArF0);
            Assert.AreEqual(false, header.IsPoint);
            Assert.AreEqual(true, header.ArF1);
            Assert.AreEqual(false, header.HasAttributes);

            // decode another test header.
            header = HeaderConvertor.Decode(new byte[] { 19 }, 0);
            Assert.AreEqual(3, header.Version);
            Assert.AreEqual(true, header.ArF0);
            Assert.AreEqual(false, header.IsPoint);
            Assert.AreEqual(false, header.ArF1);
            Assert.AreEqual(false, header.HasAttributes);
        }

        /// <summary>
        /// Tests simple encoding.
        /// </summary>
        [Test]
        public void TestEncoding1()
        {
            var data = new byte[1];
            var header = new Header()
            {
                ArF0 = false,
                IsPoint = false,
                ArF1 = false,
                HasAttributes = true,
                Version = 3
            };

            // test out of range.
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                HeaderConvertor.Encode(data, 10, header);
            });

            // test encoding header.
            HeaderConvertor.Encode(data, 0, header);
            Assert.AreEqual(11, data[0]);

            // test encoding another header.
            header = new Header()
            {
                ArF0 = false,
                IsPoint = true,
                ArF1 = false,
                HasAttributes = false,
                Version = 3
            };
            HeaderConvertor.Encode(data, 0, header);
            Assert.AreEqual(35, data[0]);

            // test encoding another header.
            header = new Header()
            {
                ArF0 = false,
                IsPoint = false,
                ArF1 = false,
                HasAttributes = false,
                Version = 3
            };
            HeaderConvertor.Encode(data, 0, header);
            Assert.AreEqual(3, data[0]);

            // test encoding another header.
            header = new Header()
            {
                ArF0 = false,
                IsPoint = false,
                ArF1 = true,
                HasAttributes = false,
                Version = 3
            };
            HeaderConvertor.Encode(data, 0, header);
            Assert.AreEqual(67, data[0]);

            // test encoding another header.
            header = new Header()
            {
                ArF0 = true,
                IsPoint = false,
                ArF1 = false,
                HasAttributes = false,
                Version = 3
            };
            HeaderConvertor.Encode(data, 0, header);
            Assert.AreEqual(19, data[0]);
        }
    }
}