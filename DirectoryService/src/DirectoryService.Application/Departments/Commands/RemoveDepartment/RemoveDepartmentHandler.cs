using Core;
using Core.Database;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using Microsoft.Extensions.Logging;
using Shared;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Commands.RemoveDepartment;

public class RemoveDepartmentHandler : ICommandHandler<RemoveDepartmentCommand, Unit>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<RemoveDepartmentHandler> _logger;

    public RemoveDepartmentHandler(
        IDepartmentRepository departmentRepository,
        ITransactionManager transactionManager,
        ILogger<RemoveDepartmentHandler> logger)
    {
        _departmentRepository = departmentRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<Result<Unit, Errors>> Handle(
        RemoveDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var removeResult = await _departmentRepository.DeleteByIdAsync(command.departmentId, cancellationToken);
        if (removeResult.IsFailure)
        {
            _logger.LogError("Не удалось удалить департамент id:{departmentId}: {Error}", command.departmentId,
                removeResult.Error);

            return removeResult.Error.ToErrors();
        }

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            _logger.LogError("Не удалось удалить департамент id:{departmentId}: {Error}", command.departmentId,
                saveResult.Error);

            return saveResult.Error.ToErrors();
        }

        return Result.Success<Unit, Errors>(Unit.Value);
    }
}