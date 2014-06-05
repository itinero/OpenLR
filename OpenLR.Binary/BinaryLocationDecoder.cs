using OpenLR.Binary.Data;
using OpenLR.Locations;
using System;

namespace OpenLR.Binary
{
    /// <summary>
    /// Abstract representation of a binary decoder.
    /// </summary>
    public abstract class BinaryLocationDecoder<TLocation> : OpenLR.Decoding.LocationDecoder<TLocation>
        where TLocation : ILocation
    {
        /// <summary>
        /// Decodes the given data into a location reference.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override TLocation Decode(string data)
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
        protected abstract TLocation Decode(byte[] data);

        /// <summary>
        /// Returns true if the given data can be decoded using this decoder.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool CanDecode(string data)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            // the data in a binary decoder should be a base64 string.
            byte[] binaryData = null;
            try
            {
                binaryData = Convert.FromBase64String(data);
            }
            catch (FormatException ex)
            { // not a base64 string.
                throw ex;
            }

            // we have binary data now.
            return this.CanDecode(binaryData);
        }

        /// <summary>
        /// Returns true if the given data can be decoded using this decoder.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract bool CanDecode(byte[] data);
    }
}