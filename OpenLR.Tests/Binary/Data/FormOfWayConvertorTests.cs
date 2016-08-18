using NUnit.Framework;
using OpenLR.Codecs.Binary.Data;
using OpenLR.Model;
using System;

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
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                FormOfWayConvertor.Decode(new byte[] { 0 }, 6);
            });

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

        /// <summary>
        /// Tests simple encoding.
        /// </summary>
        [Test]
        public void TestEncoding1()
        {
            var data = new byte[1];
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                FormOfWayConvertor.Encode(FormOfWay.Undefined, data, 0, 10);
            });

            FormOfWayConvertor.Encode(FormOfWay.Undefined, data, 0, 5);
            Assert.AreEqual(0, data[0]);
            FormOfWayConvertor.Encode(FormOfWay.Motorway, data, 0, 5);
            Assert.AreEqual(1, data[0]);
            FormOfWayConvertor.Encode(FormOfWay.MultipleCarriageWay, data, 0, 5);
            Assert.AreEqual(2, data[0]);
            FormOfWayConvertor.Encode(FormOfWay.SingleCarriageWay, data, 0, 5);
            Assert.AreEqual(3, data[0]);
            FormOfWayConvertor.Encode(FormOfWay.Roundabout, data, 0, 5);
            Assert.AreEqual(4, data[0]);
            FormOfWayConvertor.Encode(FormOfWay.TrafficSquare, data, 0, 5);
            Assert.AreEqual(5, data[0]);
            FormOfWayConvertor.Encode(FormOfWay.SlipRoad, data, 0, 5);
            Assert.AreEqual(6, data[0]);
            FormOfWayConvertor.Encode(FormOfWay.Other, data, 0, 5);
            Assert.AreEqual(7, data[0]);
        }
    }
}