using System.Text.Json.Serialization;
using DirectoryService.Application;
using DirectoryService.Infrastructure.Postgres;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Serilog;
using Serilog.Exceptions;
using Shared;

namespace DirectoryService.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services,
        IConfiguration configuration) => services
        .AddSerilogLogging(configuration)
        .AddWebDependencies()
        .AddDirectoryServiceDbContext(configuration)
        .AddApplication();

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddOpenApi(options =>
        {
            options.AddSchemaTransformer((schema, context, _) =>
            {
                var type = context.JsonTypeInfo.Type;
                var enumType = type.IsEnum ? type : Nullable.GetUnderlyingType(type);

                if (enumType is { IsEnum: true })
                {
                    schema.Type = "string";
                    schema.Enum = Enum.GetNames(enumType)
                        .Select(name => (IOpenApiAny)new OpenApiString(name))
                        .ToList();
                }

                return Task.CompletedTask;
            });
        });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e =>
                        Error.Validation(x.Key.ToLower(), e.ErrorMessage, x.Key)))
                    .ToList();

                return new BadRequestObjectResult(new Errors(errors));
            };
        });

        return services;
    }

    private static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((ServiceProvider, LoggerConfiguration) => LoggerConfiguration
            .ReadFrom
            .Configuration(
                configuration) // читает настройки Serilog из IConfiguration (тоесть например, из appsettings.json)
            .ReadFrom.Services(ServiceProvider) // позволяем Serilog использовать DI
            .Enrich.FromLogContext() //for CorelationID ? 
            .Enrich.WithExceptionDetails()
            .Enrich.WithProperty("ServiceName", "DirectoryService")); // Key-Value

        return services;
    }
}