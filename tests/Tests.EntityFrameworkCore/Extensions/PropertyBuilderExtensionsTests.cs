using System;
using Aurore.Foundation.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Aurore.Foundation.Tests.EntityFrameworkCore.Extensions;

public class PropertyBuilderExtensionsTests
{
    private sealed class TestEntity
    {
        public int Id { get; set; }

        public DateTime SomeDateTime { get; set; }

        public decimal SomeDecimal { get; set; }
    }

    [Fact(DisplayName = "AsTimestampColumn defaults to a timestamptz column type")]
    public void AsTimestampColumnDefaultsToTimestampWithTimezone()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();

        // Act
        modelBuilder.Entity<TestEntity>(e => e.Property(x => x.SomeDateTime).AsTimestampColumn());

        // Assert
        var property = modelBuilder.Model.FindEntityType(typeof(TestEntity))!.FindProperty(nameof(TestEntity.SomeDateTime));
        Assert.Equal("timestamptz", property!.GetColumnType());
    }

    [Fact(DisplayName = "AsTimestampColumn with hasTimezone false configures a timestamp column type")]
    public void AsTimestampColumnWithoutTimezoneConfiguresTimestamp()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();

        // Act
        modelBuilder.Entity<TestEntity>(e => e.Property(x => x.SomeDateTime).AsTimestampColumn(hasTimezone: false));

        // Assert
        var property = modelBuilder.Model.FindEntityType(typeof(TestEntity))!.FindProperty(nameof(TestEntity.SomeDateTime));
        Assert.Equal("timestamp", property!.GetColumnType());
    }

    [Fact(DisplayName = "AsDecimalColumn defaults to precision 10 and scale 2")]
    public void AsDecimalColumnDefaultsToPrecisionTenScaleTwo()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();

        // Act
        modelBuilder.Entity<TestEntity>(e => e.Property(x => x.SomeDecimal).AsDecimalColumn());

        // Assert
        var property = modelBuilder.Model.FindEntityType(typeof(TestEntity))!.FindProperty(nameof(TestEntity.SomeDecimal));
        Assert.Equal(10, property!.GetPrecision());
        Assert.Equal(2, property.GetScale());
    }

    [Fact(DisplayName = "AsDecimalColumn honors a custom precision and scale")]
    public void AsDecimalColumnHonorsCustomPrecisionAndScale()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();

        // Act
        modelBuilder.Entity<TestEntity>(e => e.Property(x => x.SomeDecimal).AsDecimalColumn(precision: 18, scale: 4));

        // Assert
        var property = modelBuilder.Model.FindEntityType(typeof(TestEntity))!.FindProperty(nameof(TestEntity.SomeDecimal));
        Assert.Equal(18, property!.GetPrecision());
        Assert.Equal(4, property.GetScale());
    }
}
