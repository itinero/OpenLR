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
        /// Decodes binary OpenLR SideOfRoad data into an SideOfRoad.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="byteIndex"></param>
        /// <returns></returns>
        public static SideOfRoad Decode(byte[] data, int byteIndex)
        {
            return SideOfRoadConverter.Decode(data, 0, byteIndex);
        }

        /// <summary>
        /// Decodes binary OpenLR SideOfRoad data into an SideOfRoad.
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

        /// <summary>
        /// Encodes OpenLR SideOfRoad data into a binary SideOfRoad.
        /// </summary>
        /// <param name="sideOfRoad"></param>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="byteIndex"></param>
        public static void Encode(SideOfRoad sideOfRoad, byte[] data, int startIndex, int byteIndex)
        {
            if (byteIndex > 6) { throw new ArgumentOutOfRangeException("byteIndex", "byteIndex has to be a value in the range of [0-6]."); }

            int value = 0;
            switch(sideOfRoad)
            {
                case SideOfRoad.OnOrAbove:
                    value = 0;
                    break;
                case SideOfRoad.Right:
                    value = 1;
                    break;
                case SideOfRoad.Left:
                    value = 2;
                    break;
                case SideOfRoad.Both:
                    value = 3;
                    break;
            }

            byte target = data[startIndex];

            byte mask = (byte)(3 << (6 - byteIndex));
            target = (byte)(target & ~mask); // set to zero.
            value = (byte)(value << (6 - byteIndex)); // move value to correct position.
            target = (byte)(target | value); // add to byte.

            data[startIndex] = target;
        }
    }
}