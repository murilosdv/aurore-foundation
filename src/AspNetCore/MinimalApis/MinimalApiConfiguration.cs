using System;
using System.Linq;
using Asp.Versioning;
using Aurore.Foundation.Core.Extensions;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Aurore.Foundation.AspNetCore.MinimalApis;

/// <summary>
/// Provides extension methods for discovering and mapping minimal API resource groups and their endpoints by reflection.
/// </summary>
public static class MinimalApiConfiguration
{
    /// <summary>
    /// Scans the assembly containing <typeparamref name="TAssembly"/> for <see cref="ResourceGroup"/> subclasses, maps each
    /// as a versioned route group, and maps every <see cref="MinimalEndpoint"/> declared for that group beneath it.
    /// </summary>
    /// <typeparam name="TAssembly">A marker type whose declaring assembly is scanned for resource groups and endpoints.</typeparam>
    /// <param name="builder">The endpoint route builder to map resources on.</param>
    public static void MapResourcesFromAssembly<TAssembly>(this IEndpointRouteBuilder builder)
    {
        var types = typeof(TAssembly).Assembly.GetTypes();

        var groups = types.Where(IsResourceGroup).Select(CreateInstance<ResourceGroup>);

        foreach (var group in groups)
        {
            var prefix = group.PrefixWithApi ? "api/v{version:apiVersion}" : "v{version:apiVersion}";

            var groupBuilder = builder
                .MapGroup($"{prefix}/{group.Name}")
                .ConfigureTags(group)
                .ConfigureProblems(group)
                .ConfigureVersionSet(group);

            group.Configure(groupBuilder);

            groupBuilder.ConfigureEndpoints(types, group);
        }

        return;

        static bool IsResourceGroup(Type t)
        {
            return t.IsAbstract is false && t.IsSubclassOf(typeof(ResourceGroup));
        }
    }

    internal static T CreateInstance<T>(Type type)
    {
        try
        {
            return (T)Activator.CreateInstance(type)!;
        }
        catch (MissingMethodException ex)
        {
            throw new InvalidOperationException(
                $"Failed to create an instance of {typeof(T).Name.Titleize()} '{type.FullName}'. Ensure it has a public parameterless constructor.", ex);
        }
    }

    internal static RouteGroupBuilder ConfigureTags(this RouteGroupBuilder builder, ResourceGroup definition)
    {
        return builder.WithTags([.. definition.Tags.Where(x => x.HasValue())]);
    }

    internal static RouteGroupBuilder ConfigureProblems(this RouteGroupBuilder builder, ResourceGroup definition)
    {
        foreach (var problem in definition.Problems.Concat([StatusCodes.Status500InternalServerError]).Distinct())
            builder.ProducesProblem(problem);

        return builder;
    }

    internal static RouteGroupBuilder ConfigureVersionSet(this RouteGroupBuilder builder, ResourceGroup definition)
    {
        var set = builder.NewApiVersionSet().ReportApiVersions();

        foreach (var version in definition.Versions)
            set.HasApiVersion(new ApiVersion(version));

        builder.WithApiVersionSet(set.Build());

        return builder;
    }

    internal static RouteGroupBuilder ConfigureAuthorization(this RouteGroupBuilder builder, ResourceGroup definition)
    {
        if (definition.RequireAuthorization is false)
            return builder;

        return definition.AuthorizationPolicyName.HasValue()
            ? builder.RequireAuthorization(definition.AuthorizationPolicyName)
            : builder.RequireAuthorization();
    }

    internal static void ConfigureEndpoints(this RouteGroupBuilder builder, Type[] types, ResourceGroup group)
    {
        var groupType = group.GetType();

        var endpoints = types
            .Where(x => x is { IsClass: true, IsAbstract: false })
            .Where(x => GetEndpointGroupType(x) == groupType)
            .Select(CreateInstance<MinimalEndpoint>);

        foreach (var endpoint in endpoints)
        {
            var endpointConf = endpoint
                .Map(builder)
                .WithTags(group.Tags)
                .WithName(endpoint.RouteName);

            if (endpoint.Summary is not null)
                endpointConf.WithSummary(endpoint.Summary);

            if (endpoint.Description is not null)
                endpointConf.WithDescription(endpoint.Description);

            endpointConf.MapToApiVersion(endpoint.Version);
        }
    }

    private static Type? GetEndpointGroupType(Type type)
    {
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            if (current is { IsGenericType: true } && current.GetGenericTypeDefinition() == typeof(MinimalEndpoint<>))
                return current.GetGenericArguments()[0];
        }

        return null;
    }
}
