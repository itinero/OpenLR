using OpenLR.Binary.Data;
using OpenLR.Encoding;
using OpenLR.Locations;

namespace OpenLR.Binary.Encoders
{
    /// <summary>
    /// An encoder that decodes binary data into a line.
    /// </summary>
    public class LineEncoder : BinaryLocationEncoder<LineLocation>
    {
        /// <summary>
        /// Encodes a point along line location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        protected override byte[] EncodeByteArray(LineLocation location)
        {
            byte[] data = new byte[17];

            var header = new Header();
            header.Version = 3;
            header.HasAttributes = true;
            header.ArF0 = false;
            header.IsPoint = true;
            header.ArF1 = false;
            HeaderConvertor.Encode(data, 0, header);
            CoordinateConverter.Encode(location.First.Coordinate, data, 1);
            FunctionalRoadClassConvertor.Encode(location.First.FuntionalRoadClass.Value, data, 7, 2);
            FormOfWayConvertor.Encode(location.First.FormOfWay.Value, data, 7, 5);
            FunctionalRoadClassConvertor.Encode(location.First.LowestFunctionalRoadClassToNext.Value, data, 8, 0);
            BearingConvertor.Encode(BearingConvertor.EncodeAngleToBearing(location.First.Bearing.Value), data, 8, 3);
            data[9] = DistanceToNextConvertor.Encode(location.First.DistanceToNext);

            CoordinateConverter.EncodeRelative(location.First.Coordinate, location.Last.Coordinate, data, 10);
            FunctionalRoadClassConvertor.Encode(location.Last.FuntionalRoadClass.Value, data, 14, 2);
            FormOfWayConvertor.Encode(location.Last.FormOfWay.Value, data, 14, 5);
            BearingConvertor.Encode(BearingConvertor.EncodeAngleToBearing(location.Last.Bearing.Value), data, 15, 3);

            return data;
        }
    }
}