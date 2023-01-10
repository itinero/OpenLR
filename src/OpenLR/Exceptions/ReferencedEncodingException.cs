using System;
using OpenLR.Referenced;

namespace OpenLR.Exceptions;

/// <summary>
/// Represents a referenced encoding exception.
/// </summary>
public class ReferencedEncodingException : Exception
{
    /// <summary>
    /// Creates a new referenced encoding exception.
    /// </summary>
    public ReferencedEncodingException(IReferencedLocation location, string message, Exception innerException)
        : base(message, innerException)
    {
        this.Location = location;
    }
    /// <summary>
    /// Creates a new referenced encoding exception.
    /// </summary>
    public ReferencedEncodingException(IReferencedLocation location, string message)
        : base(message)
    {
        this.Location = location;
    }

    /// <summary>
    /// Returns the location that was being encoded.
    /// </summary>
    public IReferencedLocation Location { get; }
}
