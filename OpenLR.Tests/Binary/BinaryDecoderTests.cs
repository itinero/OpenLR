using NUnit.Framework;
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
            double delta = 0.0001;

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
    }
}