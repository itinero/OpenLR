namespace OpenLR;

/// <summary>
/// Contains settings to influence encoding.
/// </summary>
public class EncodingSettings
{
    /// <summary>
    /// Creates encoding settings.
    /// </summary>
    public EncodingSettings(bool verifyShortestPath = true)
    {
        this.VerifyShortestPath = verifyShortestPath;
    }
        
    /// <summary>
    /// Gets the default settings.
    /// </summary>
    public static readonly EncodingSettings Default = new ();

    /// <summary>
    /// Gets or sets the verify shortest path flag.
    /// </summary>
    public bool VerifyShortestPath { get; }
}
