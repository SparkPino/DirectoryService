using System.Text.Json.Serialization;
using Core;
using DirectoryService.Application;
using DirectoryService.Infrastructure;
using DirectoryService.Infrastructure.Postgres;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using SharedLibrary.SharedKernel;
using Framework.Logging;

namespace DirectoryService.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services,
        IConfiguration configuration) => services
        .AddWebDependencies()
        .AddDirectoryServiceDbContext(configuration)
        .AddApplication()
        .AddScruptor(typeof(ICommand).Assembly)
        .AddBackgroundServices(configuration)
        .AddSerilogLogging(configuration, "DirectoryService");

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddCors();
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter()); //Microsoft.AspNetCore.Mvc.JsonOptions  
            });

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(
                new JsonStringEnumConverter()); //Microsoft.AspNetCore.Http.Json.JsonOptions
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
            //Просто отключить фильтр options.SuppressModelStateInvalidFilter = true;

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
}