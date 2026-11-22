using Core;
using Core.Validations;
using CSharpFunctionalExtensions;
using FluentValidation;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Decorators.Commands;

internal sealed class CommandValidationDecorator<TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand, TResponse> _inner;
    private readonly IEnumerable<IValidator<TCommand>> _validators;

    public CommandValidationDecorator(
        ICommandHandler<TCommand, TResponse> inner,
        IEnumerable<IValidator<TCommand>> validators)
    {
        _inner = inner;
        _validators = validators;
    }

    public async Task<Result<TResponse, Errors>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        // По очереди, не через Task.WhenAll: валидаторы могут ходить в БД через один DbContext.
        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(command, cancellationToken);
            if (!result.IsValid)
                return result.ToError();
        }

        return await _inner.Handle(command, cancellationToken);
    }
}