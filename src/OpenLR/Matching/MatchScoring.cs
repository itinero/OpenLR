using System;
using OpenLR.Model;

namespace OpenLR.Matching;

/// <summary>
/// Contains a matching/scoring mechanism matching two edges with FOWs and FRCs.
/// </summary>
public static class MatchScoring
{
    /// <summary>
    /// Calculates a matching and score by comparing the expected against the actual FRCs and FOWs.
    /// </summary>
    /// <param name="expectedFrc"></param>
    /// <param name="expectedFow"></param>
    /// <param name="actualFrc"></param>
    /// <param name="actualFow"></param>
    /// <returns></returns>
    public static double MatchAndScore(FunctionalRoadClass expectedFrc, FormOfWay expectedFow,
        FunctionalRoadClass actualFrc, FormOfWay actualFow)
    {
        if (expectedFow == actualFow && expectedFrc == actualFrc)
        { // perfect score.
            return 1;
        }

        // sore frc and fow separately and take frc for 75% and fow for 25%.
        const double frcWeight = .75;
        const double fowWeight = .25;
        return MatchScoring.MatchAndScore(expectedFrc, actualFrc) * frcWeight +
               MatchScoring.MatchAndScore(expectedFow, actualFow) * fowWeight;
    }

    /// <summary>
    /// Calculates a matching and score by comparing the expected agains the actual FRC's.
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="actual"></param>
    /// <returns></returns>
    public static double MatchAndScore(FunctionalRoadClass expected, FunctionalRoadClass actual)
    {
        if (expected == actual)
        { // perfect score for frc.
            return 1;
        }

        const double frcOneDifferent = 0.8;
        const double frcTwoDifferent = 0.6;
        const double frcMoreDifferent = 0.2;

        switch (expected)
        {
            case FunctionalRoadClass.Frc0:
                return actual switch
                {
                    FunctionalRoadClass.Frc1 => frcOneDifferent,
                    FunctionalRoadClass.Frc2 => frcTwoDifferent,
                    _ => frcMoreDifferent
                };
            case FunctionalRoadClass.Frc1:
                switch (actual)
                {
                    case FunctionalRoadClass.Frc0:
                    case FunctionalRoadClass.Frc2:
                        return frcOneDifferent;
                    case FunctionalRoadClass.Frc3:
                        return frcTwoDifferent;
                    case FunctionalRoadClass.Frc1:
                    case FunctionalRoadClass.Frc4:
                    case FunctionalRoadClass.Frc5:
                    case FunctionalRoadClass.Frc6:
                    case FunctionalRoadClass.Frc7:
                    default:
                        return frcMoreDifferent;
                }
            case FunctionalRoadClass.Frc2:
                switch (actual)
                {
                    case FunctionalRoadClass.Frc1:
                    case FunctionalRoadClass.Frc3:
                        return frcOneDifferent;
                    case FunctionalRoadClass.Frc4:
                    case FunctionalRoadClass.Frc0:
                        return frcTwoDifferent;
                    case FunctionalRoadClass.Frc2:
                    case FunctionalRoadClass.Frc5:
                    case FunctionalRoadClass.Frc6:
                    case FunctionalRoadClass.Frc7:
                    default:
                        return frcMoreDifferent;
                }
            case FunctionalRoadClass.Frc3:
                switch (actual)
                {
                    case FunctionalRoadClass.Frc2:
                    case FunctionalRoadClass.Frc4:
                        return frcOneDifferent;
                    case FunctionalRoadClass.Frc1:
                    case FunctionalRoadClass.Frc5:
                        return frcTwoDifferent;
                    case FunctionalRoadClass.Frc0:
                    case FunctionalRoadClass.Frc3:
                    case FunctionalRoadClass.Frc6:
                    case FunctionalRoadClass.Frc7:
                    default:
                        return frcMoreDifferent;
                }
            case FunctionalRoadClass.Frc4:
                switch (actual)
                {
                    case FunctionalRoadClass.Frc3:
                    case FunctionalRoadClass.Frc5:
                        return frcOneDifferent;
                    case FunctionalRoadClass.Frc2:
                    case FunctionalRoadClass.Frc6:
                        return frcTwoDifferent;
                    case FunctionalRoadClass.Frc0:
                    case FunctionalRoadClass.Frc1:
                    case FunctionalRoadClass.Frc4:
                    case FunctionalRoadClass.Frc7:
                    default:
                        return frcMoreDifferent;
                }
            case FunctionalRoadClass.Frc5:
                switch (actual)
                {
                    case FunctionalRoadClass.Frc4:
                    case FunctionalRoadClass.Frc6:
                        return frcOneDifferent;
                    case FunctionalRoadClass.Frc3:
                    case FunctionalRoadClass.Frc7:
                        return frcTwoDifferent;
                    case FunctionalRoadClass.Frc0:
                    case FunctionalRoadClass.Frc1:
                    case FunctionalRoadClass.Frc2:
                    case FunctionalRoadClass.Frc5:
                    default:
                        return frcMoreDifferent;
                }
            case FunctionalRoadClass.Frc6:
                switch (actual)
                {
                    case FunctionalRoadClass.Frc5:
                    case FunctionalRoadClass.Frc7:
                        return frcOneDifferent;
                    case FunctionalRoadClass.Frc4:
                        return frcTwoDifferent;
                    case FunctionalRoadClass.Frc0:
                    case FunctionalRoadClass.Frc1:
                    case FunctionalRoadClass.Frc2:
                    case FunctionalRoadClass.Frc3:
                    case FunctionalRoadClass.Frc6:
                    default:
                        return frcMoreDifferent;
                }
            case FunctionalRoadClass.Frc7:
                return actual switch
                {
                    FunctionalRoadClass.Frc6 => frcOneDifferent,
                    FunctionalRoadClass.Frc5 => frcTwoDifferent,
                    _ => frcMoreDifferent
                };
            default:
                throw new ArgumentOutOfRangeException(nameof(expected), expected, null);
        }
    }

