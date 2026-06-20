using CSharpFunctionalExtensions;
using MediatR;
using Shared;

namespace DirectoryService.Application.Abstraction;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse, Errors>>
    where TCommand : ICommand<TResponse>;