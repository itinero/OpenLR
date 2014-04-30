using OpenLR.Locations;

namespace OpenLR.Referenced.Decoding
{
    /// <summary>
    /// Decodes an OpenLR-encoded location into a Location: References an OpenLR location and decodes this into a Location with a depency on the routing network.
    /// </summary>
    public abstract class ReferencedDecoder<TReferencedLocation, TLocation>
        where TReferencedLocation : ReferencedLocation
        where TLocation : ILocation
    {
        /// <summary>
        /// Holds the decoder to decode the raw OpenLR-data.
        /// </summary>
        private OpenLR.Decoding.Decoder _rawDecoder;

        /// <summary>
        /// Creates a new 
        /// </summary>
        /// <param name="rawDecoder"></param>
        public ReferencedDecoder(OpenLR.Decoding.Decoder rawDecoder)
        {
            _rawDecoder = rawDecoder;
        }

        /// <summary>
        /// Decodes an unreferenced raw OpenLR location into a referenced Location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public abstract TReferencedLocation Decode(TLocation location);
    }
}