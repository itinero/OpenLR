using OpenLR.Binary.Data;
using OpenLR.Locations;
using OpenLR.Model;

namespace OpenLR.Binary.Decoders
{
    /// <summary>
    /// A decoder that decodes binary data into a point along line.
    /// </summary>
    public class PointAlongLineDecoder : BinaryDecoder<PointAlongLineLocation>
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
    }
}