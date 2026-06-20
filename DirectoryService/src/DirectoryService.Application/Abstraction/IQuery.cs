using CSharpFunctionalExtensions;
using MediatR;
using Shared;

namespace DirectoryService.Application.Abstraction;

public interface IQuery<TResponse> : IRequest<Result<TResponse, Errors>>
{
}