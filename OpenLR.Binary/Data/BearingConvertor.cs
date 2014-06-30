using OpenLR.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenLR.Binary.Data
{
    /// <summary>
    /// Represents a bearing convertor that encodes/decodes into the binary OpenLR format.
    /// </summary>
    public static class BearingConvertor
    {
        /// <summary>
        /// Decodes a bearing from binary data.
        /// </summary>
        /// <param name="data">The binary data.</param>
        /// <param name="byteIndex">The index of the data in the first byte.</param>
        /// <returns></returns>
        public static int Decode(byte[] data, int byteIndex)
        {
            return BearingConvertor.Decode(data, 0, byteIndex);
        }

        /// <summary>
        /// Decodes a bearing from binary data.
        /// </summary>
        /// <param name="data">The binary data.</param>
        /// <param name="startIndex">The index of the byte in data.</param>
        /// <param name="byteIndex">The index of the data in the given byte.</param>
        public static int Decode(byte[] data, int startIndex, int byteIndex)
        {
            if (byteIndex > 3) { throw new ArgumentOutOfRangeException("byteIndex", "byteIndex has to be a value in the range of [0-3]."); }

            byte classData = data[startIndex];

            // create mask.
            int mask = 31 << (3 - byteIndex);
            return (classData & mask) >> (3 - byteIndex);
        }

        /// <summary>
        /// Encodes a bearing into binary data.
        /// </summary>
        /// <param name="bearing"></param>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="byteIndex"></param>
        public static void Encode(int bearing, byte[] data, int startIndex, int byteIndex)
        {
            if (byteIndex > 3) { throw new ArgumentOutOfRangeException("byteIndex", "byteIndex has to be a value in the range of [0-3]."); }

            byte target = data[startIndex];

            byte mask = (byte)(31 << (3 - byteIndex));
            target = (byte)(target & ~mask); // set to zero.
            byte value = (byte)(bearing << (3 - byteIndex)); // move value to correct position.
            target = (byte)(target | value); // add to byte.

            data[startIndex] = target;
        }

        /// <summary>
        /// Holds the degrees per sector for the bearing calculation.
        /// </summary>
        private const double DEGREES_PER_SECTOR = 360.0 / 32;

        /// <summary>
        /// Encodes an angle into a bearing.
        /// </summary>
        /// <param name="angleInDegrees"></param>
        /// <returns></returns>
        /// <remarks>7.3.3 in OpenLR whitepaper.</remarks>
        public static int EncodeAngleToBearing(double angleInDegrees)
        {
            if (angleInDegrees < 0) { throw new ArgumentOutOfRangeException("angleInDegrees", "Angle needs to be in the range of [0-360["); }
            if (angleInDegrees >= 360) { throw new ArgumentOutOfRangeException("angleInDegrees", "Angle needs to be in the range of [0-360["); }

            return (int)(angleInDegrees / DEGREES_PER_SECTOR);
        }

        /// <summary>
        /// Decodes an angle from a bearing.
        /// </summary>
        /// <param name="bearing"></param>
        /// <returns>The angle represented by the bearing in the range [0-360[.</returns>
        public static double DecodeAngleFromBearing(int bearing)
        {
            if (bearing < 0) { throw new ArgumentOutOfRangeException("angleInDegrees", "Bearing needs to be in the range of [0-31]"); }
            if (bearing >= 32) { throw new ArgumentOutOfRangeException("angleInDegrees", "Bearing needs to be in the range of [0-31]"); }

            return bearing * DEGREES_PER_SECTOR;
        }
    }
}