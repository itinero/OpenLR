using System;
using NUnit.Framework;
using OpenLR.Codecs.Binary.Data;
using OpenLR.Model;

namespace OpenLR.Test.Binary.Data;

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

        Assert.That(OrientationConverter.Decode(new byte[] { 0 }, 0, 0), Is.EqualTo(Orientation.NoOrientation));
        Assert.That(OrientationConverter.Decode(new byte[] { 1 }, 0, 6), Is.EqualTo(Orientation.FirstToSecond));
        Assert.That(OrientationConverter.Decode(new byte[] { 2 }, 0, 6), Is.EqualTo(Orientation.SecondToFirst));
        Assert.That(OrientationConverter.Decode(new byte[] { 3 }, 0, 6), Is.EqualTo(Orientation.BothDirections));
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
            OrientationConverter.Encode(Orientation.NoOrientation, data, 0, 10);
        });

        OrientationConverter.Encode(Orientation.NoOrientation, data, 0, 6);
        Assert.That(data[0], Is.EqualTo(0));
        OrientationConverter.Encode(Orientation.FirstToSecond, data, 0, 6);
        Assert.That(data[0], Is.EqualTo(1));
        OrientationConverter.Encode(Orientation.SecondToFirst, data, 0, 6);
        Assert.That(data[0], Is.EqualTo(2));
        OrientationConverter.Encode(Orientation.BothDirections, data, 0, 6);
        Assert.That(data[0], Is.EqualTo(3));
    }
}
