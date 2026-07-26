
namespace Aurore.Foundation.Core.Errors;

/// <summary>
/// Provides a catalog of well-known, reusable <see cref="ApplicationError"/> instances shared across Aurore applications.
/// </summary>
public static class StandardErrors
{
    /// Core / System (11xx)
    /// <summary>An unexpected, unclassified error occurred.</summary>
    public static readonly ApplicationError Unknown =
        new(1100, nameof(Unknown), "Internal Server Error", "An unexpected error occurred.", ErrorCategory.Internal);

    /// <summary>An operation could not be completed.</summary>
    public static readonly ApplicationError OperationFailed =
        new(1101, nameof(OperationFailed), "Operation Failed", "The operation could not be completed.", ErrorCategory.Internal);

    /// <summary>The entity is in a state that does not allow the requested operation.</summary>
    public static readonly ApplicationError InvalidState =
        new(1102, nameof(InvalidState), "Invalid State", "The entity is in an invalid state.", ErrorCategory.Conflict);

    /// <summary>The requested action is not permitted.</summary>
    public static readonly ApplicationError NotAllowed =
        new(1103, nameof(NotAllowed), "Not Allowed", "The requested action is not permitted.", ErrorCategory.Forbidden);

    /// <summary>An operation exceeded a defined limit.</summary>
    public static readonly ApplicationError LimitExceeded =
        new(1104, nameof(LimitExceeded), "Limit Exceeded", "An operation exceeded the defined limits.", ErrorCategory.RateLimited);

    /// <summary>The operation took too long to complete.</summary>
    public static readonly ApplicationError Timeout =
        new(1105, nameof(Timeout), "Timeout", "The operation took too long to complete.", ErrorCategory.Unavailable);

    /// <summary>An external dependency required by the operation is not available.</summary>
    public static readonly ApplicationError DependencyUnavailable =
        new(1106, nameof(DependencyUnavailable), "Dependency Unavailable", "An external dependency is not available.", ErrorCategory.Unavailable);

    /// Validation / Input (12xx)
    /// <summary>One or more validation errors occurred.</summary>
    public static readonly ApplicationError Validation =
        new(1200, nameof(Validation), "Validation Error", "One or more validation errors occurred.", ErrorCategory.Validation);

    /// <summary>A required field is missing.</summary>
    public static readonly ApplicationError Required =
        new(1201, nameof(Required), "Validation Error", "A required field is missing.", ErrorCategory.Validation);

    /// <summary>The format of a provided value is invalid.</summary>
    public static readonly ApplicationError InvalidFormat =
        new(1202, nameof(InvalidFormat), "Validation Error", "The format of the provided value is invalid.", ErrorCategory.Validation);

    /// <summary>A provided value is out of the allowed range.</summary>
    public static readonly ApplicationError OutOfRange =
        new(1203, nameof(OutOfRange), "Validation Error", "The provided value is out of range.", ErrorCategory.Validation);

    /// <summary>A duplicate entry was detected.</summary>
    public static readonly ApplicationError Duplicate =
        new(1204, nameof(Duplicate), "Conflict", "A duplicate entry was detected.", ErrorCategory.Conflict);

    /// <summary>The requested resource was not found.</summary>
    public static readonly ApplicationError NotFound =
        new(1205, nameof(NotFound), "Not Found", "The requested resource was not found.", ErrorCategory.NotFound);

    /// <summary>The operation conflicts with the current state of the resource.</summary>
    public static readonly ApplicationError Conflict =
        new(1206, nameof(Conflict), "Conflict", "The operation conflicts with the current state.", ErrorCategory.Conflict);

    /// Security / Permissions (13xx)
    /// <summary>Authentication is required to perform this action.</summary>
    public static readonly ApplicationError Unauthorized =
        new(1300, nameof(Unauthorized), "Unauthorized", "Authentication is required to perform this action.", ErrorCategory.Unauthorized);

    /// <summary>The caller does not have permission to perform this action.</summary>
    public static readonly ApplicationError Forbidden =
        new(1301, nameof(Forbidden), "Forbidden", "You do not have permission to perform this action.", ErrorCategory.Forbidden);

    /// Concurrency / State (14xx)
    /// <summary>The action has already been executed.</summary>
    public static readonly ApplicationError AlreadyProcessed =
        new(1400, nameof(AlreadyProcessed), "Conflict", "This action has already been executed.", ErrorCategory.Conflict);

    /// <summary>The resource is currently locked. Uses HTTP 423 instead of its category's default status code.</summary>
    public static readonly ApplicationError Locked =
        new(1401, nameof(Locked), "Locked", "The resource is currently locked.", ErrorCategory.Conflict) { HttpStatusCode = 423 };

    /// <summary>The data is outdated due to a concurrency issue.</summary>
    public static readonly ApplicationError StaleData =
        new(1402, nameof(StaleData), "Conflict", "The data is outdated due to a concurrency issue.", ErrorCategory.Conflict);

    /// External / Integration (15xx)
    /// <summary>A call to an external service timed out.</summary>
    public static readonly ApplicationError UpstreamTimeout =
        new(1500, nameof(UpstreamTimeout), "Upstream Timeout", "A call to an external service timed out.", ErrorCategory.GatewayTimeout);

    /// <summary>An upstream service returned an invalid response.</summary>
    public static readonly ApplicationError BadGateway =
        new(1501, nameof(BadGateway), "Bad Gateway", "An upstream service returned an invalid response.", ErrorCategory.BadGateway);

    /// <summary>A dependency failed to process the request.</summary>
    public static readonly ApplicationError DependencyFailure =
        new(1502, nameof(DependencyFailure), "Dependency Failure", "A dependency failed to process the request.", ErrorCategory.BadGateway);

    /// <summary>An external service is currently unavailable.</summary>
    public static readonly ApplicationError ExternalServiceUnavailable =
        new(1503, nameof(ExternalServiceUnavailable), "Service Unavailable", "An external service is currently unavailable.", ErrorCategory.Unavailable);
}
