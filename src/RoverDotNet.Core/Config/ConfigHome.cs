namespace RoverDotNet.Core.Config;

/// <summary>
/// Resolves the Apollo configuration home directory.
/// Mirrors the path logic in <c>houston::Config::new()</c>.
/// </summary>
/// <remarks>
/// Precedence: explicit override → <c>APOLLO_CONFIG_HOME</c> env var → OS default.
/// Defaults: Windows <c>%APPDATA%\Apollo\Rover\config</c>,
/// macOS <c>~/Library/Application Support/com.Apollo.Rover</c>,
/// Linux <c>~/.config/rover</c> (respects <c>XDG_CONFIG_HOME</c>).
/// </remarks>
public static class ConfigHome
{
    internal const string EnvVarName = "APOLLO_CONFIG_HOME";

    /// <summary>
    /// Returns the resolved configuration home directory path.
    /// The directory is <b>not</b> created by this method.
    /// </summary>
    public static string Resolve(string? overridePath = null)
    {
        if (overridePath is not null)
            return overridePath;

        var envOverride = Environment.GetEnvironmentVariable(EnvVarName);
        if (!string.IsNullOrEmpty(envOverride))
            return envOverride;

        return GetOsDefault();
    }

    private static string GetOsDefault()
    {
        if (OperatingSystem.IsWindows())
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Apollo", "Rover", "config");

        if (OperatingSystem.IsMacOS())
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Library", "Application Support", "com.Apollo.Rover");

        // Linux — respect XDG_CONFIG_HOME, fall back to ~/.config
        var xdgConfigHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        var configBase = !string.IsNullOrEmpty(xdgConfigHome)
            ? xdgConfigHome
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config");

        return Path.Combine(configBase, "rover");
    }
}
