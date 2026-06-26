using System.Text.Json.Serialization;
using RoverDotNet.Core.Credentials;
using RoverDotNet.Core.Exceptions;
using Tomlyn;

namespace RoverDotNet.Core.Config;

/// <summary>
/// Reads and writes per-profile API key configuration on the local filesystem.
/// Mirrors <c>houston::Profile</c> and <c>houston::Config</c>.
/// </summary>
/// <remarks>
/// Profiles are stored as TOML files under:
/// <c>&lt;config_home&gt;/profiles/&lt;name&gt;/.sensitive</c>
/// <para>
/// The <c>APOLLO_KEY</c> environment variable, when set, overrides all
/// profile-based credentials and is returned with <see cref="CredentialOrigin.EnvVar"/>.
/// </para>
/// </remarks>
public sealed class ProfileConfig
{
    /// <summary>The environment variable that overrides the API key for all profiles.</summary>
    public const string ApiKeyEnvVar = "APOLLO_KEY";

    /// <summary>The name used when no explicit profile is specified.</summary>
    public const string DefaultProfile = "default";

    private readonly string _configHome;

    /// <param name="overridePath">
    /// Optional override for the config home directory (useful in tests).
    /// When <see langword="null"/>, <see cref="ConfigHome.Resolve"/> determines the path.
    /// </param>
    public ProfileConfig(string? overridePath = null)
    {
        _configHome = ConfigHome.Resolve(overridePath);
        Directory.CreateDirectory(_configHome);
    }

    /// <summary>
    /// Returns the credential for the named profile.
    /// </summary>
    /// <remarks>
    /// If the <c>APOLLO_KEY</c> environment variable is set, its value is returned
    /// with <see cref="CredentialOrigin.EnvVar"/>, but only after verifying that
    /// the requested profile exists (matching Rover's behavior).
    /// </remarks>
    /// <exception cref="NoProfilesException">No profiles exist at all.</exception>
    /// <exception cref="ProfileNotFoundException">The named profile does not exist.</exception>
    /// <exception cref="CorruptedProfileException">The stored key is corrupt.</exception>
    public Credential GetCredential(string profileName = DefaultProfile)
    {
        // Check that the profile exists first, even if we won't use it.
        // This matches Rover's behavior: it requires at least one profile to exist,
        // even when APOLLO_KEY is set.
        CheckProfileExists(profileName);

        var envApiKey = Environment.GetEnvironmentVariable(ApiKeyEnvVar);
        if (!string.IsNullOrEmpty(envApiKey))
            return new Credential(envApiKey, new CredentialOrigin.EnvVar());

        var apiKey = LoadApiKey(profileName);
        return new Credential(apiKey, new CredentialOrigin.ConfigFile(profileName));
    }

    /// <summary>
    /// Persists an API key for the named profile.
    /// Creates the profile directory if it does not already exist.
    /// </summary>
    public void SetApiKey(string profileName, string apiKey)
    {
        var dir = GetProfileDir(profileName);
        Directory.CreateDirectory(dir);
        File.WriteAllText(GetSensitivePath(profileName), BuildToml(apiKey));
    }

    /// <summary>
    /// Returns the names of all existing profiles in alphabetical order.
    /// Returns an empty list when no profiles directory exists yet.
    /// </summary>
    public IReadOnlyList<string> ListProfiles()
    {
        var profilesDir = Path.Combine(_configHome, "profiles");
        if (!Directory.Exists(profilesDir))
            return [];

        return Directory
            .GetDirectories(profilesDir)
            .Select(Path.GetFileName)
            .Where(n => n is not null)
            .Order()
            .ToList()!;
    }

    /// <summary>Deletes the named profile directory and all its contents.</summary>
    /// <exception cref="ProfileNotFoundException">The named profile does not exist.</exception>
    public void DeleteProfile(string profileName)
    {
        var dir = GetProfileDir(profileName);
        if (!Directory.Exists(dir))
            throw new ProfileNotFoundException(profileName);

        Directory.Delete(dir, recursive: true);
    }

    /// <summary>Removes the entire configuration home directory and all profiles within it.</summary>
    public void Clear()
    {
        if (Directory.Exists(_configHome))
            Directory.Delete(_configHome, recursive: true);
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private void CheckProfileExists(string profileName)
    {
        var profilesDir = Path.Combine(_configHome, "profiles");
        var profileDir = GetProfileDir(profileName);

        if (!Directory.Exists(profileDir))
        {
            if (!Directory.Exists(profilesDir) || !Directory.EnumerateDirectories(profilesDir).Any())
                throw new NoProfilesException();

            throw new ProfileNotFoundException(profileName);
        }
    }

    private string LoadApiKey(string profileName)
    {
        // Profile existence is already checked by CheckProfileExists, so we can
        // assume the profile directory exists here.
        var path = GetSensitivePath(profileName);
        if (!File.Exists(path))
            throw new ProfileNotFoundException(profileName);

        var content = File.ReadAllText(path);
        SensitiveToml? sensitive;
        try
        {
            sensitive = TomlSerializer.Deserialize<SensitiveToml>(content);
        }
        catch
        {
            throw new CorruptedProfileException(profileName);
        }

        if (sensitive is null || string.IsNullOrEmpty(sensitive.ApiKey))
            throw new CorruptedProfileException(profileName);

        // Old Rover versions on Windows could produce a Ctrl+V byte (0x16) as the key value
        if (sensitive.ApiKey.Length == 1 && sensitive.ApiKey[0] == '\x16')
            throw new CorruptedProfileException(profileName);

        return sensitive.ApiKey;
    }

    private string GetProfileDir(string profileName)
        => Path.Combine(_configHome, "profiles", profileName);

    private string GetSensitivePath(string profileName)
        => Path.Combine(GetProfileDir(profileName), ".sensitive");

    private static string BuildToml(string apiKey)
        => TomlSerializer.Serialize(new SensitiveToml { ApiKey = apiKey });

    // -------------------------------------------------------------------------
    // TOML DTO — mirrors houston's Sensitive struct: api_key = "..."
    // -------------------------------------------------------------------------

    private sealed class SensitiveToml
    {
        [JsonPropertyName("api_key")]
        public string ApiKey { get; set; } = string.Empty;
    }
}
