namespace RoverDotNet.Dev.Models;

/// <summary>
/// Configuration for a <see cref="DevSession"/>.
/// </summary>
/// <param name="SupergraphConfigPath">
/// Path to an existing supergraph configuration YAML file. 
/// This file should follow rover's supergraph config format.
/// If null, SupergraphConfigContent must be provided.
/// </param>
/// <param name="SupergraphConfigContent">
/// Raw YAML content of the supergraph configuration.
/// Used when the config is provided as a string instead of a file path.
/// Will be written to a temporary file. If null, SupergraphConfigPath must be provided.
/// </param>
/// <param name="RouterPort">The port on which the Apollo Router will listen (default: 4000).</param>
/// <param name="RouterBinaryPath">
/// Optional path to the Apollo Router binary. If null, the system will attempt to download
/// or locate the router automatically.
/// </param>
/// <param name="ComposedSupergraphPath">
/// Optional path to save the composed supergraph schema. If null, a temporary file is used.
/// </param>
/// <param name="RouterConfigPath">Optional path to the router configuration YAML file.</param>
/// <param name="Elv2Licence">
/// Optional ELv2 licence acceptance. If set to "accept" (case insensitive), 
/// the licence acceptance prompt will be suppressed and automatically accepted.
/// </param>
/// <param name="IgnoreSupergraphChanges">
/// When <see langword="true" />, the session will not watch the supergraph configuration file
/// for changes. Recomposition and router restarts will only occur on session start.
/// Defaults to <see langword="false" />.
/// </param>
public sealed record DevConfiguration(
    string? SupergraphConfigPath = null,
    string? SupergraphConfigContent = null,
    int RouterPort = 4000,
    string? RouterBinaryPath = null,
    string? ComposedSupergraphPath = null,
    string? RouterConfigPath = null,
    string? Elv2Licence = null,
    bool IgnoreSupergraphChanges = false);
