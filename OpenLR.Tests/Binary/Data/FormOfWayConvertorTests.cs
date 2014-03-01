using NUnit.Framework;
using OpenLR.Binary.Data;
using OpenLR.Model;

namespace OpenLR.Tests.Binary.Data
{
    /// <summary>
    /// Holds some form of way encoding/decoding tests.
    /// </summary>
    [TestFixture]
    public class FormOfWayConvertorTests
    {
        /// <summary>
        /// Tests simple decoding.
        /// </summary>
        [Test]
        public void TestDecoding1()
        {
            Assert.AreEqual(FormOfWay.Undefined, FormOfWayConvertor.Decode(new byte[] { 0 }, 0, 0));
            Assert.AreEqual(FormOfWay.Undefined, FormOfWayConvertor.Decode(new byte[] { 0 }, 5));
            Assert.AreEqual(FormOfWay.Motorway, FormOfWayConvertor.Decode(new byte[] { 1 }, 5));
            Assert.AreEqual(FormOfWay.MultipleCarriageWay, FormOfWayConvertor.Decode(new byte[] { 2 }, 5));
            Assert.AreEqual(FormOfWay.SingleCarriageWay, FormOfWayConvertor.Decode(new byte[] { 3 }, 5));
            Assert.AreEqual(FormOfWay.Roundabout, FormOfWayConvertor.Decode(new byte[] { 4 }, 5));
            Assert.AreEqual(FormOfWay.TrafficSquare, FormOfWayConvertor.Decode(new byte[] { 5 }, 5));
            Assert.AreEqual(FormOfWay.SlipRoad, FormOfWayConvertor.Decode(new byte[] { 6 }, 5));
            Assert.AreEqual(FormOfWay.Other, FormOfWayConvertor.Decode(new byte[] { 7 }, 5));

            Assert.AreEqual(FormOfWay.Undefined, FormOfWayConvertor.Decode(new byte[] { 0 }, 4));
            Assert.AreEqual(FormOfWay.Motorway, FormOfWayConvertor.Decode(new byte[] { 2 }, 4));
            Assert.AreEqual(FormOfWay.MultipleCarriageWay, FormOfWayConvertor.Decode(new byte[] { 4 }, 4));
            Assert.AreEqual(FormOfWay.SingleCarriageWay, FormOfWayConvertor.Decode(new byte[] { 6 }, 4));
            Assert.AreEqual(FormOfWay.Roundabout, FormOfWayConvertor.Decode(new byte[] { 8 }, 4));
            Assert.AreEqual(FormOfWay.TrafficSquare, FormOfWayConvertor.Decode(new byte[] { 10 }, 4));
            Assert.AreEqual(FormOfWay.SlipRoad, FormOfWayConvertor.Decode(new byte[] { 12 }, 4));
            Assert.AreEqual(FormOfWay.Other, FormOfWayConvertor.Decode(new byte[] { 14 }, 4));
        }
    }
}