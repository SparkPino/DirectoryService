using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using Shared;

namespace DirectoryService.Application.Abstraction;

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery
{
    Task<Result<TResult, Errors>> Handle(TQuery query, CancellationToken cancellationToken);
}