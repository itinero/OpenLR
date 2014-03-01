using OpenLR.Binary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenLR.Binary.Decoders
{
    /// <summary>
    /// A decoder that decodes binary data into a line location.
    /// </summary>
    public class LineLocationDecoder : BinaryDecoder
    {
        /// <summary>
        /// Decodes the given data into a location reference.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override ILocationReference Decode(byte[] data)
        {
            // decode first location reference point.
            var coordinate = CoordinateConverter.Decode(data, 1);

            return null;
        }
    }
}