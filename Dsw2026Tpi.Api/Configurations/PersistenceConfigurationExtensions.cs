using Dsw2026Tpi.Data;
using Dsw2026Tpi.Data.Extensions;
using Dsw2026Tpi.Data.Identity;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Application.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dsw2026Tpi.Api.Configurations;

public static class PersistenceConfigurationExtensions
{
    public static IServiceCollection AddApplicationPersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        //Obtener cadena de conexión desde appsettings.json
        var connectionString = configuration.GetConnectionString("PostgreSql");

        //Agregar contexto (O/RM) y utilizar SQL Server para DB
        services.AddDbContext<Dsw2026TpiDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.UseSeeding((c, t) =>
            {
                c.Seedwork<Speciality>("specialities");
                c.Seedwork<Doctor>("doctors");
            });
        });

        services.AddDbContext<AuthenticationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.UseSeeding((c, t) =>
            {
                c.Seedwork<IdentityRole>("roles");
            });
        });
        return services;
    }
    public static WebApplication ResetDbOnStart(this WebApplication app) 
    {
        using var scope = app.Services.CreateScope();
        var provider = scope.ServiceProvider;

        var domainContext = provider.GetRequiredService<Dsw2026TpiDbContext>();
        var authContext = provider.GetRequiredService<AuthenticationDbContext>();
        domainContext.Database.EnsureDeleted();
        domainContext.Database.Migrate();
        authContext.Database.Migrate();
        return app;
    }
    public static async Task<WebApplication> SeedAdminAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var email = app.Configuration.GetValue<string>("AdminEmail");
        var pass = app.Configuration.GetValue<string>("Password");

        if (email == null || pass == null) 
        {
            throw new InvalidOperationException("No se encontró los datos de usuario del Administrador en el archivo de configuración.");
        }
        var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>(); 
        await authService.Register(new RegisterModel.Request(email,pass));

        return app;
    }
}
