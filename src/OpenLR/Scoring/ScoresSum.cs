namespace OpenLR.Scoring;

/// <summary>
/// Represents an added score, a score that is the sum of two others.
/// </summary>
public class ScoresSum : Score
{
    internal ScoresSum(Score left, Score right)
        : base("[" + left.Name + "] + [" + right.Name + "]", "[" + left.Description + "] + [" + right.Description + "]",
            left.Value + right.Value, left.Reference + right.Reference)
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
