using NUnit.Framework;
using OpenLR.Codecs.Binary.Data;

namespace OpenLR.Test.Binary.Data;

/// <summary>
/// Holds some bearing encoding/decoding tests.
/// </summary>
[TestFixture]
public class DistanceToNextConvertorTests
{
    /// <summary>
    /// Tests simple decoding.
    /// </summary>
    [Test]
    public void TestDecoding1()
    {
        Assert.That(DistanceToNextConvertor.Decode((byte)0), Is.EqualTo(0).Within(0.5));
        Assert.That(DistanceToNextConvertor.Decode((byte)128), Is.EqualTo(128 * 58.6).Within(0.5));
        Assert.That(DistanceToNextConvertor.Decode((byte)255), Is.EqualTo(255 * 58.6).Within(0.5));
    }

    /// <summary>
    /// Tests simple encoding.
    /// </summary>
    [Test]
    public void TestEncoding1()
    {
        Assert.That(DistanceToNextConvertor.Encode(0), Is.EqualTo(0));
        Assert.That(DistanceToNextConvertor.Encode((int)(15000 / 2)), Is.EqualTo(127));
        Assert.That(DistanceToNextConvertor.Encode((int)(14999)), Is.EqualTo(255));
    }
}
