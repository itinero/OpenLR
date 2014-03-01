namespace OpenLR.Decoder
{
    /// <summary>
    /// Abstract representation of a decoder.
    /// </summary>
    public abstract class Decoder
    {
        /// <summary>
        /// Decodes a byte array into a location reference.
        /// </summary>
        /// <returns></returns>
        public abstract ILocationReference Decode(string data);
    }
}