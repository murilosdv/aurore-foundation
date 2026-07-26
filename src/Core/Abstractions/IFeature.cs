using System.Threading;
using System.Threading.Tasks;
using Aurore.Foundation.Core.Results;

namespace Aurore.Foundation.Core.Abstractions;

/// <summary>
/// Represents a feature (use case) that produces a <typeparamref name="TResult"/> without requiring an input request.
/// </summary>
/// <typeparam name="TResult">The type of value produced on success.</typeparam>
public interface IFeature<TResult> where TResult : class
{
    /// <summary>
    /// Executes the feature.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A <see cref="Result{T}"/> wrapping the outcome of the operation.</returns>
    ValueTask<Result<TResult>> HandleAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Represents a feature (use case) that takes a <typeparamref name="TRequest"/> and produces a <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the input request.</typeparam>
/// <typeparam name="TResult">The type of value produced on success.</typeparam>
public interface IFeature<TRequest, TResult> where TResult : class
{
    /// <summary>
    /// Executes the feature for the given request.
    /// </summary>
    /// <param name="request">The input for the operation.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A <see cref="Result{T}"/> wrapping the outcome of the operation.</returns>
    ValueTask<Result<TResult>> HandleAsync(TRequest request, CancellationToken cancellationToken);
}
