using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aurore.Foundation.EntityFrameworkCore.Extensions;

/// <summary>
/// Provides extension methods for <see cref="PropertyBuilder{TProperty}"/>.
/// </summary>
public static class PropertyBuilderExtensions
{
    /// <summary>
    /// Extension members for <see cref="PropertyBuilder{TProperty}"/> that configure common Postgres column types.
    /// </summary>
    /// <typeparam name="TProperty">The CLR type of the property being configured.</typeparam>
    extension<TProperty>(PropertyBuilder<TProperty> propertyBuilder)
    {
        /// <summary>
        /// Configures the property's column type as a Postgres <c>timestamp</c>, with or without time zone.
        /// </summary>
        /// <param name="hasTimezone">
        /// <see langword="true"/> to use <c>timestamptz</c> (with time zone); <see langword="false"/> to use <c>timestamp</c> (without time zone).
        /// Defaults to <see langword="true"/>.
        /// </param>
        /// <returns>The same property builder, for chaining.</returns>
        public PropertyBuilder<TProperty> AsTimestampColumn(bool hasTimezone = true)
        {
            var columnType = hasTimezone ? "timestamptz" : "timestamp";

            return propertyBuilder.HasColumnType(columnType);
        }

        /// <summary>
        /// Configures the property's numeric precision and scale, for use with decimal columns.
        /// </summary>
        /// <param name="precision">The total number of digits stored, including both sides of the decimal point. Defaults to <c>10</c>.</param>
        /// <param name="scale">The number of digits stored to the right of the decimal point. Defaults to <c>2</c>.</param>
        /// <returns>The same property builder, for chaining.</returns>
        public PropertyBuilder<TProperty> AsDecimalColumn(int precision = 10, int scale = 2)
        {
            return propertyBuilder.HasPrecision(precision, scale);
        }
    }
}
