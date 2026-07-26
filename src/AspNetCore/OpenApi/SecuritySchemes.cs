namespace Aurore.Foundation.AspNetCore.OpenApi;

/// <summary>
/// Names of the security schemes registered for OpenAPI documents.
/// </summary>
public static class SecuritySchemes
{
    /// <summary>
    /// The name of the JWT bearer security scheme.
    /// </summary>
    public const string JwtBearer = "Bearer";

    /// <summary>
    /// The name of the OpenID Connect security scheme.
    /// </summary>
    public const string Oidc = "OIDC";

    /// <summary>
    /// The name of the OAuth2 security scheme.
    /// </summary>
    public const string OAuth2 = "OAuth2";
}
