using System.Collections.Generic;

namespace Aurore.Foundation.Core.Errors;

/// <summary>
/// Represents a well-known application error, carrying a stable numeric code, a machine-readable name,
/// human-readable title and detail, an <see cref="ErrorCategory"/>, and an optional instance identifier.
/// </summary>
/// <param name="Code">The stable numeric code that uniquely identifies this error.</param>
/// <param name="Name">The machine-readable name of the error (typically matching the declaring member's name).</param>
/// <param name="Title">The short, human-readable summary of the error.</param>
/// <param name="Detail">The human-readable description of what went wrong.</param>
/// <param name="Category">The <see cref="ErrorCategory"/> this error belongs to, used to determine its default HTTP status code.</param>
/// <param name="Instance">An optional identifier of the specific occurrence of the error.</param>
public sealed record ApplicationError(int Code, string Name, string Title, string Detail, ErrorCategory Category, string? Instance = null)
{
    private static readonly Dictionary<ErrorCategory, int> CategoryDefaults = new()
    {
        [ErrorCategory.Validation] = 400,
        [ErrorCategory.Unauthorized] = 401,
        [ErrorCategory.Forbidden] = 403,
        [ErrorCategory.NotFound] = 404,
        [ErrorCategory.Conflict] = 409,
        [ErrorCategory.RateLimited] = 429,
        [ErrorCategory.Internal] = 500,
        [ErrorCategory.BadGateway] = 502,
        [ErrorCategory.Unavailable] = 503,
        [ErrorCategory.GatewayTimeout] = 504,
        [ErrorCategory.BusinessRule] = 400
    };

    /// <summary>
    /// Gets the HTTP status code associated with this error. If not explicitly set, it defaults to the standard
    /// status code for this error's <see cref="Category"/>.
    /// </summary>
    public int? HttpStatusCode
    {
        get => field is null ? CategoryDefaults[Category] : field;
        init;
    }

    /// <summary>
    /// Gets additional structured data associated with this error, or <see langword="null"/> if none is set.
    /// </summary>
    public Dictionary<string, object?>? Extensions { get; init; }

    /// <summary>
    /// Implicitly converts an <see cref="ApplicationError"/> to its numeric <see cref="Code"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    /// <returns>The error's <see cref="Code"/>.</returns>
    public static implicit operator int(ApplicationError error)
    {
        return error.Code;
    }
}
