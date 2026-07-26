using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Aurore.Foundation.AspNetCore.ErrorHandling;
using Aurore.Foundation.Core.Errors;
using Microsoft.Extensions.Logging.Abstractions;
using Tests.Shared.Http;

namespace Tests.AspNetCore.ErrorHandling;

public class UnexpectedExceptionHandlerTests
{
    [Fact(DisplayName = "TryHandleAsync writes a problem-details response reflecting StandardErrors.Unknown and returns true")]
    public async Task WritesUnknownProblemDetailsAndReturnsTrue()
    {
        // Arrange
        var body = new MemoryStream();
        var context = HttpContextSetup.Create().WithBody(body).Build();
        var handler = new UnexpectedExceptionHandler(NullLogger<UnexpectedExceptionHandler>.Instance);
        var exception = new InvalidOperationException("boom");

        // Act
        var handled = await handler.TryHandleAsync(context, exception, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(handled);
        Assert.Equal(StandardErrors.Unknown.HttpStatusCode, context.Response.StatusCode);

        body.Seek(0, SeekOrigin.Begin);
        Assert.True(body.Length > 0);
        using var document = await JsonDocument.ParseAsync(body, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(StandardErrors.Unknown.Title, document.RootElement.GetProperty("title").GetString());
        Assert.Equal(StandardErrors.Unknown.HttpStatusCode, document.RootElement.GetProperty("status").GetInt32());
    }
}
