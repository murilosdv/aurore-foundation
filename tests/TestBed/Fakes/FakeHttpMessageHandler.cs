using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Aurore.Foundation.TestBed.Fakes;

/// <summary>
/// A fake <see cref="HttpMessageHandler"/> that returns a fixed response or throws a fixed exception, for testing
/// code that depends on <see cref="IHttpClientFactory"/> without making a real network call.
/// </summary>
public sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;

    private FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
    {
        _respond = respond;
    }

    /// <summary>
    /// Creates a handler that always returns the given response.
    /// </summary>
    /// <param name="response">The response to return for every request.</param>
    /// <returns>The configured fake handler.</returns>
    public static FakeHttpMessageHandler ThatReturns(HttpResponseMessage response)
    {
        return new(_ => response);
    }

    /// <summary>
    /// Creates a handler that always throws the given exception instead of sending a request.
    /// </summary>
    /// <param name="exception">The exception to throw for every request.</param>
    /// <returns>The configured fake handler.</returns>
    public static FakeHttpMessageHandler ThatThrows(Exception exception)
    {
        return new(_ => throw exception);
    }

    /// <inheritdoc/>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_respond(request));
    }
}
