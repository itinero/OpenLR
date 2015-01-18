# OpenLR Encoder/decoder

** This is still a work in progress

This is an implementation of the OpenLR (Open Location Reference) protocol based on OsmSharp. Development is sponsered by via.nl (http://via.nl/) and Be-Mobile (http://www.be-mobile-international.com/). 

Decoding/encoding is possible using different datasources, OpenStreetMap, NWB and TomTom MultiNet.

## Dependencies

* OsmSharp: For a basic graph structure, loading data and routing. Use OpenStreetMap for encoding/decoding.
* OsmSharp.Routing.Shape: Use shapefiles for routing and encoding/decoding.

## Usage

### The basics

Using this library is very simple. There is one decoder and and one encoder for each type of network, OpenStreetMap, NWB or MultiNet:

```csharp
var serializer = new LiveEdgeFlatfileSerializer();
var nwbGraph = serializer.Deserialize(
     new FileInfo(@"path\to\nwbdata.dump").OpenRead());
var nwbEncoder = ReferencedNWBEncoder.CreateBinary(nwbGraph);
var nwbDecoder = ReferencedNWBDecoder.CreateBinary(nwbGraph);

var multinetGraph = serializer.Deserialize(
     new FileInfo(@"path\to\multinetdata.dump").OpenRead());
var multinetEncoder = ReferencedMultiNetEncoder.CreateBinary(multinetGraph);
var multinetDecoder = ReferencedMultiNetDecoder.CreateBinary(multinetGraph);
```

Encoding is done by calling 'Encode' on the encoder given a location to encode:

```csharp
string encoded = nwbEncoder.Encode(location);
```

Decoding is very similar and done by calling 'Decode' on the decoder given an encoded string:

```csharp
var decodedLocation = nwbDecoder.Decode(encoded);
```

### Create a memory dump

The raw loader of OsmSharp can be used to load OpenStreetMap-data or load a shapefile. This codesample loads MultiNet data from a shapefile into a routing graph:

```csharp
// create a list of usefull keys (or columns to keep).
var usefullKeys = new HashSet<string>();
usefullKeys.Add("F_JNCTID");
usefullKeys.Add("T_JNCTID");
usefullKeys.Add("ONEWAY");
usefullKeys.Add("FRC");
usefullKeys.Add("FOW");

// create an instance of the graph reader and define the columns that contain the 'node-ids'.
var graphReader = new ShapefileLiveGraphReader("F_JNCTID", "T_JNCTID");
// read the graph from the folder where the shapefiles are placed.
var graph = graphReader.Read(@"\path\to\multinetshapes", "*nw.shp", 
     new ShapefileRoutingInterpreter(usefullKeys));
```

Loading networks this way can take a while. It's best to make a memory-dump of the routing graph for reuse later:

```csharp
// reading the shapefiles can take a while, this serializes the routing graph to disk to load again later.
var stream = new FileInfo(@"path\to\data.dump").OpenWrite();
var serializer = new LiveEdgeFlatfileSerializer();
serializer.Serialize(stream, graph, new TagsCollection());
stream.Flush();
```