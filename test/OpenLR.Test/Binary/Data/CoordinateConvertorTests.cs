using NUnit.Framework;
using OpenLR.Codecs.Binary.Data;
using OpenLR.Model;

namespace OpenLR.Test.Binary.Data;

/// <summary>
/// Holds some coordinate encoding/decoding tests.
/// </summary>
[TestFixture]
public class CoordinateConvertorTests
{
    /// <summary>
    /// Tests the simple decoding case.
    /// </summary>
    [Test]
    public void TestDecoding1()
    {
        double delta = 0.0001;

        // manually specify a binary coordinate.
        byte[] data = new byte[6];
        data[0] = 0;
        data[1] = 0;
        data[2] = 0;
        data[3] = 0;
        data[4] = 0;
        data[5] = 0;

        // decode the coordinate.
        var coordinate = CoordinateConverter.Decode(data);

        Assert.IsNotNull(coordinate);
        Assert.That(coordinate.Latitude, Is.EqualTo(0));
        Assert.That(coordinate.Longitude, Is.EqualTo(0));

        // manually specify a binary coordinate.
        data[0] = 0;
        data[1] = 0;
        data[2] = 255;
        data[3] = 0;
        data[4] = 0;
        data[5] = 255;

        // decode the coordinate.
        coordinate = CoordinateConverter.Decode(data);

        Assert.IsNotNull(coordinate);
        Assert.That(coordinate.Latitude, Is.EqualTo(0.005460978).Within(delta));
        Assert.That(coordinate.Longitude, Is.EqualTo(0.005460978).Within(delta));

        // manually specify a binary coordinate (see example in OpenLR whitepaper).
        data[0] = 4;
        data[1] = 91;
        data[2] = 91;
        data[3] = 35;
        data[4] = 70;
        data[5] = 245;

        // decode the coordinate.
        coordinate = CoordinateConverter.Decode(data);

        Assert.IsNotNull(coordinate);
        Assert.That(coordinate.Latitude, Is.EqualTo(49.60851).Within(delta));
        Assert.That(coordinate.Longitude, Is.EqualTo(6.12683).Within(delta));

        // decode the coordinate (ensure full code coverage).
        coordinate = CoordinateConverter.Decode(data, 0);

        Assert.IsNotNull(coordinate);
        Assert.That(coordinate.Latitude, Is.EqualTo(49.60851).Within(delta));
        Assert.That(coordinate.Longitude, Is.EqualTo(6.12683).Within(delta));
    }

    /// <summary>
    /// Tests the simple simple relative decoding case.
    /// </summary>
    [Test]
    public void TestDecodingRelative1()
    {
        double delta = 0.0001;

        // manually specify a binary coordinate (see example in OpenLR whitepaper).
        byte[] data = new byte[4];
        data[0] = 0;
        data[1] = 155;
        data[2] = 254;
        data[3] = 59;

        // decode the coordinate relative to another coordinate.
        var reference = new Coordinate()
        {
            Latitude = 49.60851,
            Longitude = 6.12683
        };
        var coordinate = CoordinateConverter.DecodeRelative(reference, data);

        Assert.IsNotNull(coordinate);
        Assert.That(coordinate.Longitude, Is.EqualTo(6.12838).Within(delta));
        Assert.That(coordinate.Latitude, Is.EqualTo(49.60398).Within(delta));

        // (ensure full code coverage).
        coordinate = CoordinateConverter.DecodeRelative(reference, data, 0);

        Assert.IsNotNull(coordinate);
        Assert.That(coordinate.Longitude, Is.EqualTo(6.12838).Within(delta));
        Assert.That(coordinate.Latitude, Is.EqualTo(49.60398).Within(delta));
    }

    /// <summary>
    /// Tests the simple encoding case.
    /// </summary>
    [Test]
    public void TestEncoding1()
    {
        // specify coordinate.
        var coordinate = new Coordinate()
        {
            Latitude = 0,
            Longitude = 0
        };

        // encode.
        var data = new byte[6];
        CoordinateConverter.Encode(coordinate, data, 0);
        Assert.That(data[0], Is.EqualTo(0));
        Assert.That(data[1], Is.EqualTo(0));
        Assert.That(data[2], Is.EqualTo(0));
        Assert.That(data[3], Is.EqualTo(0));
        Assert.That(data[4], Is.EqualTo(0));
        Assert.That(data[5], Is.EqualTo(0));

        // specify coordinate.
        coordinate = new Coordinate()
        {
            Latitude = 0.005460978,
            Longitude = 0.005460978
        };

        // encode.
        CoordinateConverter.Encode(coordinate, data, 0);
        Assert.That(data[0], Is.EqualTo(0));
        Assert.That(data[1], Is.EqualTo(0));
        Assert.That(data[2], Is.EqualTo(255));
        Assert.That(data[3], Is.EqualTo(0));
        Assert.That(data[4], Is.EqualTo(0));
        Assert.That(data[5], Is.EqualTo(255));

        // specify coordinate.
        coordinate = new Coordinate()
        {
            Latitude = 49.60851,
            Longitude = 6.12683
        };

        // encode.
        CoordinateConverter.Encode(coordinate, data, 0);
        Assert.That(data[0], Is.EqualTo(4));
        Assert.That(data[1], Is.EqualTo(91));
        Assert.That(data[2], Is.EqualTo(91));
        Assert.That(data[3], Is.EqualTo(35));
        Assert.That(data[4], Is.EqualTo(70));
        Assert.That(data[5], Is.EqualTo(244));
    }

