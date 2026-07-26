using System;
using System.Threading;
using System.Threading.Tasks;
using Aurore.Foundation.AspNetCore.Extensions;
using Aurore.Foundation.Core.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Aurore.Foundation.AspNetCore.ErrorHandling;

internal sealed class UnexpectedExceptionHandler(ILogger<UnexpectedExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An unexpected error occurred.");

        var result = TypedResults.Problem(StandardErrors.Unknown, null);

        await result.ExecuteAsync(context);

        return true;
    }
}
