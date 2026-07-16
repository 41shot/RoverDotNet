using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoverDotNet.Core.Coprocessor;

/// <summary>
/// The router request context entries shared across the request-handling pipeline.
/// </summary>
public sealed class CoprocessorContext
{
    /// <summary>The context key/value entries.</summary>
    [JsonPropertyName("entries")]
    public Dictionary<string, JsonElement> Entries { get; set; } = new();
}

/// <summary>
/// A single coprocessor request or response payload, as sent/received via the
/// Apollo Router external coprocessor HTTP contract.
/// </summary>
/// <remarks>
/// The same JSON shape is used for both the request the router sends to a coprocessor
/// and the response the coprocessor sends back. Fields not relevant to a particular
/// <see cref="CoprocessorStage"/> are left <see langword="null"/>.
/// See: https://www.apollographql.com/docs/graphos/routing/customization/coprocessor/reference
/// </remarks>
public sealed class CoprocessorPayload
{
    /// <summary>The coprocessor protocol version. Always <c>1</c>.</summary>
    [JsonPropertyName("version")]
    public int Version { get; set; } = 1;

    /// <summary>The request-handling lifecycle stage, e.g. <c>RouterRequest</c>.</summary>
    [JsonPropertyName("stage")]
    public string Stage { get; set; } = string.Empty;

    /// <summary>Whether the router should continue processing, or terminate the request.</summary>
    [JsonPropertyName("control")]
    public CoprocessorControl Control { get; set; } = CoprocessorControl.Continue;

    /// <summary>A unique identifier correlating requests/responses for the same client operation.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>The current supergraph schema definition language (SDL), if requested.</summary>
    [JsonPropertyName("sdl")]
    public string? Sdl { get; set; }

    /// <summary>HTTP headers, keyed by header name.</summary>
    [JsonPropertyName("headers")]
    public Dictionary<string, List<string>>? Headers { get; set; }

    /// <summary>
    /// The request or response body. A string for <c>RouterRequest</c>/<c>RouterResponse</c>,
    /// or a JSON object for <c>SupergraphRequest</c>/<c>SupergraphResponse</c> and
    /// <c>SubgraphRequest</c>/<c>SubgraphResponse</c>.
    /// </summary>
    [JsonPropertyName("body")]
    public JsonElement? Body { get; set; }

    /// <summary>Router request context entries.</summary>
    [JsonPropertyName("context")]
    public CoprocessorContext? Context { get; set; }

    /// <summary>The HTTP status code (response stages only).</summary>
    [JsonPropertyName("statusCode")]
    public int? StatusCode { get; set; }

    /// <summary>The request path (router stages only).</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>The HTTP method.</summary>
    [JsonPropertyName("method")]
    public string? Method { get; set; }

    /// <summary>The target URI (subgraph stages only).</summary>
    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    /// <summary>The name of the subgraph being queried (subgraph stages only).</summary>
    [JsonPropertyName("serviceName")]
    public string? ServiceName { get; set; }

    /// <summary>A unique identifier for the subgraph request (subgraph stages only).</summary>
    [JsonPropertyName("subgraphRequestId")]
    public string? SubgraphRequestId { get; set; }
}
