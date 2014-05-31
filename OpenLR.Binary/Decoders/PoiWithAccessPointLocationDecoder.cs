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
    /// A decoder that decodes binary data into a circle location.
    /// </summary>
    public class PoiWithAccessPointLocationDecoder : BinaryLocationDecoder<PoiWithAccessPointLocation>
    {
        /// <summary>
        /// Decodes the given data into a location reference.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override PoiWithAccessPointLocation Decode(byte[] data)
        {
            // decode first location reference point.
            var first = new LocationReferencePoint();
            first.Coordinate = CoordinateConverter.Decode(data, 1);
            var orientation = OrientationConverter.Decode(data, 7, 0);
            first.FuntionalRoadClass = FunctionalRoadClassConvertor.Decode(data, 7, 2);
            first.FormOfWay = FormOfWayConvertor.Decode(data, 7, 5);
            first.LowestFunctionalRoadClassToNext = FunctionalRoadClassConvertor.Decode(data, 8, 0);
            first.BearingDistance = BearingConvertor.Decode(data, 8, 3);
            first.DistanceToNext = data[9];

            // decode last location reference point.
            var last = new LocationReferencePoint();
            // no last coordinates, identical to the first.
            last.Coordinate = CoordinateConverter.DecodeRelative(first.Coordinate, data, 10);
            var sideOfRoad = SideOfRoadConverter.Decode(data, 14, 0);
            last.FuntionalRoadClass = FunctionalRoadClassConvertor.Decode(data, 14, 2);
            last.FormOfWay = FormOfWayConvertor.Decode(data, 14, 5);
            last.BearingDistance = BearingConvertor.Decode(data, 15, 3);

            // poi details.
            var coordinate = CoordinateConverter.DecodeRelative(first.Coordinate, data, 17);

            // create line location.
            var poiWithAccessPointLocation = new PoiWithAccessPointLocation();
            poiWithAccessPointLocation.First = first;
            poiWithAccessPointLocation.Last = last;
            poiWithAccessPointLocation.Coordinate = coordinate;
            poiWithAccessPointLocation.Orientation = orientation;
            poiWithAccessPointLocation.PositiveOffset = null;
            poiWithAccessPointLocation.SideOfRoad = sideOfRoad;
            return poiWithAccessPointLocation;
        }

        /// <summary>
        /// Returns true if the given data can be decoded by this decoder.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool CanDecode(byte[] data)
        {
            return data != null && (data.Length == 20 || data.Length == 21);
        }
    }
}