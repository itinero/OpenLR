using OpenLR.Binary.Data;
using OpenLR.Locations;

namespace OpenLR.Binary.Decoders
{
    /// <summary>
    /// A decoder that decodes binary data into a circle location.
    /// </summary>
    public class CircleLocationDecoder : BinaryDecoder
    {
        /// <summary>
        /// Decodes the given data into a location reference.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override ILocation Decode(byte[] data)
        {
            var circleLocation = new CircleLocation();
            circleLocation.Coordinates = CoordinateConverter.Decode(data, 1);
            circleLocation.Radius = data[7];
            return circleLocation;
        }
    }
}