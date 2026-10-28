using Core;
using Core.Database;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Application.Departments.Failures;
using DirectoryService.Domain.Departments;
using Microsoft.Extensions.Logging;
using Shared;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Commands.ActivateDepartment;

public class ActivateDepartmentHandler : ICommandHandler<ActivateDepartmentCommand, Unit>
{
    private readonly IDepartmentRepository _repository;
    private readonly ILogger<ActivateDepartmentHandler> _logger;
    private readonly ITransactionManager _transactionManager;

    public ActivateDepartmentHandler(
        IDepartmentRepository repository,
        ILogger<ActivateDepartmentHandler> logger,
        ITransactionManager transactionManager)
    {
        _repository = repository;
        _logger = logger;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Unit, Errors>> Handle(ActivateDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Начато выполнение id:{DepartmentId}",
            command.DepartmentId);
        var timeNow = DateTimeOffset.UtcNow;

        return await _transactionManager.ExecuteInTransactionAsync<Unit>(
            async (ct) =>
            {
                var department = await _repository
                    .GetByIdAsync(command.DepartmentId, ct);
                if (department.IsFailure) return department.Error.ToErrors();

                if (department.Value.ParentId != null)
                {
                    var parent = await _repository.GetByIdWithLockAsync(department.Value.ParentId.Id, ct);
                    if (parent.IsFailure)
                    {
                        return Error.Conflict(
                            "department.activate.parent_deleted",
                            null,
                            "Родительское подразделение удалено").ToErrors();
                    }

                    var result = department.Value.Activate(parent.Value, timeNow);
                    if (result.IsFailure) return result.Error.ToErrors();
                }
                else
                {
                    var result = department.Value.Activate(null, timeNow);
                    if (result.IsFailure) return result.Error.ToErrors();
                }

                var saveResult = await _transactionManager.SaveChangesAsync(ct);
                if (saveResult.IsFailure) return saveResult.Error.ToErrors();

                return Unit.Value;
            }, cancellationToken);
    }
}