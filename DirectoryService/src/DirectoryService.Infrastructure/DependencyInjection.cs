using DirectoryService.Infrastructure.BackgroundServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBackgroundServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SoftDeleteCleanupOptions>(configuration.GetSection("SoftDeleteCleanup"));
        services.AddHostedService<SoftDeleteCleanupService>();
        return services;
    }
}