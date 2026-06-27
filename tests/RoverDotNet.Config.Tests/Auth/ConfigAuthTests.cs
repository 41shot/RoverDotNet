using RoverDotNet.Config.Auth;
using RoverDotNet.Core.Config;
using RoverDotNet.Core.Exceptions;
using Xunit;

namespace RoverDotNet.Config.Tests.Auth;

public class ConfigAuthTests : IDisposable
{
    private readonly string _tempDir;

    public ConfigAuthTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"rover-config-auth-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    private ProfileConfig ProfileConfig => new(_tempDir);

    // -------------------------------------------------------------------------
    // Successful authentication
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_saves_api_key_to_default_profile()
    {
        var prompt = new FakeApiKeyPrompt("test-api-key-123");
        var command = new ConfigAuth(ProfileConfig, prompt);

        var result = await command.ExecuteAsync();

        Assert.Equal(Core.Config.ProfileConfig.DefaultProfile, result.ProfileName);
        Assert.Contains("Successfully saved", result.Message);
        Assert.Contains("whoami", result.Message);

        // Verify the key was actually saved
        var credential = ProfileConfig.GetCredential(Core.Config.ProfileConfig.DefaultProfile);
        Assert.Equal("test-api-key-123", credential.ApiKey);
    }

    [Fact]
    public async Task ExecuteAsync_saves_api_key_to_custom_profile()
    {
        var prompt = new FakeApiKeyPrompt("custom-key-456");
        var command = new ConfigAuth(ProfileConfig, prompt);

        var result = await command.ExecuteAsync("custom");

        Assert.Equal("custom", result.ProfileName);
        Assert.Contains("Successfully saved", result.Message);

        // Verify the key was actually saved to the custom profile
        var credential = ProfileConfig.GetCredential("custom");
        Assert.Equal("custom-key-456", credential.ApiKey);
    }

    [Fact]
    public async Task ExecuteAsync_overwrites_existing_profile()
    {
        // Set up existing profile
        ProfileConfig.SetApiKey("myprofile", "old-key");

        var prompt = new FakeApiKeyPrompt("new-key-789");
        var command = new ConfigAuth(ProfileConfig, prompt);

        await command.ExecuteAsync("myprofile");

        // Verify the key was overwritten
        var credential = ProfileConfig.GetCredential("myprofile");
        Assert.Equal("new-key-789", credential.ApiKey);
    }

    // -------------------------------------------------------------------------
    // Validation errors
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_throws_when_api_key_is_empty()
    {
        var prompt = new FakeApiKeyPrompt("");
        var command = new ConfigAuth(ProfileConfig, prompt);

        await Assert.ThrowsAsync<RoverException>(
            () => command.ExecuteAsync());
    }

    [Fact]
    public async Task ExecuteAsync_throws_when_api_key_is_ctrl_v_byte()
    {
        var prompt = new FakeApiKeyPrompt("\x16");
        var command = new ConfigAuth(ProfileConfig, prompt);

        var ex = await Assert.ThrowsAsync<RoverException>(
            () => command.ExecuteAsync());

        Assert.Contains("not pasted successfully", ex.Message);
        Assert.Contains("Ctrl+V", ex.Message);
    }

    // -------------------------------------------------------------------------
    // Cancellation
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ExecuteAsync_propagates_cancellation()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var prompt = new FakeApiKeyPrompt("test-key", cts.Token);
        var command = new ConfigAuth(ProfileConfig, prompt);

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => command.ExecuteAsync(cancellationToken: cts.Token));
    }

    // -------------------------------------------------------------------------
    // Fake implementation for testing
    // -------------------------------------------------------------------------

    private sealed class FakeApiKeyPrompt : IApiKeyPrompt
    {
        private readonly string _apiKey;
        private readonly CancellationToken _cancellationToken;

        public FakeApiKeyPrompt(string apiKey, CancellationToken cancellationToken = default)
        {
            _apiKey = apiKey;
            _cancellationToken = cancellationToken;
        }

        public Task<string> PromptAsync(CancellationToken cancellationToken = default)
        {
            _cancellationToken.ThrowIfCancellationRequested();
            cancellationToken.ThrowIfCancellationRequested();

            // Validate just like ConsoleApiKeyPrompt does
            if (string.IsNullOrEmpty(_apiKey))
                throw new RoverException("Received an empty API Key. Please try again.");

            if (_apiKey.Length == 1 && _apiKey[0] == '\x16')
            {
                throw new RoverException(
                    "Your API key was not pasted successfully. " +
                    "Re-run this command, and when you are prompted to enter your API key, " +
                    "right click on the terminal and press paste instead of pressing Ctrl+V.");
            }

            return Task.FromResult(_apiKey);
        }
    }
}
