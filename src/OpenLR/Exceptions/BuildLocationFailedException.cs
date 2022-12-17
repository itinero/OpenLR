using System;

namespace OpenLR.Exceptions;

/// <summary>
/// An exception type thrown when 
/// </summary>
public class BuildLocationFailedException : Exception
{
    /// <summary>
    /// Creates a new exception.
    /// </summary>
    public BuildLocationFailedException(string message)
        : base(message)
    {

    }

    /// <summary>
    /// Creates a new exception.
    /// </summary>
    public BuildLocationFailedException(string message, params object[] args)
        : base(string.Format(message, args))
    {

    }
}