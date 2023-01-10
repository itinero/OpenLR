using System;
using Itinero.Network.Enumerators.Edges;
using OpenLR.Model;

namespace OpenLR.Networks;

/// <summary>
/// A network interpreter using edge enumeration and a cache.
/// </summary>
internal sealed class EdgeEnumeratorNetworkInterpreter
{
    private readonly NetworkInterpreter _networkInterpreter;
    private readonly MatchScoreCache _matchScoreCache = new MatchScoreCache();

    /// <summary>
    /// Creates a new edge enumerator interpreter.
    /// </summary>
    /// <param name="networkInterpreter">The network interpreter.</param>
    public EdgeEnumeratorNetworkInterpreter(NetworkInterpreter networkInterpreter)
    {
        _networkInterpreter = networkInterpreter;
    }

    /// <summary>
    /// Tries to extract fow/frc from the given attributes.
    /// </summary>
    public bool Extract(RoutingNetworkEdgeEnumerator enumerator, out FunctionalRoadClass frc, out FormOfWay fow)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Matches nwb/fow.
    /// </summary>
    public double Match(RoutingNetworkEdgeEnumerator enumerator, FormOfWay fow, FunctionalRoadClass frc)
    {
        if (!enumerator.EdgeTypeId.HasValue)
        {
            return _networkInterpreter.Match(enumerator.Attributes, fow, frc);
        }
        
        // get from cache.
        var match = _matchScoreCache.Get(enumerator.EdgeTypeId.Value);
        if (match != null) return match.Value;

        // calculate and store if not in cache.
        match = _networkInterpreter.Match(enumerator.Attributes, fow, frc);
        _matchScoreCache.Set(enumerator.EdgeTypeId.Value, match.Value);

        return match.Value;
    }
}
