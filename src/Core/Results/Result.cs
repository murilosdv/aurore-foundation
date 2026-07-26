using System.Collections.Generic;
using Aurore.Foundation.Core.Errors;

namespace Aurore.Foundation.Core.Results;

/// <summary>
/// Represents the outcome of an operation that either succeeds or fails with an error and/or a set of error details.
/// </summary>
public record Result
{
    /// <summary>
    /// Gets the well-known <see cref="ApplicationError"/> associated with a failed result, or <see langword="null"/> on success.
    /// </summary>
    public ApplicationError? Error { get; init; }

    /// <summary>
    /// Gets additional structured error details associated with a failed result. Empty when the result is successful.
    /// </summary>
    public Dictionary<string, object?> Errors { get; init; } = [];

    /// <summary>
    /// Gets a value indicating whether the operation succeeded, i.e. no <see cref="Error"/> is set and <see cref="Errors"/> is empty.
    /// </summary>
    public bool IsSuccess => Error is null && Errors.Count == 0;

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => IsSuccess is false;

    /// <summary>
    /// Creates a failed <see cref="Result"/> carrying the given error details.
    /// </summary>
    /// <param name="errors">The structured error details describing the failure.</param>
    /// <returns>A failed <see cref="Result"/>.</returns>
    public static Result Failure(Dictionary<string, object?> errors)
    {
        return new() { Errors = errors };
    }

    /// <summary>
    /// Implicitly converts an <see cref="ApplicationError"/> into a failed <see cref="Result"/>.
    /// </summary>
    /// <param name="error">The error to wrap.</param>
    /// <returns>A failed <see cref="Result"/> carrying <paramref name="error"/> and its extensions.</returns>
    public static implicit operator Result(ApplicationError error)
    {
        return new() { Error = error, Errors = error.Extensions ?? [] };
    }

    /// <summary>
    /// Implicitly converts a dictionary of error details into a failed <see cref="Result"/>.
    /// </summary>
    /// <param name="errors">The structured error details describing the failure.</param>
    /// <returns>A failed <see cref="Result"/> carrying <paramref name="errors"/>.</returns>
    public static implicit operator Result(Dictionary<string, object?> errors)
    {
        return new() { Errors = errors };
    }
}

/// <summary>
/// Represents the outcome of an operation that either succeeds with a <typeparamref name="T"/> value, or fails.
/// </summary>
/// <typeparam name="T">The type of value produced on success.</typeparam>
public sealed record Result<T> : Result
{
    /// <summary>
    /// Initializes a new, empty <see cref="Result{T}"/>.
    /// </summary>
    public Result() { }

    /// <summary>
    /// Initializes a new successful <see cref="Result{T}"/> carrying the given value.
    /// </summary>
    /// <param name="value">The value produced by the operation.</param>
    public Result(T? value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the value produced by a successful operation, or <see langword="null"/> if the result was not initialized with a value.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Implicitly converts a value into a successful <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <returns>A successful <see cref="Result{T}"/> carrying <paramref name="value"/>.</returns>
    public static implicit operator Result<T>(T? value)
    {
        return new(value);
    }

    /// <summary>
    /// Implicitly converts an <see cref="ApplicationError"/> into a failed <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="error">The error to wrap.</param>
    /// <returns>A failed <see cref="Result{T}"/> carrying <paramref name="error"/> and its extensions.</returns>
    public static implicit operator Result<T>(ApplicationError error)
    {
        return new() { Error = error, Errors = error.Extensions ?? [] };
    }

    /// <summary>
    /// Implicitly converts a dictionary of error details into a failed <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="errors">The structured error details describing the failure.</param>
    /// <returns>A failed <see cref="Result{T}"/> carrying <paramref name="errors"/>.</returns>
    public static implicit operator Result<T>(Dictionary<string, object?> errors)
    {
        return new() { Errors = errors };
    }
}
