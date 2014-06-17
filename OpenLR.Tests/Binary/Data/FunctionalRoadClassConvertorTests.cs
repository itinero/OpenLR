using NUnit.Framework;
using OpenLR.Binary.Data;
using OpenLR.Model;
using System;

namespace OpenLR.Tests.Binary.Data
{
    /// <summary>
    /// Holds some functional road class encoding/decoding tests.
    /// </summary>
    [TestFixture]
    public class FunctionalRoadClassConvertorTests
    {
        /// <summary>
        /// Tests simple decoding.
        /// </summary>
        [Test]
        public void TestDecoding1()
        {
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                FunctionalRoadClassConvertor.Decode(new byte[] { 0 }, 7);
            });

            Assert.AreEqual(FunctionalRoadClass.Frc0, FunctionalRoadClassConvertor.Decode(new byte[] { 0 }, 0, 0));
            Assert.AreEqual(FunctionalRoadClass.Frc0, FunctionalRoadClassConvertor.Decode(new byte[] { 0 }, 5));
            Assert.AreEqual(FunctionalRoadClass.Frc1, FunctionalRoadClassConvertor.Decode(new byte[] { 1 }, 5));
            Assert.AreEqual(FunctionalRoadClass.Frc2, FunctionalRoadClassConvertor.Decode(new byte[] { 2 }, 5));
            Assert.AreEqual(FunctionalRoadClass.Frc3, FunctionalRoadClassConvertor.Decode(new byte[] { 3 }, 5));
            Assert.AreEqual(FunctionalRoadClass.Frc4, FunctionalRoadClassConvertor.Decode(new byte[] { 4 }, 5));
            Assert.AreEqual(FunctionalRoadClass.Frc5, FunctionalRoadClassConvertor.Decode(new byte[] { 5 }, 5));
            Assert.AreEqual(FunctionalRoadClass.Frc6, FunctionalRoadClassConvertor.Decode(new byte[] { 6 }, 5));
            Assert.AreEqual(FunctionalRoadClass.Frc7, FunctionalRoadClassConvertor.Decode(new byte[] { 7 }, 5));

            Assert.AreEqual(FunctionalRoadClass.Frc0, FunctionalRoadClassConvertor.Decode(new byte[] { 0 }, 4));
            Assert.AreEqual(FunctionalRoadClass.Frc1, FunctionalRoadClassConvertor.Decode(new byte[] { 2 }, 4));
            Assert.AreEqual(FunctionalRoadClass.Frc2, FunctionalRoadClassConvertor.Decode(new byte[] { 4 }, 4));
            Assert.AreEqual(FunctionalRoadClass.Frc3, FunctionalRoadClassConvertor.Decode(new byte[] { 6 }, 4));
            Assert.AreEqual(FunctionalRoadClass.Frc4, FunctionalRoadClassConvertor.Decode(new byte[] { 8 }, 4));
            Assert.AreEqual(FunctionalRoadClass.Frc5, FunctionalRoadClassConvertor.Decode(new byte[] { 10 }, 4));
            Assert.AreEqual(FunctionalRoadClass.Frc6, FunctionalRoadClassConvertor.Decode(new byte[] { 12 }, 4));
            Assert.AreEqual(FunctionalRoadClass.Frc7, FunctionalRoadClassConvertor.Decode(new byte[] { 14 }, 4));
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
                FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc0, data, 0, 10);
            });

            FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc0, data, 0, 5);
            Assert.AreEqual(0, data[0]);
            FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc1, data, 0, 5);
            Assert.AreEqual(1, data[0]);
            FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc2, data, 0, 5);
            Assert.AreEqual(2, data[0]);
            FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc3, data, 0, 5);
            Assert.AreEqual(3, data[0]);
            FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc4, data, 0, 5);
            Assert.AreEqual(4, data[0]);
            FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc5, data, 0, 5);
            Assert.AreEqual(5, data[0]);
            FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc6, data, 0, 5);
            Assert.AreEqual(6, data[0]);
            FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc7, data, 0, 5);
            Assert.AreEqual(7, data[0]);
        }
    }
}