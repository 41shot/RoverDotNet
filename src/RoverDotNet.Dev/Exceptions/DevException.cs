using RoverDotNet.Core.Exceptions;

namespace RoverDotNet.Dev.Exceptions;

/// <summary>
/// Base exception for all dev session-related errors.
/// Mirrors the error-handling patterns in <c>src/command/dev/</c>.
/// </summary>
public class DevException : RoverException
{
    /// <inheritdoc />
    public DevException(string message) : base(message)
    {
    }

    /// <inheritdoc />
    public DevException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
