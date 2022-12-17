// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using Itinero.Attributes;
using Itinero.Profiles;
using OpenLR.Model;

namespace Samples.NWB
{
    /// <summary>
    /// An NWB coder profile.
    /// </summary>
    public class INwbCoderSettingsExtensions : OpenLR.CoderSettings
    {
        public static string BAANSUBSRT = "BST_CODE";
        public static string WEGBEHSRT = "WEGBEHSRT";
        public static string WEGNUMMER = "WEGNUMMER";
        public static string HECTOLTTR = "HECTO_LTTR";
        public static string RIJRICHTNG = "RIJRICHTNG";
        
        /// <summary>
        /// Creates a new NWB coder profile.
        /// </summary>
        public INwbCoderSettingsExtensions(Profile profile)
            : base(profile, 0.3f, 7500)
        {

        }

        /// <summary>
        /// Extracts nwb/fow.
        /// </summary>
        public override bool Extract(IAttributeCollection attributes, out FunctionalRoadClass frc, out FormOfWay fow)
        {
            fow = FormOfWay.Undefined;
            frc = FunctionalRoadClass.Frc7;
            string baansubsrt = string.Empty, wegbeerder = string.Empty, wegnummer = string.Empty, rijrichting = string.Empty, dvkletter_ = string.Empty;
            if (!attributes.TryGetValue(BAANSUBSRT, out baansubsrt) &
                !attributes.TryGetValue(WEGBEHSRT, out wegbeerder) &
                !attributes.TryGetValue(WEGNUMMER, out wegnummer) &
                !attributes.TryGetValue(HECTOLTTR, out dvkletter_) &
                !attributes.TryGetValue(RIJRICHTNG, out rijrichting))
            { // not even a BAANSUBSRT tag!
                // defaults: FRC5, OTHER.
                fow = FormOfWay.Other;
                frc = FunctionalRoadClass.Frc5;
                return true;
            }

            // make sure everything is lowercase.
            char? dvkletter = null; // assume dkv letter is the suffix used for exits etc. see: http://www.wegenwiki.nl/Hectometerpaal#Suffix
            if (!string.IsNullOrWhiteSpace(wegbeerder)) { wegbeerder = wegbeerder.ToLowerInvariant(); }
            if (!string.IsNullOrWhiteSpace(baansubsrt)) { baansubsrt = baansubsrt.ToLowerInvariant(); }
            if (!string.IsNullOrWhiteSpace(wegnummer)) { wegnummer = wegnummer.ToLowerInvariant(); if (!string.IsNullOrEmpty(dvkletter_)) dvkletter = dvkletter_[0]; }
            if (!string.IsNullOrWhiteSpace(rijrichting)) { rijrichting = rijrichting.ToLowerInvariant(); }

            fow = FormOfWay.Other;
            frc = FunctionalRoadClass.Frc5;
            if (wegbeerder == "r")
            {
                if (baansubsrt == "hr")
                {
                    fow = FormOfWay.Motorway;
                    frc = FunctionalRoadClass.Frc0;
                }
                else if (baansubsrt == "nrb" ||
                    baansubsrt == "mrb")
                {
                    fow = FormOfWay.Roundabout;
                    frc = FunctionalRoadClass.Frc0;
                }
                else if (baansubsrt == "pst")
                {
                    if (dvkletter.HasValue)
                    {
                        fow = FormOfWay.SlipRoad;
                        if (dvkletter == 'a' ||
                            dvkletter == 'b' ||
                            dvkletter == 'c' ||
                            dvkletter == 'd')
                        { // r  pst (a|b|c|d)
                            frc = FunctionalRoadClass.Frc3;
                        }
                        else
                        { // r  pst !(a|b|c|d)
                            frc = FunctionalRoadClass.Frc0;
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(rijrichting))
                    { // r  pst !(a|b|c|d)
                        fow = FormOfWay.SlipRoad;
                        frc = FunctionalRoadClass.Frc0;
                    }
                }
                else if (baansubsrt == "opr" ||
                    baansubsrt == "afr")
                {
                    frc = FunctionalRoadClass.Frc3;
                    fow = FormOfWay.SlipRoad;
                }
                else if (baansubsrt.StartsWith("vb"))
                {
                    if (!string.IsNullOrWhiteSpace(rijrichting))
                    {
                        fow = FormOfWay.SlipRoad;
                        frc = FunctionalRoadClass.Frc0;
                    }
                }
            }
            else if (wegbeerder == "p")
            {
                if (baansubsrt == "hr")
                {
                    frc = FunctionalRoadClass.Frc3;
                    fow = FormOfWay.MultipleCarriageWay;
                    if (string.IsNullOrWhiteSpace(rijrichting))
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