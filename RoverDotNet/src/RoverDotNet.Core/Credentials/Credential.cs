namespace RoverDotNet.Core.Credentials;

/// <summary>
/// An Apollo API key together with the source it was retrieved from.
/// Mirrors <c>houston::Credential</c>.
/// </summary>
/// <param name="ApiKey">The raw Apollo API key string.</param>
/// <param name="Origin">Where the key was sourced from.</param>
public sealed record Credential(string ApiKey, CredentialOrigin Origin);
