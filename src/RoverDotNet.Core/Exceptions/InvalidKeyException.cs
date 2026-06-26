namespace RoverDotNet.Core.Exceptions;

/// <summary>
/// Thrown when the supplied API key is not recognised by Apollo Studio,
/// or when the key represents an actor type that Rover does not accept.
/// Mirrors <c>WhoAmIError::InvalidKey</c> / <c>RoverClientError::InvalidKey</c>.
/// </summary>
public sealed class InvalidKeyException : RoverException
{
    /// <inheritdoc />
    public InvalidKeyException()
        : base("The provided API key is invalid.") { }

    /// <inheritdoc />
    public InvalidKeyException(string message) : base(message) { }
}
