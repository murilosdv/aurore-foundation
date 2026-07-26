using System.Threading.Tasks;
using Aurore.Foundation.Core.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Aurore.Foundation.AspNetCore.Middlewares;

internal sealed class RequestBodySizeLimitMiddleware(RequestDelegate next, RequestBodySizeOptions options)
{
    public Task InvokeAsync(HttpContext context)
    {
        var feature = context.Features.Get<IHttpMaxRequestBodySizeFeature>();

        if (feature is { IsReadOnly: false })
            feature.MaxRequestBodySize = options.MaxBytes;

        return next(context);
    }
}
