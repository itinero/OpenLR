using NUnit.Framework;
using OpenLR.Binary.Data;
using OpenLR.Binary.Decoders;
using OpenLR.Locations;
using System;

namespace OpenLR.Tests.Binary
{
    /// <summary>
    /// Contains tests the binary decoder base class.
    /// </summary>
    [TestFixture]
    public class BinaryDecoderTests
    {
        /// <summary>
        /// A simple test decoding from a base64 string.
        /// </summary>
        [Test]
        public void DecodeBase64Test()
        {
            // double delta = 0.0001;

            var decoder = new CircleLocationDecoder();

            // test null arguments.
            Assert.Catch<ArgumentNullException>(() => {
                decoder.Decode(null);
            });

            // test invalid arguments.
            Assert.Catch<FormatException>(() =>
            {
                decoder.Decode("InvalidCode");
            });
        }

        /// <summary>
        /// Tests encoding an angle into a bearing.
        /// </summary>
        [Test]
        public void TestBearingAngleEncoding()
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
        public void TestBearingAngleDecoding()
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