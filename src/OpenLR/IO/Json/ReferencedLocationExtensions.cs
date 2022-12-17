using System;
using System.IO;
using Itinero;
using OpenLR.Referenced.Locations;

namespace OpenLR.IO.Json;

public static class ReferencedLocationExtensions
{
    public static string ToGeoJson(this ReferencedPointAlongLine pointAlongLine, RouterDb db)
    {
        var stringWriter = new StringWriter();
        pointAlongLine.WriteAsFeatureCollection(stringWriter, db);
        return stringWriter.ToString();
    }
        
    internal static void WriteAsFeatureCollection(this ReferencedPointAlongLine pointAlongLine, TextWriter writer, RouterDb db)
    {
        if (db == null) { throw new ArgumentNullException(nameof(db)); }
        if (writer == null) { throw new ArgumentNullException(nameof(writer)); }

        var jsonWriter = new JsonWriter(writer);
        jsonWriter.OpenFeatureCollection();
            
        pointAlongLine.WriteFeaturesTo(jsonWriter, db);
            
        jsonWriter.CloseFeatureCollection();
    }
        
    internal static void WriteFeaturesTo(this ReferencedPointAlongLine pointAlongLine, JsonWriter jsonWriter, RouterDb db)
    {
        // write location.
        var coordinate = (pointAlongLine.Longitude, pointAlongLine.Latitude);
        jsonWriter.WriteOpen();
        jsonWriter.WriteProperty("type", "Feature", true, false);
        jsonWriter.WritePropertyName("geometry", false);

        jsonWriter.WriteOpen();
        jsonWriter.WriteProperty("type", "Point", true, false);
        jsonWriter.WritePropertyName("coordinates", false);
        jsonWriter.WriteArrayOpen();
        jsonWriter.WriteArrayValue(coordinate.Longitude.ToInvariantString());
        jsonWriter.WriteArrayValue(coordinate.Latitude.ToInvariantString());
        jsonWriter.WriteArrayClose();
        jsonWriter.WriteClose();

        jsonWriter.WritePropertyName("properties");
        jsonWriter.WriteOpen();
        jsonWriter.WriteProperty("orientation", pointAlongLine.Orientation.ToInvariantString(), true);
        jsonWriter.WriteClose();

        jsonWriter.WriteClose();

        pointAlongLine.Route?.WriteFeaturesTo(jsonWriter, db);
    }
        
    public static string ToGeoJson(this ReferencedLine line, RouterDb db)
    {
        var stringWriter = new StringWriter();
        line.WriteAsFeatureCollection(stringWriter, db);
        return stringWriter.ToInvariantString();
    }
        
    internal static void WriteAsFeatureCollection(this ReferencedLine line, TextWriter writer, RouterDb db)
    {
        if (db == null) { throw new ArgumentNullException(nameof(db)); }
        if (writer == null) { throw new ArgumentNullException(nameof(writer)); }


        var jsonWriter = new JsonWriter(writer);
        jsonWriter.OpenFeatureCollection();
            
        line.WriteFeaturesTo(jsonWriter, db);
            
        jsonWriter.CloseFeatureCollection();
    }
        
