using System.Threading.Tasks;
using Aurore.Foundation.Core.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Aurore.Foundation.AspNetCore.Middlewares;

internal sealed class MaintenanceModeMiddleware(RequestDelegate next, IOptionsMonitor<MaintenanceModeOptions> optionsMonitor)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var options = optionsMonitor.CurrentValue;

        if (options.Enabled is false || context.Request.Path.StartsWithSegments("/health"))
        {
            await next(context);
            return;
        }

        context.Response.Headers.RetryAfter = options.RetryAfterSeconds.ToString();

        var problem = TypedResults.Problem(
            statusCode: StatusCodes.Status503ServiceUnavailable,
            title: "Service Unavailable",
            detail: options.Message ?? "The service is temporarily unavailable for maintenance.");

        await problem.ExecuteAsync(context);
    }
}
