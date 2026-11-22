using Core;
using Core.Database;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using Shared;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Commands.BulkActivateDepartment;

public class BulkActivateHandler : ICommandHandler<BulkActivateCommand, Unit>
{
    private readonly IDepartmentRepository _repository;
    private readonly ITransactionManager _transactionManager;

    public BulkActivateHandler(
        IDepartmentRepository repository,
        ITransactionManager transactionManager)
    {
        _repository = repository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Unit, Errors>> Handle(BulkActivateCommand command, CancellationToken cancellationToken)
    {
        var timeNow = DateTimeOffset.UtcNow;

        var transactionResult = await _transactionManager.ExecuteInTransactionAsync<Unit>(
            async (ct) =>
            {
                List<Error> errors = new();
                var departments = await _repository.GetByIdsAsync(
                    command.DepartmentIds, ct, includingDeleted: true);
                var missingIds = command.DepartmentIds.Except(departments.Select(d => d.Id.Id)).ToList();

                foreach (var missingDepartment in missingIds)
                {
                    errors.Add(Error.Conflict(
                        "bulk.activate.not.all.departments",
                        missingDepartment.ToString(),
                        $"Департамент с id {missingDepartment} не найден"));
                }

                var parentIds = departments
                    .Where(d => d.ParentId is not null)
                    .Select(department => department.ParentId!.Id)
                    .Distinct()
                    .ToList();

                var parentDepartments = await _repository.GetByIdsAsync(
                    parentIds, ct, includingDeleted: true);

                var normalizedParents = parentDepartments.ToDictionary(a => a.Id.Id);

                foreach (var department in departments.OrderBy(d => d.Depth))
                {
                    var parent = department.ParentId is null
                        ? null
                        : normalizedParents[department.ParentId.Id];
                    var activateResult = department.Activate(parent, timeNow);
                    if (activateResult.IsFailure)
                    {
                        errors.Add(Error.Conflict(
                            activateResult.Error.Code,
                            department.Id.Id.ToString(),
                            activateResult.Error.Message));
                    }
                }

                if (errors.Any()) return new Errors(errors);

                var saveResult = await _transactionManager.SaveChangesAsync(ct);
                if (!saveResult.IsSuccess) return saveResult.Error.ToErrors();
                return Unit.Value;
            }, cancellationToken);

        if (transactionResult.IsFailure)
        {
            return transactionResult.Error;
        }

        return transactionResult;
    }
}