using Core;
using Core.Database;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Application.Departments.Failures;
using Microsoft.Extensions.Logging;
using Shared;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Commands.DeactivateDepartment;

public class DeactivateDepartmentHandler : ICommandHandler<DeactivateDepartmentCommand, Unit>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<DeactivateDepartmentHandler> _logger;

    public DeactivateDepartmentHandler(
        IDepartmentRepository departmentRepository,
        ITransactionManager transactionManager,
        ILogger<DeactivateDepartmentHandler> logger)
    {
        _departmentRepository = departmentRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<Result<Unit, Errors>> Handle(DeactivateDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Начато выполнение id:{DepartmentId}",
            command.DepartmentId);
        var timeNow = DateTimeOffset.UtcNow;

        return await _transactionManager.ExecuteInTransactionAsync<Unit>(
            async (ct) =>
            {
                var department = await _departmentRepository.GetByIdWithLockAsync(command.DepartmentId, ct);
                if (department.IsFailure) return department.Error.ToErrors();

                var departmentTree =
                    await _departmentRepository.GetSubtreesAsync([department.Value.Id.Id], ct);
                if (departmentTree.Count == 0) return DepartmentError.NotFound(command.DepartmentId).ToErrors();

                var descendants = departmentTree
                    .First(a => a.Key.Id == department.Value.Id);

                var result = department.Value.Deactivate(descendants.Value, timeNow);
                if (result.IsFailure) return result.Error.ToErrors();

                var saveResult = await _transactionManager.SaveChangesAsync(ct);
                if (saveResult.IsFailure) return saveResult.Error.ToErrors();

                return Unit.Value;
            }, cancellationToken);
    }
}