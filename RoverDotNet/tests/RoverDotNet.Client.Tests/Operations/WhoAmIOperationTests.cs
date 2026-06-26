using System.Net;
using System.Text;
using RoverDotNet.Client.Http;
using RoverDotNet.Client.Operations.WhoAmI;
using RoverDotNet.Core.Credentials;
using RoverDotNet.Core.Exceptions;
using Xunit;

namespace RoverDotNet.Client.Tests.Operations;

public class WhoAmIOperationTests
{
    private static WhoAmIOperation CreateOperation(string responseJson, HttpStatusCode status = HttpStatusCode.OK)
    {
        var handler = new FakeHttpMessageHandler(responseJson, status);
        var httpClient = new HttpClient(handler);
        var studioClient = new StudioHttpClient(httpClient, clientVersion: "test");
        return new WhoAmIOperation(studioClient);
    }

    // -------------------------------------------------------------------------
    // User key
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_returns_user_identity_for_user_key()
    {
        const string json = """
            {"data":{"me":{"__typename":"User","id":"gh.nobodydefinitelyhasthisusernamelol","asActor":{"type":"USER"}}}}
            """;

        var credential = new Credential("user:graph:testkey", new CredentialOrigin.EnvVar());
        var result = await CreateOperation(json).ExecuteAsync(credential);

        Assert.Equal("gh.nobodydefinitelyhasthisusernamelol", result.Id);
        Assert.Equal(Actor.User, result.KeyActorType);
        Assert.Null(result.GraphTitle);
        Assert.IsType<CredentialOrigin.EnvVar>(result.CredentialOrigin);
    }

    // -------------------------------------------------------------------------
    // Graph key
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_returns_graph_identity_for_graph_key()
    {
        const string json = """
            {"data":{"me":{"__typename":"Graph","title":"GraphKeyService","id":"big-ol-graph-key-lolol","asActor":{"type":"GRAPH"}}}}
            """;

        var credential = new Credential("service:graph:testkey", new CredentialOrigin.ConfigFile("default"));
        var result = await CreateOperation(json).ExecuteAsync(credential);

        Assert.Equal("big-ol-graph-key-lolol", result.Id);
        Assert.Equal(Actor.Graph, result.KeyActorType);
        Assert.Equal("GraphKeyService", result.GraphTitle);
        var origin = Assert.IsType<CredentialOrigin.ConfigFile>(result.CredentialOrigin);
        Assert.Equal("default", origin.ProfileName);
    }

    // -------------------------------------------------------------------------
    // Unknown actor type
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_returns_other_for_unknown_actor_type()
    {
        const string json = """
            {"data":{"me":{"__typename":"SomeOtherType","id":"some-id","asActor":{"type":"UNKNOWN_FUTURE_TYPE"}}}}
            """;

        var credential = new Credential("key", new CredentialOrigin.EnvVar());
        var result = await CreateOperation(json).ExecuteAsync(credential);

        Assert.Equal(Actor.Other, result.KeyActorType);
    }

    // -------------------------------------------------------------------------
    // Invalid key (null me)
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_throws_InvalidKeyException_when_me_is_null()
    {
        const string json = """{"data":{"me":null}}""";

        var credential = new Credential("invalid-key", new CredentialOrigin.EnvVar());

        await Assert.ThrowsAsync<InvalidKeyException>(
            () => CreateOperation(json).ExecuteAsync(credential));
    }

    // -------------------------------------------------------------------------
    // GraphQL errors
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_throws_StudioClientException_on_graphql_errors()
    {
        const string json = """{"errors":[{"message":"Unauthorized"}]}""";

        var credential = new Credential("bad-key", new CredentialOrigin.EnvVar());
        var ex = await Assert.ThrowsAsync<StudioClientException>(
            () => CreateOperation(json).ExecuteAsync(credential));

        Assert.Contains("Unauthorized", ex.Message);
        Assert.Single(ex.Errors);
    }

    // -------------------------------------------------------------------------
    // HTTP headers
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_sends_required_studio_headers()
    {
        const string json = """
            {"data":{"me":{"__typename":"User","id":"gh.user","asActor":{"type":"USER"}}}}
            """;

        var handler = new CapturingHttpMessageHandler(json);
        var httpClient = new HttpClient(handler);
        var studioClient = new StudioHttpClient(httpClient, clientVersion: "1.2.3");
        var operation = new WhoAmIOperation(studioClient);

        var credential = new Credential("my-api-key", new CredentialOrigin.EnvVar());
        await operation.ExecuteAsync(credential);

        var request = handler.LastRequest!;
        Assert.Equal("my-api-key", request.Headers.GetValues("x-api-key").Single());
        Assert.Equal("rover-client", request.Headers.GetValues("apollographql-client-name").Single());
        Assert.Equal("1.2.3", request.Headers.GetValues("apollographql-client-version").Single());
    }

    // -------------------------------------------------------------------------
    // Test helpers
    // -------------------------------------------------------------------------

    private sealed class FakeHttpMessageHandler(string responseJson, HttpStatusCode status = HttpStatusCode.OK)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(status)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }

    private sealed class CapturingHttpMessageHandler(string responseJson) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }
}
