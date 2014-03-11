using OpenLR.Binary.Data;
using OpenLR.Locations;

namespace OpenLR.Binary.Decoders
{
    /// <summary>
    /// A decoder that decodes binary data into a geo coordinate.
    /// </summary>
    public class GeoCoordinateDecoder : BinaryDecoder
    {
        /// <summary>
        /// Decodes the given data into a location reference.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override ILocation Decode(byte[] data)
        {
            var geoCoordinate = new GeoCoordinateLocation();
            geoCoordinate.Coordinate = CoordinateConverter.Decode(data, 1);
            return geoCoordinate;
        }
    }
}