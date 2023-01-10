using System.Reflection;
using Itinero;
using Itinero.Geo;
using Itinero.Network;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace OpenLR.Test;

/// <summary>
/// Builds test networks based on geojson files.
/// </summary>
public static class TestNetworks
{
    private const double Tolerance = 20; // 10 meter.

    public static async Task<RoutingNetwork> Network1()
    {
        var routerDb = new RouterDb();
        await routerDb.LoadTestNetworkAsync(
            Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OpenLR.Test.test_data.networks.network1.geojson"));
        return routerDb.Latest;
    }

    public static async Task<RoutingNetwork> Network3()
    {
        var routerDb = new RouterDb();
        await routerDb.LoadTestNetworkAsync(
            Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OpenLR.Test.test_data.networks.network3.geojson"));
        return routerDb.Latest;
    }

    /// <summary>
    /// Loads a test network.
    /// </summary>
    public static async Task LoadTestNetworkAsync(this RouterDb db, Stream? stream)
    {
        using var streamReader = new StreamReader(stream ?? throw new ArgumentNullException(nameof(stream)));
        await LoadTestNetworkAsync(db, await streamReader.ReadToEndAsync());
    }

    /// <summary>
    /// Loads a test network from geojson.
    /// </summary>
    public static async Task LoadTestNetworkAsync(this RouterDb db, string geoJson)
    {
        using var networkWriter = db.Latest.GetWriter();
        var geoJsonReader = new NetTopologySuite.IO.GeoJsonReader();
        var features = geoJsonReader.Read<FeatureCollection>(geoJson);

        // first add all vertices.
        foreach (var feature in features)
        {
            if (feature.Geometry is not Point point) continue;

            if (feature.Attributes.Exists("id") &&
                uint.TryParse(feature.Attributes["id"].ToInvariantString(), out _))
            { // has and id, add as vertex.
                networkWriter.AddVertex(point.Coordinate.X, point.Coordinate.Y);
            }
        }

        // add edges.
        var snapper = db.Latest.Snap();
        foreach (var feature in features)
        {
            if (feature.Geometry is not LineString lineString) continue;
            if (feature.Attributes.Contains("restriction", "yes")) continue;

            var vertex1 = await snapper.ToVertexAsync(lineString.Coordinates[0].X,
                lineString.Coordinates[0].Y);
            var shape = new List<(double longitude, double latitude, float? e)>();
            for (var i = 1; i < lineString.Coordinates.Length; i++)
            {
                var current = lineString.Coordinates[i];
                var vertex2 = await snapper.ToVertexAsync(current.X,
                    current.Y);
                if (vertex2.IsError)
                { // add this point as shape point.
                    shape.Add(current.ToCoordinateTuple());
                    continue;
                }

                networkWriter.AddEdge(vertex1, vertex2, shape, feature.Attributes.ToAttributes());

                shape.Clear();
                vertex1 = vertex2;
            }
        }
    }
}
