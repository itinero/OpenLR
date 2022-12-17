using System;
using Itinero.Network;

namespace OpenLR;

/// <summary>
/// Contains extensions methods for the routing networks.
/// </summary>
public static class RoutingNetworkExtensions
{
    /// <summary>
    /// Creates an OpenLR coder for the given network.
    /// </summary>
    /// <param name="routingNetwork">The routing network.</param>
    /// <param name="config">The configuration callback.</param>
    /// <returns>The configured coder.</returns>
    public static Coder Coder(this RoutingNetwork routingNetwork, Action<CoderSettings> config)
    {
        var settings = new CoderSettings();
        config(settings);

        return new Coder(routingNetwork, settings);
    }
}
