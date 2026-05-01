using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Application.Abstraction;

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<Result<Guid, Errors>> Handle(TCommand command, CancellationToken cancellationToken);
}