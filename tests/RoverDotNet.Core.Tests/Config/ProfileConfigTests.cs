using RoverDotNet.Core.Config;
using RoverDotNet.Core.Credentials;
using RoverDotNet.Core.Exceptions;
using Xunit;

namespace RoverDotNet.Core.Tests.Config;

public class ProfileConfigTests : IDisposable
{
    private readonly string _tempDir;

    public ProfileConfigTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"rover-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    private ProfileConfig CreateConfig() => new(_tempDir);

    // -------------------------------------------------------------------------
    // SetApiKey / GetCredential round-trips
    // -------------------------------------------------------------------------

    [Fact]
    public void SetApiKey_and_GetCredential_roundtrip_default_profile()
    {
        var config = CreateConfig();
        config.SetApiKey(ProfileConfig.DefaultProfile, "user:graph:testkey1234");

        var credential = config.GetCredential();

        Assert.Equal("user:graph:testkey1234", credential.ApiKey);
        var origin = Assert.IsType<CredentialOrigin.ConfigFile>(credential.Origin);
        Assert.Equal(ProfileConfig.DefaultProfile, origin.ProfileName);
    }

    [Fact]
    public void SetApiKey_and_GetCredential_roundtrip_named_profile()
    {
        var config = CreateConfig();
        config.SetApiKey("custom", "service:my-graph:customkey5678");

        var credential = config.GetCredential("custom");

        Assert.Equal("service:my-graph:customkey5678", credential.ApiKey);
        var origin = Assert.IsType<CredentialOrigin.ConfigFile>(credential.Origin);
        Assert.Equal("custom", origin.ProfileName);
    }

    [Fact]
    public void GetCredential_returns_env_var_credential_when_APOLLO_KEY_is_set()
    {
        var config = CreateConfig();
        config.SetApiKey(ProfileConfig.DefaultProfile, "file-based-key");

        // Temporarily set the env var; restore on exit
        var previous = Environment.GetEnvironmentVariable(ProfileConfig.ApiKeyEnvVar);
        try
        {
            Environment.SetEnvironmentVariable(ProfileConfig.ApiKeyEnvVar, "env-var-key");
            var credential = config.GetCredential();

            Assert.Equal("env-var-key", credential.ApiKey);
            Assert.IsType<CredentialOrigin.EnvVar>(credential.Origin);
        }
        finally
        {
            Environment.SetEnvironmentVariable(ProfileConfig.ApiKeyEnvVar, previous);
        }
    }

    [Fact]
    public void GetCredential_throws_NoProfilesException_even_when_env_var_is_set()
    {
        var config = CreateConfig();

        // Set APOLLO_KEY, but don't create any profiles
        var previous = Environment.GetEnvironmentVariable(ProfileConfig.ApiKeyEnvVar);
        try
        {
            Environment.SetEnvironmentVariable(ProfileConfig.ApiKeyEnvVar, "env-var-key");

            // Should throw because no profiles exist, matching Rover's behavior
            Assert.Throws<NoProfilesException>(() => config.GetCredential());
        }
        finally
        {
            Environment.SetEnvironmentVariable(ProfileConfig.ApiKeyEnvVar, previous);
        }
    }

    // -------------------------------------------------------------------------
    // Error cases
    // -------------------------------------------------------------------------

    [Fact]
    public void GetCredential_throws_NoProfilesException_when_no_profiles_exist()
    {
        var config = CreateConfig();
        Assert.Throws<NoProfilesException>(() => config.GetCredential());
    }

    [Fact]
    public void GetCredential_throws_ProfileNotFoundException_when_named_profile_is_missing()
    {
        var config = CreateConfig();
        config.SetApiKey("other", "some-key");

        Assert.Throws<ProfileNotFoundException>(() => config.GetCredential("nonexistent"));
    }

    // -------------------------------------------------------------------------
    // ListProfiles
    // -------------------------------------------------------------------------

    [Fact]
    public void ListProfiles_returns_all_profiles_alphabetically()
    {
        var config = CreateConfig();
        config.SetApiKey("beta", "key1");
        config.SetApiKey("alpha", "key2");
        config.SetApiKey("gamma", "key3");

        var profiles = config.ListProfiles();

        Assert.Equal(["alpha", "beta", "gamma"], profiles);
    }

    [Fact]
    public void ListProfiles_returns_empty_list_when_no_profiles_exist()
    {
        var config = CreateConfig();
        Assert.Empty(config.ListProfiles());
    }

    // -------------------------------------------------------------------------
    // DeleteProfile / Clear
    // -------------------------------------------------------------------------

    [Fact]
    public void DeleteProfile_removes_the_named_profile()
    {
        var config = CreateConfig();
        config.SetApiKey("to-delete", "some-key");
        config.SetApiKey("keep", "other-key");

        config.DeleteProfile("to-delete");

        Assert.Equal(["keep"], config.ListProfiles());
    }

    [Fact]
    public void DeleteProfile_throws_ProfileNotFoundException_for_missing_profile()
    {
        var config = CreateConfig();
        Assert.Throws<ProfileNotFoundException>(() => config.DeleteProfile("nonexistent"));
    }

    [Fact]
    public void Clear_removes_the_entire_config_home()
    {
        var config = CreateConfig();
        config.SetApiKey(ProfileConfig.DefaultProfile, "some-key");

        config.Clear();

        Assert.False(Directory.Exists(_tempDir));
    }
}
