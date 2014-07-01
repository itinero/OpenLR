using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenLR.Binary.Data
{
    /// <summary>
    /// Represents a distance to next convertor that encodes/decodes coordinates into the binary OpenLR format.
    /// </summary>
    public static class DistanceToNextConvertor
    {
        /// <summary>
        /// Holds the distance per interval for 256 intervals in 15000m 
        /// </summary>
        private const double DISTANCE_PER_INTERVAL = 58.6;

        /// <summary>
        /// Encodes the distance into a byte.
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static byte Encode(int distance)
        {
            if (distance < 0 || distance >= 15000) { throw new ArgumentOutOfRangeException("distance", "Distance cannot be encoded if not in the range [0-15000["); }

            return (byte)System.Math.Floor(distance / DISTANCE_PER_INTERVAL);
        }

        /// <summary>
        /// Decodes the distance from a byte.
        /// </summary>
        /// <param name="distanceByte"></param>
        /// <returns></returns>
        public static int Decode(byte distanceByte)
        {
            return (int)System.Math.Ceiling(distanceByte * DISTANCE_PER_INTERVAL);
        }
    }
}