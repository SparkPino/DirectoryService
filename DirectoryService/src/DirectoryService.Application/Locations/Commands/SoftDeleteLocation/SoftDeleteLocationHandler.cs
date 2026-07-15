using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Abstraction.Database;
using DirectoryService.Application.Abstraction.Repositories;
using Shared;

namespace DirectoryService.Application.Locations.Commands.SoftDeleteLocation;

public class SoftDeleteLocationHandler : ICommandHandler<SoftDeleteLocationCommand, Guid>
{
    private readonly ILocationRepository _repository;
    private readonly ITransactionManager _transactionManager;

    public SoftDeleteLocationHandler(ILocationRepository repository, ITransactionManager transactionManager)
    {
        _repository = repository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Errors>> Handle(SoftDeleteLocationCommand command,
        CancellationToken cancellationToken)
    {
        var location = await _repository.GetByAsync(l => l.Id == command.Id, cancellationToken);
        if (location.IsFailure)
        {
            return location.Error.ToErrors();
        }

        location.Value.SoftDelete();
        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
        {
            return saveChangesResult.Error.ToErrors();
        }

        return location.Value.Id.Id;
    }
}
