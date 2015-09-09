using OpenLR.Encoding;
using OpenLR.Model;
using OpenLR.Referenced.Encoding;
using OpenLR.Referenced.Router;
using OsmSharp.Collections.Tags;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Shape;
using OsmSharp.Routing.Shape.Readers;
using System;
using System.Collections.Generic;

namespace OpenLR.Referenced.NWB
{
    /// <summary>
    /// An implementation of a referenced encoder based on the Nationaal Wegenbestand (NWB) in the netherlands.
    /// </summary>
    public class ReferencedNWBEncoder : ReferencedEncoderBase
    {
        /// <summary>
        /// Creates a new referenced live edge decoder.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="locationEncoder"></param>
        public ReferencedNWBEncoder(BasicRouterDataSource<LiveEdge> graph, Encoder locationEncoder)
            : base(graph, locationEncoder)
        {

        }

        /// <summary>
        /// Returns the tags associated with the given tags id.
        /// </summary>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        public override TagsCollectionBase GetTags(uint tagsId)
        {
            return this.Graph.TagsIndex.Get(tagsId);
        }

        /// <summary>
        /// Tries to match the given tags and figure out a corresponding frc and fow.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="frc"></param>
        /// <param name="fow"></param>
        /// <returns>False if no matching was found.</returns>
        public override bool TryMatching(TagsCollectionBase tags, out FunctionalRoadClass frc, out FormOfWay fow)
        {
            return NWBMapping.ToOpenLR(tags, out fow, out frc);
        }

        /// <summary>
        /// Returns a value if a oneway restriction is found.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns>null: no restrictions, true: forward restriction, false: backward restriction.</returns>
        /// <returns></returns>
        public override bool? IsOneway(TagsCollectionBase tags)
        {
            return this.Vehicle.IsOneWay(tags);
        }

        /// <summary>
        /// Holds the encoder vehicle.
        /// </summary>
        private Vehicle _vehicle = new global::OsmSharp.Routing.Shape.Vehicles.Car("RIJRICHTNG", "H", "T", string.Empty);

        /// <summary>
        /// Returns the encoder vehicle profile.
        /// </summary>
        public override global::OsmSharp.Routing.Vehicle Vehicle
        {
            get { return _vehicle; }
        }

        /// <summary>
        /// Returns true if the given vertex is a valid candidate to use as a location reference point.
        /// </summary>
        /// <param name="vertex">The vertex is validate.</param>
        /// <returns></returns>
        public override bool IsVertexValid(long vertex)
        {
            var arcs = this.Graph.GetEdges(vertex);

            // go over each arc and count the traversible arcs.
            var traversCount = 0;
            foreach (var arc in arcs)
            {
                var tags = this.Graph.TagsIndex.Get(arc.Value.Tags);
                if (this.Vehicle.CanTraverse(tags))
                {
                    traversCount++;
                }
            }
            if (traversCount != 3)
            { // no special cases, only 1=valid, 2=invalid or 4 and up=valid.
                if (traversCount == 2)
                { // only two traversable edges, no options here!
                    return false;
                }
                return true;
            }
            else
            { // special cases possible here, we need more info here.
                var incoming = new List<Tuple<long, TagsCollectionBase, LiveEdge>>();
                var outgoing = new List<Tuple<long, TagsCollectionBase, LiveEdge>>();
                var bidirectional = new List<Tuple<long, TagsCollectionBase, LiveEdge>>();
                foreach (var arc in arcs)
                {
                    var tags = this.Graph.TagsIndex.Get(arc.Value.Tags);
                    if (this.Vehicle.CanTraverse(tags))
                    {
                        var oneway = this.Vehicle.IsOneWay(tags);
                        if (!oneway.HasValue)
                        { // bidirectional, can be used as incoming.
                            bidirectional.Add(new Tuple<long, TagsCollectionBase, LiveEdge>(arc.Key, tags, arc.Value));
                        }
                        else if (oneway.Value != arc.Value.Forward)
                        { // oneway is forward but arc is backward, arc is incoming.
                            // oneway is backward and arc is forward, arc is incoming.
                            incoming.Add(new Tuple<long, TagsCollectionBase, LiveEdge>(arc.Key, tags, arc.Value));
                        }
                        else if (oneway.Value == arc.Value.Forward)
                        { // oneway is forward and arc is forward, arc is outgoing.
                            // oneway is backward and arc is backward, arc is outgoing.
                            outgoing.Add(new Tuple<long, TagsCollectionBase, LiveEdge>(arc.Key, tags, arc.Value));
                        }
                    }
                }

                if (bidirectional.Count == 1 && incoming.Count == 1 && outgoing.Count == 1)
                { // all special cases are found here.
                    // get incoming's frc and fow.
                    FormOfWay incomingFow, outgoingFow;
                    FunctionalRoadClass incomingFrc, outgoingFrc;
                    if (this.TryMatching(incoming[0].Item2, out incomingFrc, out incomingFow))
                    {
                        if (incomingFow == FormOfWay.Roundabout)
                        { // is this a roundabout, always valid.
                            return true;
                        }
                        if (this.TryMatching(outgoing[0].Item2, out outgoingFrc, out outgoingFow))
                        {
                            if (outgoingFow == FormOfWay.Roundabout)
                            { // is this a roundabout, always valid.
                                return true;
                            }
                        }

                        // at this stage we have:
                        // - two oneways, in opposite direction
                        // - one bidirectional
                        // - all same frc.

                        // the only thing left to check is if the oneway edges go in the same general direction or not.
                        // compare bearings but only if distance is large enough.
                        var incomingShape = this.Graph.GetCoordinates(new Tuple<long, long, LiveEdge>(
                            vertex, incoming[0].Item1, incoming[0].Item3));
                        var outgoingShape = this.Graph.GetCoordinates(new Tuple<long, long, LiveEdge>(
                            vertex, outgoing[0].Item1, outgoing[0].Item3));

                        if (incomingShape.Length().Value < 25 &&
                            outgoingShape.Length().Value < 25)
                        { // edges are too short to compare bearing in a way meaningful for determining this.
                            // assume not valid.
                            return false;
                        }
                        var incomingBearing = BearingEncoder.EncodeBearing(incomingShape);
                        var outgoingBearing = BearingEncoder.EncodeBearing(outgoingShape);

                        if (incomingBearing.SmallestDifference(outgoingBearing) > 30)
                        { // edges are clearly not going in the same direction.
                            return true;
                        }
                    }
                    return false;
                }
                return true;
            }
        }

