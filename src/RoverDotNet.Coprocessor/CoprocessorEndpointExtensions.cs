using System.Text.Json;
using RoverDotNet.Core.Coprocessor;

namespace RoverDotNet.Coprocessor;

/// <summary>
/// Maps the Apollo Router external coprocessor HTTP endpoint.
/// </summary>
public static class CoprocessorEndpointExtensions
{
    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// Maps a <c>POST</c> endpoint implementing the coprocessor contract at <paramref name="pattern"/>.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <param name="pattern">The route pattern to map. Defaults to <c>"/"</c>.</param>
    public static IEndpointRouteBuilder MapCoprocessorEndpoint(this IEndpointRouteBuilder endpoints, string pattern = "/")
    {
        endpoints.MapPost(pattern, HandleAsync);
        return endpoints;
    }

    internal static async Task<IResult> HandleAsync(
        HttpRequest request,
        IEnumerable<ICoprocessorMiddleware> middlewares,
        ICoprocessorActivityLog activityLog,
        CancellationToken cancellationToken)
    {
        var requestPayload = await JsonSerializer.DeserializeAsync<CoprocessorPayload>(request.Body, JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("Coprocessor request body was empty.");

        if (!Enum.TryParse<CoprocessorStage>(requestPayload.Stage, ignoreCase: true, out var stage))
        {
            // Unknown/unsupported stage (e.g. Execution or Connector): echo the payload back unchanged.
            return Results.Json(requestPayload, JsonOptions);
        }

        var responsePayload = Clone(requestPayload);
        var context = new CoprocessorMiddlewareContext(stage, responsePayload);

        var pipeline = BuildPipeline(middlewares);
        await pipeline(context, cancellationToken);

        activityLog.Record(new CoprocessorActivityEntry(
            stage,
            requestPayload.ServiceName,
            DateTimeOffset.UtcNow,
            requestPayload,
            responsePayload));

        return Results.Json(responsePayload, JsonOptions);
    }

    private static CoprocessorMiddlewareDelegate BuildPipeline(IEnumerable<ICoprocessorMiddleware> middlewares)
    {
        CoprocessorMiddlewareDelegate pipeline = static (_, _) => Task.CompletedTask;

        foreach (var middleware in middlewares.Reverse())
        {
            var next = pipeline;
            pipeline = (context, cancellationToken) => middleware.InvokeAsync(context, next, cancellationToken);
        }

        return pipeline;
    }

    private static CoprocessorPayload Clone(CoprocessorPayload payload)
    {
        var json = JsonSerializer.SerializeToUtf8Bytes(payload, JsonOptions);
        return JsonSerializer.Deserialize<CoprocessorPayload>(json, JsonOptions)!;
    }
}
