using OpenLR.Binary.Data;
using OpenLR.Locations;

namespace OpenLR.Binary.Decoders
{
    /// <summary>
    /// A decoder that decodes binary data into a rectangle location.
    /// </summary>
    public class RectangleLocationDecoder : BinaryDecoder<RectangleLocation>
    {
        /// <summary>
        /// Decodes the given data into a location reference.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override RectangleLocation Decode(byte[] data)
        {
            var rectangleLocation = new RectangleLocation();
            rectangleLocation.LowerLeft = CoordinateConverter.Decode(data, 1);
            rectangleLocation.UpperRight = CoordinateConverter.DecodeRelative(rectangleLocation.LowerLeft, data, 7);
            return rectangleLocation;
        }
    }
}