    internal static void WriteFeaturesTo(this ReferencedLine line, JsonWriter jsonWriter, RouterDb db)
    {
        var router = new Router(db);
        if (line.Edges != null)
        {
            var edgeEnumerator = db.Network.GetEdgeEnumerator();
            foreach (var edge in line.Edges)
            {
                edgeEnumerator.MoveToEdge(edge);
                    
                router.WriteEdge(jsonWriter, edgeEnumerator);
            }
        }

        if (line.Vertices != null)
        {
            foreach (var vertex in line.Vertices)
            {
                db.WriteVertex(jsonWriter, vertex);
            }
        }

        if (line.StartLocation != null)
        {
            // write location on network.
            var coordinate = line.StartLocation.LocationOnNetwork(db);
            jsonWriter.WriteOpen();
            jsonWriter.WriteProperty("type", "Feature", true, false);
            jsonWriter.WritePropertyName("geometry", false);

            jsonWriter.WriteOpen();
            jsonWriter.WriteProperty("type", "Point", true, false);
            jsonWriter.WritePropertyName("coordinates", false);
            jsonWriter.WriteArrayOpen();
            jsonWriter.WriteArrayValue(coordinate.Longitude.ToInvariantString());
            jsonWriter.WriteArrayValue(coordinate.Latitude.ToInvariantString());
            jsonWriter.WriteArrayClose();
            jsonWriter.WriteClose();

            jsonWriter.WritePropertyName("properties");
            jsonWriter.WriteOpen();
            jsonWriter.WriteProperty("type", "location_on_network", true);
            jsonWriter.WriteProperty("offset", line.StartLocation.Offset.ToInvariantString());
            jsonWriter.WriteProperty("positive_offset_percentage", line.PositiveOffsetPercentage.ToInvariantString());
            jsonWriter.WriteClose();

            jsonWriter.WriteClose();

            // write original location.
            coordinate = line.StartLocation.Location();
            jsonWriter.WriteOpen();
            jsonWriter.WriteProperty("type", "Feature", true, false);
            jsonWriter.WritePropertyName("geometry", false);

            jsonWriter.WriteOpen();
            jsonWriter.WriteProperty("type", "Point", true, false);
            jsonWriter.WritePropertyName("coordinates", false);
            jsonWriter.WriteArrayOpen();
            jsonWriter.WriteArrayValue(coordinate.Longitude.ToInvariantString());
            jsonWriter.WriteArrayValue(coordinate.Latitude.ToInvariantString());
            jsonWriter.WriteArrayClose();
            jsonWriter.WriteClose();

            jsonWriter.WritePropertyName("properties");
            jsonWriter.WriteOpen();
            jsonWriter.WriteProperty("type", "original_location", true);
            jsonWriter.WriteClose();

            jsonWriter.WriteClose();
        }

        if (line.EndLocation != null)
        {
            // write location on network.
            var coordinate = line.EndLocation.LocationOnNetwork(db);
            jsonWriter.WriteOpen();
            jsonWriter.WriteProperty("type", "Feature", true, false);
            jsonWriter.WritePropertyName("geometry", false);

            jsonWriter.WriteOpen();
            jsonWriter.WriteProperty("type", "Point", true, false);
            jsonWriter.WritePropertyName("coordinates", false);
            jsonWriter.WriteArrayOpen();
            jsonWriter.WriteArrayValue(coordinate.Longitude.ToInvariantString());
            jsonWriter.WriteArrayValue(coordinate.Latitude.ToInvariantString());
            jsonWriter.WriteArrayClose();
            jsonWriter.WriteClose();

            jsonWriter.WritePropertyName("properties");
            jsonWriter.WriteOpen();
            jsonWriter.WriteProperty("type", "location_on_network", true);
            jsonWriter.WriteProperty("offset", line.EndLocation.Offset.ToInvariantString());
            jsonWriter.WriteProperty("negative_offset_percentage", line.NegativeOffsetPercentage.ToInvariantString());
            jsonWriter.WriteClose();

            jsonWriter.WriteClose();

            // write original location.
            coordinate = line.EndLocation.Location();
            jsonWriter.WriteOpen();
            jsonWriter.WriteProperty("type", "Feature", true, false);
            jsonWriter.WritePropertyName("geometry", false);

            jsonWriter.WriteOpen();
            jsonWriter.WriteProperty("type", "Point", true, false);
            jsonWriter.WritePropertyName("coordinates", false);
            jsonWriter.WriteArrayOpen();
            jsonWriter.WriteArrayValue(coordinate.Longitude.ToInvariantString());
            jsonWriter.WriteArrayValue(coordinate.Latitude.ToInvariantString());
            jsonWriter.WriteArrayClose();
            jsonWriter.WriteClose();

            jsonWriter.WritePropertyName("properties");
            jsonWriter.WriteOpen();
            jsonWriter.WriteProperty("type", "original_location", true);
            jsonWriter.WriteClose();

            jsonWriter.WriteClose();
        }
    }

    /// <summary>
    /// Writes a point-geometry for the given vertex.
    /// </summary>
    internal static void WriteVertex(this RouterDb db, JsonWriter jsonWriter, uint vertex)
    {
        var coordinate = db.Network.GetVertex(vertex);
            
        jsonWriter.WriteOpen();
        jsonWriter.WriteProperty("type", "Feature", true, false);
        jsonWriter.WritePropertyName("geometry", false);

        jsonWriter.WriteOpen();
        jsonWriter.WriteProperty("type", "Point", true, false);
        jsonWriter.WritePropertyName("coordinates", false);
        jsonWriter.WriteArrayOpen();
        jsonWriter.WriteArrayValue(coordinate.Longitude.ToInvariantString());
        jsonWriter.WriteArrayValue(coordinate.Latitude.ToInvariantString());
        jsonWriter.WriteArrayClose();
        jsonWriter.WriteClose();

        jsonWriter.WritePropertyName("properties");
        jsonWriter.WriteOpen();
        jsonWriter.WriteProperty("id", vertex.ToInvariantString());

        if (db.VertexData != null)
        {
            foreach (var dataName in db.VertexData.Names)
            {
                var dataCollection = db.VertexData.Get(dataName);
                if (vertex >= dataCollection.Count)
                {
                    continue;
                }
                var data = dataCollection.GetRaw(vertex);
                if (data != null)
                {
                    jsonWriter.WriteProperty(dataName, data.ToInvariantString());
                }
            }
        }

        var vertexMeta = db.VertexMeta?[vertex];
        if (vertexMeta != null)
        {
            foreach (var meta in vertexMeta)
            {
                jsonWriter.WriteProperty(meta.Key, meta.Value, true, true);
            }
        }

        jsonWriter.WriteClose();

        jsonWriter.WriteClose();
    }
        
