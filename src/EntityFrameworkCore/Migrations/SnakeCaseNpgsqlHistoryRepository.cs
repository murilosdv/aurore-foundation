using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations.Internal;

namespace Aurore.Foundation.EntityFrameworkCore.Migrations;

/// <summary>
/// An <see cref="NpgsqlHistoryRepository"/> that renames the migrations history table's columns and primary
/// key constraint to snake_case. The history table's model is built independently of
/// <see cref="Microsoft.EntityFrameworkCore.DbContext.OnModelCreating"/>, so it is otherwise unaffected by
/// <see cref="Extensions.ModelBuilderExtensions.UseSnakeCaseNamingConvention"/>.
/// </summary>
/// <param name="dependencies">The dependencies required by the base <see cref="NpgsqlHistoryRepository"/>.</param>
#pragma warning disable EF1001 // NpgsqlHistoryRepository is Npgsql-internal infrastructure; it's the only base type available for this customization.
public sealed class SnakeCaseNpgsqlHistoryRepository(HistoryRepositoryDependencies dependencies)
    : NpgsqlHistoryRepository(dependencies)
#pragma warning restore EF1001
{
    /// <summary>
    /// Configures the history table, renaming the <see cref="HistoryRow.MigrationId"/> and
    /// <see cref="HistoryRow.ProductVersion"/> columns, and the primary key constraint, to snake_case.
    /// </summary>
    /// <param name="history">The builder used to configure the history table's entity type.</param>
    protected override void ConfigureTable(EntityTypeBuilder<HistoryRow> history)
    {
        base.ConfigureTable(history);

        history
            .Property(x => x.MigrationId)
            .HasColumnName(nameof(HistoryRow.MigrationId).Underscore());

        history
            .Property(x => x.ProductVersion)
            .HasColumnName(nameof(HistoryRow.ProductVersion).Underscore());

        history
            .HasKey(x => x.MigrationId)
            .HasName($"pk_{TableName}".Underscore());
    }
}
