using CSharpFunctionalExtensions;
using MediatR;
using Shared;

namespace DirectoryService.Application.Abstraction;

public interface ICommand<TResponse> : IRequest<Result<TResponse, Errors>>
{
}