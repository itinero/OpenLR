using System.Collections.Generic;
using OpenLR.Matching;
using OpenLR.Model;

namespace OpenLR.Networks;

/// <summary>
/// Abstract representation of a network data interpreter.
/// </summary>
/// <remarks>
/// This translates attributes to the OpenLR data model.
/// </remarks>
public abstract class NetworkInterpreter
{
    /// <summary>
    /// Tries to extract fow/frc from the given attributes.
    /// </summary>
    public abstract bool Extract(IEnumerable<(string key, string value)> attributes, out FunctionalRoadClass frc, out FormOfWay fow);

    /// <summary>
    /// Matches nwb/fow.
    /// </summary>
    public virtual double Match(IEnumerable<(string key, string value)> attributes, FormOfWay fow, FunctionalRoadClass frc)
    {
        if (this.Extract(attributes, out var actualFrc, out var actualFow))
        { // a mapping was found. match and score.
            return MatchScoring.MatchAndScore(frc, fow, actualFrc, actualFow);
        }
        return 0;
    }
}
