using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Application.Abstraction;

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand
{
    Task<Result<TResult, Errors>> Handle(TCommand command, CancellationToken cancellationToken);
}