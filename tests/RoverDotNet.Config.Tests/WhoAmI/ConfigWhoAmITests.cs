using System.Net;
using System.Text;
using RoverDotNet.Client.Http;
using RoverDotNet.Client.Operations.WhoAmI;
using RoverDotNet.Config.WhoAmI;
using RoverDotNet.Core.Config;
using RoverDotNet.Core.Exceptions;
using Xunit;

namespace RoverDotNet.Config.Tests.WhoAmI;

public class ConfigWhoAmITests : IDisposable
{
    private readonly string _tempDir;

    public ConfigWhoAmITests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"rover-config-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    private ConfigWhoAmI CreateCommand(string responseJson)
    {
        var profileConfig = new ProfileConfig(_tempDir);
        var handler = new FakeHttpMessageHandler(responseJson);
        var studioClient = new StudioHttpClient(new HttpClient(handler), clientVersion: "test");
        var operation = new WhoAmIOperation(studioClient);
        return new ConfigWhoAmI(profileConfig, operation);
    }

    private ProfileConfig ProfileConfig => new(_tempDir);

    // -------------------------------------------------------------------------
    // User key
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_returns_user_result_for_user_key()
    {
        ProfileConfig.SetApiKey(Core.Config.ProfileConfig.DefaultProfile, "user:graph:mykey");

        const string json = """
            {"data":{"me":{"__typename":"User","id":"gh.myuser","asActor":{"type":"USER"}}}}
            """;

        var result = await CreateCommand(json).ExecuteAsync();

        Assert.Equal(Actor.User, result.ActorType);
        Assert.Equal("gh.myuser", result.UserId);
        Assert.Null(result.GraphId);
        Assert.Null(result.GraphTitle);
        Assert.StartsWith("--profile", result.Origin);
    }

    // -------------------------------------------------------------------------
    // Graph key
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_returns_graph_result_for_graph_key()
    {
        ProfileConfig.SetApiKey(Core.Config.ProfileConfig.DefaultProfile, "service:graph:graphkey");

        const string json = """
            {"data":{"me":{"__typename":"Graph","title":"MyGraph","id":"my-graph-id","asActor":{"type":"GRAPH"}}}}
            """;

        var result = await CreateCommand(json).ExecuteAsync();

        Assert.Equal(Actor.Graph, result.ActorType);
        Assert.Equal("my-graph-id", result.GraphId);
        Assert.Equal("MyGraph", result.GraphTitle);
        Assert.Null(result.UserId);
    }

    // -------------------------------------------------------------------------
    // API key masking
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_masks_api_key_by_default()
    {
        ProfileConfig.SetApiKey(Core.Config.ProfileConfig.DefaultProfile, "user:graph:djru4788dhsg3657fhLOLO");

        const string json = """
            {"data":{"me":{"__typename":"User","id":"gh.user","asActor":{"type":"USER"}}}}
            """;

        var result = await CreateCommand(json).ExecuteAsync();

        Assert.DoesNotContain("djru4788dhsg3657fh", result.ApiKey);
        Assert.Contains("****", result.ApiKey);
        Assert.StartsWith("user", result.ApiKey);
        Assert.EndsWith("LOLO", result.ApiKey);
    }

    [Fact]
    public async Task ExecuteAsync_returns_raw_api_key_when_unmask_requested()
    {
        ProfileConfig.SetApiKey(Core.Config.ProfileConfig.DefaultProfile, "user:graph:djru4788dhsg3657fhLOLO");

        const string json = """
            {"data":{"me":{"__typename":"User","id":"gh.user","asActor":{"type":"USER"}}}}
            """;

        var result = await CreateCommand(json).ExecuteAsync(unmaskKey: true);

        Assert.Equal("user:graph:djru4788dhsg3657fhLOLO", result.ApiKey);
    }

    // -------------------------------------------------------------------------
    // Origin formatting
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_formats_profile_origin_correctly()
    {
        ProfileConfig.SetApiKey("my-profile", "user:graph:somekey");

        const string json = """
            {"data":{"me":{"__typename":"User","id":"gh.user","asActor":{"type":"USER"}}}}
            """;

        var result = await CreateCommand(json).ExecuteAsync(profileName: "my-profile");

        Assert.Equal("--profile my-profile", result.Origin);
    }

    [Fact]
    public async Task ExecuteAsync_formats_env_var_origin_correctly()
    {
        // Create a profile first (required even when using env var, matching Rover's behavior)
        ProfileConfig.SetApiKey(Core.Config.ProfileConfig.DefaultProfile, "file-based-key");

        // Set APOLLO_KEY so the env-var path is taken
        var previous = Environment.GetEnvironmentVariable(Core.Config.ProfileConfig.ApiKeyEnvVar);
        try
        {
            Environment.SetEnvironmentVariable(Core.Config.ProfileConfig.ApiKeyEnvVar, "env-api-key");

            const string json = """
                {"data":{"me":{"__typename":"User","id":"gh.user","asActor":{"type":"USER"}}}}
                """;

            var result = await CreateCommand(json).ExecuteAsync();

            Assert.Equal($"${Core.Config.ProfileConfig.ApiKeyEnvVar}", result.Origin);
        }
        finally
        {
            Environment.SetEnvironmentVariable(Core.Config.ProfileConfig.ApiKeyEnvVar, previous);
        }
    }

    // -------------------------------------------------------------------------
    // Invalid actor type
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_throws_InvalidKeyException_for_other_actor_type()
    {
        ProfileConfig.SetApiKey(Core.Config.ProfileConfig.DefaultProfile, "some-key");

        const string json = """
            {"data":{"me":{"__typename":"Unknown","id":"some-id","asActor":{"type":"SOME_OTHER_TYPE"}}}}
            """;

        await Assert.ThrowsAsync<InvalidKeyException>(
            () => CreateCommand(json).ExecuteAsync());
    }

    // -------------------------------------------------------------------------
    // Test helper
    // -------------------------------------------------------------------------

    private sealed class FakeHttpMessageHandler(string responseJson) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }
}
