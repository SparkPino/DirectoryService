using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Abstraction.Database;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Application.Validations;
using DirectoryService.Contracts.Department.UpdateDepartmentParent;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Departments.Commands.UpdateDepartmentParent;

public class UpdateDepartmentParentHandler : ICommandHandler<UpdateDepartmentParentCommand, DepartmentParentDto>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<UpdateDepartmentParentHandler> _logger;
    private readonly ITransactionManager _transactionManager;
    private readonly IValidator<UpdateDepartmentParentCommand> _validator;

    public UpdateDepartmentParentHandler(
        IDepartmentRepository departmentRepository,
        ILogger<UpdateDepartmentParentHandler> logger,
        ITransactionManager transactionManager,
        IValidator<UpdateDepartmentParentCommand> validator)
    {
        _departmentRepository = departmentRepository;
        _logger = logger;
        _transactionManager = transactionManager;
        _validator = validator;
    }

    public async Task<Result<DepartmentParentDto, Errors>> Handle(
        UpdateDepartmentParentCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        _logger.LogInformation(
            "Обработка UpdateDepartmentParentCommand departmentId:{DepartmentId} newParentId:{ParentId}", command.Id,
            command.ParentId?.Id);

        Department? newParent = null;
        if (command.ParentId is { Id: not null })
        {
            var newParentResult =
                await _departmentRepository.GetByAsync(
                    a => a.Id == new DepartmentId(command.ParentId.Id.Value),
                    cancellationToken, true);
            if (newParentResult.IsFailure)
            {
                var error = newParentResult.Error.Message;
                return Error.NotFound("department.parent.not_found", error).ToErrors();
            }

            if (!newParentResult.Value.IsActive)
            {
                return Error.Conflict("department.move.parent_deleted", "Родительский департамент удалён").ToErrors();
            }

            newParent = newParentResult.Value;
        }

        var department =
            await _departmentRepository.GetByAsync(
                a => a.Id == new DepartmentId(command.Id),
                cancellationToken);
        if (department.IsFailure)
        {
            var error = department.Error.Message;
            return Error.NotFound("department.not_found", error).ToErrors();
        }

        if (command.ParentId?.Id == department.Value.ParentId?.Id)
        {
            _logger.LogDebug("Родитель тот же для {DepartmentId} — ничего не делаем", command.Id);
            return new DepartmentParentDto(department.Value.Id.Id, department.Value.ParentId?.Id,
                department.Value.Path.Path,
                department.Value.Depth, department.Value.UpdatedAt);
        }

        var oldPath = department.Value.Path;
        var relocateResult = department.Value.Relocate(newParent);
        if (relocateResult.IsFailure)
        {
            return relocateResult.Error;
        }

        short depthDelta = relocateResult.Value;
        FormattableString updateChildrenQuery = $"""
                                                 UPDATE departments d
                                                 SET path = {department.Value.Path.Path}::ltree || subpath(d.path,nlevel({oldPath.Path}::ltree)),
                                                     depth = d.depth + {depthDelta},
                                                 updated_at = NOW()
                                                 WHERE d.path <@ {oldPath.Path}::ltree
                                                 AND d.path != {oldPath.Path}::ltree
                                                 """;

        FormattableString childrensCountQuery = $"""
                                                 SELECT COUNT(*) AS "Value" FROM departments 
                                                 WHERE path <@ {oldPath.Path}::ltree
                                                 AND path != {oldPath.Path}::ltree
                                                 """;
        int updateCounter = 0;
        var transactionResult = await _transactionManager.ExecuteInTransactionAsync<DepartmentParentDto>(
            async o =>
            {
                var saveResult = await _transactionManager.SaveChangesAsync(o);
                if (saveResult.IsFailure)
                {
                    return saveResult.Error.ToErrors();
                }

                var childrensCount = await _departmentRepository.TakeCount(childrensCountQuery, o);
                if (childrensCount.IsFailure)
                {
                    _logger.LogCritical("Что-то кардинально пошло не так");
                    return childrensCount.Error.ToErrors();
                }

                if (childrensCount.Value != 0)
                {
                    var departmentChildrens = await _departmentRepository.UpdateBySqlAsync(updateChildrenQuery, o);
                    if (departmentChildrens.IsFailure)
                    {
                        return departmentChildrens.Error.ToErrors();
                    }

                    updateCounter = departmentChildrens.Value;
                }

                return new DepartmentParentDto(department.Value.Id.Id, department.Value.ParentId?.Id,
                    department.Value.Path.Path,
                    department.Value.Depth, department.Value.UpdatedAt!.Value);
            }, cancellationToken);

        if (transactionResult.IsFailure)
        {
            return transactionResult.Error;
        }

        _logger.LogInformation(
            "Департамент {DepartmentId} перемещён: {OldPath} → {NewPath}, затронуто потомков: {Count}", command.Id,
            oldPath.Path, department.Value.Path.Path, updateCounter);
        return transactionResult.Value;
    }
}