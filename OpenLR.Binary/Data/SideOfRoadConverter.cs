using OpenLR.Model;
using System;

namespace OpenLR.Binary.Data
{
    /// <summary>
    /// Represents a side of road convertor that encodes/decodes side of road info into the binary OpenLR format.
    /// </summary>
    public static class SideOfRoadConverter
    {
        /// <summary>
        /// Decodes binary OpenLR orientation data into an orientation.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="byteIndex"></param>
        /// <returns></returns>
        public static SideOfRoad Decode(byte[] data, int byteIndex)
        {
            return SideOfRoadConverter.Decode(data, 0, byteIndex);
        }

        /// <summary>
        /// Decodes binary OpenLR orientation data into an orientation.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="byteIndex"></param>
        /// <returns></returns>
        public static SideOfRoad Decode(byte[] data, int startIndex, int byteIndex)
        {
            if (byteIndex > 6) { throw new ArgumentOutOfRangeException("byteIndex", "byteIndex has to be a value in the range of [0-6]."); }

            byte classData = data[startIndex];

            // create mask.
            int mask = 7 << (6 - byteIndex);
            int value = (classData & mask) >> (6 - byteIndex);

            switch (value)
            {
                case 0:
                    return SideOfRoad.OnOrAbove;
                case 1:
                    return SideOfRoad.Right;
                case 2:
                    return SideOfRoad.Left;
                case 3:
                    return SideOfRoad.Both;
            }
            throw new InvalidOperationException("Decoded a value from three bits not in the range of [0-3]?!");
        }
    }
}