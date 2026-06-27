namespace RoverDotNet.Dev.Exceptions;

/// <summary>
/// Thrown when the Apollo Router process fails to start, crashes, or cannot be managed.
/// </summary>
public sealed class RouterProcessException : DevException
{
    /// <inheritdoc />
    public RouterProcessException(string message) : base(message)
    {
    }

    /// <inheritdoc />
    public RouterProcessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
