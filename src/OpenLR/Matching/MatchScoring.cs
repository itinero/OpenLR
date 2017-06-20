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

using OpenLR.Model;

namespace OpenLR.Matching
{
    /// <summary>
    /// Contains a matching/scoring mechanism matching two edges with FOW's and FRC's.
    /// </summary>
    public static class MatchScoring
    {
        /// <summary>
        /// Calculates a matching and score by comparing the expected agains the actual FRC's and FOW's.
        /// </summary>
        /// <param name="expectedFrc"></param>
        /// <param name="expectedFow"></param>
        /// <param name="actualFrc"></param>
        /// <param name="actualFow"></param>
        /// <returns></returns>
        public static float MatchAndScore(FunctionalRoadClass expectedFrc, FormOfWay expectedFow,
            FunctionalRoadClass actualFrc, FormOfWay actualFow)
        {
            if (expectedFow == actualFow && expectedFrc == actualFrc)
            { // perfect score.
                return 1;
            }

            // sore frc and fow seperately and take frc for 75% and fow for 25%.
            float frcWeight = .75f, fowWeight = .25f;
            return MatchScoring.MatchAndScore(expectedFrc, actualFrc) * frcWeight +
                MatchScoring.MatchAndScore(expectedFow, actualFow) * fowWeight;
        }

        /// <summary>
        /// Calculates a matching and score by comparing the expected agains the actual FRC's.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static float MatchAndScore(FunctionalRoadClass expected, FunctionalRoadClass actual)
        {
            if (expected == actual)
            { // perfect score for frc.
                return 1;
            }
            else
            {
                float frcOneDifferent = 0.8f;
                float frcTwoDifferent = 0.6f;
                float frcMoreDifferent = 0.2f;

                switch (expected)
                {
                    case FunctionalRoadClass.Frc0:
                        if (actual == FunctionalRoadClass.Frc1)
                        {
                            return frcOneDifferent;
                        }
                        else if (actual == FunctionalRoadClass.Frc2)
                        {
                            return frcTwoDifferent;
                        }
                        else
                        {
                            return frcMoreDifferent;
                        }
                    case FunctionalRoadClass.Frc1:
                        if (actual == FunctionalRoadClass.Frc0 ||
                            actual == FunctionalRoadClass.Frc2)
                        {
                            return frcOneDifferent;
                        }
                        else if (actual == FunctionalRoadClass.Frc3)
                        {
                            return frcTwoDifferent;
                        }
                        else
                        {
                            return frcMoreDifferent;
                        }
                    case FunctionalRoadClass.Frc2:
                        if (actual == FunctionalRoadClass.Frc1 ||
                            actual == FunctionalRoadClass.Frc3)
                        {
                            return frcOneDifferent;
                        }
                        else if (actual == FunctionalRoadClass.Frc4 ||
                            actual == FunctionalRoadClass.Frc0)
                        {
                            return frcTwoDifferent;
                        }
                        else
                        {
                            return frcMoreDifferent;
                        }
                    case FunctionalRoadClass.Frc3:
                        if (actual == FunctionalRoadClass.Frc2 ||
                            actual == FunctionalRoadClass.Frc4)
                        {
                            return frcOneDifferent;
                        }
                        else if (actual == FunctionalRoadClass.Frc1 ||
                            actual == FunctionalRoadClass.Frc5)
                        {
                            return frcTwoDifferent;
                        }
                        else
                        {
                            return frcMoreDifferent;
                        }
                    case FunctionalRoadClass.Frc4:
                        if (actual == FunctionalRoadClass.Frc3 ||
                            actual == FunctionalRoadClass.Frc5)
                        {
                            return frcOneDifferent;
                        }
                        else if (actual == FunctionalRoadClass.Frc2 ||
                            actual == FunctionalRoadClass.Frc6)
                        {
                            return frcTwoDifferent;
                        }
                        else
                        {
                            return frcMoreDifferent;
                        }
                    case FunctionalRoadClass.Frc5:
                        if (actual == FunctionalRoadClass.Frc4 ||
                            actual == FunctionalRoadClass.Frc6)
                        {
                            return frcOneDifferent;
                        }
                        else if (actual == FunctionalRoadClass.Frc3 ||
                            actual == FunctionalRoadClass.Frc7)
                        {
                            return frcTwoDifferent;
                        }
                        else
                        {
                            return frcMoreDifferent;
                        }
                    case FunctionalRoadClass.Frc6:
                        if (actual == FunctionalRoadClass.Frc5 ||
                            actual == FunctionalRoadClass.Frc7)
                        {
                            return frcOneDifferent;
                        }
                        else if (actual == FunctionalRoadClass.Frc4)
                        {
                            return frcTwoDifferent;
                        }
                        else
                        {
                            return frcMoreDifferent;
                        }
                    case FunctionalRoadClass.Frc7:
                        if (actual == FunctionalRoadClass.Frc6)
                        {
                            return frcOneDifferent;
                        }
                        else if (actual == FunctionalRoadClass.Frc5)
                        {
                            return frcTwoDifferent;
                        }
                        else
                        {
                            return frcMoreDifferent;
                        }
                }
                return 0;
            }
        }

        /// <summary>
        /// Calculates a matching and score by comparing the expected agains the actual FOW's.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static float MatchAndScore(FormOfWay expected, FormOfWay actual)
        {
            if (expected == actual)
            { // perfect score.
                return 1;
            }

            float scoreMatch = 0.8f;
            float scoreAlmostMatch = 0.6f;
            float other = 0.2f;

            switch (expected)
            {
                case FormOfWay.Motorway:
                    switch (actual)
                    {
                        case FormOfWay.MultipleCarriageWay:
                            return scoreMatch;
                        case FormOfWay.SingleCarriageWay:
                            return scoreAlmostMatch;
                        default:
                            return other;
                    }
                case FormOfWay.MultipleCarriageWay:
                    switch (actual)
                    {
                        case FormOfWay.Motorway:
                            return scoreMatch;
                        case FormOfWay.SingleCarriageWay:
                            return scoreMatch;
                        default:
                            return other;
                    }
                case FormOfWay.SingleCarriageWay:
                    switch (actual)
                    {
                        case FormOfWay.Motorway:
                            return scoreAlmostMatch;
                        case FormOfWay.MultipleCarriageWay:
                            return scoreMatch;
                        default:
                            return other;
                    }
                case FormOfWay.Roundabout:
                    switch (actual)
                    {
                        case FormOfWay.SingleCarriageWay:
                            return scoreAlmostMatch;
                        case FormOfWay.SlipRoad:
                            return scoreMatch;
                        default:
                            return other;
                    }
                case FormOfWay.SlipRoad:
                    switch (actual)
                    {
                        case FormOfWay.SingleCarriageWay:
                            return scoreAlmostMatch;
                        case FormOfWay.Roundabout:
                            return scoreMatch;
                        default:
                            return other;
                    }
                default:
                    return other;
            }
        }
    }
}