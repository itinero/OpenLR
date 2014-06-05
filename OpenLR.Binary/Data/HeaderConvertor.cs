namespace OpenLR.Binary.Data
{
    /// <summary>
    /// Represents a header convertor that encodes/decodes header information from/to the binary OpenLR format.
    /// </summary>
    public static class HeaderConvertor
    {
        /// <summary>
        /// Decodes a byte from the given data array into header information.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static Header Decode(byte[] data, int startIndex)
        {
            return HeaderConvertor.Decode(data[startIndex]);
        }

        /// <summary>
        /// Decodes the given byte into header information.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static Header Decode(byte data)
        {
            return new Header()
            {
                ArF1 = (data & (1 << 6 - 1)) != 0,
                IsPoint = (data & (1 << 5 - 1)) != 0,
                ArF0 = (data & (1 << 4 - 1)) != 0,
                HasAttributes = (data & (1 << 3 - 1)) != 0,
                Version = HeaderConvertor.DecodeVersion(data)
            };
        }

        /// <summary>
        /// Decodes the version information from the binary header.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static ushort DecodeVersion(byte data)
        {
            return (ushort)(data >> 5);
        }
    }

    /// <summary>
    /// Represents an OpenLR binary header.
    /// </summary>
    public class Header
    {
        /// <summary>
        /// Gets or sets ArF1 status bit.
        /// </summary>
        public bool ArF1 { get; set; }

        /// <summary>
        /// Gets or sets the IsPoint status bit.
        /// </summary>
        public bool IsPoint { get; set; }

        /// <summary>
        /// Gets or sets ArF0 status bit.
        /// </summary>
        public bool ArF0 { get; set; }

        /// <summary>
        /// Gets or sets the has attributes status bit.
        /// </summary>
        public bool HasAttributes { get; set; }

        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        public ushort Version { get; set; }
    }
}