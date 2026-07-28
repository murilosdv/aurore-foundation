using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Aurore.Foundation.AspNetCore.Extensions;
using Aurore.Foundation.Core.Constants;
using Aurore.Foundation.Core.Contexts;
using Aurore.Foundation.Core.Extensions;
using Aurore.Foundation.OpenTelemetry;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aurore.Foundation.AspNetCore.Middlewares;

internal sealed class RequestContextMiddleware(RequestDelegate next)
{
    private const int MaxCorrelationIdLength = 64;

    public Task InvokeAsync(HttpContext context)
    {
        var requestContext = context.RequestServices.GetRequiredService<RequestContext>();

        var correlationId = SanitizeCorrelationId(context.GetRequestHeader(StandardHeaders.CorrelationId));
        var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        var spanId = Activity.Current?.SpanId.ToString();
        var traceParent = Activity.Current?.Id;
        var idempotencyKey = context.GetRequestHeader(StandardHeaders.IdempotencyKey);

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[StandardHeaders.CorrelationId] = correlationId;

            if (traceParent.HasValue())
                context.Response.Headers[StandardHeaders.TraceParent] = traceParent;

            return Task.CompletedTask;
        });

        requestContext.Update(correlationId, traceId, spanId);
        requestContext.UpdateIdempotencyKey(idempotencyKey.HasValue() ? idempotencyKey : null);

        var logger = context.RequestServices.GetRequiredService<ILogger<RequestContextMiddleware>>();

        using var scope = logger.BeginScope(new Dictionary<string, object?>
        {
            [OpenTelemetryProperties.Attributes.Custom.CorrelationId] = correlationId,
        });

        return next(context);
    }

    private static string SanitizeCorrelationId(string value)
    {
        if (value.Length is 0 or > MaxCorrelationIdLength)
            return Guid.NewGuid().ToString("D");

        foreach (var c in value)
        {
            if (char.IsAsciiLetterOrDigit(c) is false && c is not ('-' or '_' or '.'))
                return Guid.NewGuid().ToString("D");
        }

        return value;
    }
}
