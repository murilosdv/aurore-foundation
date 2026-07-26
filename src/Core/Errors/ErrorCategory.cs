namespace Aurore.Foundation.Core.Errors;

/// <summary>
/// Classifies an <see cref="ApplicationError"/> by its general nature, determining its default HTTP status code.
/// </summary>
public enum ErrorCategory
{
    /// <summary>The input failed validation. Defaults to HTTP 400.</summary>
    Validation,

    /// <summary>The caller is not authenticated. Defaults to HTTP 401.</summary>
    Unauthorized,

    /// <summary>The caller is authenticated but lacks permission. Defaults to HTTP 403.</summary>
    Forbidden,

    /// <summary>The requested resource does not exist. Defaults to HTTP 404.</summary>
    NotFound,

    /// <summary>The request conflicts with the current state of the resource. Defaults to HTTP 409.</summary>
    Conflict,

    /// <summary>The caller has exceeded a rate limit. Defaults to HTTP 429.</summary>
    RateLimited,

    /// <summary>An unexpected internal error occurred. Defaults to HTTP 500.</summary>
    Internal,

    /// <summary>An upstream service returned an invalid response. Defaults to HTTP 502.</summary>
    BadGateway,

    /// <summary>The service or a dependency is temporarily unavailable. Defaults to HTTP 503.</summary>
    Unavailable,

    /// <summary>A call to an upstream service timed out. Defaults to HTTP 504.</summary>
    GatewayTimeout,

    /// <summary>A domain/business rule was violated. Defaults to HTTP 400.</summary>
    BusinessRule
}
