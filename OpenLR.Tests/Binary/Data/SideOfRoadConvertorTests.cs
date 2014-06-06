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

        /// <summary>
        /// Tests simple encoding.
        /// </summary>
        [Test]
        public void TestEncoding1()
        {
            var data = new byte[1];
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                SideOfRoadConverter.Encode(SideOfRoad.OnOrAbove, data, 0, 10);
            });

            SideOfRoadConverter.Encode(SideOfRoad.OnOrAbove, data, 0, 6);
            Assert.AreEqual(0, data[0]);
            SideOfRoadConverter.Encode(SideOfRoad.Right, data, 0, 6);
            Assert.AreEqual(1, data[0]);
            SideOfRoadConverter.Encode(SideOfRoad.Left, data, 0, 6);
            Assert.AreEqual(2, data[0]);
            SideOfRoadConverter.Encode(SideOfRoad.Both, data, 0, 6);
            Assert.AreEqual(3, data[0]);
        }
    }
}