namespace RoverDotNet.Core.Coprocessor;

/// <summary>
/// The mutable context passed through a coprocessor middleware pipeline for a single
/// coprocessor HTTP request.
/// </summary>
/// <param name="Stage">The request-handling lifecycle stage this payload relates to.</param>
/// <param name="Payload">
/// The coprocessor payload. Middleware can read and modify this in place; the final
/// state is returned to the router as the coprocessor's HTTP response body.
/// </param>
public sealed record CoprocessorMiddlewareContext(CoprocessorStage Stage, CoprocessorPayload Payload);

/// <summary>
/// Invokes the next middleware (or the terminal handler) in a coprocessor pipeline.
/// </summary>
public delegate Task CoprocessorMiddlewareDelegate(CoprocessorMiddlewareContext context, CancellationToken cancellationToken);

/// <summary>
/// A single stage in the coprocessor request pipeline, following the same
/// chain-of-responsibility shape as ASP.NET Core's <c>IMiddleware</c>.
/// </summary>
/// <remarks>
/// Implementations can inspect and modify <see cref="CoprocessorMiddlewareContext.Payload"/>
/// (e.g. to inject headers or authentication claims), short-circuit the pipeline by not
/// calling <c>next</c> (typically after setting
/// <see cref="CoprocessorPayload.Control"/> to <see cref="CoprocessorControl.Break"/>), or
/// call <c>next</c> to continue processing.
/// </remarks>
public interface ICoprocessorMiddleware
{
    /// <summary>
    /// Processes the given coprocessor payload, optionally invoking the next middleware.
    /// </summary>
    /// <param name="context">The mutable coprocessor request context.</param>
    /// <param name="next">The next middleware (or terminal handler) in the pipeline.</param>
    /// <param name="cancellationToken">A token used to cancel processing.</param>
    Task InvokeAsync(CoprocessorMiddlewareContext context, CoprocessorMiddlewareDelegate next, CancellationToken cancellationToken);
}
