using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenLR.Tests
{
    /// <summary>
    /// Holds some simple xml serialization/deserialization tests.
    /// </summary>
    [TestFixture]
    public class XmlSerializerTests
    {
        /// <summary>
        /// A simple XML deserialization test.
        /// </summary>
        [Test]
        public void DeSerialize()
        {
            var xmlSerializer = new XmlSerializer(typeof(D2LogicalModel));
            var deserialized = xmlSerializer.Deserialize(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OpenLR.Tests.Data.HDF-BEL.XML"));

            Assert.IsNotNull(deserialized);
            Assert.IsInstanceOf<D2LogicalModel>(deserialized);
        }
    }
}
