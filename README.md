# RoverDotNet
Experimental .NET port/wrapper for Apollo Rover.

The goal is to port some Apollo Rover GraphQL CLI operations to a C# .NET library, while wrapping others.

Currently ported:
1. Config whoami
2. Config auth
3. Dev

A .NET Winforms demo application will also be created to demonstrate the usage of the library.

## Rover Dev Command - Parameter Implementation Status

The following table shows the implementation status of `rover dev` parameters in RoverDotNet.Dev:

| Parameter | Status | Priority | Notes |
|-----------|--------|----------|-------|
| `elv2-license <ELV2_LICENSE_ACCEPTED>` | ✅ Done | High | Available as `DevConfiguration.Elv2Licence` |
| `schema <SCHEMA_PATH>` | ✅ Done | High | Available as `SubgraphDefinition.SchemaPath` |
| `supergraph-port <SUPERGRAPH_PORT>` | ✅ Done | High | Available as `DevConfiguration.RouterPort` (default: 4000) |
| `router-config <ROUTER_CONFIG_PATH>` | ✅ Done | High | Available as `DevConfiguration.RouterConfigPath` |
| `supergraph-config <SUPERGRAPH_CONFIG_PATH>` | ✅ Done | High | Available as `DevConfiguration.SupergraphConfigPath` |
| `skip-update` | 📋 Planned | Medium | Planned for future implementation |
| `polling-interval <SUBGRAPH_POLLING_INTERVAL>` | 📋 Planned | Medium | Planned for future implementation |
| `log <LOG_LEVEL>` | 📋 Planned | Medium | Planned for future implementation |
| `client-timeout <CLIENT_TIMEOUT>` | 📋 Planned | Medium | Planned for future implementation |
| `skip-update-check` | 📋 Planned | Medium | Planned for future implementation |
| `profile <PROFILE_NAME>` | 📋 Planned | Low | Planned for future implementation |
| `name <SUBGRAPH_NAME>` | ❌ Not planned | Low | Not planned for implementation |
| `url <SUBGRAPH_URL>` | ❌ Not planned | Low | Not planned for implementation |
| `subgraph-retries <SUBGRAPH_RETRIES>` | ❌ Not planned | Low | Not planned for implementation |
| `supergraph-address <SUPERGRAPH_ADDRESS>` | ❌ Not planned | Low | Not planned for implementation |
| `graph-ref <GRAPH_REF>` | ❌ Not planned | Low | Not planned for implementation |
| `federation-version <FEDERATION_VERSION>` | ❌ Not planned | Low | Not planned for implementation |
| `license <LICENSE>` | ❌ Not planned | Low | Not planned for implementation |
| `mcp [<CONFIG>]` | ❌ Not planned | Low | Not planned for implementation |
| `mcp-version <VERSION>` | ❌ Not planned | Low | Not planned for implementation |
| `format <FORMAT_KIND>` | ❌ Not planned | Low | Not planned for implementation |
| `output <OUTPUT_FILE>` | ❌ Not planned | Low | Not planned for implementation |
| `insecure-accept-invalid-certs` | ❌ Not planned | Low | Not planned for implementation |
| `insecure-accept-invalid-hostnames` | ❌ Not planned | Low | Not planned for implementation |
| `help` | ❌ Not planned | Low | Not planned for implementation |

### Legend
- ✅ **Done**: Fully implemented and available in the current version
- 📋 **Planned**: Scheduled for future implementation
- ❌ **Not planned**: No current plans for implementation
