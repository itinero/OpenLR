using System;

namespace OpenLR.Scoring.Candidates;

/// <summary>
/// A scored candidate.
/// </summary>
/// <typeparam name="T">the candidate type.</typeparam>
public class ScoredCandidate<T> : IComparable<ScoredCandidate<T>>
{
    /// <summary>
    /// Creates a new scored candidate.
    /// </summary>
    /// <param name="candidate">The candidate.</param>
    /// <param name="score">The score.</param>
    public ScoredCandidate(T candidate, Score score)
    {
        this.Candidate = candidate;
        this.Score = score;
    }

    /// <summary>
    /// The candidate.
    /// </summary>
    public T Candidate { get; }

    /// <summary>
    /// The score.
    /// </summary>
    public Score Score { get; }

    /// <summary>
    /// Returns a new candidate with the given score added.
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public ScoredCandidate<T> WithScore(Score score)
    {
        return new ScoredCandidate<T>(this.Candidate, this.Score + score);
    }

    /// <summary>
    /// Adds two candidates together.
    /// </summary>
    /// <param name="other"></param>
    /// <typeparam name="TOther"></typeparam>
    /// <returns></returns>
    public ScoredCandidate<(ScoredCandidate<T> left, ScoredCandidate<TOther> right)> Add<TOther>(ScoredCandidate<TOther> other)
    {
        return new ScoredCandidate<(ScoredCandidate<T> left, ScoredCandidate<TOther> right)>((this, other),
            this.Score + other.Score);
    }

    int IComparable<ScoredCandidate<T>>.CompareTo(ScoredCandidate<T> other)
    {
        return this.Score.CompareTo(other.Score);
    }

    /// <summary>
    /// Determines whether this object is equal to the given object.
    /// </summary>
    public override bool Equals(object obj)
    {
        if (obj is not ScoredCandidate<T> other) return false;

        if (!this.Score.Equals(other.Score)) return false;

        if (this.Candidate != null && other.Candidate != null)
        {
            return this.Candidate.Equals(other.Candidate);
        }

        return this.Candidate == null && other.Candidate == null;
    }

    /// <summary>
    /// Serves as a hash function.
    /// </summary>
    public override int GetHashCode()
    {
        return 24956 ^ this.Score.GetHashCode() ^
               (this.Candidate != null ? this.Candidate.GetHashCode() : 9637);
    }

    /// <summary>
    /// Returns a description of this candidate.
    /// </summary>
    public override string ToString()
    {
        return $"{this.Candidate?.ToString()} ({this.Score.ToString()})";
    }
}
