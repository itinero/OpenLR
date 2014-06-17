using OpenLR.Binary.Data;
using OpenLR.Encoding;
using OpenLR.Locations;

namespace OpenLR.Binary.Encoders
{
    /// <summary>
    /// An encoder that decodes binary data into a point along line.
    /// </summary>
    public class PointAlongLineEncoder : BinaryLocationEncoder<PointAlongLineLocation>
    {
        /// <summary>
        /// Encodes a point along line location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        protected override byte[] EncodeByteArray(PointAlongLineLocation location)
        {
            byte[] data = new byte[17];

            CoordinateConverter.Encode(location.First.Coordinate, data, 1);
            FunctionalRoadClassConvertor.Encode(location.First.FuntionalRoadClass.Value, data, 7, 2);
            FormOfWayConvertor.Encode(location.First.FormOfWay.Value, data, 7, 5);
            FunctionalRoadClassConvertor.Encode(location.First.LowestFunctionalRoadClassToNext.Value, data, 8, 0);
            BearingConvertor.Encode(location.First.BearingDistance.Value, data, 8, 3);

            CoordinateConverter.EncodeRelative(location.First.Coordinate, location.Last.Coordinate, data, 10);
            FunctionalRoadClassConvertor.Encode(location.Last.FuntionalRoadClass.Value, data, 14, 2);
            FormOfWayConvertor.Encode(location.Last.FormOfWay.Value, data, 14, 5);
            BearingConvertor.Encode(location.Last.BearingDistance.Value, data, 15, 3);

            OrientationConverter.Encode(location.Orientation.Value, data, 7, 0);
            SideOfRoadConverter.Encode(location.SideOfRoad.Value, data, 14, 0);
            data[16] = (byte)(location.PositiveOffset.Value);

            return data;
        }
    }
}