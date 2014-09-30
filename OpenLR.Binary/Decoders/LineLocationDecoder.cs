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
    /// A decoder that decodes binary data into a line location.
    /// </summary>
    public class LineLocationDecoder : BinaryLocationDecoder<LineLocation>
    {
        /// <summary>
        /// Decodes the given data into a location reference.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override LineLocation Decode(byte[] data)
        {
            // decode first location reference point.
            var first = new LocationReferencePoint();
            first.Coordinate = CoordinateConverter.Decode(data, 1);
            first.FuntionalRoadClass = FunctionalRoadClassConvertor.Decode(data, 7, 2);
            first.FormOfWay = FormOfWayConvertor.Decode(data, 7, 5);
            first.LowestFunctionalRoadClassToNext = FunctionalRoadClassConvertor.Decode(data, 8, 0);
            first.Bearing = BearingConvertor.DecodeAngleFromBearing(BearingConvertor.Decode(data, 8, 3));
            first.DistanceToNext = DistanceToNextConvertor.Decode(data[9]);

            // calculate the intermediate points count.
            var intermediateList = new List<LocationReferencePoint>();
            int intermediates = (data.Length - 16) / 7;
            int location = 10;
            var reference = first.Coordinate; // the reference for the relative coordinates.
            for(int idx = 0; idx < intermediates; idx++)
            {
                // create an intermediate point.
                var intermediate = new LocationReferencePoint();
                intermediate.Coordinate = CoordinateConverter.DecodeRelative(reference, data, location);
                reference = intermediate.Coordinate;
                location = location + 4;
                intermediate.FuntionalRoadClass = FunctionalRoadClassConvertor.Decode(data, location, 2);
                intermediate.FormOfWay = FormOfWayConvertor.Decode(data, location, 5);
                location = location + 1;
                intermediate.Bearing = BearingConvertor.DecodeAngleFromBearing(BearingConvertor.Decode(data, location, 3));
                intermediate.LowestFunctionalRoadClassToNext = FunctionalRoadClassConvertor.Decode(data, location, 0);
                location = location + 1;
                intermediate.DistanceToNext = DistanceToNextConvertor.Decode(data[location]);
                location = location + 1;

                intermediateList.Add(intermediate);
            }

            // decode last location reference point.
            var last = new LocationReferencePoint();
            last.Coordinate = CoordinateConverter.DecodeRelative(reference, data, location);
            location = location + 4;
            last.FuntionalRoadClass = FunctionalRoadClassConvertor.Decode(data, location, 2);
            last.FormOfWay = FormOfWayConvertor.Decode(data, location, 5);
            location = location + 1;
            last.Bearing = BearingConvertor.DecodeAngleFromBearing(BearingConvertor.Decode(data, location, 3));
            location = location + 1;                

            // create line location.
            var lineLocation = new LineLocation();
            if (location < data.Length)
            { // if present.
                lineLocation.PositiveOffset = data[location];
                location = location + 1;
            }
            if(location < data.Length)
            { // if present.
                lineLocation.NegativeOffset = data[location];
                location = location + 1;
            }
            lineLocation.First = first;
            lineLocation.Intermediate = intermediateList.ToArray();
            lineLocation.Last = last;
            return lineLocation;
        }

        /// <summary>
        /// Returns true if the given data can be decoded but this decoder.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool CanDecode(byte[] data)
        {
            if(data != null)
            {
                // decode the header first.
                var header = HeaderConvertor.Decode(data, 0);

                // check header info.
                if(header.ArF1 ||
                    header.IsPoint ||
                    header.ArF0 ||
                    !header.HasAttributes)
                { // header is incorrect.
                    return false;
                }

                //// check size.
                //int count = data.Length - 16;
                //int mod7 = count % 7;
                //return mod7 == 0 || mod7 == 1 || mod7 == 2;
                return true;
            }
            return false;
        }
    }
}