using System.CommandLine;
using RoverDotNet.Dev;
using RoverDotNet.Dev.Exceptions;
using RoverDotNet.Dev.Models;

namespace RoverDotNet.Commands;

/// <summary>
/// Factory that builds and returns the configured <c>dev</c> sub-command.
/// </summary>
internal static class DevCommandFactory
{
    private const string CommandDescription = """
        Run a supergraph locally to develop and test subgraph changes

        ⚠️  Do not run this command in production! ⚠️  It is intended for local development.

        You can navigate to the supergraph endpoint in your browser to execute operations
        and see query plans using Apollo Sandbox.
        """;

    // -------------------------------------------------------------------------
    // Supported options
    // -------------------------------------------------------------------------

    private static readonly Option<string?> SupergraphConfigOption = new("--supergraph-config")
    {
        Description = """
            The path to a supergraph configuration file. Subgraphs will be loaded from this file.

            Cannot be used with --url, --name, or --schema.

            For information on the format of this file, please see
            https://www.apollographql.com/docs/rover/commands/supergraphs/#yaml-configuration-file.
            """
    };

    private static readonly Option<string?> RouterConfigOption = new("--router-config")
    {
        Description = """
            The path to a router configuration file. This file is watched for changes
            and propagated to the router.

            For information on the format of this file, please see
            https://www.apollographql.com/docs/router/configuration/overview/#yaml-config-file.
            """
    };

    private static readonly Option<int> SupergraphPortOption = new("--supergraph-port")
    {
        Description = "The port the graph router should listen on.",
        DefaultValueFactory = _ => 4000
    };

    private static readonly Option<string?> SupergraphAddressOption = new("--supergraph-address")
    {
        Description = """
            The address the graph router should listen on.

            If you start multiple RoverDotNet dev processes on the same address and port,
            they will communicate with each other.
            """
    };

    private static readonly Option<string?> Elv2LicenceOption = new("--elv2-license")
    {
        Description = """
            Accept the terms and conditions of the ELv2 Licence without prompting for
            confirmation. Expected value: `accept`

            [env: APOLLO_ELV2_LICENSE=accept]
            """
    };

    private static readonly Option<bool> SkipUpdateOption = new("--skip-update")
    {
        Description = """
            Skip downloading a new version of the Apollo Router plugin.

            Passing this flag will attempt to use the latest compatible version of the
            router already installed on this machine.
            """
    };

    // -------------------------------------------------------------------------
    // Known-but-unsupported options (hidden from help; registered so the parser
    // accepts them and we can emit a friendly "not supported" message).
    // -------------------------------------------------------------------------

    private static readonly Option<string?> GraphRefOption = new("--graph-ref") { Hidden = true };
    private static readonly Option<string?> NameOption = new("--name") { Hidden = true };
    private static readonly Option<string?> UrlOption = new("--url") { Hidden = true };
    private static readonly Option<string?> SchemaOption = new("--schema") { Hidden = true };
    private static readonly Option<int?> PollingIntervalOption = new("--polling-interval") { Hidden = true };
    private static readonly Option<int?> SubgraphRetriesOption = new("--subgraph-retries") { Hidden = true };
    private static readonly Option<string?> FederationVersionOption = new("--federation-version") { Hidden = true };
    private static readonly Option<string?> LicenceFileOption = new("--license") { Hidden = true };
    private static readonly Option<string?> McpOption = new("--mcp") { Hidden = true };
    private static readonly Option<string?> McpVersionOption = new("--mcp-version") { Hidden = true };
    private static readonly Option<string?> ProfileOption = new("--profile") { Hidden = true };
    private static readonly Option<string?> FormatOption = new("--format") { Hidden = true };
    private static readonly Option<string?> OutputOption = new("--output") { Hidden = true };
    private static readonly Option<string?> LogOption = new("--log") { Hidden = true };
    private static readonly Option<bool> InsecureCertsOption = new("--insecure-accept-invalid-certs") { Hidden = true };
    private static readonly Option<bool> InsecureHostnamesOption = new("--insecure-accept-invalid-hostnames") { Hidden = true };
    private static readonly Option<int?> ClientTimeoutOption = new("--client-timeout") { Hidden = true };
    private static readonly Option<bool> SkipUpdateCheckOption = new("--skip-update-check") { Hidden = true };

    /// <summary>
    /// Creates the configured <c>dev</c> command ready to be added to a root command.
    /// </summary>
    internal static Command Create()
    {
        var command = new Command("dev", CommandDescription);

        // Supported
        command.Options.Add(SupergraphConfigOption);
        command.Options.Add(RouterConfigOption);
        command.Options.Add(SupergraphPortOption);
        command.Options.Add(SupergraphAddressOption);
        command.Options.Add(Elv2LicenceOption);
        command.Options.Add(SkipUpdateOption);

        // Unsupported (rover-compatible, hidden)
        command.Options.Add(GraphRefOption);
        command.Options.Add(NameOption);
        command.Options.Add(UrlOption);
        command.Options.Add(SchemaOption);
        command.Options.Add(PollingIntervalOption);
        command.Options.Add(SubgraphRetriesOption);
        command.Options.Add(FederationVersionOption);
        command.Options.Add(LicenceFileOption);
        command.Options.Add(McpOption);
        command.Options.Add(McpVersionOption);
        command.Options.Add(ProfileOption);
        command.Options.Add(FormatOption);
        command.Options.Add(OutputOption);
        command.Options.Add(LogOption);
        command.Options.Add(InsecureCertsOption);
        command.Options.Add(InsecureHostnamesOption);
        command.Options.Add(ClientTimeoutOption);
        command.Options.Add(SkipUpdateCheckOption);

        command.SetAction(RunAsync);

        return command;
    }

