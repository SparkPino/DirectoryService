using Core;
using Core.Database;
using Core.Validations;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Positions.AddPosition;

public class AddPositionHandler : ICommandHandler<AddPositionCommand, Guid>
{
    private readonly IPositionRepository _positionRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<AddPositionHandler> _logger;
    private readonly IValidator<AddPositionCommand> _validator;

    public AddPositionHandler(
        IPositionRepository positionRepository,
        ITransactionManager transactionManager,
        ILogger<AddPositionHandler> logger,
        IValidator<AddPositionCommand> validator)
    {
        _positionRepository = positionRepository;
        _transactionManager = transactionManager;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Guid, Errors>> Handle(AddPositionCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        _logger.LogInformation("Обработка AddPositionCommand name:{Name}", command.AddPositionDto.Name);

        var nameResult = PositionName.Create(command.AddPositionDto.Name);
        if (nameResult.IsFailure) return nameResult.Error;

        var positionResult = Position.Create(nameResult.Value, command.AddPositionDto.Description);
        if (positionResult.IsFailure)
        {
            _logger.LogError("Не удалось создать должность: {Error}", positionResult.Error);
            return positionResult.Error.ToErrors();
        }

        var position = positionResult.Value;

        await _positionRepository.AddAsync(position, cancellationToken);

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            _logger.LogError("Не удалось сохранить должность: {Error}", saveResult.Error);
            return saveResult.Error.ToErrors();
        }

        _logger.LogInformation("Должность создана успешно с id: {PositionId}", position.Id);

        return position.Id.Id;
    }
}