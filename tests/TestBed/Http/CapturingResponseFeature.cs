using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Aurore.Foundation.TestBed.Http;

/// <summary>
/// A minimal <see cref="IHttpResponseFeature"/> that captures the callback registered via <see cref="OnStarting"/>
/// so tests can invoke it directly, since <see cref="DefaultHttpContext"/> has no built-in server to fire it.
/// </summary>
public sealed class CapturingResponseFeature : IHttpResponseFeature
{
    private Func<object, Task>? _callback;
    private object? _state;

    /// <inheritdoc/>
    public int StatusCode { get; set; } = StatusCodes.Status200OK;

    /// <inheritdoc/>
    public string? ReasonPhrase { get; set; }

    /// <inheritdoc/>
    public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();

    /// <inheritdoc/>
    public Stream Body { get; set; } = Stream.Null;

    /// <inheritdoc/>
    public bool HasStarted { get; private set; }

    /// <inheritdoc/>
    public void OnStarting(Func<object, Task> callback, object state)
    {
        _callback = callback;
        _state = state;
    }

    /// <inheritdoc/>
    public void OnCompleted(Func<object, Task> callback, object state)
    {
    }

    /// <summary>
    /// Invokes the callback registered via <see cref="OnStarting"/>, if any, simulating the response starting.
    /// </summary>
    /// <returns>A task that completes when the callback finishes.</returns>
    public Task FireOnStartingAsync()
    {
        HasStarted = true;

        return _callback is null ? Task.CompletedTask : _callback(_state!);
    }
}
