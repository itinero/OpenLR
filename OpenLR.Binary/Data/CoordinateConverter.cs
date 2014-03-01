using OsmSharp.Math.Geo;

namespace OpenLR.Binary.Data
{
    /// <summary>
    /// Represents a coordinate endoder that encoded coordinates into the binary OpenLR format.
    /// </summary>
    public static class CoordinateConverter
    {
        /// <summary>
        /// Decodes binary OpenLR coordinate data into a coordinate.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static GeoCoordinate Decode(byte[] data)
        {
            return new GeoCoordinate(
                CoordinateConverter.DecodeDegrees(CoordinateConverter.DecodeInt24(data, 3)),
                CoordinateConverter.DecodeDegrees(CoordinateConverter.DecodeInt24(data, 0)));
        }

        /// <summary>
        /// Decodes binary OpenLR relative coordinate data into a coordinate.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static GeoCoordinate DecodeRelative(GeoCoordinate reference, byte[] data)
        {
            return new GeoCoordinate(
                reference.Latitude + (CoordinateConverter.DecodeInt16(data, 2) / 100000.0),
                reference.Longitude + (CoordinateConverter.DecodeInt16(data, 0) / 100000.0));
        }

        /// <summary>
        /// Decodes a little-endian 24-bit signed integer from the given byte array into a 32-bit signed integer.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private static int DecodeInt24(byte[] data, int startIndex)
        {
            int result = ((byte)(data[startIndex + 0] & 127) * (1 << 16)) |    // Bottom 8 bits
                (data[startIndex + 1] * (1 << 8)) |    // Next 8 bits, i.e. multiply by 256
                (data[startIndex + 2] * (1 << 0));   // Next 8 bits, i.e. multiply by 65,536
            // take into account the sign-bit.
            if ((data[startIndex + 0] & (1 << 8 - 1)) != 0)
            { // negative!
                return -result;
            }
            return result;
        }

        /// <summary>
        /// Decodes a little-endian 16-bit signed integer from the given byte array into a 32-bit signed integer.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private static int DecodeInt16(byte[] data, int startIndex)
        {
            int result = (data[startIndex + 0] * (1 << 8)) |    // Bottom 8 bits
                (data[startIndex + 1] * (1 << 0));   // Next 8 bits, i.e. multiply by 65,536
            // take into account the sign-bit.
            if ((data[startIndex + 0] & (1 << 8 - 1)) != 0)
            { // negative!
                return result-65536;
            }
            return result;
        }

        /// <summary>
        /// Decodes a 24-bit integer-encoded coordinate.
        /// </summary>
        /// <param name="valueInt"></param>
        /// <returns></returns>
        private static double DecodeDegrees(int valueInt)
        {
            return (((valueInt - System.Math.Sign(valueInt) * 0.5) * 360) / 16777216);
        }
    }
}