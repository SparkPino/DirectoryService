using Core;
using Core.Database;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Domain.Positions.ValueObjects;
using Microsoft.Extensions.Logging;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Positions.UpdatePosition;

public class UpdatePositionHandler : ICommandHandler<UpdatePositionCommand, Guid>
{
    private readonly IPositionRepository _positionRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<UpdatePositionHandler> _logger;

    public UpdatePositionHandler(
        IPositionRepository positionRepository,
        ITransactionManager transactionManager,
        ILogger<UpdatePositionHandler> logger)
    {
        _positionRepository = positionRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Handle(UpdatePositionCommand command, CancellationToken cancellationToken)
    {
        var positionResult = await _positionRepository.GetByIdAsync(command.Id, cancellationToken);
        if (positionResult.IsFailure) return positionResult.Error.ToErrors();

        PositionName? newName = null;
        if (command.UpdatePositionDto.Name != null)
        {
            var nameResult = PositionName.Create(command.UpdatePositionDto.Name);
            if (nameResult.IsFailure) return nameResult.Error;
            newName = nameResult.Value;
        }

        var renameResult = positionResult.Value.Rename(newName, command.UpdatePositionDto.Description);
        if (renameResult.IsFailure) return renameResult.Error;

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            _logger.LogError("Не удалось обновить должность id:{id}: {Error}", command.Id, saveResult.Error);
            return saveResult.Error.ToErrors();
        }

        return command.Id;
    }
}