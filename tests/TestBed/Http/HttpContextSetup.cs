using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Aurore.Foundation.TestBed.Http;

/// <summary>
/// A fluent setup helper for a <see cref="DefaultHttpContext"/> pre-wired with a service provider (logging
/// enabled by default), so tests don't need to repeat the same multi-line context/service-provider scaffolding.
/// </summary>
public sealed class HttpContextSetup
{
    private readonly DefaultHttpContext _context = new();
    private readonly IServiceCollection _services = new ServiceCollection().AddLogging();

    private HttpContextSetup() { }

    /// <summary>
    /// Starts a new <see cref="HttpContextSetup"/>.
    /// </summary>
    /// <returns>A new setup instance.</returns>
    public static HttpContextSetup Create()
    {
        return new();
    }

    /// <summary>
    /// Sets the response body stream.
    /// </summary>
    /// <param name="body">The stream to assign as the response body.</param>
    /// <returns>This setup instance, for chaining.</returns>
    public HttpContextSetup WithBody(Stream body)
    {
        _context.Response.Body = body;

        return this;
    }

    /// <summary>
    /// Sets the request path.
    /// </summary>
    /// <param name="path">The request path to assign.</param>
    /// <returns>This setup instance, for chaining.</returns>
    public HttpContextSetup WithPath(string path)
    {
        _context.Request.Path = path;

        return this;
    }

    /// <summary>
    /// Sets a request header.
    /// </summary>
    /// <param name="key">The header name.</param>
    /// <param name="value">The header value.</param>
    /// <returns>This setup instance, for chaining.</returns>
    public HttpContextSetup WithHeader(string key, string value)
    {
        _context.Request.Headers[key] = value;

        return this;
    }

    /// <summary>
    /// Registers a feature on the context's feature collection.
    /// </summary>
    /// <typeparam name="TFeature">The feature type to register.</typeparam>
    /// <param name="feature">The feature instance to register.</param>
    /// <returns>This setup instance, for chaining.</returns>
    public HttpContextSetup WithFeature<TFeature>(TFeature feature) where TFeature : class
    {
        _context.Features.Set(feature);

        return this;
    }

    /// <summary>
    /// Configures additional services on the context's request service provider (which already has logging enabled).
    /// </summary>
    /// <param name="configure">A callback used to register additional services.</param>
    /// <returns>This setup instance, for chaining.</returns>
    public HttpContextSetup WithServices(Action<IServiceCollection> configure)
    {
        configure(_services);

        return this;
    }

    /// <summary>
    /// Builds the <see cref="DefaultHttpContext"/>, assigning its <see cref="HttpContext.RequestServices"/> from
    /// the configured service collection.
    /// </summary>
    /// <returns>The configured <see cref="DefaultHttpContext"/>.</returns>
    public DefaultHttpContext Build()
    {
        _context.RequestServices = _services.BuildServiceProvider();

        return _context;
    }
}
