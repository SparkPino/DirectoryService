using Dapper;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Abstraction.Database;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Application.Locations;
using DirectoryService.Infrastructure.Postgres.Database;
using DirectoryService.Infrastructure.Postgres.Repositories;
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
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        services.AddDbContextPool<DirectoryServiceDbContext>((sp, options) =>
        {
            string? connectionString =
                configuration.GetConnectionString(DbConstants.DIRECTORY_SERVICE_DB_CONNECTION_STRING_KEY);

            IHostEnvironment hostEnvironment = sp.GetRequiredService<IHostEnvironment>(); // дает состояние проекта

            ILoggerFactory loggerFactory = sp.GetRequiredService<ILoggerFactory>(); // берем из di 


            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), null);
                npgsqlOptions.MigrationsAssembly(typeof(DirectoryServiceDbContext).Assembly.FullName);
            });

            if (hostEnvironment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }

            options.UseLoggerFactory(loggerFactory);
        });

        services.AddScoped<IReadDbContext>(sp =>
            sp.GetRequiredService<DirectoryServiceDbContext>()); // используем тоже подключение

        services.AddSingleton<IDbConnectionFactory, NpgsqlDbConnectionFactory>();

        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();

        services.AddScoped<ITransactionManager, TransactionManager>();


        return services;
    }
}