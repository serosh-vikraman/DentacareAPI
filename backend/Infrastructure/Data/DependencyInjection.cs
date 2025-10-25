using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Tenant;
using Shared.Security;
using Infrastructure.Security;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DentaCareDbContext>((sp, options) =>
        {
            var cfg = sp.GetRequiredService<IConfiguration>();
            var connectionString = cfg.GetConnectionString("DentaCare");
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
            {
                o.EnableRetryOnFailure(5);
            });
        });

        // Expose the DbContext via the application abstraction for CQRS handlers
        services.AddScoped<Application.Abstractions.IApplicationDbContext>(sp => sp.GetRequiredService<DentaCareDbContext>());

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<DentaCareDbContext>();

        services.AddSingleton<IEncryptionService, AesEncryptionService>();

        services.AddScoped<Identity.IdentitySeeder>();

        return services;
    }
}


