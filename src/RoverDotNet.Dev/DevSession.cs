using RoverDotNet.Dev.Composition;
using RoverDotNet.Dev.Exceptions;
using RoverDotNet.Dev.Models;
using RoverDotNet.Dev.Router;

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
    private RouterProcess? _routerProcess;
    private string? _supergraphConfigPath;
    private string? _currentSupergraphPath;
    private FileSystemWatcher? _configWatcher;
    private DevSessionState _state;
    private bool _disposed;
    private readonly SemaphoreSlim _compositionLock;

    /// <summary>
    /// Raised when the session state changes.
    /// </summary>
    public event EventHandler<DevSessionEvent>? StateChanged;

    /// <summary>
    /// Raised when the ELv2 licence acceptance is required for the router plugin.
    /// The handler should return true to accept the licence, false to decline.
    /// </summary>
    public event Func<Task<bool>>? LicenceAcceptanceRequired;

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
        _state = DevSessionState.Idle;
        _compositionLock = new SemaphoreSlim(1, 1);

        // Forward licence acceptance requests
        _compositionRunner.LicenceAcceptanceRequired += async () =>
        {
            // Check if licence is pre-accepted via configuration
            if (!string.IsNullOrWhiteSpace(_configuration.Elv2Licence) &&
                _configuration.Elv2Licence.Equals("accept", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (LicenceAcceptanceRequired != null)
                return await LicenceAcceptanceRequired.Invoke();
            return false; // Decline by default if no handler is registered
        };

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

            // Prepare supergraph config file path
            _supergraphConfigPath = await PrepareSupergraphConfigAsync(cancellationToken);

            // Initial composition
            RaiseStateChanged(DevSessionState.Starting, "Performing initial supergraph composition...");
            var compositionResult = await _compositionRunner.ComposeAsync(
                _supergraphConfigPath,
                cancellationToken);

            if (!compositionResult.Success)
            {
                State = DevSessionState.Error;
                throw new CompositionFailedException(compositionResult.Errors);
            }

            // Write supergraph to file
            _currentSupergraphPath = _configuration.ComposedSupergraphPath
                ?? Path.Combine(Path.GetTempPath(), $"supergraph-{Guid.NewGuid()}.graphql");

            await File.WriteAllTextAsync(
                _currentSupergraphPath,
                compositionResult.SupergraphSdl!,
                cancellationToken);

            RaiseStateChanged(DevSessionState.Starting, $"Supergraph schema written to: {_currentSupergraphPath}");

            // Start the router
            await StartRouterAsync(cancellationToken);

            // Start watching supergraph config file for changes
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
            ?? await LocateRouterBinaryAsync(cancellationToken);

        RaiseStateChanged(DevSessionState.Starting, $"Starting router from: {routerBinaryPath}");

        _routerProcess = new RouterProcess(
            routerBinaryPath,
            _configuration.RouterPort,
            _currentSupergraphPath!,
            _configuration.RouterConfigPath);

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
        if (_supergraphConfigPath == null || _configuration.IgnoreSupergraphChanges)
        {
            return;
        }

        var directory = Path.GetDirectoryName(_supergraphConfigPath);
        var fileName = Path.GetFileName(_supergraphConfigPath);

        if (string.IsNullOrEmpty(directory) || string.IsNullOrEmpty(fileName))
            return;

        _configWatcher = new FileSystemWatcher(directory, fileName)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
        };

        _configWatcher.Changed += OnSupergraphConfigChanged;
        _configWatcher.EnableRaisingEvents = true;

        RaiseStateChanged(State, $"Watching supergraph config for changes: {_supergraphConfigPath}");
    }

    private void StopWatching()
    {
        if (_configWatcher != null)
        {
            _configWatcher.Changed -= OnSupergraphConfigChanged;
            _configWatcher.Dispose();
            _configWatcher = null;
        }
    }

    private async void OnSupergraphConfigChanged(object sender, FileSystemEventArgs e)
    {
        // Prevent concurrent recomposition
        if (!await _compositionLock.WaitAsync(0))
        {
            // Ignoring config change (recomposition already in progress).
            return;
        }

        try
        {
            // Debounce: wait a bit to ensure file write is complete
            await Task.Delay(500);

            var previousState = State;
            State = DevSessionState.Composing;

            RaiseStateChanged(
                DevSessionState.Composing,
                "Supergraph config changed. Recomposing supergraph...");

            var compositionResult = await _compositionRunner.ComposeAsync(
                _supergraphConfigPath!,
                CancellationToken.None);

            if (!compositionResult.Success)
            {
                RaiseStateChanged(
                    previousState,
                    $"Composition failed after config change. Errors: {string.Join("; ", compositionResult.Errors)}");
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
                "Supergraph recomposed and router restarted after config change.");
        }
        catch (Exception ex)
        {
            State = DevSessionState.Error;
            RaiseStateChanged(
                DevSessionState.Error,
                $"Error handling config change: {ex.Message}",
                ex);
        }
        finally
        {
            _compositionLock.Release();
        }
    }

    private async Task<string> PrepareSupergraphConfigAsync(CancellationToken cancellationToken)
    {
        // If a file path is provided, use it directly
        if (!string.IsNullOrWhiteSpace(_configuration.SupergraphConfigPath))
        {
            if (!File.Exists(_configuration.SupergraphConfigPath))
            {
                throw new ArgumentException(
                    $"Supergraph config file not found: {_configuration.SupergraphConfigPath}",
                    nameof(_configuration));
            }

            return _configuration.SupergraphConfigPath;
        }

        // Otherwise, write the content to a temp file
        if (!string.IsNullOrWhiteSpace(_configuration.SupergraphConfigContent))
        {
            var tempPath = Path.Combine(Path.GetTempPath(), $"supergraph-config-{Guid.NewGuid()}.yaml");
            await File.WriteAllTextAsync(tempPath, _configuration.SupergraphConfigContent, cancellationToken);

            return tempPath;
        }

        throw new ArgumentException(
            "Either SupergraphConfigPath or SupergraphConfigContent must be provided.",
            nameof(_configuration));
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_configuration.SupergraphConfigPath) &&
            string.IsNullOrWhiteSpace(_configuration.SupergraphConfigContent))
        {
            throw new ArgumentException(
                "Either SupergraphConfigPath or SupergraphConfigContent must be provided.",
                nameof(_configuration));
        }

        if (_configuration.RouterPort < 1 || _configuration.RouterPort > 65535)
        {
            throw new ArgumentException(
                "RouterPort must be between 1 and 65535.",
                nameof(_configuration));
        }
    }

    private async Task<string> LocateRouterBinaryAsync(CancellationToken cancellationToken = default)
    {
        var routerManager = new RouterBinaryManager();

        // Forward download progress to state changed events
        routerManager.DownloadProgressChanged += (_, progress) =>
            RaiseStateChanged(State, progress.Message);

        try
        {
            return await routerManager.LocateOrDownloadAsync(
                preferredVersion: null,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new RouterProcessException(
                $"Failed to locate or download Apollo Router binary: {ex.Message}", ex);
        }
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

        // Clean up temporary supergraph config file if we created one
        if (_supergraphConfigPath is not null
            && _configuration.SupergraphConfigPath is null
            && File.Exists(_supergraphConfigPath))
        {
            try
            {
                File.Delete(_supergraphConfigPath);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        // Clean up temporary composed supergraph file if we created one
        if (_currentSupergraphPath is not null
            && _configuration.ComposedSupergraphPath is null
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
