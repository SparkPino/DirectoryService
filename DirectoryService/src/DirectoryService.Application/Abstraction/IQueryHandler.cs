using CSharpFunctionalExtensions;
using MediatR;
using Shared;

namespace DirectoryService.Application.Abstraction;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse, Errors>>
    where TQuery : IQuery<TResponse>
{
}