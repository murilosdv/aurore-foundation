using System.Linq;
using Aurore.Foundation.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Aurore.Foundation.Tests.EntityFrameworkCore.Extensions;

public class ModelBuilderExtensionsTests
{
    private sealed class TestEntity
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
    }

    [Fact(DisplayName = "UseSnakeCaseNamingConvention rewrites the table name to snake_case")]
    public void RewritesTableNameToSnakeCase()
    {
        // Arrange
        var modelBuilder = BuildModel();

        // Act
        modelBuilder.UseSnakeCaseNamingConvention();

        // Assert
        var entityType = modelBuilder.Model.GetEntityTypes().Single();
        Assert.Equal("user_profiles", entityType.GetTableName());
    }

    [Fact(DisplayName = "UseSnakeCaseNamingConvention rewrites column names to snake_case")]
    public void RewritesColumnNamesToSnakeCase()
    {
        // Arrange
        var modelBuilder = BuildModel();

        // Act
        modelBuilder.UseSnakeCaseNamingConvention();

        // Assert
        var entityType = modelBuilder.Model.GetEntityTypes().Single();
        var property = entityType.FindProperty(nameof(TestEntity.FirstName));
        Assert.Equal("first_name", property!.GetColumnName());
    }

    [Fact(DisplayName = "UseSnakeCaseNamingConvention rewrites the primary key constraint name to snake_case")]
    public void RewritesKeyNameToSnakeCase()
    {
        // Arrange
        var modelBuilder = BuildModel();

        // Act
        modelBuilder.UseSnakeCaseNamingConvention();

        // Assert
        var entityType = modelBuilder.Model.GetEntityTypes().Single();
        Assert.Equal("pk_user_profiles", entityType.FindPrimaryKey()!.GetName());
    }

    [Fact(DisplayName = "UseSnakeCaseNamingConvention rewrites index database names to snake_case")]
    public void RewritesIndexNameToSnakeCase()
    {
        // Arrange
        var modelBuilder = BuildModel();

        // Act
        modelBuilder.UseSnakeCaseNamingConvention();

        // Assert
        var entityType = modelBuilder.Model.GetEntityTypes().Single();
        var index = entityType.GetIndexes().Single();
        Assert.Equal("ix_user_profiles_first_name", index.GetDatabaseName());
    }

    private static ModelBuilder BuildModel()
    {
        var modelBuilder = new ModelBuilder();

        modelBuilder.Entity<TestEntity>(e =>
        {
            e.ToTable("UserProfiles");
            e.Property(x => x.FirstName);
            e.HasKey(x => x.Id).HasName("PK_UserProfiles");
            e.HasIndex(x => x.FirstName).HasDatabaseName("IX_UserProfiles_FirstName");
        });

        return modelBuilder;
    }
}
