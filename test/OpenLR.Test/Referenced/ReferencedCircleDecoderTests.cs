// using NUnit.Framework;
// using OpenLR.Model.Locations;
// using OpenLR.Referenced.Codecs;
//
// namespace OpenLR.Test.Referenced;
//
// /// <summary>
// /// Contains tests for decoding/encoding an OpenLR circle location to a referenced circle location.
// /// </summary>
// [TestFixture]
// public class ReferencedCircleDecoderTests
// {
//     /// <summary>
//     /// An extremely simple referenced circle location decoding test.
//     /// </summary>
//     [Test]
//     public void DecodeReferencedCircleLocation()
//     {
//         // build the location to decode.
//         var location = new CircleLocation(new Model.Coordinate(6.12699, 49.60728), 170);
//
//         // decode the location
//         var referencedLocation = ReferencedCircleCodec.Decode(location);
//
//         // confirm result.
//         Assert.That(referencedLocation, Is.Not.Null);
//         Assert.That(location.Coordinate.Longitude, Is.EqualTo(referencedLocation.Longitude));
//         Assert.That(location.Coordinate.Latitude, Is.EqualTo(referencedLocation.Latitude));
//         Assert.That(location.Radius, Is.EqualTo(referencedLocation.Radius));
//     }
// }
