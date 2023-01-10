using System;
using NUnit.Framework;
using OpenLR.Codecs.Binary.Data;
using OpenLR.Model;

namespace OpenLR.Test.Binary.Data;

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

        Assert.That(FormOfWayConvertor.Decode(new byte[] { 0 }, 0, 0), Is.EqualTo(FormOfWay.Undefined));
        Assert.That(FormOfWayConvertor.Decode(new byte[] { 0 }, 5), Is.EqualTo(FormOfWay.Undefined));
        Assert.That(FormOfWayConvertor.Decode(new byte[] { 1 }, 5), Is.EqualTo(FormOfWay.Motorway));
        Assert.That(FormOfWayConvertor.Decode(new byte[] { 2 }, 5), Is.EqualTo(FormOfWay.MultipleCarriageWay));
        Assert.That(FormOfWayConvertor.Decode(new byte[] { 3 }, 5), Is.EqualTo(FormOfWay.SingleCarriageWay));
        Assert.That(FormOfWayConvertor.Decode(new byte[] { 4 }, 5), Is.EqualTo(FormOfWay.Roundabout));
        Assert.That(FormOfWayConvertor.Decode(new byte[] { 5 }, 5), Is.EqualTo(FormOfWay.TrafficSquare));
        Assert.That(FormOfWayConvertor.Decode(new byte[] { 6 }, 5), Is.EqualTo(FormOfWay.SlipRoad));
        Assert.That(FormOfWayConvertor.Decode(new byte[] { 7 }, 5), Is.EqualTo(FormOfWay.Other));

        Assert.That(FormOfWayConvertor.Decode(new byte[] { 0 }, 4), Is.EqualTo(FormOfWay.Undefined));
        Assert.That(FormOfWayConvertor.Decode(new byte[] { 2 }, 4), Is.EqualTo(FormOfWay.Motorway));
        Assert.That(FormOfWayConvertor.Decode(new byte[] { 4 }, 4), Is.EqualTo(FormOfWay.MultipleCarriageWay));
        Assert.That(FormOfWayConvertor.Decode(new byte[] { 6 }, 4), Is.EqualTo(FormOfWay.SingleCarriageWay));
        Assert.That(FormOfWayConvertor.Decode(new byte[] { 8 }, 4), Is.EqualTo(FormOfWay.Roundabout));
        Assert.That(FormOfWayConvertor.Decode(new byte[] { 10 }, 4), Is.EqualTo(FormOfWay.TrafficSquare));
        Assert.That(FormOfWayConvertor.Decode(new byte[] { 12 }, 4), Is.EqualTo(FormOfWay.SlipRoad));
        Assert.That(FormOfWayConvertor.Decode(new byte[] { 14 }, 4), Is.EqualTo(FormOfWay.Other));
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
        Assert.That(data[0], Is.EqualTo(0));
        FormOfWayConvertor.Encode(FormOfWay.Motorway, data, 0, 5);
        Assert.That(data[0], Is.EqualTo(1));
        FormOfWayConvertor.Encode(FormOfWay.MultipleCarriageWay, data, 0, 5);
        Assert.That(data[0], Is.EqualTo(2));
        FormOfWayConvertor.Encode(FormOfWay.SingleCarriageWay, data, 0, 5);
        Assert.That(data[0], Is.EqualTo(3));
        FormOfWayConvertor.Encode(FormOfWay.Roundabout, data, 0, 5);
        Assert.That(data[0], Is.EqualTo(4));
        FormOfWayConvertor.Encode(FormOfWay.TrafficSquare, data, 0, 5);
        Assert.That(data[0], Is.EqualTo(5));
        FormOfWayConvertor.Encode(FormOfWay.SlipRoad, data, 0, 5);
        Assert.That(data[0], Is.EqualTo(6));
        FormOfWayConvertor.Encode(FormOfWay.Other, data, 0, 5);
        Assert.That(data[0], Is.EqualTo(7));
    }
}
