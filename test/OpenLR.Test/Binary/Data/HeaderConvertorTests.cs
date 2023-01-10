using System;
using NUnit.Framework;
using OpenLR.Codecs.Binary.Data;

namespace OpenLR.Test.Binary.Data;

/// <summary>
/// Holds some header encoding/decoding tests.
/// </summary>
[TestFixture]
public class HeaderConvertorTests
{
    /// <summary>
    /// Tests simple decoding.
    /// </summary>
    [Test]
    public void TestDecoding1()
    {
        Assert.Catch<ArgumentOutOfRangeException>(() =>
        {
            HeaderConvertor.Decode(new byte[] { 0 }, 4);
        });

        // decode a test header.
        var header = HeaderConvertor.Decode(new byte[] { 11 }, 0);
        Assert.That(header.Version, Is.EqualTo(3));
        Assert.That(header.ArF0, Is.EqualTo(false));
        Assert.That(header.IsPoint, Is.EqualTo(false));
        Assert.That(header.ArF1, Is.EqualTo(false));
        Assert.That(header.HasAttributes, Is.EqualTo(true));

        // decode another test header.
        header = HeaderConvertor.Decode(new byte[] { 35 }, 0);
        Assert.That(header.Version, Is.EqualTo(3));
        Assert.That(header.ArF0, Is.EqualTo(false));
        Assert.That(header.IsPoint, Is.EqualTo(true));
        Assert.That(header.ArF1, Is.EqualTo(false));
        Assert.That(header.HasAttributes, Is.EqualTo(false));

        // decode another test header.
        header = HeaderConvertor.Decode(new byte[] { 43 }, 0);
        Assert.That(header.Version, Is.EqualTo(3));
        Assert.That(header.ArF0, Is.EqualTo(false));
        Assert.That(header.IsPoint, Is.EqualTo(true));
        Assert.That(header.ArF1, Is.EqualTo(false));
        Assert.That(header.HasAttributes, Is.EqualTo(true));

        // decode another test header.
        header = HeaderConvertor.Decode(new byte[] { 3 }, 0);
        Assert.That(header.Version, Is.EqualTo(3));
        Assert.That(header.ArF0, Is.EqualTo(false));
        Assert.That(header.IsPoint, Is.EqualTo(false));
        Assert.That(header.ArF1, Is.EqualTo(false));
        Assert.That(header.HasAttributes, Is.EqualTo(false));

        // decode another test header.
        header = HeaderConvertor.Decode(new byte[] { 67 }, 0);
        Assert.That(header.Version, Is.EqualTo(3));
        Assert.That(header.ArF0, Is.EqualTo(false));
        Assert.That(header.IsPoint, Is.EqualTo(false));
        Assert.That(header.ArF1, Is.EqualTo(true));
        Assert.That(header.HasAttributes, Is.EqualTo(false));

        // decode another test header.
        header = HeaderConvertor.Decode(new byte[] { 19 }, 0);
        Assert.That(header.Version, Is.EqualTo(3));
        Assert.That(header.ArF0, Is.EqualTo(true));
        Assert.That(header.IsPoint, Is.EqualTo(false));
        Assert.That(header.ArF1, Is.EqualTo(false));
        Assert.That(header.HasAttributes, Is.EqualTo(false));
    }

    /// <summary>
    /// Tests simple encoding.
    /// </summary>
    [Test]
    public void TestEncoding1()
    {
        var data = new byte[1];
        var header = new Header()
        {
            ArF0 = false,
            IsPoint = false,
            ArF1 = false,
            HasAttributes = true,
            Version = 3
        };

        // test out of range.
        Assert.Catch<ArgumentOutOfRangeException>(() =>
        {
            HeaderConvertor.Encode(data, 10, header);
        });

        // test encoding header.
        HeaderConvertor.Encode(data, 0, header);
        Assert.That(data[0], Is.EqualTo(11));

        // test encoding another header.
        header = new Header()
        {
            ArF0 = false,
            IsPoint = true,
            ArF1 = false,
            HasAttributes = false,
            Version = 3
        };
        HeaderConvertor.Encode(data, 0, header);
        Assert.That(data[0], Is.EqualTo(35));

        // test encoding another header.
        header = new Header()
        {
            ArF0 = false,
            IsPoint = false,
            ArF1 = false,
            HasAttributes = false,
            Version = 3
        };
        HeaderConvertor.Encode(data, 0, header);
        Assert.That(data[0], Is.EqualTo(3));

        // test encoding another header.
        header = new Header()
        {
            ArF0 = false,
            IsPoint = false,
            ArF1 = true,
            HasAttributes = false,
            Version = 3
        };
        HeaderConvertor.Encode(data, 0, header);
        Assert.That(data[0], Is.EqualTo(67));

        // test encoding another header.
        header = new Header()
        {
            ArF0 = true,
            IsPoint = false,
            ArF1 = false,
            HasAttributes = false,
            Version = 3
        };
        HeaderConvertor.Encode(data, 0, header);
        Assert.That(data[0], Is.EqualTo(19));
    }
}
