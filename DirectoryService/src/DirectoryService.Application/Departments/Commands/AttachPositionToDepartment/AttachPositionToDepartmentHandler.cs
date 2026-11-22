using Core;
using Core.Database;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Commands.AttachPositionToDepartment;

public class AttachPositionToDepartmentHandler : ICommandHandler<AttachPositionToDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly ITransactionManager _transactionManager;

    public AttachPositionToDepartmentHandler(
        IDepartmentRepository departmentRepository,
        IPositionRepository positionRepository,
        ITransactionManager transactionManager)
    {
        _departmentRepository = departmentRepository;
        _positionRepository = positionRepository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Errors>> Handle(
        AttachPositionToDepartmentCommand command,
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

        var addPositionResult = department.Value.AddPosition(position.Value.Id);

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        return command.PositionId;
    }
}