using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Aurore.Foundation.AspNetCore.OpenApi;
using Aurore.Foundation.Core.Constants;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Aurore.Foundation.Tests.AspNetCore.OpenApi;

public class OpenApiExtensionsTests
{
    // OpenApiOptions doesn't expose its registered document transformers through any public API - reflection
    // into this private field is the only way to invoke a registered transformer directly, without spinning
    // up the full OpenAPI document-generation pipeline just to exercise this one delegate.
    private static IReadOnlyList<IOpenApiDocumentTransformer> GetDocumentTransformers(OpenApiOptions options)
    {
        var field = typeof(OpenApiOptions).GetField("DocumentTransformers", BindingFlags.NonPublic | BindingFlags.Instance)!;

        return (List<IOpenApiDocumentTransformer>)field.GetValue(options)!;
    }

    [Fact(DisplayName = "AddSecuritySchemeAndRequirement throws ArgumentException when the scheme has no Name")]
    public async Task AddSecuritySchemeAndRequirementThrowsWhenSchemeHasNoName()
    {
        // Arrange
        var options = new OpenApiOptions();
        var schema = new OpenApiSecurityScheme { Type = SecuritySchemeType.Http };
        options.AddSecuritySchemeAndRequirement(schema);
        var transformer = GetDocumentTransformers(options).Single();
        var document = new OpenApiDocument();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => transformer.TransformAsync(document, null!, CancellationToken.None));
    }

    [Fact(DisplayName = "AddSecuritySchemeAndRequirement registers the scheme and requires the given scopes")]
    public async Task AddSecuritySchemeAndRequirementRegistersSchemeAndScopes()
    {
        // Arrange
        var options = new OpenApiOptions();
        var schema = new OpenApiSecurityScheme { Type = SecuritySchemeType.Http, Name = "MyScheme" };
        options.AddSecuritySchemeAndRequirement(schema, "read", "write");
        var transformer = GetDocumentTransformers(options).Single();
        var document = new OpenApiDocument();

        // Act
        await transformer.TransformAsync(document, null!, CancellationToken.None);

        // Assert
        Assert.True(document.Components!.SecuritySchemes!.ContainsKey("MyScheme"));
        var requirement = Assert.Single(document.Security!);
        var scopes = requirement.Values.Single();
        Assert.Contains("read", scopes);
        Assert.Contains("write", scopes);
    }

    [Fact(DisplayName = "AddCorrelationIdHeader adds a correlation-id parameter to an operation that doesn't already declare one")]
    public async Task AddCorrelationIdHeaderAddsParameterWhenMissing()
    {
        // Arrange
        var options = new OpenApiOptions();
        options.AddCorrelationIdHeader();
        var transformer = GetDocumentTransformers(options).Single();

        var operation = new OpenApiOperation();
        var document = new OpenApiDocument
        {
            Paths = new OpenApiPaths
            {
                ["/widgets"] = new OpenApiPathItem
                {
                    Operations = new Dictionary<HttpMethod, OpenApiOperation> { [HttpMethod.Get] = operation }
                }
            }
        };

        // Act
        await transformer.TransformAsync(document, null!, CancellationToken.None);

        // Assert
        Assert.Contains(operation.Parameters!, p => p.Name == StandardHeaders.CorrelationId);
    }

    [Fact(DisplayName = "AddCorrelationIdHeader does not duplicate a correlation-id parameter an operation already declares")]
    public async Task AddCorrelationIdHeaderSkipsOperationThatAlreadyDeclaresIt()
    {
        // Arrange
        var options = new OpenApiOptions();
        options.AddCorrelationIdHeader();
        var transformer = GetDocumentTransformers(options).Single();

        var existingParameter = new OpenApiParameter { Name = StandardHeaders.CorrelationId };
        var operation = new OpenApiOperation { Parameters = [existingParameter] };
        var document = new OpenApiDocument
        {
            Paths = new OpenApiPaths
            {
                ["/widgets"] = new OpenApiPathItem
                {
                    Operations = new Dictionary<HttpMethod, OpenApiOperation> { [HttpMethod.Get] = operation }
                }
            }
        };

        // Act
        await transformer.TransformAsync(document, null!, CancellationToken.None);

        // Assert
        var parameter = Assert.Single(operation.Parameters!);
        Assert.Same(existingParameter, parameter);
    }
}
