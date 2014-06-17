using NUnit.Framework;
using OpenLR.Binary.Decoders;
using OpenLR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLR.Tests.Binary
{
    /// <summary>
    /// Contains tests for decoding/encoding a rectangle location to/from OpenLR binary representation.
    /// </summary>
    [TestFixture]
    public class RectangleLocationTests
    {
        /// <summary>
        /// A simple test decoding from a base64 string.
        /// </summary>
        [Test]
        public void DecodeBase64Test()
        {
            double delta = 0.0001;

            // define a base64 string.
            string stringData = "QwRbICNGeQFAAH0=";

            // decode.
            var decoder = new RectangleLocationDecoder();
            Assert.IsTrue(decoder.CanDecode(stringData));
            var location = decoder.Decode(stringData);

            Assert.IsNotNull(location);
            Assert.IsInstanceOf<RectangleLocation>(location);
            var rectangleLocation = (location as RectangleLocation);

            // check coordinate.
            Assert.IsNotNull(rectangleLocation);
            Assert.IsNotNull(rectangleLocation.LowerLeft);
            Assert.AreEqual(6.12555, rectangleLocation.LowerLeft.Longitude, delta); // 6.12555°
            Assert.AreEqual(49.60586, rectangleLocation.LowerLeft.Latitude, delta); // 49.60586°
            Assert.IsNotNull(rectangleLocation.UpperRight);
            Assert.AreEqual(6.12875, rectangleLocation.UpperRight.Longitude, delta); // 6.12875°
            Assert.AreEqual(49.60711, rectangleLocation.UpperRight.Latitude, delta); // 49.60711°
        }
    }
}
