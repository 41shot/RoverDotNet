using RoverDotNet.Dev.Models;
using RoverDotNet.Dev.Watchers;

namespace RoverDotNet.Dev.Tests.Watchers;

public sealed class SubgraphWatcherTests : IDisposable
{
    private readonly string _tempDir;
    private readonly string _schemaPath;

    public SubgraphWatcherTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
        _schemaPath = Path.Combine(_tempDir, "schema.graphql");
        File.WriteAllText(_schemaPath, "type Query { test: String }");
    }

    [Fact]
    public void Constructor_NonExistentFile_ThrowsArgumentException()
    {
        // Arrange
        var subgraph = new SubgraphDefinition(
            "test",
            "http://localhost:4001",
            "nonexistent.graphql");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new SubgraphWatcher(subgraph));
    }

    [Fact]
    public void Constructor_ValidFile_CreatesWatcher()
    {
        // Arrange
        var subgraph = new SubgraphDefinition(
            "test",
            "http://localhost:4001",
            _schemaPath);

        // Act
        using var watcher = new SubgraphWatcher(subgraph);

        // Assert
        Assert.NotNull(watcher);
    }

    [Fact]
    public async Task SchemaChanged_FileModified_RaisesEvent()
    {
        // Arrange
        var subgraph = new SubgraphDefinition(
            "test",
            "http://localhost:4001",
            _schemaPath);

        using var watcher = new SubgraphWatcher(subgraph, debounceMilliseconds: 100);
        var eventRaised = false;
        SubgraphDefinition? raisedSubgraph = null;

        watcher.SchemaChanged += (_, s) =>
        {
            eventRaised = true;
            raisedSubgraph = s;
        };

        watcher.Start();

        // Act
        await Task.Delay(50); // Let watcher initialise
        File.WriteAllText(_schemaPath, "type Query { updated: String }");

        // Wait for debounce
        await Task.Delay(200);

        // Assert
        Assert.True(eventRaised);
        Assert.NotNull(raisedSubgraph);
        Assert.Equal("test", raisedSubgraph.Name);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }
}
