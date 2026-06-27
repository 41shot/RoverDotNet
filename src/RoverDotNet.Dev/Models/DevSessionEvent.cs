namespace RoverDotNet.Dev.Models;

/// <summary>
/// Represents an event that occurred during a dev session.
/// </summary>
/// <param name="Timestamp">When the event occurred.</param>
/// <param name="State">The session state when the event occurred.</param>
/// <param name="Message">Human-readable event description.</param>
/// <param name="Exception">Optional exception if the event represents an error.</param>
public sealed record DevSessionEvent(
    DateTimeOffset Timestamp,
    DevSessionState State,
    string Message,
    Exception? Exception = null);
