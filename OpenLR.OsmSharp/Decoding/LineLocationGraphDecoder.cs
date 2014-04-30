using OpenLR.Locations;
using OpenLR.Model;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Graph;
using OsmSharp.Units.Distance;
using System.Collections.Generic;

namespace OpenLR.OsmSharp.Decoding
{
    /// <summary>
    /// Represents a referenced line location decoder.
    /// </summary>
    /// <typeparam name="TEdge"></typeparam>
    public class LineLocationGraphDecoder<TEdge> : GraphDecoder<LineLocationGraph<TEdge>, LineLocation, TEdge>
        where TEdge : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Holds the maximum vertex distance.
        /// </summary>
        private Meter _maxVertexDistance = 20;

        /// <summary>
        /// Creates a line location graph decoder.
        /// </summary>
        /// <param name="graph"></param>
        public LineLocationGraphDecoder(OpenLR.Decoding.Decoder rawDecoder, DynamicGraphRouterDataSource<TEdge> graph)
            : base(rawDecoder, graph)
        {

        }

        /// <summary>
        /// Decodes an OpenLR-encoded unreferenced raw OpenLR location into a referenced Location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override LineLocationGraph<TEdge> Decode(LineLocation location)
        {
            // get candidate vertices.
            var candidateLinks = new List<List<uint>>();
            candidateLinks.Add(this.FindCandidates(location.First.Coordinate));
            if(location.Intermediate != null)
            { // there are intermediates.
                for(int idx = 0; idx < location.Intermediate.Length; idx++)
                {
                    candidateLinks.Add(this.FindCandidates(location.Intermediate[idx].Coordinate));
                }
            }
            candidateLinks.Add(this.FindCandidates(location.Last.Coordinate));

            // get candidate edges.
            var candidateEdges = new List<List<TEdge>>();

            return null;
        }

        /// <summary>
        /// Returns candidate vertices.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        private List<uint> FindCandidates(Coordinate coordinate)
        {
            // create a search box.
            var box = new GeoCoordinateBox(
                new GeoCoordinate(coordinate.Latitude, coordinate.Longitude),
                new GeoCoordinate(coordinate.Latitude, coordinate.Longitude));
            box = box.Resize(0.1);

            // get arcs.
            var arcs = this.Graph.GetArcs(box);

            return null;
        }
    }
}