# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

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
