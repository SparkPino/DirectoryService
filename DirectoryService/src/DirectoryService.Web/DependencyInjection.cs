using DirectoryService.Infrastructure.Postgres;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.File;

namespace DirectoryService.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services,
        IConfiguration configuration) => services
        .AddSerilogLogging(configuration)
        .AddWebDependencies()
        .AddDirectoryServiceDbContext(configuration);

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();

        return services;
    }

    private static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((ServiceProvider, LoggerConfiguration) => LoggerConfiguration
            .ReadFrom.Configuration(configuration) // читает настройки Serilog из IConfiguration (например, из appsettings.json)
            .ReadFrom.Services(ServiceProvider) // позволяем Serilog использовать DI
            .Enrich.FromLogContext() //for CorelationID ? 
            .Enrich.WithExceptionDetails() //
            .Enrich.WithProperty("ServiceName", "DirectoryService")); // Key-Value

        return services;
    }
}