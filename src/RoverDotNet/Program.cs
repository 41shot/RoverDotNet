using System.CommandLine;
using System.Text.RegularExpressions;
using RoverDotNet.Commands;

namespace RoverDotNet;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        // Guard: reject abbreviated single-character arguments (e.g. -s, -n) except -h.
        // These would be silently ignored by the parser, so we catch them early and
        // inform the user to use the full parameter name instead.
        foreach (var arg in args)
        {
            if (Regex.IsMatch(arg, @"^-[a-zA-Z]$") && arg != "-h")
            {
                await Console.Error.WriteLineAsync(
                    $"error: abbreviated arguments are not supported: '{arg}'.");
                await Console.Error.WriteLineAsync(
                    "Use the full parameter name instead (e.g. '--supergraph-config').");
                await Console.Error.WriteLineAsync("Use -h or --help for usage information.");
                return 1;
            }
        }

        var rootCommand = new RootCommand("RoverDotNet \u2014 a .NET drop-in for Apollo Rover.");
        rootCommand.Subcommands.Add(DevCommandFactory.Create());

        return await rootCommand.Parse(args).InvokeAsync();
    }
}
