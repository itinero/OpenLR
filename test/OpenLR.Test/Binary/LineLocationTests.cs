using NUnit.Framework;
using OpenLR.Codecs.Binary.Codecs;
using OpenLR.Model;
using OpenLR.Model.Locations;

namespace OpenLR.Test.Binary;

/// <summary>
/// Contains tests for decoding/encoding a line location to/from OpenLR binary representation.
/// </summary>
[TestFixture]
public class LineLocationTests
{
    /// <summary>
    /// A simple test decoding from a base64 string.
    /// </summary>
    [Test]
    public void DecodeBase64Test()
    {
        const double delta = 0.0001;

        // define a base64 string we are sure is a line location.
        var stringData = Convert.FromBase64String("CwRbWyNG9RpsCQCb/jsbtAT/6/+jK1lE");

        // decode.
        Assert.IsTrue(LineLocationCodec.CanDecode(stringData));
        var location = LineLocationCodec.Decode(stringData);

        Assert.IsNotNull(location);
        Assert.IsInstanceOf<LineLocation>(location);
        var lineLocation = (location as LineLocation);

        // check first reference.
        Assert.IsNotNull(lineLocation.First);
        Assert.That(lineLocation.First.Coordinate.Longitude, Is.EqualTo(6.12683).Within(delta)); // 6.12683°
        Assert.That(lineLocation.First.Coordinate.Latitude, Is.EqualTo(49.60851).Within(delta)); // 49.60851°
        Assert.That(lineLocation.First.FunctionalRoadClass, Is.EqualTo(FunctionalRoadClass.Frc3));
        Assert.That(lineLocation.First.FormOfWay, Is.EqualTo(FormOfWay.MultipleCarriageWay));
        Assert.That(lineLocation.First.LowestFunctionalRoadClassToNext, Is.EqualTo(FunctionalRoadClass.Frc3));

        // check intermediates.
        Assert.IsNotNull(lineLocation.Intermediate);
        Assert.That(lineLocation.Intermediate.Length, Is.EqualTo(1));
        Assert.That(lineLocation.Intermediate[0].Coordinate.Longitude, Is.EqualTo(6.12838).Within(delta)); // 6.12838°
        Assert.That(lineLocation.Intermediate[0].Coordinate.Latitude, Is.EqualTo(49.60398).Within(delta)); // 49.60398°
        Assert.That(lineLocation.Intermediate[0].FunctionalRoadClass, Is.EqualTo(FunctionalRoadClass.Frc3));
        Assert.That(lineLocation.Intermediate[0].FormOfWay, Is.EqualTo(FormOfWay.SingleCarriageWay));
        Assert.That(lineLocation.Intermediate[0].LowestFunctionalRoadClassToNext, Is.EqualTo(FunctionalRoadClass.Frc5));

        // check second reference.
        Assert.IsNotNull(lineLocation.Last);
        Assert.That(lineLocation.Last.Coordinate.Longitude, Is.EqualTo(6.12817).Within(delta)); // 6.12817°
        Assert.That(lineLocation.Last.Coordinate.Latitude, Is.EqualTo(49.60305).Within(delta)); // 49.60305°
        Assert.That(lineLocation.Last.FunctionalRoadClass, Is.EqualTo(FunctionalRoadClass.Frc5));
        Assert.That(lineLocation.Last.FormOfWay, Is.EqualTo(FormOfWay.SingleCarriageWay));
    }

