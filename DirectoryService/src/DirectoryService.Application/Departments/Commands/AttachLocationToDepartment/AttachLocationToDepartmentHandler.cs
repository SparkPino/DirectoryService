using Core;
using Core.Database;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Commands.AttachLocationToDepartment;

public class AttachLocationToDepartmentHandler : ICommandHandler<AttachLocationToDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ITransactionManager _transactionManager;

    public AttachLocationToDepartmentHandler(
        IDepartmentRepository departmentRepository,
        ILocationRepository locationRepository,
        ITransactionManager transactionManager)
    {
        _departmentRepository = departmentRepository;
        _locationRepository = locationRepository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Errors>> Handle(
        AttachLocationToDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetByIdWithLocationsAsync(command.DepartmentId, cancellationToken);

        if (department.IsFailure)
        {
            return department.Error.ToErrors();
        }

        var locations = await _locationRepository.GetByIdAsync(command.LocationId, cancellationToken);
        if (locations.IsFailure)
        {
            return locations.Error.ToErrors();
        }
        
        // в коде нет необходимости из-за Database constraints,
        // DepartmentId и LocationId, образуют составной уникальный индекс
        /*bool isLocationAttached = await _departmentRepository.IsLocationAttachedAsync(
            command.DepartmentId,
            command.LocationId,
            cancellationToken);

        if (isLocationAttached)
        {
            return Error.Conflict("attach.location.to.department", null, "Locations уже привязан к департаменту").ToErrors();
        }*/

        var addLocationResult = department.Value.AddLocation(locations.Value.Id);
        
        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        return command.LocationId;
    }
}