namespace OpenLR.Referenced.Scoring;

/// <summary>
/// Represents a score for a specific purpose.
/// </summary>
public abstract class Score
{
    /// <summary>
    /// Holds the candidate route score name.
    /// </summary>
    public const string CandidateRoute = "candidate_route";

    /// <summary>
    /// Holds the distance comparison score name.
    /// </summary>
    public const string DistanceComparison = "distance_comparison";

    /// <summary>
    /// Holds the vertex distance score name.
    /// </summary>
    public const string VertexDistance = "vertex_distance";

    /// <summary>
    /// Holds the match arc score name.
    /// </summary>
    public const string MatchArc = "match_arc";

    /// <summary>
    /// Holds the bearing difference.
    /// </summary>
    public const string BearingDiff = "bearing_diff";

    /// <summary>
    /// Gets the name of this score.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the description of this score.
    /// </summary>
    public abstract string Description { get; }

    /// <summary>
    /// Gets or the score of this score.
    /// </summary>
    public abstract double Value { get; }

    /// <summary>
    /// Gets or sets the reference value of this score.
    /// </summary>
    /// <remarks>If value is max 1, this contains 1.</remarks>
    public abstract double Reference { get; }

    /// <summary>
    /// Gets a score by name.
    /// </summary>
    public abstract Score? TryGetByName(string key);

    /// <summary>
    /// Creates a new score.
    /// </summary>
    public static SimpleScore New(string name, string description, double value, double reference)
    {
        return new SimpleScore(name, description, value, reference);
    }

    /// <summary>
    /// Multiplies the two given scores.
    /// </summary>
    public static MultipliedScore operator *(Score left, Score right)
    {
        return new MultipliedScore(left, right);
    }

    /// <summary>
    /// Adds the two given scores.
    /// </summary>
    public static AddedScore operator +(Score left, Score right)
    {
        return new AddedScore(left, right);
    }

    /// <summary>
    /// Returns a description of this score.
    /// </summary>
    public override string ToString()
    {
        return $"{this.Name} {this.Value}/{this.Reference}";
    }
        
    /// <summary>
    /// Returns the hashcode.
    /// </summary>
    public override int GetHashCode()
    {
        return this.Name.GetHashCode() ^
               this.Description.GetHashCode() ^
               this.Value.GetHashCode() ^
               this.Reference.GetHashCode();
    }

    /// <summary>
    /// Returns true if the given object equals this object.
    /// </summary>
    public override bool Equals(object obj)
    {
        if(obj is Score otherScore)
        {
            return otherScore.Name.Equals(this.Name) &&
                   otherScore.Description.Equals(this.Description) &&
                   otherScore.Value.Equals(this.Value) &&
                   otherScore.Reference.Equals(this.Reference);
        }
        return false;
    }
}
