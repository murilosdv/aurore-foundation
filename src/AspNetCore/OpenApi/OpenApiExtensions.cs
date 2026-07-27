using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Aurore.Foundation.Core.Constants;
using Aurore.Foundation.Core.Extensions;
using Aurore.Foundation.Core.Options;
using Humanizer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace Aurore.Foundation.AspNetCore.OpenApi;

/// <summary>
/// Provides extension methods for configuring OpenAPI document generation: Scalar documents, security schemes, application metadata, and standard parameters.
/// </summary>
public static class OpenApiExtensions
{
    /// <summary>
    /// Registers a Scalar document for each given API version.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="versions">The API versions to create documents for.</param>
    /// <returns>The names of the registered documents, in <c>"v{version}"</c> form.</returns>
    [ExcludeFromCodeCoverage]
    public static string[] ConfigureScalarDocuments(this IServiceCollection services, params int[] versions)
    {
        var documents = versions.Select(v => new ScalarDocument($"v{v}", $"Version {v}"));

        services.Configure<ScalarOptions>(x => x.AddDocuments(documents));

        return [.. documents.Select(x => x.Name)];
    }

    /// <summary>
    /// Adds a document transformer that registers the given security scheme and requires the given scopes for every operation.
    /// </summary>
    /// <param name="options">The OpenAPI options to configure.</param>
    /// <param name="schema">The security scheme to register.</param>
    /// <param name="scopes">The scopes to require for the scheme.</param>
    /// <returns>The <see cref="OpenApiOptions"/> so calls can be chained.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="schema"/> does not have a valid <c>Name</c>.</exception>
    public static OpenApiOptions AddSecuritySchemeAndRequirement(this OpenApiOptions options, OpenApiSecurityScheme schema, params string[] scopes)
    {
        return options.AddDocumentTransformer((document, _, _) =>
        {
            document.Components ??= new OpenApiComponents();
            document.Security ??= [];

            if (schema.Name.HasValue() is false)
                throw new ArgumentException("Security scheme must have a valid Name property.", nameof(schema));

            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

            document.Components.SecuritySchemes[schema.Name] = schema;

            document.Security.Add(new()
            {
                { new(schema.Name, document), [.. scopes] }
            });

            return Task.CompletedTask;
        });
    }

    /// <summary>
    /// Adds a document transformer that populates the document's title, description, version, and maintainer contact information.
    /// </summary>
    /// <param name="options">The OpenAPI options to configure.</param>
    /// <param name="apiVersion">The API version to display in the document.</param>
    /// <param name="appInfo">The application metadata (component, description, maintainer) to display.</param>
    /// <returns>The <see cref="OpenApiOptions"/> so calls can be chained.</returns>
    [ExcludeFromCodeCoverage]
    public static OpenApiOptions AddAppInfo(
        this OpenApiOptions options, string apiVersion, AppInfoOptions appInfo)
    {
        var companyName = "aurorelabs";

        return options.AddDocumentTransformer((doc, _, _) =>
        {
            doc.Info.Title = $"{appInfo.Component.Humanize()} API";
            doc.Info.Description = appInfo.Description;
            doc.Info.Version = apiVersion;
            doc.Info.Contact = new()
            {
                Name = appInfo.Maintainer.Name,
                Email = $"teams.{appInfo.Maintainer.Name}@{companyName}.com",
                Url = new Uri($"https://{companyName}.com/teams/{appInfo.Maintainer.Name}")
            };

            return Task.CompletedTask;
        });
    }

    /// <summary>
    /// Registers the JWT bearer security scheme.
    /// </summary>
    /// <param name="options">The OpenAPI options to configure.</param>
    /// <returns>The <see cref="OpenApiOptions"/> so calls can be chained.</returns>
    [ExcludeFromCodeCoverage]
    public static OpenApiOptions AddJwtBearerSecurity(this OpenApiOptions options)
    {
        return options.AddSecuritySchemeAndRequirement(new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Name = SecuritySchemes.JwtBearer,
            Description = "JWT Authorization header using the Bearer scheme.",
            In = ParameterLocation.Header,
            Scheme = SecuritySchemes.JwtBearer.ToLower(),
            BearerFormat = "JWT"
        });
    }

    /// <summary>
    /// Registers the OpenID Connect security scheme, pointing at the given identity provider's discovery document.
    /// </summary>
    /// <param name="options">The OpenAPI options to configure.</param>
    /// <param name="identityProvider">The identity provider whose OIDC discovery endpoint is referenced.</param>
    /// <returns>The <see cref="OpenApiOptions"/> so calls can be chained.</returns>
    [ExcludeFromCodeCoverage]
    public static OpenApiOptions AddOidcSecurity(this OpenApiOptions options, IdentityProviderOptions identityProvider)
    {
        return options.AddSecuritySchemeAndRequirement(new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OpenIdConnect,
            Name = SecuritySchemes.Oidc,
            Description = "OIDC Authorization header using the Bearer scheme.",
            OpenIdConnectUrl = new Uri($"{identityProvider.AuthorityUrl}/.well-known/openid-configuration")
        });
    }

    /// <summary>
    /// Registers the OAuth2 authorization-code-flow security scheme, pointing at the given identity provider's authorization, token, and refresh endpoints.
    /// </summary>
    /// <param name="options">The OpenAPI options to configure.</param>
    /// <param name="identityProvider">The identity provider whose OAuth2 endpoints and scopes are referenced.</param>
    /// <returns>The <see cref="OpenApiOptions"/> so calls can be chained.</returns>
    [ExcludeFromCodeCoverage]
    public static OpenApiOptions AddOAuth2Security(this OpenApiOptions options, IdentityProviderOptions identityProvider)
    {
        var baseUrl = $"{identityProvider.AuthorityUrl}/protocol/openid-connect";

        return options.AddSecuritySchemeAndRequirement(new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Name = SecuritySchemes.OAuth2,
            Description = "OAuth2 Authorization Code Flow",
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"{baseUrl}/auth"),
                    TokenUrl = new Uri($"{baseUrl}/token"),
                    RefreshUrl = new Uri($"{baseUrl}/refresh"),
                    Scopes = identityProvider.Scopes
                }
            }
        });
    }

    /// <summary>
    /// Adds a document transformer that documents the correlation-id header as an optional parameter on every operation that doesn't already declare it.
    /// </summary>
    /// <param name="options">The OpenAPI options to configure.</param>
    /// <returns>The <see cref="OpenApiOptions"/> so calls can be chained.</returns>
    public static OpenApiOptions AddCorrelationIdHeader(this OpenApiOptions options)
    {
        return options.AddDocumentTransformer((document, _, _) =>
        {
            foreach (var pathItem in document.Paths.Values)
            {
                if (pathItem.Operations is null)
                    continue;

                foreach (var operation in pathItem.Operations.Values)
                {
                    operation.Parameters ??= [];

                    if (operation.Parameters.All(p => p.Name != StandardHeaders.CorrelationId))
                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = StandardHeaders.CorrelationId,
                            In = ParameterLocation.Header,
                            Required = false,
                            Schema = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String,
                                Format = "uuid",
                                Example = JsonValue.Create(Guid.NewGuid().ToString())
                            },
                            Description = "Correlation Id for request tracking and tracing"
                        });
                }
            }

            return Task.CompletedTask;
        });
    }
}
