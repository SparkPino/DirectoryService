using Core;
using Core.Database;
using Core.Validations;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Commands.UpdateDepartment;

public class UpdateDepartmentHandler : ICommandHandler<UpdateDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<UpdateDepartmentHandler> _logger;
    private readonly IValidator<UpdateDepartmentCommand> _validator;
    private readonly ITransactionManager _transactionManager;

    public UpdateDepartmentHandler(
        IDepartmentRepository departmentRepository,
        ILogger<UpdateDepartmentHandler> logger,
        IValidator<UpdateDepartmentCommand> validator, ITransactionManager transactionManager)
    {
        _departmentRepository = departmentRepository;
        _logger = logger;
        _validator = validator;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Errors>> Handle(UpdateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        _logger.LogInformation("Обработка UpdateDepartmentCommand id:{departmentId}", command.Id);

        var departmentResult = await _departmentRepository.GetByIdAsync(command.Id, cancellationToken);
        if (departmentResult.IsFailure) return departmentResult.Error.ToErrors();
        DepartmentName? name = null;
        if (command.DepartmentDto.Name != null)
        {
            var newNameResult = DepartmentName.Create(command.DepartmentDto.Name);
            if (newNameResult.IsFailure) return newNameResult.Error;
            name = newNameResult.Value;
        }

        DepartmentIdentifier? identifier = null;
        if (command.DepartmentDto.Identifier != null)
        {
            var newDepartmentIdentifierResult = DepartmentIdentifier.Create(command.DepartmentDto.Identifier);
            if (newDepartmentIdentifierResult.IsFailure) return newDepartmentIdentifierResult.Error;
            identifier = newDepartmentIdentifierResult.Value;
        }


        _departmentRepository.SetRowVersion(departmentResult.Value, command.DepartmentDto.RowVersion);

        var updateDepartmentResult =
            departmentResult.Value.UpdateDepartment(name, identifier);
        if (updateDepartmentResult.IsFailure) return updateDepartmentResult.Error;

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        return command.Id;
    }
}