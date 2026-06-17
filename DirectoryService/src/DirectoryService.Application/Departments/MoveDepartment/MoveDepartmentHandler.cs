using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Validations;
using DirectoryService.Domain.Departments;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Departments.MoveDepartment;

public class MoveDepartmentHandler : ICommandHandler<MoveDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<MoveDepartmentHandler> _logger;
    private readonly IValidator<MoveDepartmentCommand> _validator;

    public MoveDepartmentHandler(
        IDepartmentRepository departmentRepository,
        ITransactionManager transactionManager,
        ILogger<MoveDepartmentHandler> logger,
        IValidator<MoveDepartmentCommand> validator)
    {
        _departmentRepository = departmentRepository;
        _transactionManager = transactionManager;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Guid, Errors>> Handle(MoveDepartmentCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        _logger.LogInformation(
            "Обработка MoveDepartmentCommand departmentId:{departmentId} newParentId:{newParentId}",
            command.DepartmentId,
            command.NewParentId);

        var departmentResult = await _departmentRepository.GetByIdAsync(command.DepartmentId, cancellationToken);
        if (departmentResult.IsFailure) return departmentResult.Error.ToErrors();

        Department? newParent = null;
        if (command.NewParentId.HasValue)
        {
            var newParentResult = await _departmentRepository.GetByIdAsync(command.NewParentId.Value, cancellationToken);
            if (newParentResult.IsFailure) return newParentResult.Error.ToErrors();
            newParent = newParentResult.Value;
        }

        var department = departmentResult.Value;
        var oldPath = department.Path;

        var relocateResult = department.Relocate(newParent);
        if (relocateResult.IsFailure) return relocateResult.Error;

        short depthDelta = relocateResult.Value;

        // Перенос самого подразделения и массовый пересчёт путей/глубины у всех его потомков
        // должны быть атомарны: потомки обновляются через ExecuteUpdateAsync, который пишет в БД
        // напрямую и не входит в SaveChangesAsync, поэтому без явной транзакции при сбое на втором
        // шаге перемещённое подразделение осталось бы сохранённым с "осиротевшими" путями потомков.
        var transactionResult = await _transactionManager.TranstactionBegin(cancellationToken);
        if (transactionResult.IsFailure) return transactionResult.Error;

        using var transaction = transactionResult.Value;

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            transaction.Rollback();
            return saveResult.Error.ToErrors();
        }

        var updateDescendantsResult = await _transactionManager.ExecuteAsync(
            () => _departmentRepository.UpdateDescendantsPathAsync(oldPath, department.Path, depthDelta, cancellationToken));

        if (updateDescendantsResult.IsFailure)
        {
            transaction.Rollback();
            return updateDescendantsResult.Error.ToErrors();
        }

        var commitResult = transaction.Commit();
        if (commitResult.IsFailure)
        {
            return commitResult.Error.ToErrors();
        }

        _logger.LogInformation("Департамент {departmentId} успешно перемещён", command.DepartmentId);

        return department.Id.Id;
    }
}