using NUnit.Framework;
using OpenLR.Codecs.Binary;
using OpenLR.Codecs.Binary.Data;

namespace OpenLR.Test.Binary;

/// <summary>
/// Contains tests the binary decoder base class.
/// </summary>
[TestFixture]
public class BinaryCodecTests
{
    /// <summary>
    /// A simple test decoding from a base64 string.
    /// </summary>
    [Test]
    public void DecodeBase64Test()
    {
        // double delta = 0.0001;

        var codec = new BinaryCodec();

        // test invalid arguments.
        Assert.Catch<FormatException>(() =>
        {
            codec.Decode("InvalidCode");
        });
    }

    /// <summary>
    /// Tests encoding an angle into a bearing.
    /// </summary>
    [Test]
    public void TestBearingAngleEncoding()
    {
        Assert.That(BearingConvertor.EncodeAngleToBearing(0), Is.EqualTo(0));
        Assert.Catch<ArgumentOutOfRangeException>(() =>
        {
            BearingConvertor.EncodeAngleToBearing(-1);
        });
        Assert.Catch<ArgumentOutOfRangeException>(() =>
        {
            BearingConvertor.EncodeAngleToBearing(360);
        });
        Assert.That(BearingConvertor.EncodeAngleToBearing(90), Is.EqualTo(8));
        Assert.That(BearingConvertor.EncodeAngleToBearing(180), Is.EqualTo(16));
        Assert.That(BearingConvertor.EncodeAngleToBearing(270), Is.EqualTo(24));
    }

    /// <summary>
    /// Tests decoding an angle from a bearing.
    /// </summary>
    [Test]
    public void TestBearingAngleDecoding()
    {
        Assert.That(BearingConvertor.DecodeAngleFromBearing(0), Is.EqualTo(0));
        Assert.Catch<ArgumentOutOfRangeException>(() =>
        {
            BearingConvertor.DecodeAngleFromBearing(-1);
        });
        Assert.Catch<ArgumentOutOfRangeException>(() =>
        {
            BearingConvertor.DecodeAngleFromBearing(32);
        });
        Assert.That(BearingConvertor.DecodeAngleFromBearing(8), Is.EqualTo(90));
        Assert.That(BearingConvertor.DecodeAngleFromBearing(16), Is.EqualTo(180));
        Assert.That(BearingConvertor.DecodeAngleFromBearing(24), Is.EqualTo(270));
    }
}
