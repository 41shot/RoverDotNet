namespace RoverDotNet.Dev.Models;

/// <summary>
/// Defines a single subgraph participating in the local supergraph composition.
/// </summary>
/// <param name="Name">The subgraph name (must be unique within a supergraph).</param>
/// <param name="RoutingUrl">The URL where this subgraph can be reached at runtime.</param>
/// <param name="SchemaPath">Absolute or relative file path to the subgraph SDL schema.</param>
public sealed record SubgraphDefinition(
    string Name,
    string RoutingUrl,
    string SchemaPath);
