using Core;
using Core.Database;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Application.Departments.Failures;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Commands.DetachPositionFromDepartment;

public class DetachPositionFromDepartmentHandler : ICommandHandler<DetachPositionFromDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly ITransactionManager _transactionManager;

    public DetachPositionFromDepartmentHandler(
        IDepartmentRepository departmentRepository,
        IPositionRepository positionRepository,
        ITransactionManager transactionManager)
    {
        _departmentRepository = departmentRepository;
        _positionRepository = positionRepository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Errors>> Handle(
        DetachPositionFromDepartmentCommand command,
        CancellationToken cancellationToken)
    {
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