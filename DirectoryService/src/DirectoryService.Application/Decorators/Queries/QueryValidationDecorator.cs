using Core;
using Core.Validations;
using CSharpFunctionalExtensions;
using FluentValidation;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Decorators.Queries;

internal sealed class QueryValidationDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery
{
    private readonly IQueryHandler<TQuery, TResponse> _inner;
    private readonly IEnumerable<IValidator<TQuery>> _validators;

    public QueryValidationDecorator(
        IQueryHandler<TQuery, TResponse> inner,
        IEnumerable<IValidator<TQuery>> validators)
    {
        _inner = inner;
        _validators = validators;
    }

    public async Task<Result<TResponse, Errors>> Handle(TQuery query, CancellationToken cancellationToken)
    {
        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(query, cancellationToken);
            if (!result.IsValid)
                return result.ToError();
        }

        return await _inner.Handle(query, cancellationToken);
    }
}