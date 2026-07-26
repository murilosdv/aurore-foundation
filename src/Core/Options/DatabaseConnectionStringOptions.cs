using System.Collections.Generic;

namespace Aurore.Foundation.Core.Options;

/// <summary>
/// Describes the components needed to build a database connection string, as consumed by
/// <see cref="Extensions.ConnectionStringOptionsExtensions"/>.
/// </summary>
public sealed record DatabaseConnectionStringOptions
{
    /// <summary>
    /// Gets the database server host.
    /// </summary>
    public required string Server { get; init; }

    /// <summary>
    /// Gets the additional key/value connection options to append to the connection string.
    /// </summary>
    public Dictionary<string, object> Options { get; init; } = [];
}
