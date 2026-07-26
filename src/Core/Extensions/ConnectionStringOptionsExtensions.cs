using System;
using System.Linq;
using Aurore.Foundation.Core.Options;
using Humanizer;

namespace Aurore.Foundation.Core.Extensions;

/// <summary>
/// Provides extension methods for building connection strings from <see cref="DatabaseConnectionStringOptions"/>.
/// </summary>
public static class ConnectionStringOptionsExtensions
{
    /// <summary>
    /// Builds a connection string from the options' server segment and key/value pairs, formatting each key with <paramref name="format"/>.
    /// </summary>
    /// <param name="settings">The database connection string options to build from.</param>
    /// <param name="separator">The character used to separate each segment of the connection string.</param>
    /// <param name="aggregator">The character used to separate a key from its value within a segment.</param>
    /// <param name="server">The leading server segment (e.g. <c>"Host=..."</c>) to prepend to the connection string.</param>
    /// <param name="format">A function used to transform each option key before it is written out.</param>
    /// <returns>The assembled connection string.</returns>
    public static string BuildConnectionString(
        this DatabaseConnectionStringOptions settings,
        char separator,
        char aggregator,
        string server,
        Func<string, string> format)
    {
        return string.Join(
            separator,
            [server, .. settings.Options.Select(o => $"{format(o.Key)}{aggregator}{o.Value}")]);
    }

    /// <summary>
    /// Builds a PostgreSQL-style connection string (semicolon-separated, <c>Key=Value</c> pairs with Pascal-cased keys) from the given options.
    /// </summary>
    /// <param name="settings">The database connection string options to build from.</param>
    /// <returns>The assembled PostgreSQL connection string.</returns>
    public static string BuildPostgresConnectionString(this DatabaseConnectionStringOptions settings)
    {
        return settings.BuildConnectionString(';', '=', $"Host={settings.Server}", InflectorExtensions.Pascalize);
    }
}
