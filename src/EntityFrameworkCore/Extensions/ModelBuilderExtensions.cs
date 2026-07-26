using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace Aurore.Foundation.EntityFrameworkCore.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ModelBuilder"/>.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Rewrites table, schema, column, key, foreign key, index, and complex property JSON names
    /// in the model to snake_case, using Humanizer's <c>Underscore</c> transformation.
    /// </summary>
    /// <param name="modelBuilder">The model builder whose model names are rewritten.</param>
    /// <returns>The same <paramref name="modelBuilder"/> instance, for chaining.</returns>
    public static ModelBuilder UseSnakeCaseNamingConvention(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()?.Underscore());

            entity.SetSchema(entity.GetSchema()?.Underscore());

            // Columns
            foreach (var property in entity.GetProperties())
                property.SetColumnName(property.GetColumnName().Underscore());

            // Keys
            foreach (var key in entity.GetKeys())
                key.SetName(key.GetName()?.Underscore());

            // Foreign keys
            foreach (var fk in entity.GetForeignKeys())
                fk.SetConstraintName(fk.GetConstraintName()?.Underscore());

            // Indexes
            foreach (var index in entity.GetIndexes())
                index.SetDatabaseName(index.GetDatabaseName()?.Underscore());

            // Complex Properties
            foreach (var complex in entity.GetComplexProperties())
                complex.SetJsonPropertyName(complex.GetJsonPropertyName()?.Underscore());
        }

        return modelBuilder;
    }
}
