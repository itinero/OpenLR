namespace OpenLR.Referenced.Scoring;

/// <summary>
/// Represents an added score, a score that is the sum of two others.
/// </summary>
public class AddedScore : Score
{
    /// <summary>
    /// Creates a new added score.
    /// </summary>
    internal AddedScore(Score left, Score right)
    {
        this.Left = left;
        this.Right = right;
    }

    /// <summary>
    /// Returns the left-side of this sum.
    /// </summary>
    public Score Left { get; }

    /// <summary>
    /// Returns the right side of this sum.
    /// </summary>
    public Score Right { get; }

    /// <summary>
    /// Returns the name of this score.
    /// </summary>
    public override string Name
    {
        get { return "[" + this.Left.Name + "] + [" + this.Right.Name + "]"; }
    }

    /// <summary>
    /// Returns the description of this score.
    /// </summary>
    public override string Description
    {
        get { return "[" + this.Left.Description + "] + [" + this.Right.Description + "]"; }
    }

    /// <summary>
    /// Returns the value of this score.
    /// </summary>
    public override double Value
    {
        get { return this.Left.Value + this.Right.Value; }
    }

    /// <summary>
    /// Returns the reference value of this score.
    /// </summary>
    public override double Reference
    {
        get { return this.Left.Reference + this.Right.Reference; }
    }

    /// <summary>
    /// Gets a score by name.
    /// </summary>
    public override Score? TryGetByName(string key)
    {
        var leftScore = this.Left.TryGetByName(key);
        var rightScore = this.Right.TryGetByName(key);
        if (leftScore == null || rightScore == null) return null;
        
        return leftScore + rightScore;
    }
}
