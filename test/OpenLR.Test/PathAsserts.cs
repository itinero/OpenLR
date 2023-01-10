using Itinero.Network;
using NUnit.Framework;

namespace OpenLR.Test;

internal static class PathAsserts
{
    public static void AreEqual(IEnumerable<(EdgeId edge, bool forward)> expected,
        IEnumerable<(EdgeId edge, bool forward)> actual)
    {
        var expectedList = expected.ToList();
        var actualList = actual.ToList();

        Assert.That(actualList, Has.Count.EqualTo(expectedList.Count));
        for (var i = 0; i < expectedList.Count; i++)
        {
            Assert.Multiple(() =>
            {
                Assert.That(actualList[i].edge.LocalId, Is.EqualTo(expectedList[i].edge.LocalId));
                Assert.That(actualList[i].edge.TileId, Is.EqualTo(expectedList[i].edge.TileId));
                Assert.That(actualList[i].forward, Is.EqualTo(expectedList[i].forward));
            });
        }
    }
}
