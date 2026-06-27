using RoverDotNet.Dev.Models;

namespace RoverDotNet.Dev.Watchers;

/// <summary>
/// Watches a subgraph schema file for changes and raises events when modifications are detected.
/// Includes debouncing to handle rapid successive writes.
/// Mirrors the file-watching logic in <c>src/command/dev/protocol/follower.rs</c>.
/// </summary>
public sealed class SubgraphWatcher : IDisposable
{
    private readonly SubgraphDefinition _subgraph;
    private readonly FileSystemWatcher _watcher;
    private readonly System.Timers.Timer _debounceTimer;
    private readonly int _debounceMilliseconds;

    /// <summary>
    /// Raised when the schema file has been modified (after debounce period).
    /// </summary>
    public event EventHandler<SubgraphDefinition>? SchemaChanged;

    /// <summary>
    /// Creates a watcher for the specified subgraph.
    /// </summary>
    /// <param name="subgraph">The subgraph to watch.</param>
    /// <param name="debounceMilliseconds">
    /// Debounce period in milliseconds to wait after the last change before raising the event.
    /// Default: 500ms.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if the schema file does not exist or the path is invalid.
    /// </exception>
    public SubgraphWatcher(SubgraphDefinition subgraph, int debounceMilliseconds = 500)
    {
        _subgraph = subgraph;
        _debounceMilliseconds = debounceMilliseconds;

        if (!File.Exists(subgraph.SchemaPath))
        {
            throw new ArgumentException(
                $"Schema file not found: {subgraph.SchemaPath}",
                nameof(subgraph));
        }

        var directory = Path.GetDirectoryName(subgraph.SchemaPath)
            ?? throw new ArgumentException("Invalid schema path.", nameof(subgraph));
        var fileName = Path.GetFileName(subgraph.SchemaPath);

        _watcher = new FileSystemWatcher(directory, fileName)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
            EnableRaisingEvents = false
        };

        _watcher.Changed += OnFileChanged;

        _debounceTimer = new System.Timers.Timer(_debounceMilliseconds)
        {
            AutoReset = false
        };
        _debounceTimer.Elapsed += (_, _) => OnDebounceElapsed();
    }

    /// <summary>
    /// Starts watching the schema file for changes.
    /// </summary>
    public void Start()
    {
        _watcher.EnableRaisingEvents = true;
    }

    /// <summary>
    /// Stops watching the schema file.
    /// </summary>
    public void Stop()
    {
        _watcher.EnableRaisingEvents = false;
        _debounceTimer.Stop();
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        // Restart the debounce timer on each change
        _debounceTimer.Stop();
        _debounceTimer.Start();
    }

    private void OnDebounceElapsed()
    {
        // Debounce period has elapsed; raise the SchemaChanged event
        SchemaChanged?.Invoke(this, _subgraph);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Stop();
        _watcher.Changed -= OnFileChanged;
        _watcher.Dispose();
        _debounceTimer.Dispose();
    }
}
