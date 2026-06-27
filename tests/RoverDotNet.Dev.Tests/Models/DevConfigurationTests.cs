using RoverDotNet.Dev.Exceptions;
using RoverDotNet.Dev.Models;

namespace RoverDotNet.Dev.Tests.Models;

public sealed class DevConfigurationTests
{
    [Fact]
    public void DevConfiguration_ValidSubgraphs_CreatesInstance()
    {
        // Arrange
        var subgraphs = new List<SubgraphDefinition>
        {
            new("users", "http://localhost:4001", "users.graphql"),
            new("products", "http://localhost:4002", "products.graphql")
        };

        // Act
        var config = new DevConfiguration(subgraphs);

        // Assert
        Assert.Equal(2, config.Subgraphs.Count);
        Assert.Equal(4000, config.RouterPort);
        Assert.Null(config.RouterBinaryPath);
    }

    [Fact]
    public void DevConfiguration_CustomPort_SetsPort()
    {
        // Arrange
        var subgraphs = new List<SubgraphDefinition>
        {
            new("users", "http://localhost:4001", "users.graphql")
        };

        // Act
        var config = new DevConfiguration(subgraphs, RouterPort: 5000);

        // Assert
        Assert.Equal(5000, config.RouterPort);
    }
}
