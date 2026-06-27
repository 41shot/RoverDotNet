using RoverDotNet.Dev.Composition;
using RoverDotNet.Dev.Exceptions;
using RoverDotNet.Dev.Models;
using RoverDotNet.Dev.Router;
using RoverDotNet.Dev.Watchers;

namespace RoverDotNet.Dev;

/// <summary>
/// Orchestrates a local development session: composes the supergraph, starts the router,
/// watches for schema changes, and recomposes/restarts as needed.
/// Mirrors the session orchestration in <c>src/command/dev/</c>.
/// </summary>
public sealed class DevSession : IDisposable
{
    private readonly DevConfiguration _configuration;
    private readonly CompositionRunner _compositionRunner;
    private readonly List<SubgraphWatcher> _watchers;
    private RouterProcess? _routerProcess;
    private string? _currentSupergraphPath;
    private DevSessionState _state;
    private bool _disposed;
    private readonly SemaphoreSlim _compositionLock;

    /// <summary>
    /// Raised when the session state changes.
    /// </summary>
    public event EventHandler<DevSessionEvent>? StateChanged;

    /// <summary>
    /// Gets the current state of the dev session.
    /// </summary>
    public DevSessionState State
    {
        get => _state;
        private set
        {
            _state = value;
            RaiseStateChanged(value, $"Session state: {value}");
        }
    }

    /// <summary>
    /// Creates a new dev session.
    /// </summary>
    /// <param name="configuration">The dev session configuration.</param>
    /// <param name="compositionRunner">Optional composition runner (defaults to new instance).</param>
    public DevSession(
        DevConfiguration configuration,
        CompositionRunner? compositionRunner = null)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _compositionRunner = compositionRunner ?? new CompositionRunner();
        _watchers = new List<SubgraphWatcher>();
        _state = DevSessionState.Idle;
        _compositionLock = new SemaphoreSlim(1, 1);

