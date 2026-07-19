# RoverDotNet.Dev

`RoverDotNet.Dev` implements the orchestration behind Rover's `dev` command: composing a supergraph from subgraph schemas, launching a local Apollo Router instance, and recomposing/restarting it when configuration changes — mirroring the session orchestration in Rover's `src/command/dev/`.

## Core components

- **`DevSession`** — the main entry point. Orchestrates the full lifecycle: composes the supergraph, starts the router process, watches the supergraph configuration file for changes via `FileSystemWatcher`, and recomposes/restarts as needed.
- **`DevConfiguration`** — a record describing session configuration: `SupergraphConfigPath`/`SupergraphConfigContent` (file path or inline YAML — one is required), `RouterPort` (default `4000`), `RouterAddress`, `RouterBinaryPath`, `ComposedSupergraphPath`, `RouterConfigPath`, `Elv2Licence`, `IgnoreSupergraphChanges`, and `SkipRouterUpdate`.
- **`CompositionRunner`** (`Composition/`) — runs subgraph schema composition and produces a `CompositionResult` (success/errors, composed supergraph SDL).
- **`RouterBinaryManager` / `RouterProcess`** (`Router/`) — locate/download the Apollo Router binary and manage its process lifecycle (start, stop, output/error streaming, exit handling).
- **`DevSessionState`** / **`DevSessionEvent`** — the session's lifecycle state (`Idle`, `Starting`, `Running`, `Stopping`, `Stopped`, `Error`) and the event payload raised via `DevSession.StateChanged`.
- **Exceptions** (`Exceptions/`) — `DevException`, `CompositionFailedException`, `RouterProcessException` for session, composition, and router process failures respectively.

## Usage

```csharp
var configuration = new DevConfiguration(
	SupergraphConfigPath: "supergraph.yaml",
	RouterConfigPath: "router.yaml",
	RouterPort: 4000,
	Elv2Licence: "accept"); // or rely on the LicenceAcceptanceRequired event

using var session = new DevSession(configuration);

session.StateChanged += (_, e) =>
	Console.WriteLine($"[{e.Timestamp:HH:mm:ss}] [{e.State}] {e.Message}");

// Only needed if Elv2Licence isn't pre-accepted via configuration/environment variable.
session.LicenceAcceptanceRequired += async () =>
{
	Console.WriteLine("Accept the Elastic License v2.0 (ELv2)? [y/n]");
	return Console.ReadLine()?.Trim().Equals("y", StringComparison.OrdinalIgnoreCase) ?? false;
};

await session.StartAsync();
// ... session runs, watching for supergraph config changes ...
await session.StopAsync();
```

### Licence acceptance

The Apollo Router binary requires accepting the Elastic License v2.0 (ELv2). This can be satisfied in three ways, in priority order:
1. `DevConfiguration.Elv2Licence` set to `"accept"` (case-insensitive).
2. The `APOLLO_ELV2_LICENSE=accept` environment variable (handled by the `RoverDotNet` CLI when building `DevConfiguration`).
3. Handling the `DevSession.LicenceAcceptanceRequired` event and returning `true`/`false` interactively.

If none of these apply, the licence is declined by default and startup will fail.

### File watching

By default, `DevSession` watches the supergraph configuration file and automatically recomposes and restarts the router on changes. Set `DevConfiguration.IgnoreSupergraphChanges = true` to disable this (a .NET-specific extension with no Rover CLI equivalent) if you want a one-shot composition/start with no live reload.

## Consumed by

The `RoverDotNet` console application's `dev` command builds a `DevConfiguration` from CLI arguments and drives a `DevSession` directly — see [RoverDotNet CLI documentation](ROVERDOTNET_CLI.md).
