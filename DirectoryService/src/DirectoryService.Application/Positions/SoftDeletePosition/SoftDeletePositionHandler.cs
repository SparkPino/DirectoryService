using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Abstraction.Database;
using DirectoryService.Application.Abstraction.Repositories;
using Shared;

namespace DirectoryService.Application.Positions.SoftDeletePosition;

public class SoftDeletePositionHandler : ICommandHandler<SoftDeletePositionCommand, Guid>
{
    private readonly IPositionRepository _repository;
    private readonly ITransactionManager _transactionManager;

    public SoftDeletePositionHandler(IPositionRepository repository, ITransactionManager transactionManager)
    {
        _repository = repository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Errors>> Handle(SoftDeletePositionCommand command,
        CancellationToken cancellationToken)
    {
        var position = await _repository.GetByAsync(p => p.Id == command.Id, cancellationToken);
        if (position.IsFailure)
        {
            return position.Error.ToErrors();
        }

        position.Value.SoftDelete();
        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
        {
            return saveChangesResult.Error.ToErrors();
        }

        return position.Value.Id.Id;
    }
}
