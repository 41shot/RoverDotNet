using System.Text;
using System.Text.Json;

namespace RoverDotNet.Client.Http;

/// <summary>
/// Sends authenticated GraphQL requests to the Apollo Studio API.
/// Mirrors the header-injection logic in <c>rover-client::blocking::StudioClient::build_studio_headers()</c>.
/// </summary>
/// <remarks>
/// Each request includes:
/// <list type="bullet">
///   <item><c>x-api-key</c> — the Apollo API key</item>
///   <item><c>apollographql-client-name</c> — always <c>rover-client</c></item>
///   <item><c>apollographql-client-version</c> — the caller-supplied version string</item>
///   <item><c>Content-Type: application/json</c></item>
/// </list>
/// </remarks>
public sealed class StudioHttpClient
{
    /// <summary>The production Apollo Studio GraphQL endpoint.</summary>
    public const string DefaultEndpoint = "https://api.apollographql.com/graphql";

    internal const string ClientName = "rover-client";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly string _clientVersion;
    private readonly string _endpoint;

    /// <param name="httpClient">The underlying <see cref="HttpClient"/> instance.</param>
    /// <param name="clientVersion">Reported as <c>apollographql-client-version</c>.</param>
    /// <param name="endpoint">Overrides <see cref="DefaultEndpoint"/> (used in tests).</param>
    public StudioHttpClient(HttpClient httpClient, string clientVersion, string? endpoint = null)
    {
        _httpClient = httpClient;
        _clientVersion = clientVersion;
        _endpoint = endpoint ?? DefaultEndpoint;
    }

    /// <summary>
    /// Executes a GraphQL POST against the Studio endpoint and returns the typed data payload.
    /// </summary>
    /// <typeparam name="TVariables">GraphQL variables type (use <c>object</c> for empty variables).</typeparam>
    /// <typeparam name="TData">Expected shape of the <c>data</c> field in the response.</typeparam>
    /// <exception cref="StudioClientException">Studio returned GraphQL errors.</exception>
    /// <exception cref="HttpRequestException">The HTTP request itself failed.</exception>
    internal async Task<TData> PostAsync<TVariables, TData>(
        string query,
        TVariables variables,
        string apiKey,
        CancellationToken cancellationToken = default)
    {
        var body = JsonSerializer.Serialize(new GraphQLRequest<TVariables>(query, variables));

        using var message = new HttpRequestMessage(HttpMethod.Post, _endpoint)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };

        message.Headers.Add("x-api-key", apiKey);
        message.Headers.Add("apollographql-client-name", ClientName);
        message.Headers.Add("apollographql-client-version", _clientVersion);

        using var response = await _httpClient.SendAsync(message, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var envelope = JsonSerializer.Deserialize<GraphQLResponse<TData>>(responseJson, JsonOptions)
            ?? throw new InvalidOperationException("Studio returned a null response body.");

        if (envelope.Errors is { Count: > 0 })
            throw new StudioClientException(envelope.Errors);

        return envelope.Data
            ?? throw new InvalidOperationException("Studio response contained no data.");
    }
}
