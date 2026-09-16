# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

**Packages**: `Core 0.3.1` · `AspNetCore 0.2.2` · `EntityFrameworkCore 0.2.3`

### Changed

- Enabled `EnforceCodeStyleInBuild` so the code-style rules in `.editorconfig` (naming, braces, expression-vs-block bodies, etc.) are now enforced as real build warnings/errors instead of editor-only suggestions — brings a handful of existing files into compliance with rules that were already configured but never actually checked at build time. No public API or behavior changes.

---

## [2026-09-16]

**Packages**: `Core 0.3.0`

### Added

- `GuidExtensions.Shorten()`/`Unshorten()`/`TryUnshorten()` — a reversible, URL-safe, 22-character encoding for `Guid` values (base64url over the raw 16 bytes, no padding).

### Changed

- **Breaking**: `IEntity` (the default, unparameterized form of `IEntity<T>`) now resolves to `IEntity<Guid>` instead of `IEntity<long>`.
- **Breaking**: `Obfuscator` no longer depends on Sqids — the scrambling is now a small in-house, invertible multiply-add (see the `Obfuscator` class docs for details). Along with that: `Configure` is now `Configure(string? alphabet = null)`, returning an `ObfuscatorRegistrationOptions` directly for `.Register<T>()` chaining, instead of taking a configuration callback; `MinimumLength` and `BlockList` are gone, since encoded identifiers are now a fixed width per numeric type (6 characters for `int`, 11 for `long`, with the default alphabet) rather than variable-length with a minimum floor — there's no length left to configure. One behavior change worth knowing: decoding with the wrong type salt (or the salt-less form on a salted string) is no longer detected — it now returns a different, wrong number instead of failing, since the new encoding has no redundant/checksum information to tell a correct decode from an incorrect one.

### Removed

- **Breaking**: the `Sqids` package dependency, and the unused `ObfuscationOptions` record.

---

## [2026-09-15]

**Packages**: `CliCore 0.1.0` (new) · `Core 0.2.1` · `AspNetCore 0.2.1` · `EntityFrameworkCore 0.2.2` · `OpenTelemetry 0.1.2`

### Added

- **CliCore**: a new package providing shared building blocks for Spectre.Console.Cli-based CLI projects: attribute-driven command/branch registration (`ConfigureCommand`, `ConfigureBranch`, `CommandInfoAttribute`, `CommandExampleAttribute`, `CommandAliasAttribute`), a dependency-injection bridge (`CommandApp.CreateWithServices`, `DependencyRegistrar`), a default exception handler (`UseDefaultExceptionHandler`) that renders failures consistently and returns a fixed exit code, severity-colored/labeled console output helpers (`AnsiConsoleExtensions`), a typed `dotnet` CLI process wrapper (`Dotnet.Tool`/`Migrations`/`Database`/`PackAsync`), and small JSON/process-result helpers.

### Changed

- Bumped NuGet dependencies across `Core`, `AspNetCore`, `EntityFrameworkCore` and `OpenTelemetry` to their latest compatible versions. No public API changes.
- **AspNetCore**: `SecurityHeadersMiddleware` now sets `X-Content-Type-Options`, `X-Frame-Options` and `Content-Security-Policy` via ASP.NET Core's typed header properties instead of raw string indexers. Same header values, no behavior change.

---

## [2026-07-29]

**Packages**: `EntityFrameworkCore 0.2.1`

### Fixed

- **EntityFrameworkCore**: `UseSnakeCaseNamingConvention()` now rewrites columns nested inside `ComplexProperty` mappings to snake_case too, including complex-in-complex nesting. Previously it only rewrote the entity's own scalar properties, so table-split complex properties (e.g. `Name.First`/`Name.Last`) kept their default PascalCase, prefixed column names (`"Name_First"`, `"Name_Last"`) untouched.

---

## [2026-07-28]

**Packages**: `Core 0.2.0` · `AspNetCore 0.2.0` · `EntityFrameworkCore 0.2.0`

### Added

- `RequestContext.IdempotencyKey` exposes the client-supplied `Idempotency-Key` header directly, so application code can read it without depending on the idempotency filter.
- **EntityFrameworkCore**: `UseSnakeCaseNpgsqlHistoryTable()` renames the EF migrations history table's columns and primary key to snake_case, matching `UseSnakeCaseNamingConvention()`.
- New `Messaging` package scaffolded for upcoming RabbitMQ support (not yet functional).

### Changed

- **Breaking**: `CorrelationContext` (and its `AddCorrelationContext`/`UseCorrelationContext` registration methods) is renamed to `RequestContext`/`AddRequestContext`/`UseRequestContext`, since it now also carries the idempotency key alongside the correlation and trace identifiers.

### Fixed

- The idempotency filter no longer caches error responses — only successful (2xx) results are replayed, so a retry after a transient failure reaches the handler again instead of getting the same stale failure replayed back.

---

## [2026-07-27]

**Packages**: `Core 0.1.1` · `AspNetCore 0.1.1` · `EntityFrameworkCore 0.1.1` · `OpenTelemetry 0.1.1`

Fixed a couple of minor things that were broken in 0.1.0, nothing major.

---

## [2026-07-26]

**Packages**: `Core 0.1.0` · `AspNetCore 0.1.0` · `EntityFrameworkCore 0.1.0` · `OpenTelemetry 0.1.0`

First stable release across all four packages.

### Added

- **Core**: result/error primitives (`Result`, `ApplicationError`, `ErrorCategory`), offset and cursor pagination helpers, FluentValidation integration with configurable error response shapes, correlation context, a lightweight `IFeature` use-case pattern with assembly-wide registration, ID obfuscation for exposing safe public identifiers, and a shared options model (app info, CORS, database connections, health checks, idempotency, maintenance mode, request limits, security headers) used across the other packages.
- **AspNetCore**: minimal-API scaffolding (resource groups, versioned endpoint mapping, assembly-wide route discovery), a standard middleware set (correlation, maintenance mode, request body limits, security headers), unified unexpected-exception handling, request idempotency and validation filters, health check reporting, and OpenAPI/Scalar setup with security scheme support.
- **EntityFrameworkCore**: a base entity configuration class, model/property builder extensions (including snake_case naming), an automatic created/updated timestamp interceptor, and a PostgreSQL health check.
- **OpenTelemetry**: one-call tracing/metrics/logging setup, with configurable incoming/outgoing traffic filters that exclude noise like health check pings by default.

---
