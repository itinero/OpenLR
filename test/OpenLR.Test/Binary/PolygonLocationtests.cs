// using NUnit.Framework;
// using OpenLR.Codecs.Binary.Decoders;
// using OpenLR.Model.Locations;
// using System;
//
// namespace OpenLR.Test.Binary
// {
//     /// <summary>
//     /// Contains tests for decoding/encoding a polygon location to/from OpenLR binary representation.
//     /// </summary>
//     [TestFixture]
//     public class PolygonLocationDecoderTests
//     {
//         /// <summary>
//         /// A simple test decoding from a base64 string.
//         /// </summary>
//         [Test]
//         public void DecodeBase64Test()
//         {
//             double delta = 0.0001;
//
//             // define a base64 string we are sure is a line location.
//             var stringData = Convert.FromBase64String("EwRbHSNGdQFiAA//XADz/64AJP9b/7U=");
//
//             // decode.
//             Assert.IsTrue(PolygonLocationCodec.CanDecode(stringData));
//             var location = PolygonLocationCodec.Decode(stringData);
//
//             Assert.IsNotNull(location);
//             Assert.IsInstanceOf<PolygonLocation>(location);
//             var polygonLocation = (location as PolygonLocation);
//
//             Assert.IsNotNull(polygonLocation);
//             Assert.IsNotNull(polygonLocation.Coordinates);
//
//             Assert.AreEqual(6.12549, polygonLocation.Coordinates[0].Longitude, delta);
//             Assert.AreEqual(49.60577, polygonLocation.Coordinates[0].Latitude, delta);
//
//             Assert.AreEqual(6.12903, polygonLocation.Coordinates[1].Longitude, delta);
//             Assert.AreEqual(49.60592, polygonLocation.Coordinates[1].Latitude, delta);
//
//             Assert.AreEqual(6.12739, polygonLocation.Coordinates[2].Longitude, delta);
//             Assert.AreEqual(49.60835, polygonLocation.Coordinates[2].Latitude, delta);
//
//             Assert.AreEqual(6.12658, polygonLocation.Coordinates[3].Longitude, delta);
//             Assert.AreEqual(49.60871, polygonLocation.Coordinates[3].Latitude, delta);
//
//             Assert.AreEqual(6.12493, polygonLocation.Coordinates[4].Longitude, delta);
//             Assert.AreEqual(49.60796, polygonLocation.Coordinates[4].Latitude, delta);
//         }
//     }
// }
