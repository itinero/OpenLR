using OpenLR.Locations;
using OpenLR.Model;
using OpenLR.Referenced.Decoding.Candidates;
using OpenLR.Referenced.Locations;
using OpenLR.Referenced;
using OpenLR.Referenced.Decoding;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.Graph;
using OsmSharp.Units.Angle;
using OsmSharp.Units.Distance;
using System.Collections.Generic;
using OsmSharp.Routing.Osm.Graphs;

namespace OpenLR.Referenced.Decoding
{
    /// <summary>
    /// Represents a dynamic graph decoder: Decodes a raw OpenLR location into a location referenced to a dynamic graph.
    /// </summary>
    public abstract class ReferencedDecoder<TReferencedLocation, TLocation> : ReferencedLocationDecoder<TReferencedLocation, TLocation>
        where TReferencedLocation : ReferencedLocation
        where TLocation : ILocation
    {
        private readonly ReferencedDecoderBase _mainDecoder;

        /// <summary>
        /// Creates a new dynamic graph decoder.
        /// </summary>
        /// <param name="mainDecoder"></param>
        /// <param name="rawDecoder"></param>
        public ReferencedDecoder(ReferencedDecoderBase mainDecoder, OpenLR.Decoding.LocationDecoder<TLocation> rawDecoder)
            : base(rawDecoder)
        {
            _mainDecoder = mainDecoder;
        }

        /// <summary>
        /// Gets the main decoder.
        /// </summary>
        public ReferencedDecoderBase MainDecoder
        {
            get
            {
                return _mainDecoder;
            }
        }

        /// <summary>
        /// Decodes an OpenLR-encoded unreferenced raw OpenLR location into a referenced Location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override abstract TReferencedLocation Decode(TLocation location);

        /// <summary>
        /// Returns the max vertex distance.
        /// </summary>
        protected Meter MaxVertexDistance
        {
            get
            {
                return _mainDecoder.MaxVertexDistance;
            }
        }
    }
}