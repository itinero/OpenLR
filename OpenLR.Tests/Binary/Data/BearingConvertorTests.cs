using NUnit.Framework;
using OpenLR.Binary.Data;
using OpenLR.Model;

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
            Assert.AreEqual(0, BearingConvertor.Decode(new byte[] { 0 }, 0, 0));
            Assert.AreEqual(0, BearingConvertor.Decode(new byte[] { 0 }, 0));
            Assert.AreEqual(1, BearingConvertor.Decode(new byte[] { 1 }, 3));
            Assert.AreEqual(5, BearingConvertor.Decode(new byte[] { 5 }, 3));
            Assert.AreEqual(9, BearingConvertor.Decode(new byte[] { 9 }, 3));

            Assert.AreEqual(1, BearingConvertor.Decode(new byte[] { 4 }, 1));
            Assert.AreEqual(5, BearingConvertor.Decode(new byte[] { 20 }, 1));
            Assert.AreEqual(9, BearingConvertor.Decode(new byte[] { 36 }, 1));
        }
    }
}