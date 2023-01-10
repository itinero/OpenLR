using System;
using NUnit.Framework;
using OpenLR.Codecs.Binary.Data;
using OpenLR.Model;

namespace OpenLR.Test.Binary.Data;

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

        Assert.That(FunctionalRoadClassConvertor.Decode(new byte[] { 0 }, 0, 0), Is.EqualTo(FunctionalRoadClass.Frc0));
        Assert.That(FunctionalRoadClassConvertor.Decode(new byte[] { 0 }, 5), Is.EqualTo(FunctionalRoadClass.Frc0));
        Assert.That(FunctionalRoadClassConvertor.Decode(new byte[] { 1 }, 5), Is.EqualTo(FunctionalRoadClass.Frc1));
        Assert.That(FunctionalRoadClassConvertor.Decode(new byte[] { 2 }, 5), Is.EqualTo(FunctionalRoadClass.Frc2));
        Assert.That(FunctionalRoadClassConvertor.Decode(new byte[] { 3 }, 5), Is.EqualTo(FunctionalRoadClass.Frc3));
        Assert.That(FunctionalRoadClassConvertor.Decode(new byte[] { 4 }, 5), Is.EqualTo(FunctionalRoadClass.Frc4));
        Assert.That(FunctionalRoadClassConvertor.Decode(new byte[] { 5 }, 5), Is.EqualTo(FunctionalRoadClass.Frc5));
        Assert.That(FunctionalRoadClassConvertor.Decode(new byte[] { 6 }, 5), Is.EqualTo(FunctionalRoadClass.Frc6));
        Assert.That(FunctionalRoadClassConvertor.Decode(new byte[] { 7 }, 5), Is.EqualTo(FunctionalRoadClass.Frc7));

        Assert.That(FunctionalRoadClassConvertor.Decode(new byte[] { 0 }, 4), Is.EqualTo(FunctionalRoadClass.Frc0));
        Assert.That(FunctionalRoadClassConvertor.Decode(new byte[] { 2 }, 4), Is.EqualTo(FunctionalRoadClass.Frc1));
        Assert.That(FunctionalRoadClassConvertor.Decode(new byte[] { 4 }, 4), Is.EqualTo(FunctionalRoadClass.Frc2));
        Assert.That(FunctionalRoadClassConvertor.Decode(new byte[] { 6 }, 4), Is.EqualTo(FunctionalRoadClass.Frc3));
        Assert.That(FunctionalRoadClassConvertor.Decode(new byte[] { 8 }, 4), Is.EqualTo(FunctionalRoadClass.Frc4));
        Assert.That(FunctionalRoadClassConvertor.Decode(new byte[] { 10 }, 4), Is.EqualTo(FunctionalRoadClass.Frc5));
        Assert.That(FunctionalRoadClassConvertor.Decode(new byte[] { 12 }, 4), Is.EqualTo(FunctionalRoadClass.Frc6));
        Assert.That(FunctionalRoadClassConvertor.Decode(new byte[] { 14 }, 4), Is.EqualTo(FunctionalRoadClass.Frc7));
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
        Assert.That(data[0], Is.EqualTo(0));
        FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc1, data, 0, 5);
        Assert.That(data[0], Is.EqualTo(1));
        FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc2, data, 0, 5);
        Assert.That(data[0], Is.EqualTo(2));
        FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc3, data, 0, 5);
        Assert.That(data[0], Is.EqualTo(3));
        FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc4, data, 0, 5);
        Assert.That(data[0], Is.EqualTo(4));
        FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc5, data, 0, 5);
        Assert.That(data[0], Is.EqualTo(5));
        FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc6, data, 0, 5);
        Assert.That(data[0], Is.EqualTo(6));
        FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc7, data, 0, 5);
        Assert.That(data[0], Is.EqualTo(7));

        data[0] = 0;
        FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc7, data, 0, 2);
        Assert.That(data[0], Is.EqualTo(56));
        data[0] = 0;
        FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc4, data, 0, 2);
        Assert.That(data[0], Is.EqualTo(32));
    }
}