        #region Static Creation Helper Functions

        /// <summary>
        /// Creates a new referenced NWB encoder.
        /// </summary>
        /// <param name="folder">The folder containing the shapefile(s).</param>
        /// <param name="searchPattern">The search pattern to identify the relevant shapefiles.</param>
        /// <returns></returns>
        public static ReferencedNWBEncoder CreateBinary(string folder, string searchPattern)
        {
            return ReferencedNWBEncoder.Create(folder, searchPattern, new OpenLR.Binary.BinaryEncoder());
        }

        /// <summary>
        /// Creates a new referenced NWB encoder.
        /// </summary>
        /// <param name="graph">The graph containing the NWB network.</param>
        /// <returns></returns>
        public static ReferencedNWBEncoder CreateBinary(BasicRouterDataSource<LiveEdge> graph)
        {
            return ReferencedNWBEncoder.Create(graph, new OpenLR.Binary.BinaryEncoder());
        }

        /// <summary>
        /// Creates a new referenced NWB encoder.
        /// </summary>
        /// <param name="graph">The graph containing the NWB network.</param>
        /// <returns></returns>
        public static ReferencedNWBEncoder CreateBinary(IBasicRouterDataSource<LiveEdge> graph)
        {
            return ReferencedNWBEncoder.CreateBinary(new BasicRouterDataSource<LiveEdge>(graph));
        }

        /// <summary>
        /// Creates a new referenced NWB encoder.
        /// </summary>
        /// <param name="folder">The folder containing the shapefile(s).</param>
        /// <param name="searchPattern">The search pattern to identify the relevant shapefiles.</param>
        /// <param name="rawLocationEncoder">The raw location encoder.</param>
        /// <returns></returns>
        public static ReferencedNWBEncoder Create(string folder, string searchPattern, Encoder rawLocationEncoder)
        {
            // create an instance of the graph reader and define the columns that contain the 'node-ids'.
            var graphReader = new ShapefileLiveGraphReader("JTE_ID_BEG", "JTE_ID_END");
            // read the graph from the folder where the shapefiles are placed.
            var graph = graphReader.Read(folder, searchPattern, new ShapefileRoutingInterpreter());

            return ReferencedNWBEncoder.Create(new BasicRouterDataSource<LiveEdge>(graph), rawLocationEncoder);
        }

        /// <summary>
        /// Creates a new referenced NWB encoder.
        /// </summary>
        /// <param name="graph">The graph containing the NWB network.</param>
        /// <param name="rawLocationEncoder">The raw location encoder.</param>
        /// <returns></returns>
        public static ReferencedNWBEncoder Create(BasicRouterDataSource<LiveEdge> graph, Encoder rawLocationEncoder)
        {
            return new ReferencedNWBEncoder(graph, rawLocationEncoder);
        }

        #endregion
    }
}