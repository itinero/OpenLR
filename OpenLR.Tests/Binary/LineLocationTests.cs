using NUnit.Framework;
using OpenLR.Binary.Decoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenLR.Tests.Binary
{
    /// <summary>
    /// Contains tests for decoding/encoding a line location to/from OpenLR binary representation.
    /// </summary>
    [TestFixture]
    public class LineLocationTests
    {
        /// <summary>
        /// A simple test decoding from a base64 string.
        /// </summary>
        [Test]
        public void DecodeBase64Test()
        {
            // define a base64 string we are sure is a line location.
            string stringData = "CwRbWyNG9RpsCQCb/jsbtAT/6/+jK1lE";

            // decode.
            var decoder = new LineLocationDecoder();
            var location = decoder.Decode(stringData);

            Assert.IsNotNull(location);
        }
    }
}