    /// <summary>
    /// Writes a linestring-geometry for the edge currently in the enumerator.
    /// </summary>
    internal static void WriteEdge(this Router router, JsonWriter jsonWriter, RoutingNetwork.EdgeEnumerator edgeEnumerator, 
        bool includeProfileDetails = true)
    {
        var db = router.Db;
            
        var edgeAttributes = new Itinero.Attributes.AttributeCollection(db.EdgeMeta.Get(edgeEnumerator.Data.MetaId));
        edgeAttributes.AddOrReplace(db.EdgeProfiles.Get(edgeEnumerator.Data.Profile));

        var shape = db.Network.GetShape(edgeEnumerator.Current);
            
        jsonWriter.WriteOpen();
        jsonWriter.WriteProperty("type", "Feature", true, false);
        jsonWriter.WritePropertyName("geometry", false);

        jsonWriter.WriteOpen();
        jsonWriter.WriteProperty("type", "LineString", true, false);
        jsonWriter.WritePropertyName("coordinates", false);
        jsonWriter.WriteArrayOpen();

        foreach (var coordinate in shape)
        {
            jsonWriter.WriteArrayOpen();
            jsonWriter.WriteArrayValue(coordinate.Longitude.ToInvariantString());
            jsonWriter.WriteArrayValue(coordinate.Latitude.ToInvariantString());
            jsonWriter.WriteArrayClose();
        }

        jsonWriter.WriteArrayClose();
        jsonWriter.WriteClose();

        jsonWriter.WritePropertyName("properties");
        jsonWriter.WriteOpen();
        foreach (var attribute in edgeAttributes)
        {
            jsonWriter.WriteProperty(attribute.Key, attribute.Value, true, true);
        }
        jsonWriter.WriteProperty("edgeid", edgeEnumerator.Id.ToInvariantString());
        jsonWriter.WriteProperty("vertex1", edgeEnumerator.From.ToInvariantString());
        jsonWriter.WriteProperty("vertex2", edgeEnumerator.To.ToInvariantString());

        if (db.EdgeData != null)
        {
            foreach (var dataName in db.EdgeData.Names)
            {
                var edgeId = edgeEnumerator.Id;
                var dataCollection = db.EdgeData.Get(dataName);
                if (edgeId >= dataCollection.Count)
                {
                    continue;
                }
                var data = dataCollection.GetRaw(edgeId);
                if (data != null)
                {
                    jsonWriter.WriteProperty(dataName, data.ToInvariantString());
                }
            }
        }

        if (includeProfileDetails)
        {
            foreach (var profile in db.GetSupportedProfiles())
            {
                var profileName = profile.FullName.ToLower();
                var cache = router.GetAugmentedGetFactor(profile);
                    
                var factor = cache(edgeEnumerator.Data.Profile);
                    
                jsonWriter.WriteProperty(profileName + "_direction", 
                    factor.Direction.ToString(), true);
                    
                var speed = 1/factor.SpeedFactor*3.6;
                if (factor.SpeedFactor <= 0) speed = 65536;
                jsonWriter.WriteProperty(profileName + "_speed", 
                    System.Math.Round(speed, 2).ToInvariantString(), true);
                    
                speed = 1/factor.Value*3.6;
                if (factor.Value <= 0) speed = 65536;
                jsonWriter.WriteProperty(profileName + "_speed_corrected", 
                    System.Math.Round(speed, 2).ToInvariantString(), true);
            }
        }

        jsonWriter.WriteClose();

        jsonWriter.WriteClose();
    }
        
    internal static void OpenFeatureCollection(this JsonWriter jsonWriter)
    {
        jsonWriter.WriteOpen();
        jsonWriter.WriteProperty("type", "FeatureCollection", true, false);
        jsonWriter.WritePropertyName("features", false);
        jsonWriter.WriteArrayOpen();
    }

    internal static void CloseFeatureCollection(this JsonWriter jsonWriter)
    {
        jsonWriter.WriteArrayClose();
        jsonWriter.WriteClose();
    }
}
