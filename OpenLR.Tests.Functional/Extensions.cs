using Itinero;
using Itinero.LocalGeo;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace OpenLR.Tests.Functional
{
    public static class Extensions
    {
        public static string ToGeoJson(this FeatureCollection featureCollection)
        {
            var jsonSerializer = new NetTopologySuite.IO.GeoJsonSerializer();
            var jsonStream = new StringWriter();
            jsonSerializer.Serialize(jsonStream, featureCollection);
            var json = jsonStream.ToInvariantString();
            return json;
        }

        public static FeatureCollection FromGeoJson(string geoJson)
        {
            var jsonSerializer = new NetTopologySuite.IO.GeoJsonSerializer();
            var jsonStream = new StringReader(geoJson);
            return jsonSerializer.Deserialize<FeatureCollection>(new JsonTextReader(jsonStream)) as FeatureCollection;
        }
        
        public static FeatureCollection FromGeoJsonFile(string geoJsonFile)
        {
            return FromGeoJson(File.ReadAllText(geoJsonFile));
        }

        public static Coordinate[] PointsFromGeoJsonFile(string geoJsonFile)
        {
            var coordinates = new List<Coordinate>();
            var features = FromGeoJsonFile(geoJsonFile);
            foreach(var feature in features.Features)
            {
                if (feature.Geometry is Point)
                {
                    var p = feature.Geometry as Point;
                    coordinates.Add(new Coordinate((float)p.Coordinate.Y, (float)p.Coordinate.X));
                }
            }
            return coordinates.ToArray();
        }
    }
}