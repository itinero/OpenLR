using OpenLR.Model;
using System;

namespace OpenLR.Binary.Data
{
    /// <summary>
    /// Represents a functional road class convertor that encodes/decodes into the binary OpenLR format.
    /// </summary>
    public static class FunctionalRoadClassConvertor
    {
        /// <summary>
        /// Decodes a functional road class from binary data.
        /// </summary>
        /// <param name="data">The binary data.</param>
        /// <param name="byteIndex">The index of the data in the first byte.</param>
        /// <returns></returns>
        public static FunctionalRoadClass Decode(byte[] data, int byteIndex)
        {
            return FunctionalRoadClassConvertor.Decode(data, 0, byteIndex);
        }

        /// <summary>
        /// Decodes a functional road class from binary data.
        /// </summary>
        /// <param name="data">The binary data.</param>
        /// <param name="startIndex">The index of the byte in data.</param>
        /// <param name="byteIndex">The index of the data in the given byte.</param>
        public static FunctionalRoadClass Decode(byte[] data, int startIndex, int byteIndex)
        {
            if (byteIndex > 5) { throw new ArgumentOutOfRangeException("byteIndex", "byteIndex has to be a value in the range of [0-5]."); }

            byte classData = data[startIndex];

            // create mask.
            int mask = 7 << (5 - byteIndex);
            int value = (classData & mask) >> (5 - byteIndex);

            switch(value)
            {
                case 0:
                    return FunctionalRoadClass.Frc0;
                case 1:
                    return FunctionalRoadClass.Frc1;
                case 2:
                    return FunctionalRoadClass.Frc2;
                case 3:
                    return FunctionalRoadClass.Frc3;
                case 4:
                    return FunctionalRoadClass.Frc4;
                case 5:
                    return FunctionalRoadClass.Frc5;
                case 6:
                    return FunctionalRoadClass.Frc6;
                case 7:
                    return FunctionalRoadClass.Frc7;
            }
            throw new InvalidOperationException("Decoded a value from three bits not in the range of [0-7]?!");
        }

        /// <summary>
        /// Encodes a functional road class into binary data.
        /// </summary>
        /// <param name="functionalRoadClass"></param>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="byteIndex"></param>
        public static void Encode(FunctionalRoadClass functionalRoadClass, byte[] data, int startIndex, int byteIndex)
        {
            if (byteIndex > 5) { throw new ArgumentOutOfRangeException("byteIndex", "byteIndex has to be a value in the range of [0-5]."); }

            byte value = 0;
            switch (functionalRoadClass)
            {
                case FunctionalRoadClass.Frc0:
                    value = 0;
                    break;
                case FunctionalRoadClass.Frc1:
                    value = 1;
                    break;
                case FunctionalRoadClass.Frc2:
                    value = 2;
                    break;
                case FunctionalRoadClass.Frc3:
                    value = 3;
                    break;
                case FunctionalRoadClass.Frc4:
                    value = 4;
                    break;
                case FunctionalRoadClass.Frc5:
                    value = 5;
                    break;
                case FunctionalRoadClass.Frc6:
                    value = 6;
                    break;
                case FunctionalRoadClass.Frc7:
                    value = 7;
                    break;
            }

            byte target = data[startIndex];

            byte mask = (byte)(7 << (5 - byteIndex));
            target = (byte)(target & ~mask); // set to zero.
            value = (byte)(value << (5 - byteIndex)); // move value to correct position.
            target = (byte)(target | value); // add to byte.

            data[startIndex] = target;
        }
    }
}