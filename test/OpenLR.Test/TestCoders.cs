using Itinero.Profiles;
using OpenLR.Networks.Osm;

namespace OpenLR.Test;

internal static class TestCoders
{
    public static async Task<Coder> Coder1(Profile? profile = null)
    {
        profile ??= new DefaultProfile();

        var network = await TestNetworks.Network1();
        return new Coder(network,
            new CoderSettings()
            {
                NetworkInterpreter = new OsmNetworkInterpreter(),
                RoutingSettings = { Profile = profile }
            });
    }

    public static async Task<Coder> Coder3(Profile? profile = null)
    {
        profile ??= new DefaultProfile();

        var network = await TestNetworks.Network3();
        return new Coder(network,
            new CoderSettings()
            {
                NetworkInterpreter = new OsmNetworkInterpreter(),
                RoutingSettings = { Profile = profile }
            });
    }
}
