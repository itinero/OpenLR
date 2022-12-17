using Itinero.Routing;
using OpenLR.Codecs.Binary;
using OpenLR.Networks;

namespace OpenLR;

/// <summary>
/// A profile that can be used by the coder to implement network-specifics.
/// </summary>
public class CoderSettings
{
    /// <summary>
    /// Gets the routing settings.
    /// </summary>
    public RoutingSettings RoutingSettings { get; } = new ();

    /// <summary>
    /// Gets the score threshold.
    /// </summary>
    public float ScoreThreshold { get; set; }
    
    /// <summary>
    /// Gets the network interpreter.
    /// </summary>
    public NetworkInterpreter NetworkInterpreter { get; set; } = null!;

    /// <summary>
    /// Gets the raw codec.
    /// </summary>
    public Codecs.CodecBase RawCodec { get; } = new BinaryCodec();
}
