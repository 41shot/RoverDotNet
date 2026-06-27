namespace RoverDotNet.Dev.Models;

/// <summary>
/// Represents the current state of a <see cref="DevSession"/>.
/// </summary>
public enum DevSessionState
{
    /// <summary>Session has not started yet.</summary>
    Idle,

    /// <summary>Session is starting (initial composition and router launch).</summary>
    Starting,

    /// <summary>Router is running and serving requests.</summary>
    Running,

    /// <summary>Recomposing the supergraph due to schema changes.</summary>
    Composing,

    /// <summary>Session is shutting down.</summary>
    Stopping,

    /// <summary>Session has stopped.</summary>
    Stopped,

    /// <summary>Session encountered a fatal error.</summary>
    Error
}
