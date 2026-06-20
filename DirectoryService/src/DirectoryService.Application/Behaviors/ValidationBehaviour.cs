using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Validations;
using FluentValidation;
using MediatR;
using Shared;

namespace DirectoryService.Application.Behaviors;

public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse, Errors>>
    where TRequest : ICommand<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<Result<TResponse, Errors>> Handle(
        TRequest request,
        RequestHandlerDelegate<Result<TResponse, Errors>> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request); // контекс нужен для комуникации между валидаторами
        // в даном случае он не нужен просто повторяю за преподом
        // но это ошибка так как такой код создает риск race condition.Напоминалка context = request так как внутри происходит context(request)
        var result = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(request, cancellationToken)));

        var failures = result
            .Where(a => !a.IsValid)
            .ToList();

        if (failures.Count != 0)
        {
            return failures.ToError();
        }

        return await next(cancellationToken);
    }
}