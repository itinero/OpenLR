using NUnit.Framework;
using OpenLR.Binary.Data;
using OpenLR.Model;
using System;

namespace OpenLR.Tests.Binary.Data
{
    /// <summary>
    /// Holds some orientation encoding/decoding tests.
    /// </summary>
    [TestFixture]
    public class OrientationConvertorTests
    {
        /// <summary>
        /// Tests simple decoding.
        /// </summary>
        [Test]
        public void TestDecoding1()
        {
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                OrientationConverter.Decode(new byte[] { 0 }, 10);
            });

            Assert.AreEqual(Orientation.NoOrientation, OrientationConverter.Decode(new byte[] { 0 }, 0, 0));
            Assert.AreEqual(Orientation.FirstToSecond, OrientationConverter.Decode(new byte[] { 1 }, 0, 6));
            Assert.AreEqual(Orientation.SecondToFirst, OrientationConverter.Decode(new byte[] { 2 }, 0, 6));
            Assert.AreEqual(Orientation.BothDirections, OrientationConverter.Decode(new byte[] { 3 }, 0, 6));
        }
    }
}