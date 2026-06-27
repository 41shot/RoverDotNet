using Microsoft.Extensions.DependencyInjection;
using RoverDotNet.Client.Http;
using RoverDotNet.Client.Operations.WhoAmI;
using RoverDotNet.Config.WhoAmI;
using RoverDotNet.Core.Config;
using RoverDotNet.Demo.Forms;

namespace RoverDotNet.Demo;

/// <summary>
/// Configures dependency injection services for the RoverDotNet Demo application.
/// </summary>
internal static class ServiceConfiguration
{
    /// <summary>
    /// Configures all services required by the application.
    /// </summary>
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        // Core configuration
        services.AddSingleton<ProfileConfig>();

        // HTTP Client for StudioHttpClient
        services.AddHttpClient<StudioHttpClient>((serviceProvider, httpClient) =>
        {
            // Configure any default HTTP client settings here if needed
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler());

        // Register StudioHttpClient as a singleton factory
        services.AddSingleton<StudioHttpClient>(serviceProvider =>
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(StudioHttpClient));
            return new StudioHttpClient(httpClient, clientVersion: "demo-1.0.0");
        });

        // Operations
        services.AddSingleton<WhoAmIOperation>();

        // Config services
        services.AddSingleton<ConfigWhoAmI>();

        // Forms
        services.AddSingleton<MainForm>();
        services.AddTransient<ConfigWhoAmIForm>();

        return services;
    }
}
