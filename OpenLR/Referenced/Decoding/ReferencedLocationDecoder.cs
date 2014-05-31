using OpenLR.Decoding;
using OpenLR.Locations;

namespace OpenLR.Referenced.Decoding
{
    /// <summary>
    /// Decodes an OpenLR-encoded location into a Location: References an OpenLR location and decodes this into a Location with a depency on the routing network.
    /// </summary>
    public abstract class ReferencedLocationDecoder<TReferencedLocation, TLocation>
        where TReferencedLocation : ReferencedLocation
        where TLocation : ILocation
    {
        /// <summary>
        /// Holds the decoder to decode the raw OpenLR-data.
        /// </summary>
        private OpenLR.Decoding.LocationDecoder<TLocation> _rawDecoder;

        /// <summary>
        /// Creates a new referenced location decoder.
        /// </summary>
        /// <param name="rawDecoder"></param>
        public ReferencedLocationDecoder(OpenLR.Decoding.LocationDecoder<TLocation> rawDecoder)
        {
            _rawDecoder = rawDecoder;
        }

        /// <summary>
        /// Returns true if the given data can be decoded using this decoder.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool CanDecode(string data)
        {
            return _rawDecoder.CanDecode(data);
        }

        /// <summary>
        /// Decodes an encoded OpenLR location into a referenced Location.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public TReferencedLocation Decode(string data)
        {
            return this.Decode(_rawDecoder.Decode(data));
        }

        /// <summary>
        /// Decodes an unreferenced raw OpenLR location into a referenced Location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public abstract TReferencedLocation Decode(TLocation location);
    }
}