    /// <summary>
    /// A simple test encoding from a base64 string.
    /// </summary>
    [Test]
    public void EncodeBase64Test()
    {
        double delta = 0.0001;

        // build the location to decode.
        var location = new LineLocation();
        location.First = new LocationReferencePoint();
        location.First.Coordinate = new Coordinate() { Latitude = 49.60851, Longitude = 6.12683 };
        location.First.DistanceToNext = 10;
        location.First.FunctionalRoadClass = FunctionalRoadClass.Frc3;
        location.First.FormOfWay = FormOfWay.MultipleCarriageWay;
        location.First.LowestFunctionalRoadClassToNext = FunctionalRoadClass.Frc3;
        location.First.Bearing = 0;
        location.Intermediate = new LocationReferencePoint[1];
        location.Intermediate[0] = new LocationReferencePoint();
        location.Intermediate[0].Coordinate = new Coordinate() { Latitude = 49.60398, Longitude = 6.12838 };
        location.Intermediate[0].DistanceToNext = 10;
        location.Intermediate[0].FunctionalRoadClass = FunctionalRoadClass.Frc3;
        location.Intermediate[0].FormOfWay = FormOfWay.SingleCarriageWay;
        location.Intermediate[0].LowestFunctionalRoadClassToNext = FunctionalRoadClass.Frc5;
        location.Intermediate[0].Bearing = 0;
        location.Last = new LocationReferencePoint();
        location.Last.Coordinate = new Coordinate() { Latitude = 49.60305, Longitude = 6.12817 };
        location.Last.DistanceToNext = 10;
        location.Last.FunctionalRoadClass = FunctionalRoadClass.Frc5;
        location.Last.FormOfWay = FormOfWay.SingleCarriageWay;
        location.Last.Bearing = 0;

        // encode.
        var stringData = LineLocationCodec.Encode(location);

        // decode again (decoding was tested above).
        var decodedLocation = LineLocationCodec.Decode(stringData);

        Assert.IsNotNull(decodedLocation);
        Assert.IsInstanceOf<LineLocation>(decodedLocation);
        var lineLocation = (decodedLocation as LineLocation);

        // check first reference.
        Assert.IsNotNull(lineLocation.First);
        Assert.That(lineLocation.First.Coordinate.Longitude, Is.EqualTo(6.12683).Within(delta)); // 6.12683°
        Assert.That(lineLocation.First.Coordinate.Latitude, Is.EqualTo(49.60851).Within(delta)); // 49.60851°
        Assert.That(lineLocation.First.FunctionalRoadClass, Is.EqualTo(FunctionalRoadClass.Frc3));
        Assert.That(lineLocation.First.FormOfWay, Is.EqualTo(FormOfWay.MultipleCarriageWay));
        Assert.That(lineLocation.First.LowestFunctionalRoadClassToNext, Is.EqualTo(FunctionalRoadClass.Frc3));

        // check intermediates.
        Assert.IsNotNull(lineLocation.Intermediate);
        Assert.That(lineLocation.Intermediate.Length, Is.EqualTo(1));
        Assert.That(lineLocation.Intermediate[0].Coordinate.Longitude, Is.EqualTo(6.12838).Within(delta)); // 6.12838°
        Assert.That(lineLocation.Intermediate[0].Coordinate.Latitude, Is.EqualTo(49.60398).Within(delta)); // 49.60398°
        Assert.That(lineLocation.Intermediate[0].FunctionalRoadClass, Is.EqualTo(FunctionalRoadClass.Frc3));
        Assert.That(lineLocation.Intermediate[0].FormOfWay, Is.EqualTo(FormOfWay.SingleCarriageWay));
        Assert.That(lineLocation.Intermediate[0].LowestFunctionalRoadClassToNext, Is.EqualTo(FunctionalRoadClass.Frc5));

        // check second reference.
        Assert.IsNotNull(lineLocation.Last);
        Assert.That(lineLocation.Last.Coordinate.Longitude, Is.EqualTo(6.12817).Within(delta)); // 6.12817°
        Assert.That(lineLocation.Last.Coordinate.Latitude, Is.EqualTo(49.60305).Within(delta)); // 49.60305°
        Assert.That(lineLocation.Last.FunctionalRoadClass, Is.EqualTo(FunctionalRoadClass.Frc5));
        Assert.That(lineLocation.Last.FormOfWay, Is.EqualTo(FormOfWay.SingleCarriageWay));

        // compare again with reference encoded string.
        var referenceBinary = Convert.FromBase64String("CwRbWyNG9BpgAACa/jsboAD/6/+kKwAAAA==");
        var referenceDecodedLocation = LineLocationCodec.Decode(referenceBinary);

        // check first reference.
        Assert.IsNotNull(lineLocation.First);
        Assert.That(lineLocation.First.Coordinate.Longitude, Is.EqualTo(referenceDecodedLocation.First.Coordinate.Longitude).Within(delta)); // 6.12829°
        Assert.That(lineLocation.First.Coordinate.Latitude, Is.EqualTo(referenceDecodedLocation.First.Coordinate.Latitude).Within(delta)); // 49.60597°
        Assert.That(lineLocation.First.FunctionalRoadClass, Is.EqualTo(referenceDecodedLocation.First.FunctionalRoadClass));
        Assert.That(lineLocation.First.FormOfWay, Is.EqualTo(referenceDecodedLocation.First.FormOfWay));
        Assert.That(lineLocation.First.LowestFunctionalRoadClassToNext, Is.EqualTo(referenceDecodedLocation.First.LowestFunctionalRoadClassToNext));
        Assert.That(lineLocation.First.Bearing.Value, Is.EqualTo(referenceDecodedLocation.First.Bearing.Value).Within(11.25)); // binary encode loses accuracy for bearing.

        // check intermediates.
        Assert.IsNotNull(referenceDecodedLocation.Intermediate);
        Assert.That(lineLocation.Intermediate.Length, Is.EqualTo(referenceDecodedLocation.Intermediate.Length));
        Assert.That(lineLocation.Intermediate[0].Coordinate.Longitude, Is.EqualTo(referenceDecodedLocation.Intermediate[0].Coordinate.Longitude).Within(delta)); // 6.12838°
        Assert.That(lineLocation.Intermediate[0].Coordinate.Latitude, Is.EqualTo(referenceDecodedLocation.Intermediate[0].Coordinate.Latitude).Within(delta)); // 49.60398°
        Assert.That(lineLocation.Intermediate[0].FunctionalRoadClass, Is.EqualTo(referenceDecodedLocation.Intermediate[0].FunctionalRoadClass));
        Assert.That(lineLocation.Intermediate[0].FormOfWay, Is.EqualTo(referenceDecodedLocation.Intermediate[0].FormOfWay));
        Assert.That(lineLocation.Intermediate[0].LowestFunctionalRoadClassToNext, Is.EqualTo(referenceDecodedLocation.Intermediate[0].LowestFunctionalRoadClassToNext));

        // check second reference.
        Assert.IsNotNull(lineLocation.Last);
        Assert.That(lineLocation.Last.Coordinate.Longitude, Is.EqualTo(referenceDecodedLocation.Last.Coordinate.Longitude).Within(delta)); // 6.12779°
        Assert.That(lineLocation.Last.Coordinate.Latitude, Is.EqualTo(referenceDecodedLocation.Last.Coordinate.Latitude).Within(delta)); // 49.60521°
        Assert.That(lineLocation.Last.FunctionalRoadClass, Is.EqualTo(referenceDecodedLocation.Last.FunctionalRoadClass));
        Assert.That(lineLocation.Last.FormOfWay, Is.EqualTo(referenceDecodedLocation.Last.FormOfWay));
        Assert.That(lineLocation.Last.Bearing.Value, Is.EqualTo(referenceDecodedLocation.Last.Bearing.Value).Within(11.25)); // binary encode loses accuracy for bearing.
    }

