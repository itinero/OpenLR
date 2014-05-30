using OpenLR.Referenced;
using OpenLR.Referenced.Decoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenLR
{
    /// <summary>
    /// A facade class to all functionality of the OpenLR decoding/encoding.
    /// </summary>
    public static class Facade
    {
        /// <summary>
        /// Holds a list of a referenced decoder.
        /// </summary>
        private static List<IReferencedDecoder> _decoders = new List<IReferencedDecoder>();

        /// <summary>
        /// Registers a new decoder.
        /// </summary>
        /// <param name="decoder"></param>
        public static void RegisterDecoder(IReferencedDecoder decoder)
        {
            _decoders.Add(decoder);
        }

        /// <summary>
        /// Clears all decoders.
        /// </summary>
        public static void ClearDecoders()
        {
            _decoders.Clear();
        }

        /// <summary>
        /// Decodes a base 64 string into a referenced location.
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static ReferencedLocation DecodeBinary(string base64)
        {
            foreach(var decoder in _decoders)
            {
                if(decoder.CanDecode(base64))
                {
                    return decoder.Decode(base64);
                }
            }
            throw new ArgumentOutOfRangeException("Could not find a decoder to decode: {0}", base64);
        }
    }
}