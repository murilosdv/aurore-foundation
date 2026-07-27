using Microsoft.AspNetCore.Http.Features;

namespace Aurore.Foundation.TestBed.Fakes;

/// <summary>
/// A minimal <see cref="IHttpMaxRequestBodySizeFeature"/> whose <see cref="IsReadOnly"/> can be toggled,
/// for testing code that conditionally sets <see cref="MaxRequestBodySize"/>.
/// </summary>
public sealed class FakeMaxRequestBodySizeFeature : IHttpMaxRequestBodySizeFeature
{
    /// <inheritdoc/>
    public bool IsReadOnly { get; init; }

    /// <inheritdoc/>
    public long? MaxRequestBodySize { get; set; }
}
