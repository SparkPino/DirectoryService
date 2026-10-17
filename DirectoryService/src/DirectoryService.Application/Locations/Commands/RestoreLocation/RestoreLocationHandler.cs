using Core;
using Core.Database;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.Extensions.Logging;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Locations.Commands.RestoreLocation;

public class RestoreLocationHandler : ICommandHandler<RestoreLocationCommand, Guid>
{
    private readonly ILocationRepository _repository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<RestoreLocationHandler> _logger;

    public RestoreLocationHandler(
        ILocationRepository repository,
        ITransactionManager transactionManager,
        ILogger<RestoreLocationHandler> logger)
    {
        _repository = repository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Handle(RestoreLocationCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Обработка RestoreLocationCommand id:{locationId}", command.LocationId);

        var locationId = new LocationId(command.LocationId);

        // удалённая локация скрыта query filter-ом, поэтому ищем в обход него
        var location = await _repository.GetByAsync(l => l.Id == locationId, cancellationToken, true);
        if (location.IsFailure)
        {
            return location.Error.ToErrors();
        }

        // имя уникально только среди активных — пока локация была в архиве, его могли занять
        if (!location.Value.IsActive && await _repository.IsNameTakenAsync(location.Value.Name, cancellationToken))
        {
            return Error.Conflict(
                "location.name.taken",
                null,
                $"Нельзя восстановить: активная локация с названием «{location.Value.Name.Name}» уже существует")
                .ToErrors();
        }

        location.Value.Restore();

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        return command.LocationId;
    }
}
