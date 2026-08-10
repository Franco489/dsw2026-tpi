using System.Text.Json;
using System.Threading.RateLimiting;

namespace Dsw2026Tpi.Api.Configurations;

public static class RateLimitingConfigurationExtensions
{
    public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services, IConfiguration config)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, token) =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning("Rate limit excedido para la IP/Usuario {IP}", context.HttpContext.Connection.RemoteIpAddress);

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    errorCode = "TOO_MANY_REQUESTS",
                    message = "Se ha excedido el límite de solicitudes. Intente nuevamente más tarde."
                };

                await context.HttpContext.Response.WriteAsync(
                    JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    token);
            };

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = config.GetValue<int>("RateLimiting:Global:PermitLimit"),
                        QueueLimit = 0, // Regla TP: No encolar
                        Window = config.GetValue<TimeSpan>("RateLimiting:Global:Window")
                    }));

          
            options.AddPolicy("AdminLoginPolicy", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = config.GetValue<int>("RateLimiting:AdminLogin:PermitLimit"),
                        QueueLimit = 0,
                        Window = config.GetValue<TimeSpan>("RateLimiting:AdminLogin:Window")
                    }));

            options.AddPolicy("PatientLoginPolicy", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = config.GetValue<int>("RateLimiting:PatientLogin:PermitLimit"),
                        QueueLimit = 0,
                        Window = config.GetValue<TimeSpan>("RateLimiting:PatientLogin:Window")
                    }));

            options.AddPolicy("BookingPolicy", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                   
                    partitionKey: httpContext.User.Identity?.Name ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = config.GetValue<int>("RateLimiting:AppointmentBooking:PermitLimit"),
                        QueueLimit = 0,
                        Window = config.GetValue<TimeSpan>("RateLimiting:AppointmentBooking:Window")
                    }));
        });

        return services;
    }
}
