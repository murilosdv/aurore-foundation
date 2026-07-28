using Aurore.Foundation.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aurore.Foundation.EntityFrameworkCore.Extensions;

/// <summary>
/// Provides extension methods for <see cref="DbContextOptionsBuilder"/>.
/// </summary>
public static class DbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Replaces the registered <see cref="IHistoryRepository"/> with <see cref="SnakeCaseNpgsqlHistoryRepository"/>,
    /// so the migrations history table's columns and primary key constraint are named consistently with
    /// <see cref="ModelBuilderExtensions.UseSnakeCaseNamingConvention"/>.
    /// </summary>
    /// <param name="optionsBuilder">The options builder to configure.</param>
    /// <returns>The same <paramref name="optionsBuilder"/> instance, for chaining.</returns>
    public static DbContextOptionsBuilder UseSnakeCaseNpgsqlHistoryTable(this DbContextOptionsBuilder optionsBuilder)
    {
        return optionsBuilder.ReplaceService<IHistoryRepository, SnakeCaseNpgsqlHistoryRepository>();
    }
}
