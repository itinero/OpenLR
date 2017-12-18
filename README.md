# OpenLR for .NET

[![Build status](http://build.itinero.tech:8080/app/rest/builds/buildType:(id:Itinero_Openlr)/statusIcon)](https://build.itinero.tech/viewType.html?buildTypeId=Itinero_Openlr)
[![Visit our website](https://img.shields.io/badge/website-itinero.tech-020031.svg) ](http://www.itinero.tech/)
[![GPL licensed](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](https://github.com/itinero/openlr/blob/develop/LICENSE.md)

- OpenLR: [![NuGet Badge](https://buildstats.info/nuget/OpenLR)](https://www.nuget.org/packages/OpenLR/)
- OpenLR.Geo: [![NuGet Badge](https://buildstats.info/nuget/OpenLR.Geo)](https://www.nuget.org/packages/OpenLR.Geo/)

This is an implementation of the OpenLR (Open Location Reference) protocol using Itinero. Development was initially sponsered by via.nl (http://via.nl/) and Be-Mobile (http://www.be-mobile-international.com/). 

## Dependencies

* [Itinero](https://github.com/itinero/routing): For a basic routing graph structure, loading data and routing.

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

### Samples & Docs

Check the samples here: https://github.com/itinero/OpenLR/tree/develop/samples/

There is also basic documentation here: https://github.com/itinero/OpenLR/wiki
