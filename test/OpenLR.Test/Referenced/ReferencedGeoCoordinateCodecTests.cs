// using NUnit.Framework;
// using OpenLR.Model.Locations;
// using OpenLR.Referenced.Codecs;
//
// namespace OpenLR.Test.Referenced
// {
//     /// <summary>
//     /// Contains tests for decoding/encoding an OpenLR geo coordinate location to a referenced geo coordinate location.
//     /// </summary>
//     [TestFixture]
//     public class ReferencedGeoCoordinateCodecTests
//     {
//         /// <summary>
//         /// An extremely simple referenced geo coordinate location decoding test.
//         /// </summary>
//         [Test]
//         public void DecodeReferencedGeoCoordinateLocation()
//         {
//             // build the location to decode.
//             var location = new GeoCoordinateLocation();
//             location.Coordinate = new Model.Coordinate();
//             location.Coordinate.Latitude = 49.60728;
//             location.Coordinate.Longitude = 6.12699;
//
//             // decode the location
//             //var decoder = new GeoCoordinateLocationDecoder();
//             var referencedLocation = ReferencedGeoCoordinateCodec.Decode(location);
//
//             // confirm result.
//             Assert.IsNotNull(referencedLocation);
//             Assert.AreEqual(referencedLocation.Longitude, location.Coordinate.Longitude);
//             Assert.AreEqual(referencedLocation.Latitude, location.Coordinate.Latitude);
//         }
//     }
// }
