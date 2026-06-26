using System.Text.Json.Serialization;

namespace RoverDotNet.Client.Http;

/// <summary>
/// The JSON body sent to a GraphQL endpoint.
/// </summary>
internal sealed record GraphQLRequest<TVariables>(
    [property: JsonPropertyName("query")] string Query,
    [property: JsonPropertyName("variables")] TVariables Variables);

/// <summary>
/// The JSON body received from a GraphQL endpoint.
/// </summary>
internal sealed record GraphQLResponse<TData>(
    [property: JsonPropertyName("data")] TData? Data,
    [property: JsonPropertyName("errors")] IReadOnlyList<GraphQLError>? Errors);

/// <summary>
/// A single error entry returned inside a GraphQL error response.
/// </summary>
public sealed record GraphQLError(
    [property: JsonPropertyName("message")] string Message);
