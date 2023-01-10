// using NUnit.Framework;
// using OpenLR.Codecs.Binary.Decoders;
// using OpenLR.Model.Locations;
// using System;
//
// namespace OpenLR.Test.Binary
// {
//     /// <summary>
//     /// Contains tests for decoding/encoding a circle location to/from OpenLR binary representation.
//     /// </summary>
//     [TestFixture]
//     public class CircleLocationTests
//     {
//         /// <summary>
//         /// A simple test decoding from a base64 string.
//         /// </summary>
//         [Test]
//         public void DecodeBase64Test()
//         {
//             double delta = 0.0001;
//
//             // define a base64 string.
//             var stringData = Convert.FromBase64String("AwRbYyNGu6o=");
//
//             // decode.
//             Assert.IsTrue(CircleLocationCodec.CanDecode(stringData));
//             var location = CircleLocationCodec.Decode(stringData);
//
//             Assert.IsNotNull(location);
//             Assert.IsInstanceOf<CircleLocation>(location);
//             var circleLocation = (location as CircleLocation);
//
//             // check coordinate.
//             Assert.IsNotNull(circleLocation.Coordinate);
//             Assert.AreEqual(6.12699, circleLocation.Coordinate.Longitude, delta); // 6.12699°
//             Assert.AreEqual(49.60728, circleLocation.Coordinate.Latitude, delta); // 49.60728°
//             Assert.AreEqual(170, circleLocation.Radius);
//         }
//     }
// }
