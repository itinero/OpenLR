using NUnit.Framework;
using OpenLR.Referenced.Codecs;

namespace OpenLR.Test;

/// <summary>
/// Holds tests for converting bearings to/from angles.
/// </summary>
[TestFixture]
public class BearingExtensionsTests
{
    [Test]
    public void Bearing_North_ShouldBe0()
    {
        var coordinates = new (double longitude, double latitude, float? e)[]
        {
            (4.461725265149994,
                51.22935086090999, null),
            (
                4.461721222122435,
                51.22976606793631, null)
        };

        Assert.That(coordinates.Bearing(), Is.EqualTo(0).Within(1).Or.EqualTo(360).Within(1));
    }

    [Test]
    public void Bearing_East_ShouldBe90()
    {
        var coordinates = new (double longitude, double latitude, float? e)[]
        {
            (
                4.461726655514013,
                51.22935132058359, null),
            (
                4.462709111030989,
                51.229350054702564, null)
        };

        Assert.That(coordinates.Bearing(), Is.EqualTo(90).Within(1));
    }

    [Test]
    public void Bearing_South_ShouldBe180()
    {
        var coordinates = new (double longitude, double latitude, float? e)[]
        {
            (
                4.461721222122435,
                51.22976606793631, null),
            (4.461725265149994,
                51.22935086090999, null),
        };

        Assert.That(coordinates.Bearing(), Is.EqualTo(180).Within(1));
    }

    [Test]
    public void Bearing_West_ShouldBe270()
    {
        var coordinates = new (double longitude, double latitude, float? e)[]
        {
            (
                4.462709111030989,
                51.229350054702564, null),
            (
                4.461726655514013,
                51.22935132058359, null),
        };

        Assert.That(coordinates.Bearing(), Is.EqualTo(270).Within(1));
    }
}
