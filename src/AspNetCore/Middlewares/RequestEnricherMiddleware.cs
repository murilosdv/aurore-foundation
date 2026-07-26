using System.Collections.Generic;
using System.Threading.Tasks;
using Aurore.Foundation.AspNetCore.Extensions;
using Aurore.Foundation.Core.Constants;
using Aurore.Foundation.OpenTelemetry;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Aurore.Foundation.AspNetCore.Middlewares;

internal sealed class RequestEnricherMiddleware(ILogger<RequestEnricherMiddleware> logger, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        using var scope = logger.BeginScope(new Dictionary<string, object?>
        {
            [OpenTelemetryProperties.Attributes.Standard.HttpHost] = context.Request.Host.Value,
            [OpenTelemetryProperties.Attributes.Standard.UserAgentOriginal] = context.GetRequestHeader(StandardHeaders.UserAgent),
            [OpenTelemetryProperties.Attributes.Standard.NetPeerIp] = context.Connection.RemoteIpAddress?.ToString() ?? "Unidentified",
        });

        await next(context);
    }
}
