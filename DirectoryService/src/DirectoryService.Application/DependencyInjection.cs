using Core;
using DirectoryService.Application.Decorators.Commands;
using DirectoryService.Application.Decorators.Queries;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.Scan(scan => scan
            .FromAssemblies(typeof(DependencyInjection).Assembly)
            .AddClasses(classes => classes
                .AssignableToAny(typeof(ICommandHandler<,>), typeof(IQueryHandler<,>), typeof(IQueryHandler<>))
                .Where(t => !t.Name.Contains("Decorator")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Последний Decorate - внешний слой: Logging -> Validation -> Handler.
        services.Decorate(typeof(ICommandHandler<,>), typeof(CommandValidationDecorator<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(CommandLoggingDecorator<,>));

        services.Decorate(typeof(IQueryHandler<,>), typeof(QueryValidationDecorator<,>));
        services.Decorate(typeof(IQueryHandler<,>), typeof(QueryLoggingDecorator<,>));

        return services;
    }
}