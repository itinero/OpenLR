using NUnit.Framework;
using OpenLR.Codecs.Binary.Data;
using OpenLR.Model;

namespace OpenLR.Test.Binary.Data;

/// <summary>
/// Holds tests for encoding/decoding multiple fields to/from one byte.
/// </summary>
[TestFixture]
public class CombinedConversionTests
{
    /// <summary>
    /// Test encoding/decoding orientation/frc and fow.
    /// </summary>
    [Test]
    public void TestOrientationFrcFoW()
    {
        var data = new byte[1];
        data[0] = 127;

        // orientation-frc-fow.
        OrientationConverter.Encode(Orientation.FirstToSecond, data, 0, 0);
        FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc3, data, 0, 2);
        FormOfWayConvertor.Encode(FormOfWay.Roundabout, data, 0, 5);
        Assert.That(data[0], Is.EqualTo(92));

        // frc-orientation-fow.
        data[0] = 127;
        FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc3, data, 0, 2);
        OrientationConverter.Encode(Orientation.FirstToSecond, data, 0, 0);
        FormOfWayConvertor.Encode(FormOfWay.Roundabout, data, 0, 5);
        Assert.That(data[0], Is.EqualTo(92));

        // frc-fow-orientation.
        data[0] = 127;
        FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc3, data, 0, 2);
        FormOfWayConvertor.Encode(FormOfWay.Roundabout, data, 0, 5);
        OrientationConverter.Encode(Orientation.FirstToSecond, data, 0, 0);
        Assert.That(data[0], Is.EqualTo(92));
    }

    /// <summary>
    /// Test encoding/decoding frc and bearing.
    /// </summary>
    [Test]
    public void TestFrcBearing()
    {
        var data = new byte[1];
        data[0] = 0;

        // bearing-frc.
        BearingConvertor.Encode(17, data, 0, 3);
        FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc3, data, 0, 0);
        Assert.That(data[0], Is.EqualTo(113));

        // frc-bearing.
        FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc3, data, 0, 0);
        BearingConvertor.Encode(17, data, 0, 3);
        Assert.That(data[0], Is.EqualTo(113));
    }

    /// <summary>
    /// Tests encoding/decoding sideofroad-frc and fow.
    /// </summary>
    [Test]
    public void TestSideOfRoadFrcFow()
    {
        var data = new byte[1];
        data[0] = 0;

        // sideofroad-frc-fow.
        SideOfRoadConverter.Encode(SideOfRoad.Left, data, 0, 0);
        FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc3, data, 0, 2);
        FormOfWayConvertor.Encode(FormOfWay.Roundabout, data, 0, 5);
        Assert.That(data[0], Is.EqualTo(156));

        // frc-sideofroad-fow.
        data[0] = 0;
        FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc3, data, 0, 2);
        SideOfRoadConverter.Encode(SideOfRoad.Left, data, 0, 0);
        FormOfWayConvertor.Encode(FormOfWay.Roundabout, data, 0, 5);
        Assert.That(data[0], Is.EqualTo(156));

        // frc-fow-sideofroad.
        data[0] = 0;
        FunctionalRoadClassConvertor.Encode(FunctionalRoadClass.Frc3, data, 0, 2);
        FormOfWayConvertor.Encode(FormOfWay.Roundabout, data, 0, 5);
        SideOfRoadConverter.Encode(SideOfRoad.Left, data, 0, 0);
        Assert.That(data[0], Is.EqualTo(156));
    }

    /// <summary>
    /// Tests encoding/decoding poff-noff and bearing.
    /// </summary>
    [Test]
    public void TestPOffNOffBearing()
    {
        var data = new byte[1];
        data[0] = 0;

        // poff-noff-bearing.
        OffsetConvertor.EncodeFlag(true, data, 0, 1);
        OffsetConvertor.EncodeFlag(true, data, 0, 2);
        BearingConvertor.Encode(17, data, 0, 3);
        Assert.That(data[0], Is.EqualTo(113));

        // poff-bearing-noff.
        OffsetConvertor.EncodeFlag(true, data, 0, 1);
        BearingConvertor.Encode(17, data, 0, 3);
        OffsetConvertor.EncodeFlag(true, data, 0, 2);
        Assert.That(data[0], Is.EqualTo(113));

        // bearing-poff-noff.
        BearingConvertor.Encode(17, data, 0, 3);
        OffsetConvertor.EncodeFlag(true, data, 0, 1);
        OffsetConvertor.EncodeFlag(true, data, 0, 2);
        Assert.That(data[0], Is.EqualTo(113));
    }
}
