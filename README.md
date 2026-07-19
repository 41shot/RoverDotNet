# RoverDotNet
Experimental .NET port/wrapper for Apollo Rover and Coprocessor for Apollo Router, crafted by Claude.

## Purpose

RoverDotNet aims to bring core [Apollo Rover](https://github.com/apollographql/rover) GraphQL CLI/tooling capabilities into the .NET ecosystem, so .NET teams working with Apollo Federation supergraphs can integrate the same operations (auth, `rover dev`, router coprocessors) directly into C# applications and services, rather than shelling out to the Rover CLI. Some operations are ported as native .NET implementations; others wrap the underlying Rover CLI/router behaviour.

A .NET WinForms demo application (`RoverDotNet.Demo`) is included to exercise and showcase the library's functionality.

## Projects

| Project | Purpose |
|---------|---------|
| `RoverDotNet` | Console app, similar in spirit to the Apollo Rover CLI — see [CLI documentation](docs/ROVERDOTNET_CLI.md). |
| `RoverDotNet.Config` | Rover config operations (`whoami`, `auth`) — see [Config documentation](docs/ROVERDOTNET_CONFIG.md). |
| `RoverDotNet.Client` | Apollo API client. |
| `RoverDotNet.Dev` | `rover dev` operation — composes and runs a local supergraph from subgraph schemas — see [Dev documentation](docs/ROVERDOTNET_DEV.md). |
| `RoverDotNet.Core` | Shared interfaces, models and functionality used across the solution. |
| `RoverDotNet.Coprocessor` | Apollo Router external coprocessor host — see [Coprocessor documentation](docs/COPROCESSOR.md) for details and a usage guide. |
| `RoverDotNet.Demo` | WinForms demo application showcasing the library. |
| `RoverDotNet.Demo.Api.Users` / `RoverDotNet.Demo.Api.Products` | Sample GraphQL subgraphs used for demos/testing — see [Demo Services documentation](docs/DEMO_SERVICES.md). |

Currently ported:
1. Config whoami
2. Config auth
3. Dev
4. Router coprocessor (RoverDotNet.Coprocessor)

## Rover Dev Command - Parameter Implementation Status

The following table shows the implementation status of `rover dev` parameters in RoverDotNet.Dev:

| Parameter | Status | Priority | Notes |
|-----------|--------|----------|-------|
| `elv2-license <ELV2_LICENSE_ACCEPTED>` | ✅ | High | Available as `DevConfiguration.Elv2Licence` |
| `schema <SCHEMA_PATH>` | ✅ | High | Available as `SubgraphDefinition.SchemaPath` |
| `supergraph-port <SUPERGRAPH_PORT>` | ✅ | High | Available as `DevConfiguration.RouterPort` (default: 4000) |
| `router-config <ROUTER_CONFIG_PATH>` | ✅ | High | Available as `DevConfiguration.RouterConfigPath` |
| `supergraph-config <SUPERGRAPH_CONFIG_PATH>` | ✅ | High | Available as `DevConfiguration.SupergraphConfigPath` |
| `skip-update` | 📋 | Medium | Planned for future implementation |
| `log <LOG_LEVEL>` | 📋 | Medium | Planned for future implementation |
| `client-timeout <CLIENT_TIMEOUT>` | 📋 | Medium | Planned for future implementation |
| `skip-update-check` | 📋 | Medium | Planned for future implementation |
| `profile <PROFILE_NAME>` | 📋 | Low | Planned for future implementation |
| `polling-interval <SUBGRAPH_POLLING_INTERVAL>` | ❌ | Low | Superseded by `FileSystemWatcher`-based change detection |
| `name <SUBGRAPH_NAME>` | ❌ | Low | Not planned for implementation |
| `url <SUBGRAPH_URL>` | ❌ | Low | Not planned for implementation |
| `subgraph-retries <SUBGRAPH_RETRIES>` | ❌ | Low | Not planned for implementation |
| `supergraph-address <SUPERGRAPH_ADDRESS>` | ❌ | Low | Not planned for implementation |
| `graph-ref <GRAPH_REF>` | ❌ | Low | Not planned for implementation |
| `federation-version <FEDERATION_VERSION>` | ❌ | Low | Not planned for implementation |
| `license <LICENSE>` | ❌ | Low | Not planned for implementation |
| `mcp [<CONFIG>]` | ❌ | Low | Not planned for implementation |
| `mcp-version <VERSION>` | ❌ | Low | Not planned for implementation |
| `format <FORMAT_KIND>` | ❌ | Low | Not planned for implementation |
| `output <OUTPUT_FILE>` | ❌ | Low | Not planned for implementation |
| `insecure-accept-invalid-certs` | ❌ | Low | Not planned for implementation |
| `insecure-accept-invalid-hostnames` | ❌ | Low | Not planned for implementation |
| `help` | ❌ | Low | Not planned for implementation |

### .NET-specific extensions

| Property | Notes |
|----------|-------|
| `DevConfiguration.IgnoreSupergraphChanges` | When `true`, disables `FileSystemWatcher` so the session won't recompose/restart on config file changes. No rover CLI equivalent. |

### Legend
- ✅ **Done**: Fully implemented and available in the current version
- 📋 **Planned**: Scheduled for future implementation
- ❌ **Not planned**: No current plans for implementation

