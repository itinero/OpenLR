using OpenLR.Decoding;
using OpenLR.Model;
using OpenLR.Referenced.Decoding.Candidates;
using OpenLR.Referenced.Router;
using OpenLR.Referenced.Scoring;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Units.Angle;
using OsmSharp.Units.Distance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenLR.Referenced
{
    /// <summary>
    /// A reference decoder base class for live edges.
    /// </summary>
    public abstract class ReferencedDecoderBaseLiveEdge : ReferencedDecoderBase
    {
        /// <summary>
        /// Creates a new referenced live edge decoder.
        /// </summary>
        public ReferencedDecoderBaseLiveEdge(BasicRouterDataSource<LiveEdge> graph, Vehicle vehicle, Decoder locationDecoder)
            : base(graph, vehicle, locationDecoder)
        {

        }

        /// <summary>
        /// Creates a new referenced live edge decoder.
        /// </summary>
        public ReferencedDecoderBaseLiveEdge(BasicRouterDataSource<LiveEdge> graph, Vehicle vehicle, Decoder locationDecoder, Meter maxVertexDistance,
            float candidateSearchBoxSize)
            : base(graph, vehicle, locationDecoder, maxVertexDistance, candidateSearchBoxSize)
        {

        }
    }
}