using Microsoft.Extensions.DependencyInjection;
using RoverDotNet.Core.Coprocessor;

namespace RoverDotNet.Coprocessor;

/// <summary>
/// Dependency injection registration helpers for the coprocessor.
/// </summary>
public static class CoprocessorServiceCollectionExtensions
{
    /// <summary>
    /// Registers the core coprocessor services (currently, the in-memory
    /// <see cref="ICoprocessorActivityLog"/>). Call <see cref="AddCoprocessorMiddleware{TMiddleware}"/>
    /// to register pipeline middleware.
    /// </summary>
    public static IServiceCollection AddCoprocessor(this IServiceCollection services)
    {
        services.AddSingleton<ICoprocessorActivityLog, InMemoryCoprocessorActivityLog>();
        return services;
    }

    /// <summary>
    /// Registers an <see cref="ICoprocessorMiddleware"/> in the pipeline. Middleware runs in
    /// registration order.
    /// </summary>
    /// <typeparam name="TMiddleware">The middleware implementation to register.</typeparam>
    public static IServiceCollection AddCoprocessorMiddleware<TMiddleware>(this IServiceCollection services)
        where TMiddleware : class, ICoprocessorMiddleware
    {
        services.AddSingleton<ICoprocessorMiddleware, TMiddleware>();
        return services;
    }
}