    /// <summary>
    /// Tests the simple simple relative decoding case.
    /// </summary>
    [Test]
    public void TestEncodeRelative1()
    {
        // decode the coordinate relative to another coordinate.
        var reference = new Coordinate()
        {
            Latitude = 49.60851,
            Longitude = 6.12683
        };
        var coordinate = new Coordinate()
        {
            Latitude = 49.60398,
            Longitude = 6.12838
        };

        // encode.
        byte[] data = new byte[4];
        CoordinateConverter.EncodeRelative(reference, coordinate, data, 0);
        Assert.That(data[0], Is.EqualTo(0));
        Assert.That(data[1], Is.EqualTo(154));
        Assert.That(data[2], Is.EqualTo(254));
        Assert.That(data[3], Is.EqualTo(59));
    }

    /// <summary>
    /// Tests encoding decoding negative lat/lons, regression test for issue:
    /// https://github.com/itinero/OpenLR/issues/76
    /// </summary>
    [Test]
    public void RegressionTestEncodeDecodeNegative()
    {
        var delta = 0.0001;

        // specify coordinate.
        var coordinate = new Coordinate()
        {
            Latitude = -52.932136535644531,
            Longitude = -1.5213972330093384
        };

        // encode.
        var data = new byte[1024];
        CoordinateConverter.Encode(coordinate, data, 0);
        var decoded = CoordinateConverter.Decode(data);

        Assert.That(decoded.Latitude, Is.EqualTo(coordinate.Latitude).Within(delta));
        Assert.That(decoded.Longitude, Is.EqualTo(coordinate.Longitude).Within(delta));
    }

    /// <summary>
    /// Encodes and decodes negative integers, regression test for issue:
    /// https://github.com/itinero/OpenLR/issues/76
    /// </summary>
    [Test]
    public void RegressionEncodeDecodeNegativeInt()
    {
        var i = -10284;
        var data = new byte[1024];
        CoordinateConverter.EncodeInt24(i, data, 0);

        Assert.That(CoordinateConverter.DecodeInt24(data, 0), Is.EqualTo(i));

        i = -10284;
        CoordinateConverter.EncodeInt24(i, data, 0);
        Assert.That(CoordinateConverter.DecodeInt24(data, 0), Is.EqualTo(i));

        i = -184;
        CoordinateConverter.EncodeInt24(i, data, 0);
        Assert.That(CoordinateConverter.DecodeInt24(data, 0), Is.EqualTo(i));

        i = -78124;
        CoordinateConverter.EncodeInt24(i, data, 0);
        Assert.That(CoordinateConverter.DecodeInt24(data, 0), Is.EqualTo(i));

        i = 10284;
        CoordinateConverter.EncodeInt24(i, data, 0);

        Assert.That(CoordinateConverter.DecodeInt24(data, 0), Is.EqualTo(i));

        i = 10284;
        CoordinateConverter.EncodeInt24(i, data, 0);
        Assert.That(CoordinateConverter.DecodeInt24(data, 0), Is.EqualTo(i));

        i = 184;
        CoordinateConverter.EncodeInt24(i, data, 0);
        Assert.That(CoordinateConverter.DecodeInt24(data, 0), Is.EqualTo(i));

        i = 78124;
        CoordinateConverter.EncodeInt24(i, data, 0);
        Assert.That(CoordinateConverter.DecodeInt24(data, 0), Is.EqualTo(i));

        // try the 16-bit code.
        i = -10000;
        CoordinateConverter.EncodeInt16(i, data, 0);
        Assert.That(CoordinateConverter.DecodeInt16(data, 0), Is.EqualTo(i));

        i = -1024;
        CoordinateConverter.EncodeInt16(i, data, 0);
        Assert.That(CoordinateConverter.DecodeInt16(data, 0), Is.EqualTo(i));

        i = -184;
        CoordinateConverter.EncodeInt16(i, data, 0);
        Assert.That(CoordinateConverter.DecodeInt16(data, 0), Is.EqualTo(i));

        i = -781;
        CoordinateConverter.EncodeInt16(i, data, 0);
        Assert.That(CoordinateConverter.DecodeInt16(data, 0), Is.EqualTo(i));

        i = 14;
        CoordinateConverter.EncodeInt16(i, data, 0);

        Assert.That(CoordinateConverter.DecodeInt16(data, 0), Is.EqualTo(i));

        i = 104;
        CoordinateConverter.EncodeInt16(i, data, 0);
        Assert.That(CoordinateConverter.DecodeInt16(data, 0), Is.EqualTo(i));

        i = 184;
        CoordinateConverter.EncodeInt16(i, data, 0);
        Assert.That(CoordinateConverter.DecodeInt16(data, 0), Is.EqualTo(i));

        i = 724;
        CoordinateConverter.EncodeInt16(i, data, 0);
        Assert.That(CoordinateConverter.DecodeInt16(data, 0), Is.EqualTo(i));
    }
}
