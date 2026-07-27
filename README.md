# Aurore Foundation

Packages, Cli and other common things used by Aurore Apps.

## Installation

Packages are published to **GitHub Packages**, not nuget.org, so restoring them needs an extra one-time setup step: add the feed as a NuGet source, authenticated with a GitHub personal access token that has the `read:packages` scope (GitHub Packages requires auth to restore even from a public repo).

```sh
dotnet nuget add source https://nuget.pkg.github.com/murilosdv/index.json \
  --name aurore \
  --username <your-github-username> \
  --password <your-read:packages-token>
```

On macOS/Linux, add `--store-password-in-clear-text` — there's no OS-level credential store to encrypt into, so `dotnet nuget` refuses without it. On Windows the credential is encrypted via DPAPI automatically and the flag isn't needed.

Once the source is configured, add any of the packages below to a project the usual way:

```sh
dotnet add package Aurore.Foundation.Core
dotnet add package Aurore.Foundation.AspNetCore
dotnet add package Aurore.Foundation.EntityFrameworkCore
dotnet add package Aurore.Foundation.OpenTelemetry
```

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

## Development

`dotnet build Foundation.slnx` builds everything. `make test` runs the whole test suite with coverage in Docker and builds one merged HTML report — that's the only command you need day to day.

**Docker is the default and preferred way to run tests here.** Some tests (the Postgres health check) spin up a real dependency via [Testcontainers](https://dotnet.testcontainers.org/), which needs a working Docker connection from the .NET test process itself — and that connection is not reliably configured the same way on every machine (a bare `dotnet test` on native Windows, in particular, can't always reach Docker Desktop's engine even when `docker` works fine from a shell). Running the whole suite *inside* a container sidesteps that: the `tests` container always talks to Docker the same way (a mounted `docker.sock`, Docker-outside-of-Docker — the containerized test run launches sibling containers on the host's own daemon, not nested ones), so results are reproducible regardless of host OS or local Docker configuration.

- `make test` — run every test project with coverage, in Docker (via the `run-tests.sh` script baked into the [Dockerfile](./Dockerfile)'s `test` stage), and build a merged HTML report under `test-results/report`.
- `make report` — open `test-results/report/index.html` in the default browser.
- `make test-local` — the same, but run directly on the host, no Docker, no coverage. Only reach for this if you're confident your local setup can actually run the Postgres/Testcontainers-based tests — otherwise a failure here doesn't necessarily mean anything is broken.
- `make clean` — remove everything under `test-results/`.

## Documentation

- [Changelog](./CHANGELOG.md)
- [Contributing](./CONTRIBUTING.md)
- [Third-party notices](./NOTICE.md)
- [License](./LICENSE) ([MIT](https://mit-license.org))
