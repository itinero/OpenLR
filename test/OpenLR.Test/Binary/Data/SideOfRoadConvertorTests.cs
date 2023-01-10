using System;
using NUnit.Framework;
using OpenLR.Codecs.Binary.Data;
using OpenLR.Model;

namespace OpenLR.Test.Binary.Data;

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

        Assert.That(SideOfRoadConverter.Decode(new byte[] { 0 }, 0, 0), Is.EqualTo(SideOfRoad.OnOrAbove));
        Assert.That(SideOfRoadConverter.Decode(new byte[] { 1 }, 0, 6), Is.EqualTo(SideOfRoad.Right));
        Assert.That(SideOfRoadConverter.Decode(new byte[] { 2 }, 0, 6), Is.EqualTo(SideOfRoad.Left));
        Assert.That(SideOfRoadConverter.Decode(new byte[] { 3 }, 0, 6), Is.EqualTo(SideOfRoad.Both));
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
        Assert.That(data[0], Is.EqualTo(0));
        SideOfRoadConverter.Encode(SideOfRoad.Right, data, 0, 6);
        Assert.That(data[0], Is.EqualTo(1));
        SideOfRoadConverter.Encode(SideOfRoad.Left, data, 0, 6);
        Assert.That(data[0], Is.EqualTo(2));
        SideOfRoadConverter.Encode(SideOfRoad.Both, data, 0, 6);
        Assert.That(data[0], Is.EqualTo(3));
    }
}
