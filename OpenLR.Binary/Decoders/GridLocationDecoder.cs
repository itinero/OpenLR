using OpenLR.Binary.Data;
using OpenLR.Locations;

namespace OpenLR.Binary.Decoders
{
    /// <summary>
    /// A decoder that decodes binary data into a grid location.
    /// </summary>
    public class GridLocationDecoder : BinaryDecoder<GridLocation>
    {
        /// <summary>
        /// Decodes the given data into a location reference.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override GridLocation Decode(byte[] data)
        {
            // decode box.
            var lowerLeft = CoordinateConverter.Decode(data, 1);
            var upperRight = CoordinateConverter.DecodeRelative(lowerLeft, data, 7);
            
            // decode column/row info.
            var columns = data[11] * 256 + data[12];
            var rows = data[13] * 256 + data[14];

            // create grid location.
            var grid = new GridLocation();
            grid.LowerLeft = lowerLeft;
            grid.UpperRight = upperRight;
            grid.Columns = columns;
            grid.Rows = rows;
            return grid;
        }

        /// <summary>
        /// Returns true if the given data can be decoded by this decoder.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool CanDecode(byte[] data)
        {
            return data != null && (data.Length == 15 || data.Length == 17);
        }
    }
}