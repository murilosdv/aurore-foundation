[back](../../README.md)

# Getting started with Aurore.Foundation.AspNetCore

This is a usage guide for wiring the pieces together — for the full API reference, every public
member has XML docs (surfaced via IntelliSense, or the generated package documentation).

## Wiring `Program.cs`

A typical setup registers versioning, authentication, health checks, error handling, and the
request pipeline middleware, then maps minimal API resources:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDefaultApiVersioning()
    .AddJwtBearerAuthentication(identityProviderOptions)
    .AddDefaultJwtBearerAuthorization()
    .AddCorrelationContext()
    .AddUnexpectedErrorHandler()
    .AddIdempotency(idempotencyOptions)
    .AddHealthCheckRateLimiter(permitLimit: 30, limitSeconds: 60)
    .AddHealthCheckOutputCache(expirationSeconds: 5);

builder.Services.ConfigureScalarDocuments(1);
builder.Services.AddOpenApi("v1", options => options
    .AddAppInfo("v1", appInfoOptions)
    .AddJwtBearerSecurity()
    .AddCorrelationIdHeader());

var app = builder.Build();

app.UseCorrelationContext()
    .UseRequestEnricher()
    .UseRequestBodySizeLimit()
    .UseSecurityHeaders(securityHeadersOptions, app.Environment.IsProduction())
    .UseMaintenanceMode()
    .UseUnexpectedErrorHandler();

app.MapHealthCheckEndpoints(healthCheckEndpointOptions);
app.MapResourcesFromAssembly<Program>();

app.MapScalarApiReference();

app.Run();
```

Order matters for the middleware chain above: correlation must run first (everything downstream
wants the correlation id in scope), maintenance mode should short-circuit before real work happens,
and the exception handler needs to be registered last so it wraps everything before it.

## Minimal APIs: resource groups and endpoints

Routes are organized into **resource groups** (a `ResourceGroup` subclass — the route prefix, tags,
supported versions, and authorization requirement for everything mapped under it) and **endpoints**
(a `MinimalEndpoint<TResourceGroup>` subclass per route). `MapResourcesFromAssembly<TAssembly>`
finds every `ResourceGroup` and `MinimalEndpoint` in the given assembly by reflection and wires them
up — no manual route registration needed.

```csharp
// The route group: mapped at v{version}/widgets, tagged "Widgets", authorization required by default.
public sealed class WidgetsResource : ResourceGroup;

// One endpoint under that group.
internal sealed class GetWidgetEndpoint : MinimalEndpoint<WidgetsResource>
{
    public override RouteHandlerBuilder Map(IEndpointRouteBuilder builder)
    {
        return builder.MapGet("/{id:guid}", async (
            Guid id,
            IFeature<Guid, WidgetResponse> feature,
            CancellationToken cancellationToken) =>
        {
            var result = await feature.HandleAsync(id, cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(result.Value)
                : TypedResults.Problem(result);
        });
    }
}
```

`ResourceGroup` and `MinimalEndpoint` members are all `virtual` with sensible defaults derived from
the class name (route segment, tags, summary) — override only what needs to differ. See
[Features.md](./Features.md) for what `IFeature<Guid, WidgetResponse>` is doing above and how
`TypedResults.Problem(result)` turns a failed `Result` into an RFC 7807 response.

To require a specific authorization policy, opt out of authorization, or add extra versions/problem
status codes for a group, override the relevant `ResourceGroup` properties:

```csharp
public sealed class ReportsResource : ResourceGroup
{
    public override bool RequireAuthorization => false;
    public override int[] Versions => [1, 2];
}
```

## Filters

`WithEndpointValidation()` and `WithIdempotency()` are `RouteHandlerBuilder` extensions applied per
endpoint, chained onto whatever `Map(...)` returns:

```csharp
public override RouteHandlerBuilder Map(IEndpointRouteBuilder builder)
{
    return builder
        .MapPost("/", CreateWidget)
        .WithEndpointValidation()
        .WithIdempotency();
}
```

`WithEndpointValidation` runs the request's registered FluentValidation validator (if any) before the
handler executes. `WithIdempotency` requires `AddIdempotency(idempotencyOptions)` to be registered
(it backs onto `HybridCache`) and replays the cached response for a repeated request carrying the
same idempotency key.

[back](../../README.md)
