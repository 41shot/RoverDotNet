using RoverDotNet.Core.Credentials;

namespace RoverDotNet.Client.Operations.WhoAmI;

/// <summary>
/// The identity returned by Apollo Studio for a given API key.
/// Mirrors <c>rover-client::operations::config::who_am_i::RegistryIdentity</c>.
/// </summary>
/// <param name="Id">The user ID or graph ID, depending on <paramref name="KeyActorType"/>.</param>
/// <param name="GraphTitle">The graph title; only set when <paramref name="KeyActorType"/> is <see cref="Actor.Graph"/>.</param>
/// <param name="KeyActorType">Whether this key belongs to a user, a graph, or an unknown actor.</param>
/// <param name="CredentialOrigin">Where the API key used for this request was sourced from.</param>
public sealed record RegistryIdentity(
    string Id,
    string? GraphTitle,
    Actor KeyActorType,
    CredentialOrigin CredentialOrigin);
