using System.Collections.Generic;
using Aurore.Foundation.AspNetCore.MinimalApis;
using Aurore.Foundation.Core.Errors;
using Aurore.Foundation.Core.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Aurore.Foundation.AspNetCore.Extensions;

/// <summary>
/// Provides extension members on <see cref="TypedResults"/> for building Aurore's standard <see cref="IResult"/> responses:
/// created/accepted results without a body location, versioned "created at route" results, and RFC 7807 problem-details
/// responses built from a <see cref="Result"/> or <see cref="ApplicationError"/>.
/// </summary>
public static class TypedResultExtensions
{
    extension(TypedResults)
    {
        /// <summary>
        /// Creates a 201 Created result carrying <paramref name="value"/> without a <c>Location</c> header value.
        /// </summary>
        /// <typeparam name="T">The type of the created value.</typeparam>
        /// <param name="value">The created value to return in the response body.</param>
        /// <returns>An <see cref="IResult"/> representing the created response.</returns>
        public static IResult Created<T>(T value)
        {
            return TypedResults.Created(string.Empty, value);
        }

        /// <summary>
        /// Creates a 202 Accepted result carrying <paramref name="value"/> without a <c>Location</c> header value.
        /// </summary>
        /// <typeparam name="T">The type of the accepted value.</typeparam>
        /// <param name="value">The value to return in the response body.</param>
        /// <returns>An <see cref="IResult"/> representing the accepted response.</returns>
        public static IResult Accepted<T>(T value)
        {
            return TypedResults.Accepted(string.Empty, value);
        }

        /// <summary>
        /// Creates a 201 Created result whose <c>Location</c> points at the route mapped by <typeparamref name="TEndpoint"/>,
        /// using that endpoint's configured API version.
        /// </summary>
        /// <typeparam name="TEndpoint">The minimal API endpoint whose route the result points to.</typeparam>
        /// <param name="value">The created value to return in the response body.</param>
        /// <param name="routeValues">Additional route values to include when building the location URL, or <see langword="null"/> for none.</param>
        /// <returns>An <see cref="IResult"/> representing the created response.</returns>
        public static IResult CreatedAt<TEndpoint>(object? value, object? routeValues = null)
            where TEndpoint : MinimalEndpoint, new()
        {
            var values = new RouteValueDictionary(routeValues) { ["version"] = EndpointInfo<TEndpoint>.Version.ToString() };

            return TypedResults.CreatedAtRoute(value, EndpointInfo<TEndpoint>.RouteName, values);
        }

        /// <summary>
        /// Converts a failed <see cref="Result"/> into a problem-details response, using its <see cref="Result.Error"/> if set,
        /// or a generic validation error carrying its <see cref="Result.Errors"/> otherwise.
        /// </summary>
        /// <param name="result">The failed result to convert.</param>
        /// <returns>An <see cref="IResult"/> representing the problem-details response.</returns>
        public static IResult Problem(Result result)
        {
            return result.Error is not null
                ? Problem(result.Error, result.Errors)
                : Problem(StandardErrors.Validation, result.Errors);
        }

        /// <summary>
        /// Creates a validation-error problem-details response carrying the given error details.
        /// </summary>
        /// <param name="errors">The structured validation error details, or <see langword="null"/> for none.</param>
        /// <returns>An <see cref="IResult"/> representing the validation-error response.</returns>
        public static IResult ValidationError(Dictionary<string, object?>? errors)
        {
            var response = Problem(StandardErrors.Validation, errors);

            return response;
        }

        /// <summary>
        /// Creates a problem-details response for the given application error, with additional structured extensions.
        /// </summary>
        /// <param name="error">The application error describing the failure.</param>
        /// <param name="extensions">Additional structured data to include in the problem-details response, or <see langword="null"/> for none.</param>
        /// <returns>A <see cref="ProblemHttpResult"/> representing the problem-details response.</returns>
        public static ProblemHttpResult Problem(ApplicationError error, IDictionary<string, object?>? extensions)
        {
            return Problem(error, null, extensions);
        }

        /// <summary>
        /// Creates a problem-details response for the given application error.
        /// </summary>
        /// <param name="error">The application error describing the failure.</param>
        /// <param name="detail">The detail text to use instead of <see cref="ApplicationError.Detail"/>, or <see langword="null"/> to use the error's own detail.</param>
        /// <param name="extensions">Additional structured data to include in the problem-details response, or <see langword="null"/> for none.</param>
        /// <param name="instance">The URI identifying the specific occurrence of the problem, or <see langword="null"/> if none is set.</param>
        /// <returns>A <see cref="ProblemHttpResult"/> representing the problem-details response.</returns>
        public static ProblemHttpResult Problem(
            ApplicationError error,
            string? detail = null,
            IDictionary<string, object?>? extensions = null,
            string? instance = null)
        {
            return TypedResults.Problem(
                detail ?? error.Detail,
                instance,
                error.HttpStatusCode,
                error.Title,
                $"https://aurorelabs.com/docs/errors/{error.Code}",
                extensions);
        }

    }

    private static class EndpointInfo<TEndpoint> where TEndpoint : MinimalEndpoint, new()
    {
        public static readonly string RouteName;
        public static readonly int Version;

        static EndpointInfo()
        {
            var endpoint = new TEndpoint();

            RouteName = endpoint.RouteName;
            Version = endpoint.Version;
        }
    }
}
