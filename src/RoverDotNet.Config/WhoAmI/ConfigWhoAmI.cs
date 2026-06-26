using RoverDotNet.Client.Operations.WhoAmI;
using RoverDotNet.Core.Config;
using RoverDotNet.Core.Credentials;
using RoverDotNet.Core.Exceptions;

namespace RoverDotNet.Config.WhoAmI;

/// <summary>
/// Implements the <c>config whoami</c> command: loads the credential for a profile,
/// verifies it against Apollo Studio, and returns a display-ready result.
/// Mirrors <c>src/command/config/whoami.rs</c>.
/// </summary>
public sealed class ConfigWhoAmI
{
    private readonly ProfileConfig _profileConfig;
    private readonly WhoAmIOperation _operation;

    /// <param name="profileConfig">Provides credential loading from the local config store.</param>
    /// <param name="operation">Executes the Studio <c>ConfigWhoAmIQuery</c>.</param>
    public ConfigWhoAmI(ProfileConfig profileConfig, WhoAmIOperation operation)
    {
        _profileConfig = profileConfig;
        _operation = operation;
    }

    /// <summary>
    /// Verifies the API key for <paramref name="profileName"/> against Apollo Studio
    /// and returns the identity details.
    /// </summary>
    /// <param name="profileName">
    /// The profile to authenticate. Defaults to <c>default</c>.
    /// </param>
    /// <param name="unmaskKey">
    /// When <see langword="true"/> the raw API key is included in the result.
    /// Defaults to <see langword="false"/> (key is masked).
    /// </param>
    /// <param name="cancellationToken">Propagates cancellation.</param>
    /// <exception cref="InvalidKeyException">
    /// The key is not recognised by Studio, or its actor type is not <c>User</c> or <c>Graph</c>.
    /// </exception>
    public async Task<WhoAmIResult> ExecuteAsync(
        string profileName = ProfileConfig.DefaultProfile,
        bool unmaskKey = false,
        CancellationToken cancellationToken = default)
    {
        var credential = _profileConfig.GetCredential(profileName);
        var identity = await _operation.ExecuteAsync(credential, cancellationToken);

        if (identity.KeyActorType == Actor.Other)
            throw new InvalidKeyException(
                "The key provided is invalid. Rover only accepts personal and graph API keys.");

        var apiKey = unmaskKey
            ? credential.ApiKey
            : ApiKeyMasker.Mask(credential.ApiKey);

        var origin = credential.Origin switch
        {
            CredentialOrigin.ConfigFile cf => $"--profile {cf.ProfileName}",
            CredentialOrigin.EnvVar       => $"${ProfileConfig.ApiKeyEnvVar}",
            _                             => "unknown"
        };

        return new WhoAmIResult(
            ApiKey:           apiKey,
            ActorType:        identity.KeyActorType,
            UserId:           identity.KeyActorType == Actor.User  ? identity.Id : null,
            GraphId:          identity.KeyActorType == Actor.Graph ? identity.Id : null,
            GraphTitle:       identity.GraphTitle,
            Origin:           origin,
            CredentialOrigin: credential.Origin);
    }
}
