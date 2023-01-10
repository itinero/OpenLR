using OpenLR.Codecs.Binary.Data;
using OpenLR.Model;
using OpenLR.Model.Locations;

namespace OpenLR.Codecs.Binary.Codecs;

/// <summary>
/// A decoder that decodes binary data into a point along line.
/// </summary>
public static class PointAlongLineLocationCodec
{
    /// <summary>§
    /// Decodes the given data into a location reference.
    /// </summary>
    public static PointAlongLineLocation Decode(byte[] data)
    {
        var pointAlongLine = new PointAlongLineLocation();

        // decode first location reference point.
        var first = new LocationReferencePoint
        {
            Coordinate = CoordinateConverter.Decode(data, 1), FunctionalRoadClass = FunctionalRoadClassConvertor.Decode(data, 7, 2),
            FormOfWay = FormOfWayConvertor.Decode(data, 7, 5),
            LowestFunctionalRoadClassToNext = FunctionalRoadClassConvertor.Decode(data, 8, 0),
            Bearing = BearingConvertor.DecodeAngleFromBearing(BearingConvertor.Decode(data, 8, 3)),
            DistanceToNext = DistanceToNextConvertor.Decode(data[9])
        };

        // decode second location reference point.
        var last = new LocationReferencePoint
        {
            Coordinate = CoordinateConverter.DecodeRelative(first.Coordinate, data, 10),
            FunctionalRoadClass = FunctionalRoadClassConvertor.Decode(data, 14, 2),
            FormOfWay = FormOfWayConvertor.Decode(data, 14, 5),
            Bearing = BearingConvertor.DecodeAngleFromBearing(BearingConvertor.Decode(data, 15, 3))
        };

        pointAlongLine.First = first;
        pointAlongLine.Orientation = OrientationConverter.Decode(data, 7, 0);
        pointAlongLine.SideOfRoad = SideOfRoadConverter.Decode(data, 14, 0);
        pointAlongLine.PositiveOffsetPercentage = OffsetConvertor.Decode(data, 16);
        pointAlongLine.Last = last;

        return pointAlongLine;
    }

    /// <summary>
    /// Returns true if the given data can be decoded by this decoder.
    /// </summary>
    public static bool CanDecode(byte[] data)
    {
        // decode the header first.
        var header = HeaderConvertor.Decode(data, 0);

        // check header info.
        if (header.ArF1 ||
            !header.IsPoint ||
            header.ArF0 ||
            !header.HasAttributes)
        { // header is incorrect.
            return false;
        }

        return data.Length is 16 or 17;
    }

    /// <summary>
    /// Encodes a point along line location.
    /// </summary>
    public static byte[] Encode(PointAlongLineLocation location)
    {
        var data = new byte[17];

        var header = new Header { Version = 3, HasAttributes = true, ArF0 = false, IsPoint = true,
            ArF1 = false
        };
        HeaderConvertor.Encode(data, 0, header);
        CoordinateConverter.Encode(location.First.Coordinate, data, 1);
        FunctionalRoadClassConvertor.Encode(location.First.FunctionalRoadClass.Value, data, 7, 2);
        FormOfWayConvertor.Encode(location.First.FormOfWay.Value, data, 7, 5);
        FunctionalRoadClassConvertor.Encode(location.First.LowestFunctionalRoadClassToNext.Value, data, 8, 0);
        BearingConvertor.Encode(BearingConvertor.EncodeAngleToBearing(location.First.Bearing.Value), data, 8, 3);
        data[9] = DistanceToNextConvertor.Encode(location.First.DistanceToNext);

        CoordinateConverter.EncodeRelative(location.First.Coordinate, location.Last.Coordinate, data, 10);
        FunctionalRoadClassConvertor.Encode(location.Last.FunctionalRoadClass.Value, data, 14, 2);
        FormOfWayConvertor.Encode(location.Last.FormOfWay.Value, data, 14, 5);
        BearingConvertor.Encode(BearingConvertor.EncodeAngleToBearing(location.Last.Bearing.Value), data, 15, 3);

        OrientationConverter.Encode(location.Orientation.Value, data, 7, 0);
        SideOfRoadConverter.Encode(location.SideOfRoad.Value, data, 14, 0);
        if (location.PositiveOffsetPercentage.HasValue)
        { // positive offset percentage is present.
            OffsetConvertor.Encode(location.PositiveOffsetPercentage.Value, data, 16);
        }

        return data;
    }
}
