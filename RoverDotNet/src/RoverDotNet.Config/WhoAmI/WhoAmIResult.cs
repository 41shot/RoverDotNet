using RoverDotNet.Client.Operations.WhoAmI;
using RoverDotNet.Core.Credentials;

namespace RoverDotNet.Config.WhoAmI;

/// <summary>
/// The display-ready result of a <c>config whoami</c> execution.
/// Mirrors the fields returned in <c>RoverOutput::ConfigWhoAmIOutput</c>.
/// </summary>
/// <param name="ApiKey">The API key, masked by default unless the caller requested otherwise.</param>
/// <param name="ActorType">Whether the key belongs to a user or a graph.</param>
/// <param name="UserId">The user ID; set only when <paramref name="ActorType"/> is <see cref="Actor.User"/>.</param>
/// <param name="GraphId">The graph ID; set only when <paramref name="ActorType"/> is <see cref="Actor.Graph"/>.</param>
/// <param name="GraphTitle">The graph title; set only when <paramref name="ActorType"/> is <see cref="Actor.Graph"/>.</param>
/// <param name="Origin">
/// Human-readable description of where the credential came from,
/// e.g. <c>--profile default</c> or <c>$APOLLO_KEY</c>.
/// </param>
/// <param name="CredentialOrigin">The structured credential origin for programmatic use.</param>
public sealed record WhoAmIResult(
    string ApiKey,
    Actor ActorType,
    string? UserId,
    string? GraphId,
    string? GraphTitle,
    string Origin,
    CredentialOrigin CredentialOrigin);
