using System;

namespace OpenLR.Scoring;

/// <summary>
/// Represents a score for a specific purpose.
/// </summary>
public class Score : IComparable<Score>
{
    /// <summary>
    /// Creates a new simple score.
    /// </summary>
    internal Score(string name, string description, double value, double reference)
    {
        this.Name = name;
        this.Description = description;
        this.Value = value;
        this.Reference = reference;
    }

    /// <summary>
    /// Gets the name of this score.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the description of this score.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets or the score of this score.
    /// </summary>
    public double Value { get; }

    /// <summary>
    /// Gets or sets the reference value of this score.
    /// </summary>
    /// <remarks>If value is max 1, this contains 1.</remarks>
    public double Reference { get; }

    /// <summary>
    /// Returns true if score is perfect.
    /// </summary>
    public bool IsPerfect
    {
        get
        {
            return this.Value >= this.Reference;
        }
    }

    /// <summary>
    /// Gets a score by name.
    /// </summary>
    public virtual Score? TryGetByName(string key)
    {
        if(this.Name == key)
        {
            return this;
        }
        return null;
    }

    /// <summary>
    /// Creates a new score.
    /// </summary>
    public static Score New(string name, string description, double value, double reference)
    {
        return new Score(name, description, value, reference);
    }

    /// <summary>
    /// Multiplies the two given scores.
    /// </summary>
    public static ScoresMultiplied operator *(Score left, Score right)
    {
        return new ScoresMultiplied(left, right);
    }

    /// <summary>
    /// Adds the two given scores.
    /// </summary>
    public static ScoresSum operator +(Score left, Score right)
    {
        return new ScoresSum(left, right);
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

    /// <inhertidoc/>
    public int CompareTo(Score other)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (this.Reference == other.Reference)
        {
            return -this.Value.CompareTo(other.Value);
        }

        var thisValue = this.Value / this.Reference;
        var otherValue = other.Value / this.Reference;
        return -thisValue.CompareTo(otherValue);
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

    /// <summary>
    /// Returns an infinite score.
    /// </summary>
    public static Score Infinite
    {
        get
        {
            return new Score("infinite", "A score without a meaning, just infinite", double.PositiveInfinity,
                double.PositiveInfinity);
        }
    }

    /// <summary>
    /// Returns an zero score.
    /// </summary>
    public static Score Zero
    {
        get
        {
            return new Score("zero", "A score without a meaning, just zero", 0,1);
        }
    }
    
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
}
