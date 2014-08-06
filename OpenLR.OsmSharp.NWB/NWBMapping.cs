using OpenLR.Model;
using OsmSharp.Collections.Tags;

namespace OpenLR.OsmSharp.NWB
{
    /// <summary>
    /// Contains the NWB mapping, mapping NWB attributes to OpenLR FOW and FRC.
    /// </summary>
    public static class NWBMapping
    {
        /// <summary>
        /// Maps NWB attributes and translates them to OpenLR FOW and FRC.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="fow"></param>
        /// <param name="frc"></param>
        /// <returns>False if no valid mapping was found.</returns>
        public static bool ToOpenLR(this TagsCollectionBase tags, out FormOfWay fow, out FunctionalRoadClass frc)
        {
            fow = FormOfWay.Undefined;
            frc = FunctionalRoadClass.Frc7;
            string baansubsrt, wegbeerder, wegnummer, rijrichting;
            if (!tags.TryGetValue("BAANSUBSRT", out baansubsrt) ||
                !tags.TryGetValue("WEGBEHSRT", out wegbeerder) ||
                !tags.TryGetValue("WEGNUMMER", out wegnummer) ||
                !tags.TryGetValue("RIJRICHTING", out rijrichting))
            { // not even a BAANSUBSRT tag!
                return false;
            }

            // make sure everything is lowercase.
            char? dvkletter = null; // assume dkv letter is the suffix use for exists etc. see: http://www.wegenwiki.nl/Hectometerpaal#Suffix
            if (!string.IsNullOrWhiteSpace(wegbeerder)) { wegbeerder = wegbeerder.ToLowerInvariant(); }
            if (!string.IsNullOrWhiteSpace(baansubsrt)) { baansubsrt = baansubsrt.ToLowerInvariant(); }
            if (!string.IsNullOrWhiteSpace(wegnummer)) { wegnummer = wegnummer.ToLowerInvariant(); dvkletter = wegnummer[wegnummer.Length - 1]; }
            if (!string.IsNullOrWhiteSpace(wegnummer)) { rijrichting = rijrichting.ToLowerInvariant(); }

            fow = FormOfWay.Other;
            frc = FunctionalRoadClass.Frc5;
            if(wegbeerder == "r")
            {
                if(baansubsrt == "hr")
                {
                    fow = FormOfWay.Motorway;
                    frc = FunctionalRoadClass.Frc0;
                }
                else if(baansubsrt == "nrb" || 
                    baansubsrt == "mrb")
                {
                    fow = FormOfWay.Roundabout;
                    frc = FunctionalRoadClass.Frc0;
                }
                else if(baansubsrt == "pst")
                {
                    if (!string.IsNullOrWhiteSpace(rijrichting))
                    {
                        fow = FormOfWay.SlipRoad;
                        frc = FunctionalRoadClass.Frc0;
                    }
                    else
                    {
                        if (dvkletter.HasValue)
                        {
                            fow = FormOfWay.SlipRoad;
                            if (dvkletter == 'a' ||
                                dvkletter == 'b' ||
                                dvkletter == 'c' ||
                                dvkletter == 'd')
                            {
                                frc = FunctionalRoadClass.Frc3;
                            }
                            else
                            {
                                frc = FunctionalRoadClass.Frc0;
                            }
                        }
                    }
                }
                else if(baansubsrt == "opr" ||
                    baansubsrt == "afr")
                {
                    frc = FunctionalRoadClass.Frc3;
                    fow = FormOfWay.SlipRoad;
                }
                else if(baansubsrt.StartsWith("vb"))
                {
                    fow = FormOfWay.SlipRoad;
                    if (!string.IsNullOrWhiteSpace(rijrichting))
                    {
                        frc = FunctionalRoadClass.Frc0;
                    }
                }
            }
            else if(wegbeerder == "p")
            {
                if (baansubsrt == "hr")
                {
                    frc = FunctionalRoadClass.Frc3;
                    fow = FormOfWay.MultipleCarriageWay;
                    if(string.IsNullOrWhiteSpace(rijrichting))
                    {
                        frc = FunctionalRoadClass.Frc2;
                        fow = FormOfWay.SingleCarriageWay;
                    }
                }
                else if (baansubsrt == "nrb" ||
                    baansubsrt == "mrb")
                {
                    frc = FunctionalRoadClass.Frc3;
                    fow = FormOfWay.Roundabout;
                }
                else if (baansubsrt == "opr" ||
                    baansubsrt == "afr")
                {
                    frc = FunctionalRoadClass.Frc3;
                    fow = FormOfWay.SlipRoad;
                }
                else if (baansubsrt == "pst" ||
                    baansubsrt.StartsWith("vb"))
                {
                    frc = FunctionalRoadClass.Frc3;
                    fow = FormOfWay.SlipRoad;
                }
            }
            return true;
        }
    }
}