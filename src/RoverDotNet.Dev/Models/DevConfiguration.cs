namespace RoverDotNet.Dev.Models;

/// <summary>
/// Configuration for a <see cref="DevSession"/>.
/// </summary>
/// <param name="Subgraphs">The list of subgraphs to compose and watch.</param>
/// <param name="RouterPort">The port on which the Apollo Router will listen (default: 4000).</param>
/// <param name="RouterBinaryPath">
/// Optional path to the Apollo Router binary. If null, the system will attempt to download
/// or locate the router automatically.
/// </param>
/// <param name="SupergraphConfigPath">
/// Optional path to save the composed supergraph schema. If null, a temporary file is used.
/// </param>
/// <param name="RouterConfigPath">Optional path to the router configuration YAML file.</param>
/// <param name="Elv2Licence">
/// Optional ELv2 licence acceptance. If set to "accept" (case insensitive), 
/// the licence acceptance prompt will be suppressed and automatically accepted.
/// </param>
public sealed record DevConfiguration(
    IReadOnlyList<SubgraphDefinition> Subgraphs,
    int RouterPort = 4000,
    string? RouterBinaryPath = null,
    string? SupergraphConfigPath = null,
    string? RouterConfigPath = null,
    string? Elv2Licence = null);
