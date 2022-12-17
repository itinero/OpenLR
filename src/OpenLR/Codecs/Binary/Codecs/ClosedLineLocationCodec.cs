using OpenLR.Codecs.Binary.Data;
using OpenLR.Model;
using OpenLR.Model.Locations;
using System.Collections.Generic;
using System;

namespace OpenLR.Codecs.Binary.Codecs;

/// <summary>
/// A decoder that decodes binary data into a circle location.
/// </summary>
public class ClosedLineLocationCodec
{
    /// <summary>
    /// Decodes the given data into a location reference.
    /// </summary>
    public static ClosedLineLocation Decode(byte[] data)
    {
        // decode first location reference point.
        var first = new LocationReferencePoint
        {
            Coordinate = CoordinateConverter.Decode(data, 1), 
            FuntionalRoadClass = FunctionalRoadClassConvertor.Decode(data, 7, 2),
            FormOfWay = FormOfWayConvertor.Decode(data, 7, 5),
            LowestFunctionalRoadClassToNext = FunctionalRoadClassConvertor.Decode(data, 8, 0),
            Bearing = BearingConvertor.DecodeAngleFromBearing(BearingConvertor.Decode(data, 8, 3)),
            DistanceToNext = DistanceToNextConvertor.Decode(data[9])
        };

        // calculate the intermediate points count.
        var intermediateList = new List<LocationReferencePoint>();
        int intermediates = (data.Length - 12) / 7;
        int location = 10;
        var reference = first.Coordinate; // the reference for the relative coordinates.
        for (int idx = 0; idx < intermediates; idx++)
        {
            // create an intermediate point.
            var intermediate = new LocationReferencePoint { Coordinate = CoordinateConverter.DecodeRelative(reference, data, location) };
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
        var last = new LocationReferencePoint
        {
            // no last coordinates, identical to the first.
            Coordinate = first.Coordinate,
            FuntionalRoadClass = FunctionalRoadClassConvertor.Decode(data, location, 2),
            FormOfWay = FormOfWayConvertor.Decode(data, location, 5)
        };
        location = location + 1;
        last.LowestFunctionalRoadClassToNext = FunctionalRoadClassConvertor.Decode(data, location, 0);
        last.Bearing = BearingConvertor.DecodeAngleFromBearing(BearingConvertor.Decode(data, location, 3));
        location = location + 1;

        // create line location.
        var lineLocation = new ClosedLineLocation
        {
            First = first, 
            Intermediate = intermediateList.ToArray(), 
            Last = last
        };
        return lineLocation;
    }

    /// <summary>
    /// Returns true if the given data can be decoded by this decoder.
    /// </summary>
    public static bool CanDecode(byte[] data)
    {
        // decode the header first.
        var header = HeaderConvertor.Decode(data, 0);

        // check header info.
        if (!header.ArF1 ||
            header.IsPoint ||
            !header.ArF0 ||
            !header.HasAttributes)
        { // header is incorrect.
            return false;
        }
        return true;
    }
}
