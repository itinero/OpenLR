using NUnit.Framework;
using OpenLR.Binary.Decoders;
using OpenLR.Locations;

namespace OpenLR.Tests.Binary
{
    /// <summary>
    /// Contains tests for decoding/encoding a grid location to/from OpenLR binary representation.
    /// </summary>
    [TestFixture]
    public class GridLocationTests
    {
        /// <summary>
        /// A simple test decoding from a base64 string.
        /// </summary>
        [Test]
        public void DecodeBase64Test()
        {
            double delta = 0.0001;

            // define a base64 string.
            string stringData = "QwRbICNGeQBKAB8ABQAD";

            // decode.
            var decoder = new GridLocationDecoder();
            var location = decoder.Decode(stringData);

            Assert.IsNotNull(location);
            Assert.IsInstanceOf<GridLocation>(location);
            var gridLocation = (location as GridLocation);

            // check coordinate.
            Assert.IsNotNull(gridLocation.Box);
            Assert.AreEqual(6.12555, gridLocation.Box.BottomLeft[0], delta);
            Assert.AreEqual(49.60586, gridLocation.Box.BottomLeft[1], delta);
            Assert.AreEqual(6.126291, gridLocation.Box.TopRight[0], delta);
            Assert.AreEqual(49.606170, gridLocation.Box.TopRight[1], delta);
            Assert.AreEqual(5, gridLocation.Columns, delta);
            Assert.AreEqual(3, gridLocation.Rows, delta);
        }
    }
}