    /// <summary>
    /// A regression tests for issue:
    /// https://github.com/itinero/OpenLR/issues/76
    /// </summary>
    [Test]
    public void RegressionEncodeDecodeNegativeLongitude()
    {
        double delta = 0.0001;

        var location = new LineLocation()
        {
            First = new LocationReferencePoint()
            {
                Bearing = 101,
                Coordinate = new Coordinate()
                {
                    Latitude = 52.932136535644531f,
                    Longitude = -1.5213972330093384f
                },
                DistanceToNext = 1111,
                FormOfWay = FormOfWay.Motorway,
                FunctionalRoadClass = FunctionalRoadClass.Frc0,
                LowestFunctionalRoadClassToNext = FunctionalRoadClass.Frc0
            },
            Last = new LocationReferencePoint()
            {
                Bearing = 276,
                Coordinate = new Coordinate()
                {
                    Latitude = 52.929317474365234f,
                    Longitude = -1.5055110454559326
                },
                DistanceToNext = 0,
                FormOfWay = FormOfWay.Motorway,
                FunctionalRoadClass = FunctionalRoadClass.Frc0
            },
            NegativeOffsetPercentage = 2.12239432f,
            PositiveOffsetPercentage = 1.46861947f
        };

        // encode.
        var stringData = LineLocationCodec.Encode(location);

        // decode again (decoding was tested above).
        var decodedLocation = LineLocationCodec.Decode(stringData);

        Assert.IsNotNull(decodedLocation);
        Assert.IsInstanceOf<LineLocation>(decodedLocation);
        var lineLocation = (decodedLocation as LineLocation);

        // check first reference.
        Assert.IsNotNull(lineLocation.First);
        Assert.That(lineLocation.First.Coordinate.Latitude, Is.EqualTo(52.932136535644531f).Within(delta));
        Assert.That(lineLocation.First.Coordinate.Longitude, Is.EqualTo(-1.5213972330093384f).Within(delta));
        Assert.That(lineLocation.First.FunctionalRoadClass, Is.EqualTo(FunctionalRoadClass.Frc0));
        Assert.That(lineLocation.First.FormOfWay, Is.EqualTo(FormOfWay.Motorway));
        Assert.That(lineLocation.First.LowestFunctionalRoadClassToNext, Is.EqualTo(FunctionalRoadClass.Frc0));

        // check second reference.
        Assert.IsNotNull(lineLocation.Last);
        Assert.That(lineLocation.Last.Coordinate.Longitude, Is.EqualTo(-1.5055110454559326f).Within(delta)); // 6.12817°
        Assert.That(lineLocation.Last.Coordinate.Latitude, Is.EqualTo(52.929317474365234f).Within(delta)); // 49.60305°
        Assert.That(lineLocation.Last.FunctionalRoadClass, Is.EqualTo(FunctionalRoadClass.Frc0));
        Assert.That(lineLocation.Last.FormOfWay, Is.EqualTo(FormOfWay.Motorway));
    }

