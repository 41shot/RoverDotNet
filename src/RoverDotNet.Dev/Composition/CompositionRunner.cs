using System.Diagnostics;
using System.Text;
using RoverDotNet.Core.Config;
using RoverDotNet.Dev.Exceptions;
using RoverDotNet.Dev.Models;

namespace RoverDotNet.Dev.Composition;

/// <summary>
/// Wraps <c>rover supergraph compose</c> to generate a supergraph schema from a supergraph config file.
/// Mirrors the composition logic in <c>src/composition/supergraph/</c>.
/// </summary>
public sealed class CompositionRunner
{
    private const string RoverExecutable = "rover.exe";
    private readonly string _roverPath;

    /// <summary>
    /// Creates a new composition runner.
    /// </summary>
    /// <param name="roverPath">
    /// Optional path to the rover executable. If null, assumes "rover.exe" is in PATH.
    /// </param>
    public CompositionRunner(string? roverPath = null)
    {
        _roverPath = roverPath ?? RoverExecutable;
    }

    /// <summary>
    /// Event raised when the ELv2 licence acceptance is required.
    /// The handler should return true to accept the licence, false to decline.
    /// </summary>
    public event Func<Task<bool>>? LicenceAcceptanceRequired;

    /// <summary>
    /// Composes a supergraph from a supergraph configuration file.
    /// </summary>
    /// <param name="supergraphConfigPath">Path to the supergraph configuration YAML file.</param>
    /// <param name="cancellationToken">Propagates cancellation.</param>
    /// <returns>A <see cref="CompositionResult"/> containing the supergraph SDL or errors.</returns>
    /// <exception cref="DevException">Thrown if rover.exe cannot be executed.</exception>
    public async Task<CompositionResult> ComposeAsync(
        string supergraphConfigPath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(supergraphConfigPath))
        {
            return new CompositionResult(
                Success: false,
                SupergraphSdl: null,
                Errors: new[] { "Supergraph config path is required." });
        }

        if (!File.Exists(supergraphConfigPath))
        {
            return new CompositionResult(
                Success: false,
                SupergraphSdl: null,
                Errors: new[] { $"Supergraph config file not found: {supergraphConfigPath}" });
        }

        // Check if ELv2 licence needs to be accepted
        var acceptLicence = await EnsureLicenceAcceptanceAsync();

        // Execute: rover supergraph compose --config <path>
        var result = await ExecuteRoverComposeAsync(supergraphConfigPath, cancellationToken, acceptLicence);

        return result;
    }

    /// <summary>
    /// Parses composition errors from rover's stderr output.
    /// </summary>
    private static IReadOnlyList<string> ParseCompositionErrors(string errorOutput)
    {
        if (string.IsNullOrWhiteSpace(errorOutput))
            return new[] { "Composition failed with unknown error." };

        // Split by line and filter out empty lines
        var lines = errorOutput
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrEmpty(line))
            .ToList();

        return lines.Count > 0
            ? lines
            : new[] { "Composition failed with unknown error." };
    }

    /// <summary>
    /// Executes the rover compose command.
    /// </summary>
    private async Task<CompositionResult> ExecuteRoverComposeAsync(
        string configPath,
        CancellationToken cancellationToken,
        bool acceptLicence = false)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = _roverPath,
            Arguments = $"supergraph compose --config \"{configPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        // Set environment variable to accept ELv2 licence
        if (acceptLicence)
        {
            EnvironmentVariableHelper.SetUserValue("APOLLO_ELV2_LICENSE", "accept");
        }

        using var process = new Process { StartInfo = startInfo };
        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null) outputBuilder.AppendLine(e.Data);
        };
        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null) errorBuilder.AppendLine(e.Data);
        };

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync(cancellationToken);

            var output = outputBuilder.ToString();
            var error = errorBuilder.ToString();

            if (process.ExitCode == 0)
            {
                // Success: output contains the supergraph SDL
                return new CompositionResult(
                    Success: true,
                    SupergraphSdl: output,
                    Errors: Array.Empty<string>());
            }
            else
            {
                // Failure: parse errors from stderr
                var errors = ParseCompositionErrors(error);
                return new CompositionResult(
                    Success: false,
                    SupergraphSdl: null,
                    Errors: errors);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new DevException(
                $"Failed to execute rover composition: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Checks if the ELv2 licence has been accepted via environment variable.
    /// If not, requests acceptance from the user.
    /// </summary>
    /// <returns>True if the licence should be accepted for this execution, false otherwise.</returns>
    private async Task<bool> EnsureLicenceAcceptanceAsync()
    {
        // Check if the environment variable is already set
        var envValue = EnvironmentVariableHelper.GetValue("APOLLO_ELV2_LICENSE");
        if (string.Equals(envValue, "accept", StringComparison.OrdinalIgnoreCase))
        {
            return true; // Already accepted globally
        }

        // Not accepted yet - prompt the user
        if (LicenceAcceptanceRequired == null)
            return false; // No handler registered, decline by default

        try
        {
            return await LicenceAcceptanceRequired.Invoke();
        }
        catch
        {
            return false; // On error, decline by default
        }
    }
}
