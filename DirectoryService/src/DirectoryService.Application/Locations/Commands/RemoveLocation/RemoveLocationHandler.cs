using Core;
using Core.Database;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using Microsoft.Extensions.Logging;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Locations.Commands.RemoveLocation;

public class RemoveLocationHandler : ICommandHandler<RemoveLocationCommand, Guid>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<RemoveLocationHandler> _logger;

    public RemoveLocationHandler(
        ILocationRepository locationRepository,
        ITransactionManager transactionManager,
        ILogger<RemoveLocationHandler> logger)
    {
        _locationRepository = locationRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Handle(RemoveLocationCommand command, CancellationToken cancellationToken)
    {
        var removeResult = await _locationRepository.DeleteAsync(command.LocationId, cancellationToken);
        if (removeResult.IsFailure)
        {
            _logger.LogError("Не удалось удалить локацию id:{locationId}: {Error}", command.LocationId,
                removeResult.Error);
            return removeResult.Error.ToErrors();
        }

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            _logger.LogError("Не удалось удалить локацию id:{locationId}: {Error}", command.LocationId,
                saveResult.Error);
            return saveResult.Error.ToErrors();
        }

        return command.LocationId;
    }
}