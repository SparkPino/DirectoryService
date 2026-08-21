using Core;
using Core.Database;
using Core.Validations;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Locations.Commands.RemoveLocation;

public class RemoveLocationHandler : ICommandHandler<RemoveLocationCommand, Guid>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<RemoveLocationHandler> _logger;
    private readonly IValidator<RemoveLocationCommand> _validator;

    public RemoveLocationHandler(
        ILocationRepository locationRepository,
        ITransactionManager transactionManager,
        ILogger<RemoveLocationHandler> logger,
        IValidator<RemoveLocationCommand> validator)
    {
        _locationRepository = locationRepository;
        _transactionManager = transactionManager;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Guid, Errors>> Handle(RemoveLocationCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        _logger.LogInformation("Обработка RemoveLocationCommand id:{locationId}", command.LocationId);

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

        _logger.LogInformation("Локация {locationId} успешно удалена", command.LocationId);

        return command.LocationId;
    }
}