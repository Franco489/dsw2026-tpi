using Dsw2026Tpi.Data;
using Dsw2026Tpi.Data.Extensions;
using Dsw2026Tpi.Data.Identity;
using Dsw2026Tpi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dsw2026Tpi.Api.Configurations;

public static class PersistenceConfigurationExtensions
{
    public static IServiceCollection AddApplicationPersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        //Obtener cadena de conexión desde appsettings.json
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        //Agregar contexto (O/RM) y utilizar SQL Server para DB
        services.AddDbContext<Dsw2026TpiDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
            options.UseSeeding((c, t) =>
            {
                c.Seedwork<Speciality>("specialities");
                c.Seedwork<Doctor>("doctors");
            });
        });

        services.AddDbContext<AuthenticationDbContext>(options =>
        {
            options.UseSqlServer(connectionString, 
                config => config.MigrationsHistoryTable("__EFMigrationsHistory_Auth"));
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
}
