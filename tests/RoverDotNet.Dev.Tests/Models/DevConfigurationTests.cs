using RoverDotNet.Dev.Models;

namespace RoverDotNet.Dev.Tests.Models;

public sealed class DevConfigurationTests
{
    [Fact]
    public void DevConfiguration_WithConfigPath_CreatesInstance()
    {
        // Arrange
        var configPath = "supergraph.yaml";

        // Act
        var config = new DevConfiguration(SupergraphConfigPath: configPath);

        // Assert
        Assert.Equal(configPath, config.SupergraphConfigPath);
        Assert.Null(config.SupergraphConfigContent);
        Assert.Equal(4000, config.RouterPort);
        Assert.Null(config.RouterBinaryPath);
    }

    [Fact]
    public void DevConfiguration_WithConfigContent_CreatesInstance()
    {
        // Arrange
        var yamlContent = "federation_version: =2.4.0\nsubgraphs:\n  users:\n    routing_url: http://localhost:4001";

        // Act
        var config = new DevConfiguration(SupergraphConfigContent: yamlContent);

        // Assert
        Assert.Null(config.SupergraphConfigPath);
        Assert.Equal(yamlContent, config.SupergraphConfigContent);
        Assert.Equal(4000, config.RouterPort);
    }

    [Fact]
    public void DevConfiguration_CustomPort_SetsPort()
    {
        // Arrange
        var configPath = "supergraph.yaml";

        // Act
        var config = new DevConfiguration(
            SupergraphConfigPath: configPath,
            RouterPort: 5000);

        // Assert
        Assert.Equal(5000, config.RouterPort);
    }

    [Fact]
    public void DevConfiguration_WithElv2Licence_SetsLicence()
    {
        // Arrange
        var configPath = "supergraph.yaml";

        // Act
        var config = new DevConfiguration(
            SupergraphConfigPath: configPath,
            Elv2Licence: "accept");

        // Assert
        Assert.Equal("accept", config.Elv2Licence);
    }
}
