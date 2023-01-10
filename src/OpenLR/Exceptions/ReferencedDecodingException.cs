using OpenLR.Model.Locations;
using System;

namespace OpenLR.Exceptions;

/// <summary>
/// Represents a referenced decoding exception.
/// </summary>
public class ReferencedDecodingException : Exception
{
    /// <summary>
    /// Creates a new referenced decoding exception.
    /// </summary>
    public ReferencedDecodingException(ILocation location, string message, Exception innerException)
        : base(message, innerException)
    {
        this.Location = location;
    }

    /// <summary>
    /// Creates a new referenced decoding exception.
    /// </summary>
    public ReferencedDecodingException(ILocation location, string message)
        : base(message)
    {
        this.Location = location;
    }

    /// <summary>
    /// Returns the location that was being encoded.
    /// </summary>
    public ILocation Location { get; }
}
