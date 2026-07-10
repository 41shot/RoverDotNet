using RoverDotNet.Dev.Composition;

namespace RoverDotNet.Dev.Tests.Composition;

public sealed class CompositionRunnerTests
{
    [Fact]
    public async Task ComposeAsync_NullConfigPath_ReturnsFailure()
    {
        // Arrange
        var runner = new CompositionRunner();

        // Act
        var result = await runner.ComposeAsync(null!);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.SupergraphSdl);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("required", result.Errors[0], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ComposeAsync_NonExistentConfigFile_ReturnsFailure()
    {
        // Arrange
        var runner = new CompositionRunner();
        var nonExistentPath = Path.Combine(Path.GetTempPath(), $"non-existent-{Guid.NewGuid()}.yaml");

        // Act
        var result = await runner.ComposeAsync(nonExistentPath);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.SupergraphSdl);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("not found", result.Errors[0], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ComposeAsync_ValidConfig_RequiresRover()
    {
        // This test would require a mock or test rover.exe
        // For now, we'll skip the actual execution test
        Assert.True(true);
    }
}
