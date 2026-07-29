using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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

            // Keys
            foreach (var key in entity.GetKeys())
                key.SetName(key.GetName()?.Underscore());

            // Foreign keys
            foreach (var fk in entity.GetForeignKeys())
                fk.SetConstraintName(fk.GetConstraintName()?.Underscore());

            // Indexes
            foreach (var index in entity.GetIndexes())
                index.SetDatabaseName(index.GetDatabaseName()?.Underscore());

            // Columns, including those nested inside (possibly nested) complex properties
            var storeObject = StoreObjectIdentifier.Table(entity.GetTableName()!, entity.GetSchema());
            UnderscoreColumns(entity, storeObject);
        }

        return modelBuilder;
    }

    // Entity-level properties are exposed by GetProperties(), but properties nested inside a
    // ComplexProperty (e.g. Name.First) are not - they live on complex.ComplexType.GetProperties().
    // We recurse so nested complex properties (and complex-in-complex) are covered too.
    private static void UnderscoreColumns(IMutableTypeBase typeBase, in StoreObjectIdentifier storeObject)
    {
        foreach (var property in typeBase.GetProperties())
        {
            // Table-splitting complex properties default their column name to "Complex_Property"
            // (e.g. "Name_First"). That default must be read via the StoreObjectIdentifier overload,
            // since the parameterless GetColumnName() ignores the complex property prefix.
            var columnName = property.GetColumnName(storeObject);
            if (columnName is not null)
                property.SetColumnName(columnName.Underscore());
        }

        foreach (var complex in typeBase.GetComplexProperties())
        {
            complex.SetJsonPropertyName(complex.GetJsonPropertyName()?.Underscore());
            UnderscoreColumns(complex.ComplexType, storeObject);
        }
    }
}
