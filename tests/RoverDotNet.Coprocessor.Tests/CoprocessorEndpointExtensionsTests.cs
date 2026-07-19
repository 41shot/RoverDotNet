using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RoverDotNet.Core.Coprocessor;
using Xunit;

namespace RoverDotNet.Coprocessor.Tests;

public class CoprocessorEndpointExtensionsTests
{
    [Fact]
    public async Task HandleAsync_UnknownStage_EchoesPayloadUnchanged()
    {
        var payload = new CoprocessorPayload { Stage = "Execution" };
        var request = CreateRequest(payload);

        var result = await CoprocessorEndpointExtensions.HandleAsync(
            request, middlewares: [], activityLog: new InMemoryCoprocessorActivityLog(), CancellationToken.None);

        var responsePayload = await GetResponsePayloadAsync(result);
        Assert.Equal("Execution", responsePayload.Stage);
    }

    [Fact]
    public async Task HandleAsync_RunsMiddlewareInRegistrationOrder()
    {
        var order = new List<string>();
        var first = new RecordingMiddleware("first", order);
        var second = new RecordingMiddleware("second", order);

        var payload = new CoprocessorPayload { Stage = nameof(CoprocessorStage.RouterRequest) };
        var request = CreateRequest(payload);

        await CoprocessorEndpointExtensions.HandleAsync(
            request, middlewares: [first, second], activityLog: new InMemoryCoprocessorActivityLog(), CancellationToken.None);

        Assert.Equal(["first", "second"], order);
    }

    [Fact]
    public async Task HandleAsync_MiddlewareShortCircuits_SkipsSubsequentMiddleware()
    {
        var order = new List<string>();
        var breaking = new BreakingMiddleware(statusCode: 401);
        var second = new RecordingMiddleware("second", order);

        var payload = new CoprocessorPayload { Stage = nameof(CoprocessorStage.RouterRequest) };
        var request = CreateRequest(payload);

        var result = await CoprocessorEndpointExtensions.HandleAsync(
            request, middlewares: [breaking, second], activityLog: new InMemoryCoprocessorActivityLog(), CancellationToken.None);

        Assert.Empty(order);

        var responsePayload = await GetResponsePayloadAsync(result);
        Assert.True(responsePayload.Control.IsBreak);
        Assert.Equal(401, responsePayload.Control.BreakStatusCode);
    }

    [Fact]
    public async Task HandleAsync_DoesNotMutateOriginalRequestPayload()
    {
        var addsClaims = new ClaimsAddingMiddleware();

        var payload = new CoprocessorPayload { Stage = nameof(CoprocessorStage.RouterRequest) };
        var request = CreateRequest(payload);

        var activityLog = new InMemoryCoprocessorActivityLog();
        CoprocessorActivityEntry? recorded = null;
        activityLog.EntryRecorded += (_, entry) => recorded = entry;

        await CoprocessorEndpointExtensions.HandleAsync(
            request, middlewares: [addsClaims], activityLog, CancellationToken.None);

        Assert.NotNull(recorded);
        Assert.Null(recorded!.Request.Context);
        Assert.NotNull(recorded.Response.Context);
        Assert.True(recorded.Response.Context!.Entries.ContainsKey("apollo::authentication::jwt_claims"));
    }

    [Fact]
    public async Task HandleAsync_RecordsActivityLogEntry()
    {
        var payload = new CoprocessorPayload { Stage = nameof(CoprocessorStage.SubgraphRequest), ServiceName = "products" };
        var request = CreateRequest(payload);

        var activityLog = new InMemoryCoprocessorActivityLog();

        await CoprocessorEndpointExtensions.HandleAsync(
            request, middlewares: [], activityLog, CancellationToken.None);

        var entry = Assert.Single(activityLog.GetLatestEntries());
        Assert.Equal(CoprocessorStage.SubgraphRequest, entry.Stage);
        Assert.Equal("products", entry.ServiceName);
    }

    private static HttpRequest CreateRequest(CoprocessorPayload payload)
    {
        var context = new DefaultHttpContext();
        var json = JsonSerializer.SerializeToUtf8Bytes(payload, CoprocessorEndpointExtensions.JsonOptions);
        context.Request.Body = new MemoryStream(json);
        return context.Request;
    }

    private static async Task<CoprocessorPayload> GetResponsePayloadAsync(IResult result)
    {
        using var stream = new MemoryStream();
        var services = new ServiceCollection()
            .AddSingleton(Options.Create(new JsonOptions()))
            .AddLogging()
            .BuildServiceProvider();
        var context = new DefaultHttpContext
        {
            RequestServices = services,
        };
        context.Response.Body = stream;

        await result.ExecuteAsync(context);

        stream.Position = 0;
        return JsonSerializer.Deserialize<CoprocessorPayload>(stream, CoprocessorEndpointExtensions.JsonOptions)!;
    }

    private sealed class RecordingMiddleware(string name, List<string> order) : ICoprocessorMiddleware
    {
        public Task InvokeAsync(CoprocessorMiddlewareContext context, CoprocessorMiddlewareDelegate next, CancellationToken cancellationToken)
        {
            order.Add(name);
            return next(context, cancellationToken);
        }
    }

    private sealed class BreakingMiddleware(int statusCode) : ICoprocessorMiddleware
    {
        public Task InvokeAsync(CoprocessorMiddlewareContext context, CoprocessorMiddlewareDelegate next, CancellationToken cancellationToken)
        {
            context.Payload.Control = CoprocessorControl.Break(statusCode);
            // Intentionally does not call `next`, short-circuiting the pipeline.
            return Task.CompletedTask;
        }
    }

    private sealed class ClaimsAddingMiddleware : ICoprocessorMiddleware
    {
        public Task InvokeAsync(CoprocessorMiddlewareContext context, CoprocessorMiddlewareDelegate next, CancellationToken cancellationToken)
        {
            if (context.Stage == CoprocessorStage.RouterRequest)
            {
                var payload = context.Payload;
                payload.Context ??= new CoprocessorContext();
                payload.Context.Entries["apollo::authentication::jwt_claims"] = JsonSerializer.SerializeToElement(new { scope = "profile:read profile:write" });
            }

            return next(context, cancellationToken);
        }
    }
}
