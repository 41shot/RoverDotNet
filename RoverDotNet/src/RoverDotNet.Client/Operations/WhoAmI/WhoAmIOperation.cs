using System.Text.Json.Serialization;
using RoverDotNet.Client.Http;
using RoverDotNet.Core.Credentials;
using RoverDotNet.Core.Exceptions;

namespace RoverDotNet.Client.Operations.WhoAmI;

/// <summary>
/// Sends the <c>ConfigWhoAmIQuery</c> to Apollo Studio and maps the response
/// to a <see cref="RegistryIdentity"/>.
/// Mirrors <c>rover-client::operations::config::who_am_i</c>.
/// </summary>
public sealed class WhoAmIOperation
{
    // Matches who_am_i_query.graphql in the original rover-client crate.
    private const string _query = """
        query ConfigWhoAmIQuery {
          me {
            __typename
            ... on Graph {
              title
            }
            id
            asActor {
              type
            }
          }
        }
        """;

    private readonly StudioHttpClient _client;

    /// <param name="client">The Studio HTTP client to use for the request.</param>
    public WhoAmIOperation(StudioHttpClient client) => _client = client;

    /// <summary>
    /// Executes the who-am-I query against Apollo Studio.
    /// </summary>
    /// <param name="credential">The credential whose API key will be used.</param>
    /// <param name="cancellationToken">Propagates cancellation.</param>
    /// <returns>The registry identity associated with the API key.</returns>
    /// <exception cref="InvalidKeyException">
    /// The API key was not recognised by Studio (<c>me</c> was <see langword="null"/>).
    /// </exception>
    /// <exception cref="StudioClientException">Studio returned GraphQL errors.</exception>
    public async Task<RegistryIdentity> ExecuteAsync(
        Credential credential,
        CancellationToken cancellationToken = default)
    {
        var data = await _client.PostAsync<object, WhoAmIData>(
            _query,
            new { },
            credential.ApiKey,
            cancellationToken);

        if (data.Me is null)
            throw new InvalidKeyException();

        var actorType = data.Me.AsActor?.Type switch
        {
            "GRAPH" => Actor.Graph,
            "USER"  => Actor.User,
            _       => Actor.Other
        };

        // graph_title is only populated via the inline fragment "... on Graph { title }"
        var graphTitle = data.Me.TypeName == "Graph" ? data.Me.Title : null;

        return new RegistryIdentity(
            data.Me.Id,
            graphTitle,
            actorType,
            credential.Origin);
    }

    // -------------------------------------------------------------------------
    // Response DTOs — internal; not part of the public API surface
    // -------------------------------------------------------------------------

    private sealed class WhoAmIData
    {
        [JsonPropertyName("me")]
        public MeNode? Me { get; init; }
    }

    private sealed class MeNode
    {
        [JsonPropertyName("__typename")]
        public string TypeName { get; init; } = string.Empty;

        [JsonPropertyName("id")]
        public string Id { get; init; } = string.Empty;

        [JsonPropertyName("title")]
        public string? Title { get; init; }

        [JsonPropertyName("asActor")]
        public AsActorNode? AsActor { get; init; }
    }

    private sealed record AsActorNode(
        [property: JsonPropertyName("type")] string Type);
}
