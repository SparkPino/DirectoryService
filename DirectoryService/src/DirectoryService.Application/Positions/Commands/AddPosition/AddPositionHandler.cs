using Core;
using Core.Database;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using Microsoft.Extensions.Logging;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Positions.AddPosition;

public class AddPositionHandler : ICommandHandler<AddPositionCommand, Guid>
{
    private readonly IPositionRepository _positionRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<AddPositionHandler> _logger;

    public AddPositionHandler(
        IPositionRepository positionRepository,
        ITransactionManager transactionManager,
        ILogger<AddPositionHandler> logger)
    {
        _positionRepository = positionRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Handle(AddPositionCommand command, CancellationToken cancellationToken)
    {
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

        return position.Id.Id;
    }
}