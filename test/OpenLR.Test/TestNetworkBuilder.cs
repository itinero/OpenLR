// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Itinero.Data.Network;
using Itinero.LocalGeo;
using System.Collections.Generic;
using System.IO;
using Itinero.Geo;
using Itinero.Data.Network.Restrictions;
using Itinero;

namespace OpenLR.Test
{
    /// <summary>
    /// Builds test networks based on geojson files.
    /// </summary>
    public static class TestNetworkBuilder
    {
        private static float Tolerance = 20; // 10 meter.

        /// <summary>
        /// Loads a test network.
        /// </summary>
        public static void LoadTestNetwork(this RouterDb db, Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                db.LoadTestNetwork(streamReader.ReadToEnd());
            }
        }

        /// <summary>
        /// Loads a test network from geojson.
        /// </summary>
        public static void LoadTestNetwork(this RouterDb db, string geoJson)
        {
            var geoJsonReader = new NetTopologySuite.IO.GeoJsonReader();
            var features = geoJsonReader.Read<FeatureCollection>(geoJson);

            foreach (var feature in features.Features)
            {
                if (feature.Geometry is Point)
                {
                    var point = feature.Geometry as Point;
                    uint id;
                    if (feature.Attributes.Exists("id") &&
                       uint.TryParse(feature.Attributes["id"].ToInvariantString(), out id))
                    { // has and id, add as vertex.
                        db.Network.AddVertex(id,
                            (float)point.Coordinate.Y,
                            (float)point.Coordinate.X);
                    }
                }
            }

            foreach (var feature in features.Features)
            {
                if (feature.Geometry is LineString)
                {
                    if (feature.Attributes.Contains("restriction", "yes"))
                    {
                        continue;
                    }

                    var line = feature.Geometry as LineString;
                    var profile = new Itinero.Attributes.AttributeCollection();
                    var names = feature.Attributes.GetNames();
                    foreach (var name in names)
                    {
                        if (!name.StartsWith("meta:") &&
                            !name.StartsWith("stroke"))
                        {
                            profile.AddOrReplace(name, feature.Attributes[name].ToInvariantString());
                        }
                    }
                    var meta = new Itinero.Attributes.AttributeCollection();
                    foreach (var name in names)
                    {
                        if (name.StartsWith("meta:"))
                        {
                            meta.AddOrReplace(name.Remove(0, "meta:".Length),
                                feature.Attributes[name].ToInvariantString());
                        }
                    }

                    var profileId = db.EdgeProfiles.Add(profile);
                    var metaId = db.EdgeMeta.Add(meta);

                    var vertex1 = db.SearchVertexFor(
                        (float)line.Coordinates[0].Y,
                        (float)line.Coordinates[0].X);
                    var distance = 0.0;
                    var shape = new List<Coordinate>();
                    for (var i = 1; i < line.Coordinates.Length; i++)
                    {
                        var vertex2 = db.SearchVertexFor(
                            (float)line.Coordinates[i].Y,
                            (float)line.Coordinates[i].X);
                        distance += Coordinate.DistanceEstimateInMeter(
                            (float)line.Coordinates[i - 1].Y, (float)line.Coordinates[i - 1].X,
                            (float)line.Coordinates[i].Y, (float)line.Coordinates[i].X);
                        if (vertex2 == Itinero.Constants.NO_VERTEX)
                        { // add this point as shapepoint.
                            shape.Add(line.Coordinates[i].FromCoordinate());
                            continue;
                        }
                        db.Network.AddEdge(vertex1, vertex2, new Itinero.Data.Network.Edges.EdgeData()
                        {
                            Distance = (float)distance,
                            MetaId = metaId,
                            Profile = (ushort)profileId
                        }, shape);
                        shape.Clear();
                        vertex1 = vertex2;
                        distance = 0;
                    }
                }
            }

            foreach (var feature in features.Features)
            {
                if (feature.Geometry is LineString &&
                    feature.Attributes.Contains("restriction", "yes"))
                {
                    var line = feature.Geometry as LineString;
                    var sequence = new List<uint>();
                    sequence.Add(db.SearchVertexFor(
                        (float)line.Coordinates[0].Y,
                        (float)line.Coordinates[0].X));
                    for (var i = 1; i < line.Coordinates.Length; i++)
                    {
                        sequence.Add(db.SearchVertexFor(
                            (float)line.Coordinates[i].Y,
                            (float)line.Coordinates[i].X));
                    }

                    var vehicleType = string.Empty;
                    if (!feature.Attributes.TryGetValueAsString("vehicle_type", out vehicleType))
                    {
                        vehicleType = string.Empty;
                    }
                    RestrictionsDb restrictions;
                    if (!db.TryGetRestrictions(vehicleType, out restrictions))
                    {
                        restrictions = new RestrictionsDb();
                        db.AddRestrictions(vehicleType, restrictions);
                    }

                    restrictions.Add(sequence.ToArray());
                }
            }
        }

        /// <summary>
        /// Searches a vertex for the given location.
        /// </summary>
        public static uint SearchVertexFor(this RouterDb db, float latitude, float longitude)
        {
            for (uint vertex = 0; vertex < db.Network.VertexCount; vertex++)
            {
                float lat, lon;
                if (db.Network.GetVertex(vertex, out lat, out lon))
                {
                    var dist = Itinero.LocalGeo.Coordinate.DistanceEstimateInMeter(latitude, longitude,
                        lat, lon);
                    if (dist < Tolerance)
                    {
                        return vertex;
                    }
                }
            }
            return Itinero.Constants.NO_VERTEX;
        }
    }
}
