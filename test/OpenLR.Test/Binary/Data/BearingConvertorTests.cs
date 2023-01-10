﻿using System;
using NUnit.Framework;
using OpenLR.Codecs.Binary.Data;

namespace OpenLR.Test.Binary.Data;

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
        Assert.Catch<ArgumentOutOfRangeException>(() =>
        {
            BearingConvertor.Decode(new byte[] { 0 }, 4);
        });

        Assert.That(BearingConvertor.Decode(new byte[] { 0 }, 0, 0), Is.EqualTo(0));
        Assert.That(BearingConvertor.Decode(new byte[] { 0 }, 0), Is.EqualTo(0));
        Assert.That(BearingConvertor.Decode(new byte[] { 1 }, 3), Is.EqualTo(1));
        Assert.That(BearingConvertor.Decode(new byte[] { 5 }, 3), Is.EqualTo(5));
        Assert.That(BearingConvertor.Decode(new byte[] { 9 }, 3), Is.EqualTo(9));

        Assert.That(BearingConvertor.Decode(new byte[] { 4 }, 1), Is.EqualTo(1));
        Assert.That(BearingConvertor.Decode(new byte[] { 20 }, 1), Is.EqualTo(5));
        Assert.That(BearingConvertor.Decode(new byte[] { 36 }, 1), Is.EqualTo(9));
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
            BearingConvertor.Encode(0, data, 0, 10);
        });

        BearingConvertor.Encode(0, data, 0, 0);
        Assert.That(data[0], Is.EqualTo(0));
        BearingConvertor.Encode(1, data, 0, 3);
        Assert.That(data[0], Is.EqualTo(1));
        BearingConvertor.Encode(5, data, 0, 3);
        Assert.That(data[0], Is.EqualTo(5));
        BearingConvertor.Encode(9, data, 0, 3);
        Assert.That(data[0], Is.EqualTo(9));

        data[0] = 0;
        BearingConvertor.Encode(1, data, 0, 1);
        Assert.That(data[0], Is.EqualTo(4));
        data[0] = 0;
        BearingConvertor.Encode(5, data, 0, 1);
        Assert.That(data[0], Is.EqualTo(20));
        data[0] = 0;
        BearingConvertor.Encode(9, data, 0, 1);
        Assert.That(data[0], Is.EqualTo(36));
    }
}
