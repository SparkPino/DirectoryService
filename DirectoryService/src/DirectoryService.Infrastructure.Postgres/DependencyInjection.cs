using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace DirectoryService.Infrastructure.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddDirectoryServiceDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContextPool<DirectoryServiceDbContext>((sp, options) =>
        {
            string? connectionString =
                configuration.GetConnectionString(DbConstants.DIRECTORY_SERVICE_DB_CONNECTION_STRING_KEY);

            IHostEnvironment hostEnviroment = sp.GetRequiredService<IHostEnvironment>(); // дает состояние проекта

            ILoggerFactory loggerFactory = sp.GetRequiredService<ILoggerFactory>(); // берем из di 

            options.UseNpgsql(connectionString);

            if (hostEnviroment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }


            options.UseLoggerFactory(loggerFactory);
        });


        return services;
    }
}