    private static async Task<int> RunAsync(ParseResult parseResult, CancellationToken cancellationToken)
    {
        // Reject any unsupported-but-recognised options that were actually supplied
        var unsupportedErrors = GetUnsupportedOptionErrors(parseResult);
        if (unsupportedErrors.Count > 0)
        {
            foreach (var error in unsupportedErrors)
                await Console.Error.WriteLineAsync(error);
            await Console.Error.WriteLineAsync(
                "Only --supergraph-config and --router-config based workflows are currently supported.");
            return 1;
        }

        var supergraphConfigPath = parseResult.GetValue(SupergraphConfigOption);
        if (string.IsNullOrWhiteSpace(supergraphConfigPath))
        {
            await Console.Error.WriteLineAsync(
                "error: --supergraph-config <SUPERGRAPH_CONFIG_PATH> is required.");
            await Console.Error.WriteLineAsync("Use -h or --help for usage information.");
            return 1;
        }

        // --elv2-license from CLI takes precedence over the environment variable.
        // If neither is supplied, DevSession will prompt interactively via the event.
        var elv2Licence = parseResult.GetValue(Elv2LicenceOption)
            ?? Environment.GetEnvironmentVariable("APOLLO_ELV2_LICENSE");

        var configuration = new DevConfiguration(
            SupergraphConfigPath: supergraphConfigPath,
            RouterConfigPath: parseResult.GetValue(RouterConfigOption),
            RouterPort: parseResult.GetValue(SupergraphPortOption),
            RouterAddress: parseResult.GetValue(SupergraphAddressOption),
            Elv2Licence: elv2Licence,
            SkipRouterUpdate: parseResult.GetValue(SkipUpdateOption));

        using var session = new DevSession(configuration);

        session.StateChanged += (_, e) =>
        {
            Console.WriteLine($"[{e.Timestamp:HH:mm:ss}] [{e.State}] {e.Message}");
            if (e.Exception != null)
                Console.WriteLine($"  Exception: {e.Exception.Message}");
        };

        session.LicenceAcceptanceRequired += () =>
        {
            Console.WriteLine();
            Console.WriteLine("The Apollo Router requires the Elastic Licence v2.0 (ELv2).");
            Console.WriteLine();
            Console.WriteLine("By running this command, you accept the terms and conditions");
            Console.WriteLine("outlined by this licence.");
            Console.WriteLine();
            Console.WriteLine("More information on the ELv2 licence can be found here:");
            Console.WriteLine("  https://go.apollo.dev/elv2");
            Console.WriteLine();
            Console.Write("Do you accept the terms and conditions of the ELv2 licence? [y/N] ");

            var response = Console.ReadLine();
            var accepted = response?.Trim().Equals("y", StringComparison.OrdinalIgnoreCase) == true;

            Console.WriteLine(accepted ? "ELv2 licence accepted." : "ELv2 licence declined. Aborting.");
            Console.WriteLine();

            return Task.FromResult(accepted);
        };

        try
        {
            await session.StartAsync(cancellationToken);

            // Block until Ctrl+C (or other cancellation)
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine();
            Console.WriteLine("Shutting down...");
        }
        catch (CompositionFailedException ex)
        {
            await Console.Error.WriteLineAsync("error: Supergraph composition failed.");
            foreach (var error in ex.CompositionErrors)
                await Console.Error.WriteLineAsync($"  {error}");
            return 1;
        }
        catch (DevException ex)
        {
            await Console.Error.WriteLineAsync($"error: {ex.Message}");
            return 1;
        }
        finally
        {
            try
            {
                await session.StopAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync($"Warning: error during shutdown — {ex.Message}");
            }
        }

        return 0;
    }

    /// <summary>
    /// Returns a list of error messages for any unsupported rover options that were
    /// explicitly supplied on the command line.
    /// </summary>
    private static IReadOnlyList<string> GetUnsupportedOptionErrors(ParseResult parseResult)
    {
        var errors = new List<string>();

        void CheckString(Option<string?> option)
        {
            if (parseResult.GetValue(option) is not null)
                errors.Add($"error: '{option.Name}' is not supported by RoverDotNet.");
        }

        void CheckNullableInt(Option<int?> option)
        {
            if (parseResult.GetValue(option) is not null)
                errors.Add($"error: '{option.Name}' is not supported by RoverDotNet.");
        }

        void CheckFlag(Option<bool> option)
        {
            if (parseResult.GetValue(option))
                errors.Add($"error: '{option.Name}' is not supported by RoverDotNet.");
        }

        CheckString(GraphRefOption);
        CheckString(NameOption);
        CheckString(UrlOption);
        CheckString(SchemaOption);
        CheckNullableInt(PollingIntervalOption);
        CheckNullableInt(SubgraphRetriesOption);
        CheckString(FederationVersionOption);
        CheckString(LicenceFileOption);
        CheckString(McpOption);
        CheckString(McpVersionOption);
        CheckString(ProfileOption);
        CheckString(FormatOption);
        CheckString(OutputOption);
        CheckString(LogOption);
        CheckFlag(InsecureCertsOption);
        CheckFlag(InsecureHostnamesOption);
        CheckNullableInt(ClientTimeoutOption);
        CheckFlag(SkipUpdateCheckOption);

        return errors;
    }
}
