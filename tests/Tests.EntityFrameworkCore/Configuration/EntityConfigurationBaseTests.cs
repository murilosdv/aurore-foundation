using Aurore.Foundation.Core.Abstractions;
using Aurore.Foundation.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Aurore.Foundation.Tests.EntityFrameworkCore.Configuration;

public class EntityConfigurationBaseTests
{
    private sealed class WidgetEntity : IEntity<int>
    {
        public int Id { get; init; }
    }

    private sealed class WidgetEntityConfiguration : EntityConfigurationBase<WidgetEntity>;

    private sealed class CustomWidgetEntityConfiguration : EntityConfigurationBase<WidgetEntity>
    {
        protected override string TableName { get; } = "CustomWidgets";

        protected override string SchemaName { get; } = "Custom";
    }

    [Fact(DisplayName = "Configure maps the table name to the pluralized entity name with the trailing \"Entity\" suffix removed")]
    public void ConfiguresDefaultTableName()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();

        // Act
        modelBuilder.ApplyConfiguration(new WidgetEntityConfiguration());

        // Assert
        var entityType = modelBuilder.Model.FindEntityType(typeof(WidgetEntity));
        Assert.Equal("Widgets", entityType!.GetTableName());
    }

    [Fact(DisplayName = "Configure maps the schema name to \"App\" by default")]
    public void ConfiguresDefaultSchemaName()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();

        // Act
        modelBuilder.ApplyConfiguration(new WidgetEntityConfiguration());

        // Assert
        var entityType = modelBuilder.Model.FindEntityType(typeof(WidgetEntity));
        Assert.Equal("App", entityType!.GetSchema());
    }

    [Fact(DisplayName = "Configure names the primary key constraint \"PK_{TableName}\"")]
    public void ConfiguresPrimaryKeyConstraintName()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();

        // Act
        modelBuilder.ApplyConfiguration(new WidgetEntityConfiguration());

        // Assert
        var entityType = modelBuilder.Model.FindEntityType(typeof(WidgetEntity));
        Assert.Equal("PK_Widgets", entityType!.FindPrimaryKey()!.GetName());
    }

    [Fact(DisplayName = "A derived configuration that overrides TableName and SchemaName has its overrides honored")]
    public void HonorsOverriddenTableAndSchemaNames()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();

        // Act
        modelBuilder.ApplyConfiguration(new CustomWidgetEntityConfiguration());

        // Assert
        var entityType = modelBuilder.Model.FindEntityType(typeof(WidgetEntity))!;
        Assert.Equal("CustomWidgets", entityType.GetTableName());
        Assert.Equal("Custom", entityType.GetSchema());
        Assert.Equal("PK_CustomWidgets", entityType.FindPrimaryKey()!.GetName());
    }
}
