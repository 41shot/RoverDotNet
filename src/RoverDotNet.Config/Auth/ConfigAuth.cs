using RoverDotNet.Core.Config;
using RoverDotNet.Core.Exceptions;

namespace RoverDotNet.Config.Auth;

/// <summary>
/// Implements the <c>config auth</c> command: prompts for an API key,
/// validates it, and stores it in the named profile.
/// Mirrors <c>src/command/config/auth.rs</c>.
/// </summary>
public sealed class ConfigAuth
{
    private readonly ProfileConfig _profileConfig;
    private readonly IApiKeyPrompt _apiKeyPrompt;

    /// <param name="profileConfig">Provides credential storage to the local config store.</param>
    /// <param name="apiKeyPrompt">Prompts the user for an API key.</param>
    public ConfigAuth(ProfileConfig profileConfig, IApiKeyPrompt apiKeyPrompt)
    {
        _profileConfig = profileConfig;
        _apiKeyPrompt = apiKeyPrompt;
    }

    /// <summary>
    /// Prompts for an API key and saves it to the specified profile.
    /// </summary>
    /// <param name="profileName">
    /// The profile to authenticate. Defaults to <c>default</c>.
    /// </param>
    /// <param name="cancellationToken">Propagates cancellation.</param>
    /// <returns>
    /// An <see cref="AuthResult"/> indicating success and suggesting verification
    /// via <c>config whoami</c>.
    /// </returns>
    /// <exception cref="RoverException">
    /// The API key was empty or invalid.
    /// </exception>
    public async Task<AuthResult> ExecuteAsync(
        string profileName = ProfileConfig.DefaultProfile,
        CancellationToken cancellationToken = default)
    {
        // Prompt for the API key
        var apiKey = await _apiKeyPrompt.PromptAsync(cancellationToken);

        // Store the API key in the profile
        _profileConfig.SetApiKey(profileName, apiKey);

        // Verify storage by reading it back (matches Rover's behavior)
        _ = _profileConfig.GetCredential(profileName);

        return new AuthResult(
            profileName,
            $"Successfully saved API key for profile '{profileName}'. " +
            $"Consider running 'config whoami' to verify your API authentication.");
    }
}
