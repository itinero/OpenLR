using System;
using System.Threading.Tasks;
using Itinero;
using Itinero.Network;
using OpenLR.Model.Locations;
using OpenLR.Networks;
using OpenLR.Referenced;
using OpenLR.Referenced.Locations;

namespace OpenLR;

/// <summary>
/// The OpenLR encoder/decoder.
/// </summary>
public class Coder
{
    /// <summary>
    /// Creates a new coder with a default binary codec.
    /// </summary>
    /// <param name="routingNetwork">The routing network.</param>
    /// <param name="settings">The settings.</param>
    public Coder(RoutingNetwork routingNetwork, CoderSettings settings)
    {
        if (settings.NetworkInterpreter == null) throw new Exception("No network data interpreter set.");
        if (settings.RawCodec == null) throw new Exception("No raw codec set.");

        this.Network = routingNetwork;
        this.Settings = settings;

        this.Interpreter = new EdgeEnumeratorNetworkInterpreter(this.Settings.NetworkInterpreter);
    }

    internal RoutingNetwork Network { get; }

    internal EdgeEnumeratorNetworkInterpreter Interpreter { get; }

    /// <summary>
    /// Gets the profile.
    /// </summary>
    public CoderSettings Settings { get; }

    /// <summary>
    /// Encodes a location into an OpenLR string.
    /// </summary>
    public string Encode(ReferencedCircle location)
    {
        return this.Settings.RawCodec.Encode(Referenced.Codecs.ReferencedCircleCodec.Encode(location));
    }

    /// <summary>
    /// Encodes a location into an OpenLR string.
    /// </summary>
    public string Encode(ReferencedGeoCoordinate location)
    {
        return this.Settings.RawCodec.Encode(Referenced.Codecs.ReferencedGeoCoordinateCodec.Encode(location));
    }

    /// <summary>
    /// Encodes a location into an OpenLR string.
    /// </summary>
    public string Encode(ReferencedGrid location)
    {
        return this.Settings.RawCodec.Encode(Referenced.Codecs.ReferencedGridCodec.Encode(location));
    }

    /// <summary>
    /// Encodes a location into an OpenLR string.
    /// </summary>
    public string Encode(ReferencedLine location, EncodingSettings? settings = null)
    {
        settings ??= EncodingSettings.Default;
        return this.Settings.RawCodec.Encode(
            Referenced.Codecs.ReferencedLineCodec.Encode(location, this, settings));
    }

    /// <summary>
    /// Encodes a location into an OpenLR string.
    /// </summary>
    public string Encode(ReferencedPointAlongLine location)
    {
        return this.Settings.RawCodec.Encode(Referenced.Codecs.ReferencedPointAlongLineCodec.Encode(location, this));
    }

    /// <summary>
    /// Encodes a location into an OpenLR string.
    /// </summary>
    public string Encode(ReferencedPolygon location, EncodingSettings? settings = null)
    {
        return this.Settings.RawCodec.Encode(Referenced.Codecs.ReferencedPolygonCodec.Encode(location));
    }

    /// <summary>
    /// Encodes a location into an OpenLR string.
    /// </summary>
    public string Encode(ReferencedRectangle location, EncodingSettings? settings = null)
    {
        return this.Settings.RawCodec.Encode(Referenced.Codecs.ReferencedRectangleCodec.Encode(location));
    }

    /// <summary>
    /// Decodes an OpenLR string into a location.
    /// </summary>
    public async Task<Result<IReferencedLocation>> Decode(string encoded)
    {
        var location = this.Settings.RawCodec.Decode(encoded);

        return location switch
        {
            CircleLocation circleLocation => Referenced.Codecs.ReferencedCircleCodec.Decode(circleLocation),
            GeoCoordinateLocation coordinateLocation => Referenced.Codecs.ReferencedGeoCoordinateCodec.Decode(
                coordinateLocation),
            GridLocation gridLocation => Referenced.Codecs.ReferencedGridCodec.Decode(gridLocation),
            LineLocation lineLocation => Result<IReferencedLocation>.Create(await Referenced.Codecs.ReferencedLineCodec.Decode(lineLocation, this)),
            PointAlongLineLocation lineLocation => Referenced.Codecs.ReferencedPointAlongLineCodec.Decode(
                lineLocation, this),
            PolygonLocation polygonLocation => Referenced.Codecs.ReferencedPolygonCodec.Decode(polygonLocation),
            RectangleLocation rectangleLocation => Referenced.Codecs.ReferencedRectangleCodec.Decode(
                rectangleLocation),
            _ => throw new ArgumentOutOfRangeException(nameof(encoded), "Unknown encoded string.")
        };
    }

    /// <summary>
    /// Decodes a line location.
    /// </summary>
    /// <param name="lineLocation">The line location.</param>
    /// <returns>A referenced line, if success.</returns>
    public async Task<Result<ReferencedLine>> Decode(LineLocation lineLocation)
    {
        return await Referenced.Codecs.ReferencedLineCodec.Decode(lineLocation, this);
    }
}
