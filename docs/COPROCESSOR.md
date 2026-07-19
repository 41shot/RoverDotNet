# RoverDotNet.Coprocessor

`RoverDotNet.Coprocessor` is a .NET implementation of an [Apollo Router external coprocessor](https://www.apollographql.com/docs/graphos/routing/customization/coprocessor). It lets you customise Apollo Router request/response handling — auth, header/context manipulation, logging, validation, etc. — by writing middleware in C#.

## Why use it?

The Apollo Router coprocessor protocol is a simple JSON-over-HTTP contract: the router calls out to your service at specific stages of a request's lifecycle, sends it a JSON payload describing the request/response (headers, body, GraphQL context, etc.), and applies whatever changes your service returns. Typical use cases include:

- Injecting or validating authentication/authorization claims (e.g. populating `apollo::authentication::jwt_claims` for `@requiresScopes`/`@requiresPolicy`).
- Rewriting or enriching headers before they reach a subgraph.
- Centralised request logging/auditing across the supergraph.
- Rejecting requests early (short-circuiting the pipeline) based on custom business rules.
- Modifying subgraph responses before they're merged back into the client response.

RoverDotNet.Coprocessor handles the protocol plumbing (deserializing the payload, dispatching by stage, serializing the response) so you only need to write the business logic.

## Core concepts

- **`CoprocessorStage`** — the router lifecycle stage a payload relates to: `RouterRequest`, `RouterResponse`, `SupergraphRequest`, `SupergraphResponse`, `SubgraphRequest`, `SubgraphResponse`.
- **`CoprocessorPayload`** — the mutable JSON payload for the current stage (headers, body, context, control, etc.), matching the Apollo Router coprocessor JSON contract.
- **`ICoprocessorMiddleware`** — implement this to inspect/modify the `CoprocessorMiddlewareContext` (stage + payload). Middleware follows a chain-of-responsibility pattern similar to ASP.NET Core's `IMiddleware`: call `next` to continue the pipeline, or omit the call (typically after setting `CoprocessorPayload.Control` to `CoprocessorControl.Break`) to short-circuit it.
- **`CoprocessorAppBuilder`** — builds a `WebApplication` exposing the coprocessor endpoint, for use standalone or embedded in another host.
- **`CoprocessorServiceCollectionExtensions`** — `AddCoprocessor()` registers core services; `AddCoprocessorMiddleware<TMiddleware>()` registers pipeline middleware (middleware runs in registration order).
- **`ICoprocessorActivityLog`** — an in-memory log of processed requests/responses, useful for diagnostics and the demo application's activity view.

## Quick start

### 1. Write a middleware

```csharp
public sealed class MyAuthMiddleware : ICoprocessorMiddleware
{
	public Task InvokeAsync(CoprocessorMiddlewareContext context, CoprocessorMiddlewareDelegate next, CancellationToken cancellationToken)
	{
		if (context.Stage == CoprocessorStage.RouterRequest)
		{
			// Inspect/modify context.Payload here, e.g. inject JWT claims into the context.
		}

		return next(context, cancellationToken);
	}
}
```

### 2. Host the coprocessor

**Standalone** (using the `RoverDotNet.Coprocessor` executable or your own minimal host):

```csharp
var app = CoprocessorAppBuilder.Build(args, configureServices: services =>
{
	services.AddCoprocessorMiddleware<MyAuthMiddleware>();
});

app.Run();
```

By default the host listens on `http://127.0.0.1:8081` (`CoprocessorAppBuilder.DefaultUrl`).

**Embedded** in an existing ASP.NET Core app:

```csharp
builder.Services.AddCoprocessor();
builder.Services.AddCoprocessorMiddleware<MyAuthMiddleware>();

// ...

app.MapCoprocessorEndpoint(); // defaults to "/"
```

### 3. Point the router at it

Configure Apollo Router to call your coprocessor, selecting which stages/fields it should receive:

```yaml
coprocessor:
  url: http://127.0.0.1:8081
  timeout: 2s
  router:
	request:
	  headers: true
	  body: false
	  context: all
	response:
	  headers: true
	  body: false
	  context: all
  supergraph:
	request:
	  headers: true
	  body: true
	  context: all
	response:
	  headers: true
	  body: true
	  context: all
```

See `src/RoverDotNet.Demo/Router-with-coprocessor.yaml` for a complete example, and `src/RoverDotNet.Demo/Middleware/AuthClaimsCoprocessorMiddleware.cs` for a sample middleware that injects authorization claims.

## Further reading

- Apollo docs: [The Coprocessor customization](https://www.apollographql.com/docs/graphos/routing/customization/coprocessor)
