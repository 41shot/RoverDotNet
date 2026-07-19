# RoverDotNet (CLI)

`RoverDotNet` is the console application entry point for the solution — a .NET drop-in replacement for a subset of the [Apollo Rover](https://github.com/apollographql/rover) CLI, built with `System.CommandLine`.

## Purpose

It exposes Rover-compatible sub-commands so existing scripts/workflows built around `rover` can be adapted to a native .NET tool. Currently this is limited to the `dev` command; other projects in the solution (`RoverDotNet.Config`, `RoverDotNet.Dev`) provide functionality that is consumed as a library rather than via the CLI.

## `dev` command

Runs a supergraph locally to develop and test subgraph changes, backed by `RoverDotNet.Dev`'s `DevSession`. Do not run it in production — it's intended for local development only.

### Supported options

| Option | Notes |
|--------|-------|
| `--supergraph-config <path>` | Required. Path to a supergraph configuration YAML file (subgraphs to compose). Cannot be combined with `--url`, `--name`, or `--schema`. |
| `--router-config <path>` | Path to a router configuration YAML file. Watched for changes and propagated to the router. |
| `--supergraph-port <port>` | Port the router listens on (default `4000`). |
| `--supergraph-address <address>` | Address the router listens on. |
| `--elv2-license accept` | Accepts the Elastic License v2.0 non-interactively. Can also be supplied via the `APOLLO_ELV2_LICENSE=accept` environment variable. |
| `--skip-update` | Skip downloading a new Apollo Router binary; use the latest compatible version already installed. |

### Example

```powershell
RoverDotNet dev --supergraph-config supergraph.yaml --router-config router.yaml --supergraph-port 4000
```

Once running, navigate to the router's endpoint in a browser to execute operations and view query plans via Apollo Sandbox.

### Unsupported (recognised but rejected) options

For compatibility, a number of `rover dev` options are recognised by the parser but rejected with an explanatory error if supplied, since they aren't implemented (see the [root README](../README.md#rover-dev-command---parameter-implementation-status) for the full status table): `--graph-ref`, `--name`, `--url`, `--schema`, `--polling-interval`, `--subgraph-retries`, `--federation-version`, `--license`, `--mcp`, `--mcp-version`, `--profile`, `--format`, `--output`, `--log`, `--insecure-accept-invalid-certs`, `--insecure-accept-invalid-hostnames`, `--client-timeout`, `--skip-update-check`.

Abbreviated single-character flags (e.g. `-s`) are also rejected with a message to use the full option name instead — only `-h`/`--help` is treated as a valid short form.
