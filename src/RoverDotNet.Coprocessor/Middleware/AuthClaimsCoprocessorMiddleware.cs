using System.Text.Json;
using RoverDotNet.Core.Coprocessor;

namespace RoverDotNet.Coprocessor.Middleware;

/// <summary>
/// Sample middleware demonstrating how to add authorization claims to the request context so
/// that router-side <c>@requiresScopes</c>/<c>@requiresPolicy</c> authorization directives can
/// consume them.
/// </summary>
/// <remarks>
/// See: https://www.apollographql.com/docs/graphos/routing/customization/coprocessor#adding-authorization-claims-via-coprocessor
/// </remarks>
public sealed class AuthClaimsCoprocessorMiddleware : ICoprocessorMiddleware
{
    private const string JwtClaimsContextKey = "apollo::authentication::jwt_claims";

    /// <inheritdoc/>
    public Task InvokeAsync(CoprocessorMiddlewareContext context, CoprocessorMiddlewareDelegate next, CancellationToken cancellationToken)
    {
        if (context.Stage == CoprocessorStage.RouterRequest)
        {
            var payload = context.Payload;
            payload.Context ??= new CoprocessorContext();

            // In a real implementation, claims would be derived from an incoming
            // Authorization header rather than hardcoded here.
            var claims = new { scope = "profile:read profile:write" };
            payload.Context.Entries[JwtClaimsContextKey] = JsonSerializer.SerializeToElement(claims);
        }

        return next(context, cancellationToken);
    }
}
