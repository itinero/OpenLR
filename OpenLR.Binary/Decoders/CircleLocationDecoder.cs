using OpenLR.Binary.Data;
using OpenLR.Locations;

namespace OpenLR.Binary.Decoders
{
    /// <summary>
    /// A decoder that decodes binary data into a circle location.
    /// </summary>
    public class CircleLocationDecoder : BinaryLocationDecoder<CircleLocation>
    {
        /// <summary>
        /// Decodes the given data into a location reference.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override CircleLocation Decode(byte[] data)
        {
            var circleLocation = new CircleLocation();
            circleLocation.Coordinate = CoordinateConverter.Decode(data, 1);
            circleLocation.Radius = data[7];
            return circleLocation;
        }

        /// <summary>
        /// Returns true if the given data can be decoded by this decoder.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool CanDecode(byte[] data)
        {
            // decode the header first.
            var header = HeaderConvertor.Decode(data, 0);

            // check header info.
            if (header.ArF1 ||
                header.IsPoint ||
                header.ArF0 ||
                header.HasAttributes)
            { // header is incorrect.
                return false;
            }

            return data != null && (data.Length == 8 || data.Length == 9 || data.Length == 10 || data.Length == 11);
        }
    }
}