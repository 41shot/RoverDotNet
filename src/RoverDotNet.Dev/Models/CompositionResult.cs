namespace RoverDotNet.Dev.Models;

/// <summary>
/// Result of a supergraph composition operation.
/// </summary>
/// <param name="Success">Whether the composition succeeded.</param>
/// <param name="SupergraphSdl">The composed supergraph SDL (null if composition failed).</param>
/// <param name="Errors">List of composition errors (empty if successful).</param>
public sealed record CompositionResult(
    bool Success,
    string? SupergraphSdl,
    IReadOnlyList<string> Errors);
