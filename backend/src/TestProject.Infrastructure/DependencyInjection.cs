using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestProject.Application.Common.Interfaces;
using TestProject.Infrastructure.Persistence;
using TestProject.Infrastructure.Persistence.Interceptors;
using TestProject.Infrastructure.Persistence.ReadStores;
using TestProject.Infrastructure.Persistence.Repositories;
using TestProject.Infrastructure.Services;

namespace TestProject.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddScoped<AuditableEntityInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var auditInterceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            options.AddInterceptors(auditInterceptor);
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductReadStore, ProductReadStore>();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>("postgresql");

        return services;
    }
}
