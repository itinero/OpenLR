using NUnit.Framework;
using OpenLR.Model;
using OsmSharp.Collections.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenLR.OsmSharp.NWB;

namespace OpenLR.Tests.Referenced.NWB
{
    /// <summary>
    /// Contains tests for the NWB to OpenLR mapping.
    /// </summary>
    [TestFixture]
    public class NWBMappingTests
    {
        [Test]
        public void TestMapping()
        {
            var tags = new TagsCollection();
            FormOfWay fow;
            FunctionalRoadClass frc;

            // R HR => FRC0/MOTORWAY
            tags.Add(NWBMapping.WEGBEHSRT, "R");
            tags.Add(NWBMapping.BAANSUBSRT, "HR");
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, string.Empty);
            tags.Add(NWBMapping.HECTOLTTR, string.Empty);
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc0, frc);
            Assert.AreEqual(FormOfWay.Motorway, fow);
            tags.Clear();

            // R NRB => FRC0/ROUNDABOUT
            tags.Add(NWBMapping.WEGBEHSRT, "R");
            tags.Add(NWBMapping.BAANSUBSRT, "NRB");
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, string.Empty);
            tags.Add(NWBMapping.HECTOLTTR, string.Empty);
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc0, frc);
            Assert.AreEqual(FormOfWay.Roundabout, fow);
            tags.Clear();

            // R MRB => FRC0/ROUNDABOUT
            tags.Add(NWBMapping.WEGBEHSRT, "R");
            tags.Add(NWBMapping.BAANSUBSRT, "MRB");
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, string.Empty);
            tags.Add(NWBMapping.HECTOLTTR, string.Empty);
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc0, frc);
            Assert.AreEqual(FormOfWay.Roundabout, fow);
            tags.Clear();

            // R PST DKV!=(a|b|c|d) => FRC0/SLIPROAD
            tags.Add(NWBMapping.WEGBEHSRT, "R");
            tags.Add(NWBMapping.BAANSUBSRT, "PST");
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, "A1");
            tags.Add(NWBMapping.HECTOLTTR, "1");
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc0, frc);
            Assert.AreEqual(FormOfWay.SlipRoad, fow);
            tags.Clear();

            // R PST DKV!=(a|b|c|d| ) => FRC5/OTHER
            tags.Add(NWBMapping.WEGBEHSRT, "R");
            tags.Add(NWBMapping.BAANSUBSRT, "PST");
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, string.Empty);
            tags.Add(NWBMapping.HECTOLTTR, string.Empty);
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc5, frc);
            Assert.AreEqual(FormOfWay.Other, fow);
            tags.Clear();

            // R OPR => FRC3/SLIPROAD
            tags.Add(NWBMapping.WEGBEHSRT, "R");
            tags.Add(NWBMapping.BAANSUBSRT, "OPR");
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, "A1");
            tags.Add(NWBMapping.HECTOLTTR, "A");
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc3, frc);
            Assert.AreEqual(FormOfWay.SlipRoad, fow);
            tags.Clear();

            // R AFR => FRC3/SLIPROAD
            tags.Add(NWBMapping.WEGBEHSRT, "R");
            tags.Add(NWBMapping.BAANSUBSRT, "AFR");
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, "A1");
            tags.Add(NWBMapping.HECTOLTTR, "R");
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc3, frc);
            Assert.AreEqual(FormOfWay.SlipRoad, fow);
            tags.Clear();

            // R AFR => FRC3/SLIPROAD
            tags.Add(NWBMapping.WEGBEHSRT, "R");
            tags.Add(NWBMapping.BAANSUBSRT, "AFR");
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, "A1");
            tags.Add(NWBMapping.HECTOLTTR, "1");
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc3, frc);
            Assert.AreEqual(FormOfWay.SlipRoad, fow);
            tags.Clear();

            // R PST DKV=(a|b|c|d) => FRC3/SLIPROAD
            tags.Add(NWBMapping.WEGBEHSRT, "R");
            tags.Add(NWBMapping.BAANSUBSRT, "PST");
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, "A1a");
            tags.Add(NWBMapping.HECTOLTTR, "a");
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc3, frc);
            Assert.AreEqual(FormOfWay.SlipRoad, fow);
            tags.Clear();
            tags.Add(NWBMapping.WEGBEHSRT, "R");
            tags.Add(NWBMapping.BAANSUBSRT, "PST");
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, "A1b");
            tags.Add(NWBMapping.HECTOLTTR, "b");
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc3, frc);
            Assert.AreEqual(FormOfWay.SlipRoad, fow);
            tags.Clear();
            tags.Add(NWBMapping.WEGBEHSRT, "R");
            tags.Add(NWBMapping.BAANSUBSRT, "PST");
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, "A1c");
            tags.Add(NWBMapping.HECTOLTTR, "c");
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc3, frc);
            Assert.AreEqual(FormOfWay.SlipRoad, fow);
            tags.Clear();
            tags.Add(NWBMapping.WEGBEHSRT, "R");
            tags.Add(NWBMapping.BAANSUBSRT, "PST");
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, "A1d");
            tags.Add(NWBMapping.HECTOLTTR, "d");
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc3, frc);
            Assert.AreEqual(FormOfWay.SlipRoad, fow);
            tags.Clear();

            // R PST RIJRICHTING!=null => FRC0/SLIPROAD
            tags.Add(NWBMapping.WEGBEHSRT, "R");
            tags.Add(NWBMapping.BAANSUBSRT, "PST");
            tags.Add(NWBMapping.RIJRICHTNG, "N");
            tags.Add(NWBMapping.HECTOLTTR, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, string.Empty);
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc0, frc);
            Assert.AreEqual(FormOfWay.SlipRoad, fow);
            tags.Clear();

            // R VB? RIJRICHTING!=null => FRC0/SLIPROAD
            tags.Add(NWBMapping.WEGBEHSRT, "R");
            tags.Add(NWBMapping.BAANSUBSRT, "VB2");
            tags.Add(NWBMapping.RIJRICHTNG, "N");
            tags.Add(NWBMapping.HECTOLTTR, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, string.Empty);
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc0, frc);
            Assert.AreEqual(FormOfWay.SlipRoad, fow);
            tags.Clear();

            // P HR RIJRICHTING==null => FRC2/SINGLE_CARRIAGEWAY
            tags.Add(NWBMapping.WEGBEHSRT, "P");
            tags.Add(NWBMapping.BAANSUBSRT, "HR");
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, string.Empty);
            tags.Add(NWBMapping.HECTOLTTR, string.Empty);
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc2, frc);
            Assert.AreEqual(FormOfWay.SingleCarriageWay, fow);
            tags.Clear();

            // P HR RIJRICHTING!=null => FRC2/MULTIPLE_CARRIAGEWAY
            tags.Add(NWBMapping.WEGBEHSRT, "P");
            tags.Add(NWBMapping.BAANSUBSRT, "HR");
            tags.Add(NWBMapping.RIJRICHTNG, "N");
            tags.Add(NWBMapping.HECTOLTTR, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, string.Empty);
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc3, frc);
            Assert.AreEqual(FormOfWay.MultipleCarriageWay, fow);
            tags.Clear();

            // P NRB => FRC3/ROUNDABOUT
            tags.Add(NWBMapping.WEGBEHSRT, "P");
            tags.Add(NWBMapping.BAANSUBSRT, "NRB");
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, string.Empty);
            tags.Add(NWBMapping.HECTOLTTR, string.Empty);
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc3, frc);
            Assert.AreEqual(FormOfWay.Roundabout, fow);
            tags.Clear();

            // P MRB => FRC3/ROUNDABOUT
            tags.Add(NWBMapping.WEGBEHSRT, "P");
            tags.Add(NWBMapping.BAANSUBSRT, "MRB");
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, string.Empty);
            tags.Add(NWBMapping.HECTOLTTR, string.Empty);
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc3, frc);
            Assert.AreEqual(FormOfWay.Roundabout, fow);
            tags.Clear();

            // P OPR => FRC3/SLIPROAD
            tags.Add(NWBMapping.WEGBEHSRT, "P");
            tags.Add(NWBMapping.BAANSUBSRT, "OPR");
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, string.Empty);
            tags.Add(NWBMapping.HECTOLTTR, string.Empty);
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc3, frc);
            Assert.AreEqual(FormOfWay.SlipRoad, fow);
            tags.Clear();

            // P AFR => FRC3/SLIPROAD
            tags.Add(NWBMapping.WEGBEHSRT, "P");
            tags.Add(NWBMapping.BAANSUBSRT, "AFR");
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, string.Empty);
            tags.Add(NWBMapping.HECTOLTTR, "R");
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc3, frc);
            Assert.AreEqual(FormOfWay.SlipRoad, fow);
            tags.Clear();

            // P PST => FRC3/SLIPROAD
            tags.Add(NWBMapping.WEGBEHSRT, "P");
            tags.Add(NWBMapping.BAANSUBSRT, "AFR");
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, string.Empty);
            tags.Add(NWBMapping.HECTOLTTR, "R");
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc3, frc);
            Assert.AreEqual(FormOfWay.SlipRoad, fow);
            tags.Clear();

            // P VB? => FRC3/SLIPROAD
            tags.Add(NWBMapping.WEGBEHSRT, "P");
            tags.Add(NWBMapping.BAANSUBSRT, "VB2");
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, string.Empty);
            tags.Add(NWBMapping.HECTOLTTR, "2");
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc3, frc);
            Assert.AreEqual(FormOfWay.SlipRoad, fow);
            tags.Clear();

            // (empty) => FRC5/OTHER
            tags.Add(NWBMapping.WEGBEHSRT, string.Empty);
            tags.Add(NWBMapping.BAANSUBSRT, string.Empty);
            tags.Add(NWBMapping.RIJRICHTNG, string.Empty);
            tags.Add(NWBMapping.WEGNUMMER, string.Empty);
            tags.Add(NWBMapping.HECTOLTTR, string.Empty);
            Assert.IsTrue(tags.ToOpenLR(out fow, out frc));
            Assert.AreEqual(FunctionalRoadClass.Frc5, frc);
            Assert.AreEqual(FormOfWay.Other, fow);
            tags.Clear();
        }
    }
}