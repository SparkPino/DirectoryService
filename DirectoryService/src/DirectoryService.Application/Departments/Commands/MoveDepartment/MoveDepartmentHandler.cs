using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Abstraction.Database;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Application.Validations;
using DirectoryService.Domain.Departments;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Departments.Commands.MoveDepartment;

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
            var newParentResult =
                await _departmentRepository.GetByIdAsync(command.NewParentId.Value, cancellationToken);
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
        // напрямую и не входит в SaveChangesAsync, поэтому без транзакции при сбое на втором шаге
        // перемещённое подразделение осталось бы сохранённым с "осиротевшими" путями потомков.
        // Вся операция обёрнута в ExecuteInTransactionAsync (а не в ручной BeginTransactionAsync),
        // потому что EF Core не разрешает вручную открытые транзакции при включённом
        // EnableRetryOnFailure — стратегия ретраев должна владеть всей операцией целиком.
        var moveResult = await _transactionManager.ExecuteInTransactionAsync<Guid>(
            async ct =>
            {
                var saveResult = await _transactionManager.SaveChangesAsync(ct);
                if (saveResult.IsFailure) return saveResult.Error.ToErrors();

                await _departmentRepository.UpdateDescendantsPathAsync(oldPath, department.Path, depthDelta, ct);

                return department.Id.Id;
            },
            cancellationToken);

        if (moveResult.IsFailure) return moveResult.Error;

        _logger.LogInformation("Департамент {departmentId} успешно перемещён", command.DepartmentId);

        return moveResult.Value;
    }
}