        ValidateConfiguration();
    }

    /// <summary>
    /// Starts the dev session: performs initial composition, starts the router, and begins watching for changes.
    /// </summary>
    /// <param name="cancellationToken">Propagates cancellation.</param>
    /// <exception cref="DevException">Thrown if the session fails to start.</exception>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (State != DevSessionState.Idle)
            throw new InvalidOperationException("Session has already been started.");

        try
        {
            State = DevSessionState.Starting;

            // Initial composition
            RaiseStateChanged(DevSessionState.Starting, "Performing initial supergraph composition...");
            var compositionResult = await _compositionRunner.ComposeAsync(
                _configuration.Subgraphs,
                cancellationToken);

            if (!compositionResult.Success)
            {
                State = DevSessionState.Error;
                throw new CompositionFailedException(compositionResult.Errors);
            }

            // Write supergraph to file
            _currentSupergraphPath = _configuration.SupergraphConfigPath
                ?? Path.Combine(Path.GetTempPath(), $"supergraph-{Guid.NewGuid()}.graphql");

            await File.WriteAllTextAsync(
                _currentSupergraphPath,
                compositionResult.SupergraphSdl!,
                cancellationToken);

            RaiseStateChanged(DevSessionState.Starting, $"Supergraph schema written to: {_currentSupergraphPath}");

            // Start the router
            await StartRouterAsync(cancellationToken);

            // Start watching subgraphs
            StartWatching();

            State = DevSessionState.Running;
            RaiseStateChanged(
                DevSessionState.Running,
                $"Dev session running. Router listening on http://localhost:{_configuration.RouterPort}");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            State = DevSessionState.Error;
            RaiseStateChanged(DevSessionState.Error, $"Failed to start session: {ex.Message}", ex);
            throw;
        }
    }

    /// <summary>
    /// Stops the dev session gracefully: stops watching, shuts down the router, and cleans up.
    /// </summary>
    /// <param name="cancellationToken">Propagates cancellation.</param>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (State == DevSessionState.Stopped || State == DevSessionState.Idle)
            return;

        try
        {
            State = DevSessionState.Stopping;

            // Stop watching
            StopWatching();

            // Stop the router
            if (_routerProcess is not null)
            {
                RaiseStateChanged(DevSessionState.Stopping, "Stopping router...");
                await _routerProcess.StopAsync(cancellationToken);
                _routerProcess.Dispose();
                _routerProcess = null;
            }

            State = DevSessionState.Stopped;
            RaiseStateChanged(DevSessionState.Stopped, "Session stopped.");
        }
        catch (Exception ex)
        {
            State = DevSessionState.Error;
            RaiseStateChanged(DevSessionState.Error, $"Error stopping session: {ex.Message}", ex);
            throw;
        }
    }

    private async Task StartRouterAsync(CancellationToken cancellationToken)
    {
        var routerBinaryPath = _configuration.RouterBinaryPath
            ?? await LocateRouterBinaryAsync();

        RaiseStateChanged(DevSessionState.Starting, $"Starting router from: {routerBinaryPath}");

        _routerProcess = new RouterProcess(
            routerBinaryPath,
            _configuration.RouterPort,
            _currentSupergraphPath!);

        _routerProcess.OutputReceived += (_, message) =>
            RaiseStateChanged(State, $"[Router] {message}");

        _routerProcess.ErrorReceived += (_, message) =>
            RaiseStateChanged(State, $"[Router Error] {message}");

        _routerProcess.ProcessExited += async (_, exitCode) =>
        {
            if (State == DevSessionState.Running)
            {
                RaiseStateChanged(
                    DevSessionState.Error,
                    $"Router exited unexpectedly with code {exitCode}. Attempting restart...");

                try
                {
                    await _routerProcess.StartAsync(cancellationToken);
                    State = DevSessionState.Running;
                }
                catch (Exception ex)
                {
                    State = DevSessionState.Error;
                    RaiseStateChanged(DevSessionState.Error, $"Failed to restart router: {ex.Message}", ex);
                }
            }
        };

        await _routerProcess.StartAsync(cancellationToken);
    }

    private void StartWatching()
    {
        foreach (var subgraph in _configuration.Subgraphs)
        {
            var watcher = new SubgraphWatcher(subgraph);
            watcher.SchemaChanged += OnSchemaChanged;
            watcher.Start();
            _watchers.Add(watcher);
        }

        RaiseStateChanged(State, $"Watching {_watchers.Count} subgraph(s) for changes...");
    }

    private void StopWatching()
    {
        foreach (var watcher in _watchers)
        {
            watcher.SchemaChanged -= OnSchemaChanged;
            watcher.Stop();
            watcher.Dispose();
        }
        _watchers.Clear();
    }

    private async void OnSchemaChanged(object? sender, SubgraphDefinition subgraph)
    {
        // Prevent concurrent recomposition
        if (!await _compositionLock.WaitAsync(0))
        {
            RaiseStateChanged(State, $"Ignoring change to {subgraph.Name} (recomposition already in progress).");
            return;
        }

        try
        {
            var previousState = State;
            State = DevSessionState.Composing;

            RaiseStateChanged(
                DevSessionState.Composing,
                $"Schema changed: {subgraph.Name}. Recomposing supergraph...");

            var compositionResult = await _compositionRunner.ComposeAsync(
                _configuration.Subgraphs,
                CancellationToken.None);

            if (!compositionResult.Success)
            {
                RaiseStateChanged(
                    previousState,
                    $"Composition failed after change to {subgraph.Name}. Errors: {string.Join("; ", compositionResult.Errors)}");
                State = previousState;
                return;
            }

            // Write the new supergraph
            await File.WriteAllTextAsync(
                _currentSupergraphPath!,
                compositionResult.SupergraphSdl!,
                CancellationToken.None);

            // Restart the router with the new schema
            if (_routerProcess is not null)
            {
                RaiseStateChanged(DevSessionState.Composing, "Restarting router with updated schema...");
                await _routerProcess.RestartAsync(_currentSupergraphPath!, CancellationToken.None);
            }

            State = DevSessionState.Running;
            RaiseStateChanged(
                DevSessionState.Running,
                $"Supergraph recomposed and router restarted after change to {subgraph.Name}.");
        }
        catch (Exception ex)
        {
            State = DevSessionState.Error;
            RaiseStateChanged(
                DevSessionState.Error,
                $"Error handling schema change for {subgraph.Name}: {ex.Message}",
                ex);
        }
        finally
        {
            _compositionLock.Release();
        }
    }

    private void ValidateConfiguration()
    {
        if (_configuration.Subgraphs.Count == 0)
            throw new ArgumentException("At least one subgraph must be provided.", nameof(_configuration));

        var uniqueNames = new HashSet<string>();
        foreach (var subgraph in _configuration.Subgraphs)
        {
            if (string.IsNullOrWhiteSpace(subgraph.Name))
                throw new ArgumentException("Subgraph name cannot be empty.");

            if (!uniqueNames.Add(subgraph.Name))
                throw new ArgumentException($"Duplicate subgraph name: {subgraph.Name}");

            if (!File.Exists(subgraph.SchemaPath))
                throw new ArgumentException($"Schema file not found: {subgraph.SchemaPath}");
        }
    }

    private async Task<string> LocateRouterBinaryAsync()
    {
        // For now, assume "router" or "router.exe" is in PATH
        // In a real implementation, we could query the Studio API for the latest version
        // and download it if necessary
        var routerName = OperatingSystem.IsWindows() ? "router.exe" : "router";

        // Check if it's in PATH
        var pathDirs = Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator) ?? Array.Empty<string>();
        foreach (var dir in pathDirs)
        {
            var candidate = Path.Combine(dir, routerName);
            if (File.Exists(candidate))
                return candidate;
        }

        throw new RouterProcessException(
            $"Apollo Router binary not found. Please download the router and add it to PATH, or specify RouterBinaryPath in the configuration.");
    }

    private void RaiseStateChanged(DevSessionState state, string message, Exception? exception = null)
    {
        StateChanged?.Invoke(
            this,
            new DevSessionEvent(DateTimeOffset.Now, state, message, exception));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        StopWatching();
        _routerProcess?.Dispose();
        _compositionLock.Dispose();

        // Clean up temporary supergraph file if we created one
        if (_currentSupergraphPath is not null
            && _configuration.SupergraphConfigPath is null
            && File.Exists(_currentSupergraphPath))
        {
            try
            {
                File.Delete(_currentSupergraphPath);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
