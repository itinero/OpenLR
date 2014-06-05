using OpenLR.Binary.Data;
using OpenLR.Locations;
using OpenLR.Model;

namespace OpenLR.Binary.Decoders
{
    /// <summary>
    /// A decoder that decodes binary data into a point along line.
    /// </summary>
    public class PointAlongLineDecoder : BinaryLocationDecoder<PointAlongLineLocation>
    {
        /// <summary>
        /// Decodes the given data into a location reference.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override PointAlongLineLocation Decode(byte[] data)
        {
            var pointAlongLine = new PointAlongLineLocation();

            // decode first location reference point.
            var first = new LocationReferencePoint();
            first.Coordinate = CoordinateConverter.Decode(data, 1);
            first.FuntionalRoadClass = FunctionalRoadClassConvertor.Decode(data, 7, 2);
            first.FormOfWay = FormOfWayConvertor.Decode(data, 7, 5);
            first.LowestFunctionalRoadClassToNext = FunctionalRoadClassConvertor.Decode(data, 8, 0);
            first.BearingDistance = BearingConvertor.Decode(data, 8, 3);

            // decode second location reference point.
            var last = new LocationReferencePoint();
            last.Coordinate = CoordinateConverter.DecodeRelative(first.Coordinate, data, 10);
            last.FuntionalRoadClass = FunctionalRoadClassConvertor.Decode(data, 14, 2);
            last.FormOfWay = FormOfWayConvertor.Decode(data, 14, 5);
            last.BearingDistance = BearingConvertor.Decode(data, 15, 3);

            pointAlongLine.First = first;
            pointAlongLine.Orientation = OrientationConverter.Decode(data, 7, 0);
            pointAlongLine.SideOfRoad = SideOfRoadConverter.Decode(data, 14, 0);
            pointAlongLine.PositiveOffset = data[16];
            pointAlongLine.Last = last;

            return pointAlongLine;
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
            if (!header.ArF1 &&
                header.IsPoint &&
                !header.ArF0 &&
                header.HasAttributes)
            { // header is incorrect.
                return false;
            }

            return data != null && (data.Length == 16 || data.Length == 17);
        }
    }
}