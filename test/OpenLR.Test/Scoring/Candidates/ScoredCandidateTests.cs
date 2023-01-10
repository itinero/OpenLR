using NUnit.Framework;
using OpenLR.Scoring;
using OpenLR.Scoring.Candidates;

namespace OpenLR.Test.Scoring.Candidates;

public class ScoredCandidateTests
{
    [Test]
    public void ScoredCandidate_TwoInSortedList_ShouldSortBestToWorst()
    {
        var candidate1 = new ScoredCandidate<double>(0.2, Score.New(string.Empty, string.Empty, 0.2, 1));
        var candidate2 = new ScoredCandidate<double>(0.5, Score.New(string.Empty, string.Empty, 0.5, 1));

        var sortedSet = new SortedSet<ScoredCandidate<double>> { candidate1, candidate2 };

        var first = sortedSet.First();
        var second = sortedSet.Skip(1).First();
        Assert.Multiple(() =>
        {
            Assert.That(first.Score, Is.EqualTo(candidate2.Score));
            Assert.That(second.Score, Is.EqualTo(candidate1.Score));
        });
    }
}
