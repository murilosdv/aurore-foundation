namespace Aurore.Foundation.Core.Options;

/// <summary>
/// Identifies the maintainer responsible for an application.
/// </summary>
/// <param name="Name">The maintainer's name.</param>
/// <param name="Email">The maintainer's email address.</param>
public sealed record MaintainerOptions(string Name, string Email);