    /// <summary>
    /// Calculates a matching and score by comparing the expected agains the actual FOW's.
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="actual"></param>
    /// <returns></returns>
    public static double MatchAndScore(FormOfWay expected, FormOfWay actual)
    {
        if (expected == actual)
        { // perfect score.
            return 1;
        }

        const double scoreMatch = 0.8;
        const double scoreAlmostMatch = 0.6;
        const double other = 0.2;

        switch (expected)
        {
            case FormOfWay.Motorway:
                return actual switch
                {
                    FormOfWay.MultipleCarriageWay => scoreMatch,
                    FormOfWay.SingleCarriageWay => scoreAlmostMatch,
                    _ => other
                };
            case FormOfWay.MultipleCarriageWay:
                return actual switch
                {
                    FormOfWay.Motorway => scoreMatch,
                    FormOfWay.SingleCarriageWay => scoreMatch,
                    _ => other
                };
            case FormOfWay.SingleCarriageWay:
                return actual switch
                {
                    FormOfWay.Motorway => scoreAlmostMatch,
                    FormOfWay.MultipleCarriageWay => scoreMatch,
                    _ => other
                };
            case FormOfWay.Roundabout:
                return actual switch
                {
                    FormOfWay.SingleCarriageWay => scoreAlmostMatch,
                    FormOfWay.SlipRoad => scoreMatch,
                    _ => other
                };
            case FormOfWay.SlipRoad:
                return actual switch
                {
                    FormOfWay.SingleCarriageWay => scoreAlmostMatch,
                    FormOfWay.Roundabout => scoreMatch,
                    _ => other
                };
            case FormOfWay.Undefined:
            case FormOfWay.TrafficSquare:
            case FormOfWay.Other:
            default:
                return other;
        }
    }
}
