// using NUnit.Framework;
// using OpenLR.Codecs.Binary.Decoders;
// using OpenLR.Model.Locations;
// using System;
//
// namespace OpenLR.Test.Binary
// {
//     /// <summary>
//     /// Contains tests for decoding/encoding a grid location to/from OpenLR binary representation.
//     /// </summary>
//     [TestFixture]
//     public class GridLocationTests
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
//             var stringData = Convert.FromBase64String("QwRbICNGeQBKAB8ABQAD");
//
//             // decode.
//             Assert.IsTrue(GridLocationCodec.CanDecode(stringData));
//             var location = GridLocationCodec.Decode(stringData);
//
//             Assert.IsNotNull(location);
//             Assert.IsInstanceOf<GridLocation>(location);
//             var gridLocation = (location as GridLocation);
//
//             // check coordinate.
//             Assert.IsNotNull(gridLocation.LowerLeft);
//             Assert.AreEqual(6.12555, gridLocation.LowerLeft.Longitude, delta);
//             Assert.AreEqual(49.60586, gridLocation.LowerLeft.Latitude, delta);
//             Assert.IsNotNull(gridLocation.UpperRight);
//             Assert.AreEqual(6.126291, gridLocation.UpperRight.Longitude, delta);
//             Assert.AreEqual(49.606170, gridLocation.UpperRight.Latitude, delta);
//             Assert.AreEqual(5, gridLocation.Columns, delta);
//             Assert.AreEqual(3, gridLocation.Rows, delta);
//         }
//     }
// }
