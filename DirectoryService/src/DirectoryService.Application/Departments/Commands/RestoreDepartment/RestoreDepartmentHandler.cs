using Core;
using Core.Database;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Commands.RestoreDepartment;

public class RestoreDepartmentHandler : ICommandHandler<RestoreDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _repository;
    private readonly ITransactionManager _transactionManager;

    public RestoreDepartmentHandler(
        IDepartmentRepository repository,
        ITransactionManager transactionManager)
    {
        _repository = repository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Errors>> Handle(RestoreDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _transactionManager.ExecuteInTransactionAsync<Guid>(
            async (cancellationToken) =>
            {
                var departments = await _repository
                    .GetSubtreesAsync([command.Departmentid], cancellationToken);

                if (!departments.Any())
                {
                    return Error.NotFound("department.not.found", $"Департамент {command.Departmentid} не найден")
                        .ToErrors();
                }

                var department =
                    departments.Keys
                        .First(d => d.Id.Id == command.Departmentid);

                if (department.ParentId is not null)
                {
                    var parentDepartment =
                        await _repository.GetByIdAsync(department.ParentId.Id, cancellationToken);

                    if (parentDepartment.IsFailure)
                    {
                        return Error.Conflict(
                            "root.department.deleted", null,
                            "Нельзя восстановить подразделение если его отцовское подразделение удалено").ToErrors();
                    }

                    // восстановление включает поддерево, а под выключенным родителем активных быть не может
                    if (!parentDepartment.Value.IsActive)
                    {
                        return Error.Conflict(
                            "department.restore.parent_inactive", null,
                            "Нельзя восстановить подразделение если его родительское подразделение не активно").ToErrors();
                    }
                }

                if (await _repository.IsPathTakenAsync(department.Path, cancellationToken))
                {
                    return Error.Conflict(
                        "department.path.taken", null,
                        "На месте подразделения уже создано другое с таким же идентификатором").ToErrors();
                }

                foreach (var childDepartment in departments)
                {
                    childDepartment.Key.RestoreDepartment(childDepartment.Value, DateTimeOffset.UtcNow);
                }

                var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
                if (saveResult.IsFailure) return saveResult.Error.ToErrors();
                return command.Departmentid;
            }, cancellationToken);

        return result;
    }
}