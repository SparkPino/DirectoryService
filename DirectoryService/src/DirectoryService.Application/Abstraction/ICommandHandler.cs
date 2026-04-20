using CSharpFunctionalExtensions;

namespace DirectoryService.Application.Abstraction;

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<UnitResult<string>> Handle(TCommand command, CancellationToken cancellationToken);
}