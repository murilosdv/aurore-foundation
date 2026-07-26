# Changelog

All notable changes to this project will be documented in this file.

Read [the instructions file](./docs/instructions/changelog.md) to learn what and how to write here.

## [Unreleased]

---

## [0.1.0] - 2026-07-26

First stable release across all four packages.

### Added

- **Core**: result/error primitives (`Result`, `ApplicationError`, `ErrorCategory`), offset and cursor pagination helpers, FluentValidation integration with configurable error response shapes, correlation context, a lightweight `IFeature` use-case pattern with assembly-wide registration, ID obfuscation for exposing safe public identifiers, and a shared options model (app info, CORS, database connections, health checks, idempotency, maintenance mode, request limits, security headers) used across the other packages.
- **AspNetCore**: minimal-API scaffolding (resource groups, versioned endpoint mapping, assembly-wide route discovery), a standard middleware set (correlation, maintenance mode, request body limits, security headers), unified unexpected-exception handling, request idempotency and validation filters, health check reporting, and OpenAPI/Scalar setup with security scheme support.
- **EntityFrameworkCore**: a base entity configuration class, model/property builder extensions (including snake_case naming), an automatic created/updated timestamp interceptor, and a PostgreSQL health check.
- **OpenTelemetry**: one-call tracing/metrics/logging setup, with configurable incoming/outgoing traffic filters that exclude noise like health check pings by default.

---