using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenLR.Referenced.Decoding
{
    /// <summary>
    /// An interface to a referenced decoder.
    /// </summary>
    public interface IReferencedDecoder
    {
        ReferencedLocation DecodeBinary(string base64String);
    }
}