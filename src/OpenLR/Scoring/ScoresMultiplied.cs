namespace OpenLR.Scoring;

/// <summary>
/// Represents a multiplied score, a score that is the multiplication of two others.
/// </summary>
public class ScoresMultiplied : Score
{
    internal ScoresMultiplied(Score left, Score right)
        : base("[" + left.Name + "] * [" + right.Name + "]", "[" + left.Description + "] * [" + right.Description + "]",
            left.Value * right.Value, left.Reference * right.Reference)
    {
        this.Left = left;
        this.Right = right;
    }

    /// <summary>
    /// Returns the left-side.
    /// </summary>
    public Score Left { get; }

    /// <summary>
    /// Returns the right side.
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
