using Microsoft.OpenApi;
using System.Text.Json.Nodes;

namespace Dsw2026Tpi.Api.Configurations;

public static class SwaggerConfigurationExtensions
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(o =>
        {
            const string schemeId = "Bearer";
            o.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Desarollo de Software 2026",
                Version = "v1",
            });
            o.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Ingrese el token JWT con el prefijo 'Bearer ' (ej: Bearer eyJhbG...)",
                Type = SecuritySchemeType.ApiKey
            });
            o.AddSecurityRequirement(doc =>
            {
                return new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference(schemeId, doc),
                        new List<string>()
                    }
                };
            });

            // Configura nombres únicos para schemas con tipos anidados
            o.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
            o.MapType<TimeOnly>(() => new OpenApiSchema
            {
                
                Format = "time",
                Example = JsonValue.Create("08:00") // ¡Esta es la forma moderna de hacerlo!
            });
        });
        return services;
    }
}
