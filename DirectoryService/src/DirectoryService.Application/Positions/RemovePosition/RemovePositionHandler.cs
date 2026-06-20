using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Validations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Positions.RemovePosition;

public class RemovePositionHandler : ICommandHandler<RemovePositionCommand, Unit>
{
    private readonly IPositionRepository _positionRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<RemovePositionHandler> _logger;
    private readonly IValidator<RemovePositionCommand> _validator;

    public RemovePositionHandler(
        IPositionRepository positionRepository,
        ITransactionManager transactionManager,
        ILogger<RemovePositionHandler> logger,
        IValidator<RemovePositionCommand> validator)
    {
        _positionRepository = positionRepository;
        _transactionManager = transactionManager;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Unit, Errors>> Handle(RemovePositionCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        _logger.LogInformation("Обработка RemovePositionCommand id:{positionId}", command.PositionId);

        var removeResult = await _positionRepository.DeleteByIdAsync(command.PositionId, cancellationToken);
        if (removeResult.IsFailure)
        {
            _logger.LogError("Не удалось удалить должность id:{positionId}: {Error}", command.PositionId,
                removeResult.Error);
            return removeResult.Error.ToErrors();
        }

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            _logger.LogError("Не удалось удалить должность id:{positionId}: {Error}", command.PositionId,
                saveResult.Error);
            return saveResult.Error.ToErrors();
        }

        _logger.LogInformation("Должность {positionId} успешно удалена", command.PositionId);

        return Result.Success<Unit, Errors>(Unit.Value);
    }
}