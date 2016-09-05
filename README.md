# OpenLR for .NET

This is an implementation of the OpenLR (Open Location Reference) protocol using Itinero. Development was initially sponsered by via.nl (http://via.nl/) and Be-Mobile (http://www.be-mobile-international.com/). 

## Dependencies

* Itinero: For a basic routing graph structure, loading data and routing.
* NetTopologySuite: A geo library for .NET.

## Usage

### The basics

By default, just like in Itinero, all code is there to decode/encode based on an OpenStreetMap network. 

The most basic code sample encoding/decoding a line location:

```csharp
    // build routerdb from raw OSM data.
    // check this for more info on RouterDb's: https://github.com/itinero/routing/wiki/RouterDb
    var routerDb = new RouterDb();
    using (var sourceStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "luxembourg-latest.osm.pbf")))
    {
        routerDb.LoadOsmData(sourceStream, Vehicle.Car);
    }

    // create coder.
    var coder = new Coder(routerDb, new OsmCoderProfile());

    // build a line location from a shortest path.
    var line = coder.BuildLine(new Itinero.LocalGeo.Coordinate(49.67218282319583f, 6.142280101776122f),
        new Itinero.LocalGeo.Coordinate(49.67776489459803f, 6.1342549324035645f));

    // encode this location.
    var encoded = coder.Encode(line);

    // decode this location.
    var decodedLine = coder.Decode(encoded) as ReferencedLine;
```
### Samples

Check the samples here: https://github.com/itinero/OpenLR/tree/develop/samples/
