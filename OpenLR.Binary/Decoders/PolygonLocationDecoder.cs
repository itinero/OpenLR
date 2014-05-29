using OpenLR.Binary.Data;
using OpenLR.Locations;
using OpenLR.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenLR.Binary.Decoders
{
    /// <summary>
    /// A decoder that decodes binary data into a polygon location.
    /// </summary>
    public class PolygonLocationDecoder : BinaryDecoder<PolygonLocation>
    {
        /// <summary>
        /// Decodes the given data into a location reference.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override PolygonLocation Decode(byte[] data)
        {
            // just need to decode list of coordinate.
            var coordinates = new List<Coordinate>();
            coordinates.Add(CoordinateConverter.Decode(data, 1));

            // calculate the number of points.
            var previous = coordinates[0];
            var location = 7;
            int points = 1 + (data.Length - 6) / 4;
            for (int idx = 0; idx < points - 1; idx++)
            {
                coordinates.Add(CoordinateConverter.DecodeRelative(
                    coordinates[coordinates.Count - 1], data, location + (idx * 4)));
            }

            var polygonLocation = new PolygonLocation();
            polygonLocation.Coordinates = coordinates.ToArray();
            return polygonLocation;
        }
    }
}
