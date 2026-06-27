using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using RoverDotNet.Dev.Exceptions;

namespace RoverDotNet.Dev.Router;

/// <summary>
/// Manages the lifecycle of an Apollo Router process.
/// Mirrors the router process management in <c>src/command/dev/router/</c>.
/// </summary>
public sealed class RouterProcess : IDisposable
{
    private readonly string _routerBinaryPath;
    private readonly int _port;
    private string _supergraphSchemaPath;
    private Process? _process;
    private bool _disposed;

    /// <summary>
    /// Raised when the router process writes to stdout.
    /// </summary>
    public event EventHandler<string>? OutputReceived;

    /// <summary>
    /// Raised when the router process writes to stderr.
    /// </summary>
    public event EventHandler<string>? ErrorReceived;

    /// <summary>
    /// Raised when the router process exits unexpectedly.
    /// </summary>
    public event EventHandler<int>? ProcessExited;

    /// <summary>
    /// Gets a value indicating whether the router process is currently running.
    /// </summary>
    public bool IsRunning => _process is { HasExited: false };

    /// <summary>
    /// Creates a new router process manager.
    /// </summary>
    /// <param name="routerBinaryPath">Path to the Apollo Router executable.</param>
    /// <param name="port">The port on which the router will listen.</param>
    /// <param name="supergraphSchemaPath">Path to the supergraph schema file.</param>
    public RouterProcess(string routerBinaryPath, int port, string supergraphSchemaPath)
    {
        _routerBinaryPath = routerBinaryPath;
        _port = port;
        _supergraphSchemaPath = supergraphSchemaPath;
    }

    /// <summary>
    /// Starts the router process.
    /// </summary>
    /// <param name="cancellationToken">Propagates cancellation.</param>
    /// <exception cref="RouterProcessException">Thrown if the router fails to start.</exception>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (IsRunning)
            throw new RouterProcessException("Router is already running.");

        if (!File.Exists(_routerBinaryPath))
        {
            throw new RouterProcessException(
                $"Router binary not found: {_routerBinaryPath}");
        }

        if (!File.Exists(_supergraphSchemaPath))
        {
            throw new RouterProcessException(
                $"Supergraph schema not found: {_supergraphSchemaPath}");
        }

        // Ensure the port is available
        if (!IsPortAvailable(_port))
        {
            throw new RouterProcessException(
                $"Port {_port} is already in use. Choose a different port.");
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = _routerBinaryPath,
            Arguments = $"--supergraph \"{_supergraphSchemaPath}\" --port {_port}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _process = new Process { StartInfo = startInfo };
        _process.EnableRaisingEvents = true;
        _process.Exited += OnProcessExited;

        _process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null) OutputReceived?.Invoke(this, e.Data);
        };
        _process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null) ErrorReceived?.Invoke(this, e.Data);
        };

        try
        {
            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            // Wait for the router to become ready (health check)
            await WaitForHealthCheckAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Clean up the process if startup fails
            try
            {
                _process?.Kill();
            }
            catch
            {
                // Ignore
            }
            _process?.Dispose();
            _process = null;

            throw new RouterProcessException(
                $"Failed to start router: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Stops the router process gracefully.
    /// </summary>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_process is null || _process.HasExited)
            return;

        try
        {
            // Attempt graceful shutdown
            _process.Kill();
            await _process.WaitForExitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new RouterProcessException(
                $"Failed to stop router: {ex.Message}", ex);
        }
        finally
        {
            _process?.Dispose();
            _process = null;
        }
    }

    /// <summary>
    /// Restarts the router process with a new supergraph schema.
    /// </summary>
    /// <param name="newSupergraphSchemaPath">Path to the updated supergraph schema.</param>
    /// <param name="cancellationToken">Propagates cancellation.</param>
    public async Task RestartAsync(
        string newSupergraphSchemaPath,
        CancellationToken cancellationToken = default)
    {
        await StopAsync(cancellationToken);

        // Update the schema path
        _supergraphSchemaPath = newSupergraphSchemaPath;

        await StartAsync(cancellationToken);
    }

    /// <summary>
    /// Waits for the router's health check endpoint to respond.
    /// </summary>
    private async Task WaitForHealthCheckAsync(CancellationToken cancellationToken)
    {
        var healthUrl = $"http://localhost:{_port}/health";
        using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(1) };

        var maxAttempts = 30; // 30 seconds max
        for (var i = 0; i < maxAttempts; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var response = await httpClient.GetAsync(healthUrl, cancellationToken);
                if (response.IsSuccessStatusCode)
                    return; // Router is ready
            }
            catch
            {
                // Ignore and retry
            }

            await Task.Delay(1000, cancellationToken);
        }

        throw new RouterProcessException(
            "Router health check timed out. The router may have failed to start.");
    }

    /// <summary>
    /// Checks if a TCP port is available for binding.
    /// </summary>
    private static bool IsPortAvailable(int port)
    {
        try
        {
            using var listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            listener.Stop();
            return true;
        }
        catch (SocketException)
        {
            return false;
        }
    }

    private void OnProcessExited(object? sender, EventArgs e)
    {
        if (_process is not null)
        {
            var exitCode = _process.ExitCode;
            ProcessExited?.Invoke(this, exitCode);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        if (_process is not null)
        {
            _process.Exited -= OnProcessExited;
            if (!_process.HasExited)
            {
                try
                {
                    _process.Kill();
                    _process.WaitForExit(5000);
                }
                catch
                {
                    // Ignore
                }
            }
            _process.Dispose();
        }
    }
}
