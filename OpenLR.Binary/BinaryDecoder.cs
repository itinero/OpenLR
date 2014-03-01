using System;

namespace OpenLR.Binary
{
    /// <summary>
    /// Abstract representation of a binary decoder.
    /// </summary>
    public abstract class BinaryDecoder : OpenLR.Decoder.Decoder
    {
        /// <summary>
        /// Decodes the given data into a location reference.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override ILocationReference Decode(string data)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            // the data in a binary decoder should be a base64 string.
            byte[] binaryData = null;
            try
            {
                binaryData = Convert.FromBase64String(data);
            }
            catch(FormatException ex)
            { // not a base64 string.
                throw ex;
            }

            // we have binary data now.
            return this.Decode(binaryData);
        }

        /// <summary>
        /// Decodes the given data into a location reference.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract ILocationReference Decode(byte[] data);
    }
}