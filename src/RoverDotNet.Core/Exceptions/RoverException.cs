namespace RoverDotNet.Core.Exceptions;

/// <summary>
/// Base class for all RoverDotNet exceptions.
/// </summary>
public class RoverException : Exception
{
    /// <inheritdoc />
    public RoverException(string message) : base(message) { }

    /// <inheritdoc />
    public RoverException(string message, Exception innerException)
        : base(message, innerException) { }
}
