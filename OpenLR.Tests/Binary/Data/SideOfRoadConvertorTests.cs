using NUnit.Framework;
using OpenLR.Binary.Data;
using OpenLR.Model;
using System;

namespace OpenLR.Tests.Binary.Data
{
    /// <summary>
    /// Holds some SideOfRoad encoding/decoding tests.
    /// </summary>
    [TestFixture]
    public class SideOfRoadConvertorTests
    {
        /// <summary>
        /// Tests simple decoding.
        /// </summary>
        [Test]
        public void TestDecoding1()
        {
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                SideOfRoadConverter.Decode(new byte[] { 0 }, 10);
            });

            Assert.AreEqual(SideOfRoad.OnOrAbove, SideOfRoadConverter.Decode(new byte[] { 0 }, 0, 0));
            Assert.AreEqual(SideOfRoad.Right, SideOfRoadConverter.Decode(new byte[] { 1 }, 0, 6));
            Assert.AreEqual(SideOfRoad.Left, SideOfRoadConverter.Decode(new byte[] { 2 }, 0, 6));
            Assert.AreEqual(SideOfRoad.Both, SideOfRoadConverter.Decode(new byte[] { 3 }, 0, 6));
        }
    }
}