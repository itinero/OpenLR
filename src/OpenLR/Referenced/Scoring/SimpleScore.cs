namespace OpenLR.Referenced.Scoring;

/// <summary>
/// Represents a score with a single value and a single name.
/// </summary>
public class SimpleScore : Score
{
    /// <summary>
    /// Creates a new simple score.
    /// </summary>
    internal SimpleScore(string name, string description, double value, double reference)
    {
        this.Name = name;
        this.Description = description;
        this.Value = value;
        this.Reference = reference;
    }

    /// <summary>
    /// Gets the name of this score.
    /// </summary>
    public override string Name { get; }

    /// <summary>
    /// Gets the description of this score.
    /// </summary>
    public override string Description { get; }

    /// <summary>
    /// Gets the value of this score.
    /// </summary>
    public override double Value { get; }

    /// <summary>
    /// Gets the references of this score.
    /// </summary>
    public override double Reference { get; }

    /// <summary>
    /// Gets a score by name.
    /// </summary>
    public override Score? TryGetByName(string key)
    {
        if(this.Name == key)
        {
            return this;
        }
        return null;
    }
}
