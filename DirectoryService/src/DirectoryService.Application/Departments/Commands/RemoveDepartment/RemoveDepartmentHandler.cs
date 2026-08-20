using Core;
using Core.Database;
using Core.Validations;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Commands.RemoveDepartment;

public class RemoveDepartmentHandler : ICommandHandler<RemoveDepartmentCommand, Unit>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<RemoveDepartmentHandler> _logger;
    private readonly IValidator<RemoveDepartmentCommand> _validator;

    public RemoveDepartmentHandler(
        IDepartmentRepository departmentRepository,
        ITransactionManager transactionManager,
        ILogger<RemoveDepartmentHandler> logger,
        IValidator<RemoveDepartmentCommand> validator)
    {
        _departmentRepository = departmentRepository;
        _transactionManager = transactionManager;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Unit, Errors>> Handle(
        RemoveDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        _logger.LogInformation("Обработка RemoveDepartmentCommand id:{departmentId}", command.departmentId);

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

        _logger.LogInformation("Департамент {departmentId} успешно удалён", command.departmentId);

        return Result.Success<Unit, Errors>(Unit.Value);
    }
}