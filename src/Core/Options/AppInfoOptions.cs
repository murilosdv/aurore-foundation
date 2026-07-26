namespace Aurore.Foundation.Core.Options;

/// <summary>
/// Describes identifying metadata about an application: its version, domain/boundary placement, description, and maintainer.
/// </summary>
public sealed record AppInfoOptions
{
    /// <summary>
    /// Gets the application version.
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// Gets the business domain the application belongs to.
    /// </summary>
    public required string Domain { get; init; }

    /// <summary>
    /// Gets the bounded context (boundary) the application belongs to within its <see cref="Domain"/>.
    /// </summary>
    public required string Boundary { get; init; }

    /// <summary>
    /// Gets the name of the application component.
    /// </summary>
    public required string Component { get; init; }

    /// <summary>
    /// Gets the fully qualified namespace, computed as <c>"{Domain}.{Boundary}"</c>.
    /// </summary>
    public string Namespace => $"{Domain}.{Boundary}";

    /// <summary>
    /// Gets a human-readable description of the application.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Gets the maintainer responsible for the application.
    /// </summary>
    public required MaintainerOptions Maintainer { get; init; }

    /// <summary>
    /// Gets the application's default locale. Defaults to <c>"en-US"</c>.
    /// </summary>
    public string Locale { get; init; } = "en-US";
}
