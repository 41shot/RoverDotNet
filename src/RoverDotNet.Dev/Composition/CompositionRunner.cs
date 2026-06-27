using System.Diagnostics;
using System.Text;
using RoverDotNet.Dev.Exceptions;
using RoverDotNet.Dev.Models;

namespace RoverDotNet.Dev.Composition;

/// <summary>
/// Wraps <c>rover supergraph compose</c> to generate a supergraph schema from subgraph definitions.
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
    /// Composes a supergraph from the given subgraph definitions.
    /// </summary>
    /// <param name="subgraphs">The list of subgraphs to compose.</param>
    /// <param name="cancellationToken">Propagates cancellation.</param>
    /// <returns>A <see cref="CompositionResult"/> containing the supergraph SDL or errors.</returns>
    /// <exception cref="DevException">Thrown if rover.exe cannot be executed.</exception>
    public async Task<CompositionResult> ComposeAsync(
        IReadOnlyList<SubgraphDefinition> subgraphs,
        CancellationToken cancellationToken = default)
    {
        if (subgraphs.Count == 0)
        {
            return new CompositionResult(
                Success: false,
                SupergraphSdl: null,
                Errors: new[] { "No subgraphs provided for composition." });
        }

        // Create a temporary supergraph config file
        var configPath = Path.GetTempFileName();
        try
        {
            await WriteSupergraphConfigAsync(configPath, subgraphs, cancellationToken);

            // Execute: rover supergraph compose --config <path>
            var startInfo = new ProcessStartInfo
            {
                FileName = _roverPath,
                Arguments = $"supergraph compose --config \"{configPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

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
        finally
        {
            // Clean up temporary config file
            try
            {
                if (File.Exists(configPath))
                    File.Delete(configPath);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    /// <summary>
    /// Writes a supergraph config YAML file for rover.
    /// Format:
    /// federation_version: =2.4.0
    /// subgraphs:
    ///   subgraph-name:
    ///     routing_url: http://...
    ///     schema:
    ///       file: ./path/to/schema.graphql
    /// </summary>
    private static async Task WriteSupergraphConfigAsync(
        string configPath,
        IReadOnlyList<SubgraphDefinition> subgraphs,
        CancellationToken cancellationToken)
    {
        var yaml = new StringBuilder();
        yaml.AppendLine("federation_version: =2.4.0");
        yaml.AppendLine("subgraphs:");

        foreach (var subgraph in subgraphs)
        {
            yaml.AppendLine($"  {subgraph.Name}:");
            yaml.AppendLine($"    routing_url: {subgraph.RoutingUrl}");
            yaml.AppendLine("    schema:");

            // Normalise path separators for YAML
            var schemaPath = subgraph.SchemaPath.Replace("\\", "/");
            yaml.AppendLine($"      file: {schemaPath}");
        }

        await File.WriteAllTextAsync(configPath, yaml.ToString(), cancellationToken);
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
}
