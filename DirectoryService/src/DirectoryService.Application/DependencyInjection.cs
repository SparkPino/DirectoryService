using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Behaviors;
using DirectoryService.Application.Decorators;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Locations.AddLocation;
using DirectoryService.Contracts;
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
            .AddClasses(classes => classes.AssignableToAny(
                typeof(ICommandHandler<,>), typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Декоратор логирования оборачивает резолв через ICommandHandler<,>/IQueryHandler<,> напрямую из DI
        // (как в LocationController). На диспетчеризацию через MediatR (ISender.Send, как в DepartmentController)
        // он не влияет, потому что MediatR резолвит хендлеры по отдельной регистрации IRequestHandler<,>.
        services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingCommandHandlerDecorator<,>));
        services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingQueryHandlerDecorator<,>));

        services.AddMediatR(c =>
        {
            c.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            
            c.AddOpenBehavior(typeof(LoggingBehaviour<,>));
            c.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        });

        return services;
    }
}