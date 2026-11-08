using Core;
using Core.Database;
using Core.Validations;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Commands.BulkRestoreDepartment;

public class BulkRestoreHandler : ICommandHandler<BulkRestoreCommand, Unit>
{
    private readonly IDepartmentRepository _repository;
    private readonly ITransactionManager _transactionManager;
    private readonly IValidator<BulkRestoreCommand> _validator;
    private readonly ILogger<BulkRestoreHandler> _logger;


    public BulkRestoreHandler(
        IDepartmentRepository repository,
        ITransactionManager transactionManager,
        IValidator<BulkRestoreCommand> validator,
        ILogger<BulkRestoreHandler> logger
    )
    {
        _repository = repository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Unit, Errors>> Handle(BulkRestoreCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid) return validationResult.ToError();

        _logger.LogInformation(
            "Начато массовое восстановление подразделений, количество: {Count}",
            command.DepartmentIds.Count);

        var transactionResult = await _transactionManager.ExecuteInTransactionAsync<Unit>(
            async (ct) =>
            {
                DateTimeOffset timeNow = DateTimeOffset.UtcNow;
                List<Error> errors = new();
                var departments = await _repository.GetSubtreesAsync(command.DepartmentIds, ct);
                var missIds = command.DepartmentIds.Except(departments
                        .Select(a => a.Key.Id.Id))
                    .ToList();

                foreach (var id in missIds)
                {
                    errors.Add(Error.Conflict(
                        "bulk.restore.not.all.exist",
                        id.ToString(),
                        $"Департамент с id {id} не найден"));
                }

                var parentIds = departments.Keys
                    .Where(d => d.ParentId is not null)
                    .Select(a => a.ParentId!.Id)
                    .Distinct()
                    .ToList();

                var pathToRestore = departments.Keys
                    .Where(a => a.DeletedAt is not null)
                    .Select(d => d.Path.Path)
                    .ToList();

                var takenPath = await _repository.GetTakenPathsAsync(pathToRestore, ct);
                var parents = await _repository.GetByIdsAsync(parentIds, ct, includingDeleted: true);
                var parentsById = parents.ToDictionary(a => a.Id.Id);

                foreach (var item in departments.OrderBy(a => a.Key.Depth))
                {
                    var department = item.Key;
                    if (department.DeletedAt == null) continue;
                    if (department.ParentId is not null)
                    {
                        var parent = parentsById[department.ParentId.Id];
                        if (parent.DeletedAt != null)
                        {
                            errors.Add(Error.Conflict(
                                "department.restore.parent_deleted",
                                department.Id.Id.ToString(),
                                $"Нельзя восстановить подразделение {department.Name.Value}: родительское подразделение удалено"));
                            continue;
                        }

                        if (!parent.IsActive)
                        {
                            errors.Add(Error.Conflict("department.restore.parent_inactive",
                                department.Id.Id.ToString(),
                                $"Нельзя восстановить подразделение {department.Name.Value}: родительское подразделение не активно"));
                            continue;
                        }
                    }

                    if (takenPath.Contains(department.Path.Path))
                    {
                        errors.Add(Error.Conflict(
                            "bulk.restore.path.is.taken",
                            department.Id.Id.ToString(),
                            $"Путь подразделения id:{department.Id.Id} занят другим не удаленным подразделением"));
                        continue;
                    }

                    var restoreResult = department.RestoreDepartment(item.Value, timeNow);
                    if (restoreResult.IsFailure)
                    {
                        errors.Add(Error.Conflict(
                            restoreResult.Error.Code,
                            department.Id.Id.ToString(),
                            restoreResult.Error.Message));
                    }
                }

                if (errors.Any()) return new Errors(errors);

                var saveResult = await _transactionManager.SaveChangesAsync(ct);
                if (saveResult.IsFailure) return saveResult.Error.ToErrors();

                return Unit.Value;
            }, cancellationToken);

        if (transactionResult.IsFailure)
        {
            _logger.LogWarning(
                "Массовое восстановление подразделений завершилось ошибкой, количество ошибок: {ErrorCount}",
                transactionResult.Error.Count());

            return transactionResult.Error;
        }

        _logger.LogInformation(
            "Массовое восстановление подразделений успешно завершено, количество: {Count}",
            command.DepartmentIds.Count);

        return transactionResult;
    }
}