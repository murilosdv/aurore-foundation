using System;
using System.Threading;
using System.Threading.Tasks;
using Aurore.Foundation.Core.Abstractions;
using Aurore.Foundation.EntityFrameworkCore.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Aurore.Foundation.Tests.EntityFrameworkCore.Interceptors;

public class TimestampInterceptorTests
{
    private sealed class TimestampedEntity : IEntity<int>, ITimestampedEntity
    {
        public int Id { get; init; }

        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; init; }

        public DateTime UpdatedAt { get; set; }
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
    {
        public DbSet<TimestampedEntity> TimestampedEntities => Set<TimestampedEntity>();
    }

    private static TestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new TimestampInterceptor())
            .Options;

        return new TestDbContext(options);
    }

    [Fact(DisplayName = "SaveChangesAsync stamps CreatedAt and UpdatedAt to the same UTC time when adding a new entity")]
    public async Task SaveChangesAsyncStampsCreatedAndUpdatedOnAdd()
    {
        // Arrange
        await using var context = CreateContext();
        var entity = new TimestampedEntity { Name = "widget" };
        context.TimestampedEntities.Add(entity);

        var before = DateTime.UtcNow;

        // Act
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var after = DateTime.UtcNow;

        // Assert
        Assert.InRange(entity.CreatedAt, before.AddSeconds(-5), after.AddSeconds(5));
        Assert.Equal(entity.CreatedAt, entity.UpdatedAt);
    }

    [Fact(DisplayName = "SaveChanges updates UpdatedAt but leaves CreatedAt unchanged when modifying an existing entity")]
    public void SaveChangesUpdatesUpdatedAtOnlyOnModify()
    {
        // Arrange
        using var context = CreateContext();
        var entity = new TimestampedEntity { Name = "widget" };
        context.TimestampedEntities.Add(entity);
        context.SaveChanges();

        var originalCreatedAt = entity.CreatedAt;
        var originalUpdatedAt = entity.UpdatedAt;
        Thread.Sleep(10);

        // Act
        entity.Name = "gadget";
        context.SaveChanges();

        // Assert
        Assert.Equal(originalCreatedAt, entity.CreatedAt);
        Assert.True(entity.UpdatedAt > originalUpdatedAt);
    }
}
