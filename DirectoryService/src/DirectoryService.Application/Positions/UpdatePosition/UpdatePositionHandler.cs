using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Validations;
using DirectoryService.Domain.Positions.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Positions.UpdatePosition;

public class UpdatePositionHandler : ICommandHandler<UpdatePositionCommand, Guid>
{
    private readonly IPositionRepository _positionRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<UpdatePositionHandler> _logger;
    private readonly IValidator<UpdatePositionCommand> _validator;

    public UpdatePositionHandler(
        IPositionRepository positionRepository,
        ITransactionManager transactionManager,
        ILogger<UpdatePositionHandler> logger,
        IValidator<UpdatePositionCommand> validator)
    {
        _positionRepository = positionRepository;
        _transactionManager = transactionManager;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Guid, Errors>> Handle(UpdatePositionCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        _logger.LogInformation("Обработка UpdatePositionCommand id:{id}", command.Id);

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

        _logger.LogInformation("Должность с id:{id} успешно обновлена", command.Id);

        return command.Id;
    }
}