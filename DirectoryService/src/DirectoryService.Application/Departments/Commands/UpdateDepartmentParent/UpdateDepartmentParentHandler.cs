using System.Data;
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

        DepartmentPath? oldPath = null;
        DepartmentPath? newPath = null;
        int updateCounter = 0;
        var transactionResult = await _transactionManager.ExecuteInTransactionAsync<DepartmentParentDto>(
            async ct =>
            {
                var lockResult = await LockParticipantsAsync(command, ct);
                if (lockResult.IsFailure)
                {
                    return lockResult.Error;
                }

                var (department, newParent) = lockResult.Value;

                if (newParent is { IsActive: false })
                {
                    return Error.Conflict("department.move.parent_deleted", "Родительский департамент удалён")
                        .ToErrors();
                }

                if (command.ParentId?.Id == department.ParentId?.Id)
                {
                    _logger.LogDebug("Родитель тот же для {DepartmentId} — ничего не делаем", command.Id);
                    return new DepartmentParentDto(
                        department.Id.Id, department.ParentId?.Id, department.Path.Path,
                        department.Depth, department.UpdatedAt);
                }

                oldPath = department.Path;
                var relocateResult = department.Relocate(newParent);
                if (relocateResult.IsFailure)
                {
                    return relocateResult.Error;
                }

                newPath = department.Path;
                short depthDelta = relocateResult.Value;

                var saveResult = await _transactionManager.SaveChangesAsync(ct);
                if (saveResult.IsFailure)
                {
                    return saveResult.Error.ToErrors();
                }

                updateCounter = await _departmentRepository.UpdateDescendantsPathAsync(
                    oldPath, newPath, depthDelta, ct);

                _logger.LogInformation(
                    "Департамент {DepartmentId} перемещён: {OldPath} → {NewPath}, затронуто потомков: {Count}",
                    command.Id, oldPath.Path, newPath.Path, updateCounter);

                return new DepartmentParentDto(department.Id.Id, department.ParentId?.Id,
                    department.Path.Path,
                    department.Depth, department.UpdatedAt);
            },
            cancellationToken,
            IsolationLevel.ReadCommitted);

        if (transactionResult.IsFailure)
        {
            if (transactionResult.Error.Any(e => e.Code == "database.deadlock_detected"))
            {
                return Error.Conflict("department.move.conflict",
                    "Перенос конфликтует с параллельной операцией, повторите запрос").ToErrors();
            }

            return transactionResult.Error;
        }

        return transactionResult.Value;
    }

    private async Task<Result<(Department Department, Department? NewParent), Errors>> LockParticipantsAsync(
        UpdateDepartmentParentCommand command, CancellationToken ct)
    {
        var listParticipants = new List<(Guid Id, bool IgnoreQueryFilter, string NotFoundCode)>()
        {
            (command.Id, false, "department.not_found"),
        };
        if (command.ParentId is { Id: not null })
        {
            listParticipants.Add((command.ParentId.Id.Value, true, "department.parent.not_found"));
        }

        var lockedById = new Dictionary<Guid, Department>();

        foreach (var request in
                 listParticipants.OrderBy(x => x.Id)
                     .DistinctBy(a =>
                         a.Id)) //OrderBy применяет CompareTo что обеспечивает всегда одинаковый порядок блокировок тех же айдишников, убирая таким образом проблему АБ БА
        {
            var lockResult =
                await _departmentRepository.GetByIdWithLockAsync(request.Id, ct, request.IgnoreQueryFilter);
            if (lockResult.IsFailure)
            {
                return Error.NotFound(request.NotFoundCode, lockResult.Error.Message).ToErrors();
            }

            lockedById[request.Id] = lockResult.Value;
        }

        var department = lockedById[command.Id];
        Department? newParent = command.ParentId is { Id: not null } ? lockedById[command.ParentId.Id.Value] : null;

        return (department, newParent);
    }
}