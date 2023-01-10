// using NUnit.Framework;
// using OpenLR.Codecs.Binary.Decoders;
// using OpenLR.Model;
// using OpenLR.Model.Locations;
// using System;
//
// namespace OpenLR.Test.Binary
// {
//     /// <summary>
//     /// Contains tests for decoding/encoding a point along location to/from OpenLR binary representation.
//     /// </summary>
//     [TestFixture]
//     public class PoiWithAccessPointLocationDecoderTests
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
//             var stringData = Convert.FromBase64String("KwRboCNGfhJRAf/O/7SSQ03/fgCD");
//
//             // decode.
//             Assert.IsTrue(PoiWithAccessPointLocationCodec.CanDecode(stringData));
//             var location = PoiWithAccessPointLocationCodec.Decode(stringData);
//
//             Assert.IsNotNull(location);
//             Assert.IsInstanceOf<PoiWithAccessPointLocation>(location);
//             var poiWithAccessPointLocation = (location as PoiWithAccessPointLocation);
//
//             // check first reference.
//             Assert.IsNotNull(poiWithAccessPointLocation.First);
//             Assert.AreEqual(6.12829, poiWithAccessPointLocation.First.Coordinate.Longitude, delta); // 6.12829°
//             Assert.AreEqual(49.60597, poiWithAccessPointLocation.First.Coordinate.Latitude, delta); // 49.60597°
//             Assert.AreEqual(FunctionalRoadClass.Frc2, poiWithAccessPointLocation.First.FuntionalRoadClass);
//             Assert.AreEqual(FormOfWay.MultipleCarriageWay, poiWithAccessPointLocation.First.FormOfWay);
//             Assert.AreEqual(FunctionalRoadClass.Frc2, poiWithAccessPointLocation.First.LowestFunctionalRoadClassToNext);
//             // Assert.AreEqual(92, poiWithAccessPointLocation.First.DistanceToNext);
//             // Assert.AreEqual(202, poiWithAccessPointLocation.First.BearingDistance);
//
//             // check second reference.
//             Assert.IsNotNull(poiWithAccessPointLocation.Last);
//             Assert.AreEqual(6.12779, poiWithAccessPointLocation.Last.Coordinate.Longitude, delta); // 6.12779°
//             Assert.AreEqual(49.60521, poiWithAccessPointLocation.Last.Coordinate.Latitude, delta); // 49.60521°
//             Assert.AreEqual(FunctionalRoadClass.Frc2, poiWithAccessPointLocation.Last.FuntionalRoadClass);
//             Assert.AreEqual(FormOfWay.MultipleCarriageWay, poiWithAccessPointLocation.Last.FormOfWay);
//             // Assert.AreEqual(42, poiWithAccessPointLocation.Last.BearingDistance);
//
//             // check other properties.
//             Assert.AreEqual(6.12699, poiWithAccessPointLocation.Coordinate.Longitude, delta);
//             Assert.AreEqual(49.60728, poiWithAccessPointLocation.Coordinate.Latitude, delta);
//             Assert.AreEqual(Orientation.NoOrientation, poiWithAccessPointLocation.Orientation);
//             Assert.AreEqual(SideOfRoad.Left, poiWithAccessPointLocation.SideOfRoad);
//         }
//     }
// }
