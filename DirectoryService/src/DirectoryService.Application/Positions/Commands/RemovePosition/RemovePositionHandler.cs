using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using Microsoft.Extensions.Logging;
using Core;
using Core.Database;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Positions.RemovePosition;

public class RemovePositionHandler : ICommandHandler<RemovePositionCommand, Guid>
{
    private readonly IPositionRepository _positionRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<RemovePositionHandler> _logger;

    public RemovePositionHandler(
        IPositionRepository positionRepository,
        ITransactionManager transactionManager,
        ILogger<RemovePositionHandler> logger)
    {
        _positionRepository = positionRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Handle(RemovePositionCommand command, CancellationToken cancellationToken)
    {
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

        return command.PositionId;
    }
}