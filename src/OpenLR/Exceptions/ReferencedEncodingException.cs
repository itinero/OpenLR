using System;
using OpenLR.Referenced;

namespace OpenLR.Exceptions;

/// <summary>
/// Represents a referenced encoding exception.
/// </summary>
public class ReferencedEncodingException : Exception
{
    private readonly ReferencedLocation _referencedLocation;

    /// <summary>
    /// Creates a new referenced encoding exception.
    /// </summary>
    public ReferencedEncodingException(ReferencedLocation location, string message, Exception innerException)
        : base(message, innerException)
    {
        _referencedLocation = location;
    }
    /// <summary>
    /// Creates a new referenced encoding exception.
    /// </summary>
    public ReferencedEncodingException(ReferencedLocation location, string message)
        : base(message)
    {
        _referencedLocation = location;
    }

    /// <summary>
    /// Returns the location that was being encoded.
    /// </summary>
    public ReferencedLocation Location
    {
        get
        {
            return _referencedLocation;
        }
    }
}