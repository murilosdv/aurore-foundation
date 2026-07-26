using Aurore.Foundation.Core.Abstractions;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aurore.Foundation.EntityFrameworkCore.Configuration;

/// <summary>
/// Base class for <see cref="IEntityTypeConfiguration{TEntity}"/> implementations whose entity is keyed by an <see cref="int"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type being configured.</typeparam>
public abstract class EntityConfigurationBase<TEntity> : EntityConfigurationBase<TEntity, int>
    where TEntity : class, IEntity<int>;

/// <summary>
/// Base class for <see cref="IEntityTypeConfiguration{TEntity}"/> implementations that applies conventional
/// table naming and primary key configuration, leaving further customization to derived classes.
/// </summary>
/// <typeparam name="TEntity">The entity type being configured.</typeparam>
/// <typeparam name="TKey">The type of the entity's identifier.</typeparam>
public abstract class EntityConfigurationBase<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
    where TEntity : class, IEntity<TKey>
    where TKey : struct
{
    /// <summary>
    /// Gets the table name for <typeparamref name="TEntity"/>. Defaults to the pluralized entity type name
    /// with any trailing "Entity" suffix removed.
    /// </summary>
    protected virtual string TableName { get; } = typeof(TEntity).Name.Replace("Entity", string.Empty).Pluralize();

    /// <summary>
    /// Gets the database schema name in which the table is created. Defaults to <c>"App"</c>.
    /// </summary>
    protected virtual string SchemaName { get; } = "App";

    /// <summary>
    /// Configures the entity type by applying the table/schema mapping, primary key configuration,
    /// and any additional configuration supplied by <see cref="Extend"/>.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.ToTable(TableName, SchemaName);

        ConfigureIds(builder);

        Extend(builder);
    }

    /// <summary>
    /// Applies additional, entity-specific configuration. Override this method in derived classes
    /// to customize the entity type beyond the conventional table and key configuration.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    protected virtual void Extend(EntityTypeBuilder<TEntity> builder)
    {
        // Customize extensions
    }

    /// <summary>
    /// Configures the entity's primary key, naming the key constraint <c>PK_{TableName}</c>.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    protected virtual void ConfigureIds(EntityTypeBuilder<TEntity> builder)
    {
        builder
            .HasKey(x => x.Id)
            .HasName($"PK_{TableName}");
    }
}
