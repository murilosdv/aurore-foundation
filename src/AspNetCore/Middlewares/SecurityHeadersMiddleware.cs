using System.Threading.Tasks;
using Aurore.Foundation.Core.Extensions;
using Aurore.Foundation.Core.Options;
using Microsoft.AspNetCore.Http;

namespace Aurore.Foundation.AspNetCore.Middlewares;

internal sealed class SecurityHeadersMiddleware(RequestDelegate next, SecurityHeadersOptions options)
{
    public Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Frame-Options"] = "DENY";
            context.Response.Headers["Referrer-Policy"] = "no-referrer";

            if (options.ContentSecurityPolicy.HasValue())
                context.Response.Headers["Content-Security-Policy"] = options.ContentSecurityPolicy;

            return Task.CompletedTask;
        });

        return next(context);
    }
}
