using Itinero.Snapping;
using OpenLR.Referenced.Scoring;

namespace OpenLR.Referenced.Codecs.Candidates;

/// <summary>
/// Represents a candidate snap point and associated score.
/// </summary>
public class CandidateSnapPoint
{
    /// <summary>
    /// Creates a new candidate.
    /// </summary>
    /// <param name="location">The location.</param>
    /// <param name="score">The score.</param>
    public CandidateSnapPoint(SnapPoint location, Score score)
    {
        this.Score = score;
        this.Location = location;
    }

    /// <summary>
    /// The combined score of vertex and edge.
    /// </summary>
    public Score Score { get; }

    /// <summary>
    /// Gets or sets the candidate location.
    /// </summary>
    public SnapPoint Location { get; }

    /// <summary>
    /// Determines whether this object is equal to the given object.
    /// </summary>
    public override bool Equals(object obj)
    {
        var other = (obj as CandidateSnapPoint);
        return other != null && other.Score == this.Score && 
               other.Location.EdgeId == this.Location.EdgeId &&
               other.Location.Offset == this.Location.Offset;
    }

    /// <summary>
    /// Serves as a hash function.
    /// </summary>
    public override int GetHashCode()
    {
        return this.Score.GetHashCode() ^
               this.Location.EdgeId.GetHashCode() ^
               this.Location.Offset.GetHashCode();
    }

    /// <summary>
    /// Returns a description of this candidate.
    /// </summary>
    public override string ToString()
    {
        return $"{this.Location.ToString()} ({this.Score.ToString()})";
    }
}
