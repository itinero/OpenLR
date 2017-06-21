using Itinero;
using NetTopologySuite.Features;
using System.IO;

namespace Samples.EncodeRoute
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
    }
}
