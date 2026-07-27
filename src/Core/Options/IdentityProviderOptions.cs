using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Aurore.Foundation.Core.Options;

/// <summary>
/// Configures the identity provider used for authenticating and authorizing requests.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record IdentityProviderOptions
{
    /// <summary>
    /// Gets the authority URL of the identity provider.
    /// </summary>
    public required string AuthorityUrl { get; init; }

    /// <summary>
    /// Gets the expected token audience.
    /// </summary>
    public required string Audience { get; init; }

    /// <summary>
    /// Gets the supported scopes, keyed by scope name, with a human-readable description as the value.
    /// </summary>
    public Dictionary<string, string> Scopes { get; init; } = [];

    /// <summary>
    /// Gets a value indicating whether HTTPS metadata retrieval is required from the identity provider. Defaults to <see langword="true"/>.
    /// </summary>
    public bool RequireHttpsMetadata { get; init; } = true;
}
