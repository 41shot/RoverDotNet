using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace RoverDotNet.Coprocessor;

/// <summary>
/// Builds a <see cref="WebApplication"/> exposing the coprocessor endpoint. Used both by the
/// standalone <see cref="Program"/> entry point and by hosts that want to embed the coprocessor
/// in-process (e.g. the RoverDotNet demo application).
/// </summary>
public static class CoprocessorAppBuilder
{
    /// <summary>The default address the coprocessor listens on.</summary>
    public const string DefaultUrl = "http://127.0.0.1:8081";

    /// <summary>
    /// Builds (but does not start) a coprocessor <see cref="WebApplication"/>.
    /// </summary>
    /// <param name="args">Command-line arguments, forwarded to <see cref="WebApplication.CreateBuilder(string[])"/>.</param>
    /// <param name="url">The address to listen on. Defaults to <see cref="DefaultUrl"/>.</param>
    /// <param name="configureServices">An optional callback to register additional services, such as middleware.</param>
    public static WebApplication Build(string[]? args = null, string? url = null, Action<IServiceCollection>? configureServices = null)
    {
        var builder = WebApplication.CreateBuilder(args ?? []);

        builder.WebHost.UseUrls(url ?? DefaultUrl);

        builder.Services.AddCoprocessor();
        configureServices?.Invoke(builder.Services);

        var app = builder.Build();
        app.MapCoprocessorEndpoint();

        return app;
    }
}
