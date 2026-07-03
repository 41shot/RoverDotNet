namespace RoverDotNet.Core.Config;

/// <summary>
/// Provides cross-platform environment variable access with scope-priority resolution.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="GetValue"/> searches Process → User → Machine scopes in priority order.
/// </para>
/// <para>
/// <see cref="SetValue"/> targets User scope on Windows; falls back to Process scope
/// on Linux/macOS where user-level persistence is not supported by .NET APIs.
/// </para>
/// </remarks>
public static class EnvironmentVariableHelper
{
    /// <summary>
    /// Gets an environment variable value by checking all available scopes in priority order.
    /// </summary>
    /// <param name="key">The environment variable name.</param>
    /// <returns>
    /// The first non-null value found when checking Process, User, then Machine scopes;
    /// <see langword="null"/> if the variable is not defined in any scope.
    /// </returns>
    /// <remarks>
    /// Checks scopes in order: Process → User → Machine.
    /// On Unix systems, User and Machine scopes may not contain values set outside the current process.
    /// </remarks>
    public static string? GetValue(string key)
    {
        // Priority 1: Process scope (current process environment)
        var value = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
        if (value is not null)
            return value;

        // Priority 2: User scope (current user, persisted across sessions on Windows)
        try
        {
            value = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.User);
            if (value is not null)
                return value;
        }
        catch (PlatformNotSupportedException)
        {
            // User scope not supported on this platform (Linux/macOS)
        }

        // Priority 3: Machine scope (all users, requires admin on Windows)
        try
        {
            value = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Machine);
            if (value is not null)
                return value;
        }
        catch (PlatformNotSupportedException)
        {
            // Machine scope not supported on this platform (Linux/macOS)
        }

        return null;
    }

    /// <summary>
    /// Sets an environment variable to the User scope if supported, otherwise Process scope.
    /// </summary>
    /// <param name="key">The environment variable name.</param>
    /// <param name="value">The value to set. Pass <see langword="null"/> to remove the variable.</param>
    /// <remarks>
    /// <para>
    /// On Windows, sets the variable in User scope (persisted in registry, available to future processes).
    /// </para>
    /// <para>
    /// On Linux/macOS, sets the variable in Process scope only (affects current process and child processes,
    /// not persisted). User-level persistence on Unix requires shell profile modifications not provided by .NET.
    /// </para>
    /// </remarks>
    public static void SetValue(string key, string? value)
    {
        try
        {
            // Attempt User scope first (works on Windows)
            Environment.SetEnvironmentVariable(key, value, EnvironmentVariableTarget.User);
        }
        catch (PlatformNotSupportedException)
        {
            // User scope not supported (Linux/macOS) - fall back to Process scope
            Environment.SetEnvironmentVariable(key, value, EnvironmentVariableTarget.Process);
        }
    }
}
