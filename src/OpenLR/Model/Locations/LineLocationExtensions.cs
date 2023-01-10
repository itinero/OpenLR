using System.Collections.Generic;

namespace OpenLR.Model.Locations;

internal static class LineLocationExtensions
{
    public static IEnumerable<(LocationReferencePoint lrp, bool isLast)> LocationReferencePoints(this LineLocation lineLocation)
    {
        yield return (lineLocation.First, false);

        foreach (var intermediate in lineLocation.Intermediate)
        {
            yield return (intermediate, false);
        }

        yield return (lineLocation.Last, true);
    }
}