    /// <summary>
    /// A regression test for encoding a location with intermediate points.
    /// </summary>
    [Test]
    public void RegressionEncodeDecode()
    {
        var e = 0.0001f;

        var json = "{\r\n  \"First\": {\r\n    \"Coordinate\": {\r\n      \"Latitude\": 52.275665283203125,\r\n      \"Longitude\": 6.825572490692139\r\n    },\r\n    \"Bearing\": 83,\r\n    \"FunctionalRoadClass\": 5,\r\n    \"FormOfWay\": 7,\r\n    \"LowestFunctionalRoadClassToNext\": 5,\r\n    \"DistanceToNext\": 7012\r\n  },\r\n  \"Intermediate\": [\r\n    {\r\n      \"Coordinate\": {\r\n        \"Latitude\": 52.30723190307617,\r\n        \"Longitude\": 6.911031246185303\r\n      },\r\n      \"Bearing\": 95,\r\n      \"FunctionalRoadClass\": 5,\r\n      \"FormOfWay\": 7,\r\n      \"LowestFunctionalRoadClassToNext\": 5,\r\n      \"DistanceToNext\": 70\r\n    },\r\n    {\r\n      \"Coordinate\": {\r\n        \"Latitude\": 52.307552337646484,\r\n        \"Longitude\": 6.9113054275512695\r\n      },\r\n      \"Bearing\": 27,\r\n      \"FunctionalRoadClass\": 3,\r\n      \"FormOfWay\": 6,\r\n      \"LowestFunctionalRoadClassToNext\": 3,\r\n      \"DistanceToNext\": 1924\r\n    },\r\n    {\r\n      \"Coordinate\": {\r\n        \"Latitude\": 52.32027053833008,\r\n        \"Longitude\": 6.928611755371094\r\n      },\r\n      \"Bearing\": 73,\r\n      \"FunctionalRoadClass\": 3,\r\n      \"FormOfWay\": 2,\r\n      \"LowestFunctionalRoadClassToNext\": 5,\r\n      \"DistanceToNext\": 7658\r\n    }\r\n  ],\r\n  \"Last\": {\r\n    \"Coordinate\": {\r\n      \"Latitude\": 52.36980438232422,\r\n      \"Longitude\": 6.999289512634277\r\n    },\r\n    \"Bearing\": 230,\r\n    \"FunctionalRoadClass\": 5,\r\n    \"FormOfWay\": 7,\r\n    \"LowestFunctionalRoadClassToNext\": null,\r\n    \"DistanceToNext\": 0\r\n  },\r\n  \"PositiveOffsetPercentage\": 0,\r\n  \"NegativeOffsetPercentage\": 0.638994753\r\n}";
        var location = Newtonsoft.Json.JsonConvert.DeserializeObject<LineLocation>(json);

        // encode.
        var stringData = LineLocationCodec.Encode(location);

        // decode again (decoding was tested above).
        var decodedLocation = LineLocationCodec.Decode(stringData);

        Assert.That(decodedLocation.First.Coordinate.Latitude, Is.EqualTo(location.First.Coordinate.Latitude).Within(e));
        Assert.That(decodedLocation.First.Coordinate.Longitude, Is.EqualTo(location.First.Coordinate.Longitude).Within(e));

        Assert.That(decodedLocation.Intermediate.Length, Is.EqualTo(location.Intermediate.Length));
        for (var i = 0; i < location.Intermediate.Length; i++)
        {
            Assert.That(decodedLocation.Intermediate[i].Coordinate.Latitude, Is.EqualTo(location.Intermediate[i].Coordinate.Latitude).Within(e));
            Assert.That(decodedLocation.Intermediate[i].Coordinate.Longitude, Is.EqualTo(location.Intermediate[i].Coordinate.Longitude).Within(e));
        }

        Assert.That(decodedLocation.Last.Coordinate.Latitude, Is.EqualTo(location.Last.Coordinate.Latitude).Within(e));
        Assert.That(decodedLocation.Last.Coordinate.Longitude, Is.EqualTo(location.Last.Coordinate.Longitude).Within(e));
    }
}
