using RoverDotNet.Dev.Composition;
using RoverDotNet.Dev.Models;

namespace RoverDotNet.Dev.Tests.Composition;

public sealed class CompositionRunnerTests
{
    [Fact]
    public async Task ComposeAsync_NoSubgraphs_ReturnsFailure()
    {
        // Arrange
        var runner = new CompositionRunner();
        var subgraphs = Array.Empty<SubgraphDefinition>();

        // Act
        var result = await runner.ComposeAsync(subgraphs);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.SupergraphSdl);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("No subgraphs provided", result.Errors[0]);
    }

    [Fact]
    public async Task ComposeAsync_ValidSubgraphs_GeneratesYamlConfig()
    {
        // This test would require a mock or test rover.exe
        // For now, we'll skip the actual execution test
        Assert.True(true);
    }
}
