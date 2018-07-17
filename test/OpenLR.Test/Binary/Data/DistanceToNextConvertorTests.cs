using NUnit.Framework;
using OpenLR.Codecs.Binary.Data;

namespace OpenLR.Test.Binary.Data
{
    /// <summary>
    /// Holds some bearing encoding/decoding tests.
    /// </summary>
    [TestFixture]
    public class DistanceToNextConvertorTests
    {
        /// <summary>
        /// Tests simple decoding.
        /// </summary>
        [Test]
        public void TestDecoding1()
        {
            Assert.AreEqual(0, DistanceToNextConvertor.Decode((byte)0), 0.5);
            Assert.AreEqual(128 * 58.6, DistanceToNextConvertor.Decode((byte)128), 0.5);
            Assert.AreEqual(255 * 58.6, DistanceToNextConvertor.Decode((byte)255), 0.5);
        }

        /// <summary>
        /// Tests simple encoding.
        /// </summary>
        [Test]
        public void TestEncoding1()
        {
            Assert.AreEqual(0, DistanceToNextConvertor.Encode(0));
            Assert.AreEqual(127, DistanceToNextConvertor.Encode((int)(15000 / 2)));
            Assert.AreEqual(255, DistanceToNextConvertor.Encode((int)(14999)));
        }
    }
}
