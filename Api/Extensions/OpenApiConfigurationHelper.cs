using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

internal static class OpenApiConfigurationHelper
{
    public static SwaggerGenOptions ConfigureSwaggerToAcceptJwtBearer(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition(SecurityDefinitionNameForJwt, SecurityDefinitionSchemeForJwt);
        options.AddSecurityRequirement(SecurityRequirementForJwt);
        return options;
    }

    private static string SecurityDefinitionName => "Bearer";
    private static string SecurityDefinitionDescription => "JWT Bearer Token";

    private static OpenApiSecurityScheme SecurityRequirementScheme =>
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Id = SecurityDefinitionName,
                Type = ReferenceType.SecurityScheme
            }
        };

    /// <summary>
    /// Name of the Security Definition
    /// </summary>
    private static string SecurityDefinitionNameForJwt => SecurityDefinitionName;

    /// <summary>
    /// Security Definition for JWT
    /// References:
    ///  - https://learn.microsoft.com/en-us/dotnet/api/Microsoft.OpenApi.Models.OpenApiSecurityScheme
    ///  - https://swagger.io/docs/specification/authentication/
    /// </summary>
    private static OpenApiSecurityScheme SecurityDefinitionSchemeForJwt =>
        new OpenApiSecurityScheme
        {

            Description = SecurityDefinitionDescription,
            In = ParameterLocation.Header,
            Name = "Authorization",
            Scheme = "bearer",
            Type = SecuritySchemeType.Http,
        };

    /// <summary>
    /// The Security Requirement
    /// References
    /// - https://learn.microsoft.com/en-us/dotnet/api/microsoft.openapi.models.openapisecurityrequirement
    /// </summary>
    private static OpenApiSecurityRequirement SecurityRequirementForJwt =>
        new OpenApiSecurityRequirement()
        {
            {
                SecurityRequirementScheme,
                Array.Empty<string>() //This list is for oauth2 or openidconnect. N/A here, so use an empty array.
            }
        };
}