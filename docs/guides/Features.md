[back](../../README.md)

# Getting started with the `IFeature` pattern (Aurore.Foundation.Core)

A **feature** is a single use case: one class, one job, injected wherever it's needed instead of
piling logic into controllers/endpoints or fat services. This is a usage guide — the full API
reference is in the XML docs on `IFeature<TResult>`, `IFeature<TRequest, TResult>`, and
`FeatureConfiguration`.

## Defining a feature

Two shapes, depending on whether the use case needs an input:

```csharp
// No input.
public sealed class ListWidgetsFeature(WidgetDbContext db) : IFeature<IReadOnlyList<WidgetResponse>>
{
    public async ValueTask<Result<IReadOnlyList<WidgetResponse>>> HandleAsync(CancellationToken cancellationToken)
    {
        return await db.Widgets
            .Select(w => new WidgetResponse(w.Id, w.Name))
            .ToListAsync(cancellationToken);
    }
}

// With input.
public sealed class GetWidgetFeature(WidgetDbContext db) : IFeature<Guid, WidgetResponse>
{
    public async ValueTask<Result<WidgetResponse>> HandleAsync(Guid request, CancellationToken cancellationToken)
    {
        var widget = await db.Widgets.FindAsync([request], cancellationToken);

        return widget is null
            ? StandardErrors.NotFound
            : new WidgetResponse(widget.Id, widget.Name);
    }
}
```

`Result<T>` has implicit conversions from both `T` (success) and `ApplicationError` (failure, e.g.
`StandardErrors.NotFound`) — a feature returns whichever fits without constructing `Result` by hand.
`StandardErrors` (also in Core) is a catalog of common, pre-classified errors; define your own
`ApplicationError` instances for anything domain-specific.

## Registering features

`AddFeaturesFromAssembly<TAssembly>` scans the given assembly for every concrete `IFeature<TResult>`
or `IFeature<TRequest, TResult>` implementation and registers each with the container — no per-feature
`services.AddScoped<...>()` line needed:

```csharp
builder.Services.AddFeaturesFromAssembly<Program>();
```

Defaults to `ServiceLifetime.Scoped`; pass a different `ServiceLifetime` if a feature genuinely needs
to be singleton/transient.

## Using a feature

Inject the feature's interface (not its concrete type) wherever it's needed — a minimal API endpoint,
another feature, a background service:

```csharp
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

`TypedResults.Problem(result)` (from `Aurore.Foundation.AspNetCore`) converts a failed `Result` into
an RFC 7807 problem-details response automatically — using the feature's `ApplicationError` if it set
one, or a generic validation error carrying `Result.Errors` otherwise. See
[AspNetCore.md](./AspNetCore.md) for the rest of that endpoint's setup.

[back](../../README.md)
