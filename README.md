# Aurore Foundation

Packages, Cli and other common things used by Aurore Apps.

## Packages

### Aurore.Foundation.Core

Dependency-light core and common utilities shared by the other packages. Result/error primitives (`Result`, `ApplicationError`, `ErrorCategory`) for explicit success/failure without exceptions; offset and cursor pagination helpers; FluentValidation integration with configurable error response shapes; a correlation context for tracing a request end-to-end; a lightweight use-case pattern (`IFeature`) for encapsulating business logic outside controllers/endpoints, with assembly-wide registration; ID obfuscation for exposing safe public identifiers instead of raw database keys; and the shared options model (app info, CORS, database connections, health checks, idempotency, maintenance mode, request limits, security headers) that `AspNetCore`, `EntityFrameworkCore`, and `OpenTelemetry` all build on.

#### Getting Started Guides

- [The `IFeature` pattern](./docs/guides/Features.md)

### Aurore.Foundation.AspNetCore

ASP.NET Core extensions and utilities. Minimal-API scaffolding (`ResourceGroup`, versioned endpoint mapping via `MinimalEndpoint<T>`, and assembly-wide route discovery via `MapResourcesFromAssembly`); a standard middleware set (correlation, maintenance mode, request body size limits, security headers); unified unexpected-exception handling; request idempotency and endpoint validation filters; health check reporting (readiness and dependency reports); and OpenAPI/Scalar setup with security scheme support.

#### Getting Started Guides

- [Wiring up Aurore.Foundation.AspNetCore](./docs/guides/AspNetCore.md)

### Aurore.Foundation.EntityFrameworkCore

Entity Framework Core extensions. A base entity configuration class to cut down on boilerplate; model/property builder extensions, including snake_case naming convention support; an interceptor that automatically maintains created/updated timestamps; and a PostgreSQL health check.

### Aurore.Foundation.OpenTelemetry

OpenTelemetry extensions. One-call tracing/metrics/logging setup, with configurable incoming/outgoing traffic filters that exclude noise like health check pings by default.

## Documentation

- [Changelog](./CHANGELOG.md)
- [Contributing](./CONTRIBUTING.md)
- [Third-party notices](./NOTICE.md)
- [License](./LICENSE) ([MIT](https://mit-license.org))
