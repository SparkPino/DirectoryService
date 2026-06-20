using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Departments.Failures;
using DirectoryService.Application.Validations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Departments.DetachPositionFromDepartment;

public class DetachPositionFromDepartmentHandler : ICommandHandler<DetachPositionFromDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<DetachPositionFromDepartmentHandler> _logger;
    private readonly IValidator<DetachPositionFromDepartmentCommand> _validator;

    public DetachPositionFromDepartmentHandler(
        IDepartmentRepository departmentRepository,
        IPositionRepository positionRepository,
        ITransactionManager transactionManager,
        ILogger<DetachPositionFromDepartmentHandler> logger,
        IValidator<DetachPositionFromDepartmentCommand> validator)
    {
        _departmentRepository = departmentRepository;
        _positionRepository = positionRepository;
        _transactionManager = transactionManager;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Guid, Errors>> Handle(
        DetachPositionFromDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        var department = await _departmentRepository.GetByIdWithPositionsAsync(command.DepartmentId, cancellationToken);
        if (department.IsFailure)
        {
            return department.Error.ToErrors();
        }

        var position = await _positionRepository.GetByIdAsync(command.PositionId, cancellationToken);
        if (position.IsFailure)
        {
            return position.Error.ToErrors();
        }

        bool isPositionAttached = await _departmentRepository.IsPositionAttachedAsync(
            command.DepartmentId,
            command.PositionId,
            cancellationToken);
        if (!isPositionAttached)
        {
            return DepartmentError.PositionNotAttached(command.DepartmentId).ToErrors();
        }

        department.Value.DetachPosition(position.Value.Id);

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        return command.PositionId;
    }
}