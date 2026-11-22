using Core;
using Core.Database;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Application.Departments.Failures;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Commands.DetachLocationFromDepartment;

public class DetachLocationFromDepartmentHandler : ICommandHandler<DetachLocationFromDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ITransactionManager _transactionManager;

    public DetachLocationFromDepartmentHandler(
        IDepartmentRepository departmentRepository,
        ILocationRepository locationRepository,
        ITransactionManager transactionManager)
    {
        _departmentRepository = departmentRepository;
        _locationRepository = locationRepository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Errors>> Handle(
        DetachLocationFromDepartmentCommand command,
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

        bool isLocationAttached = await _departmentRepository.IsLocationAttachedAsync(command.DepartmentId,
            command.LocationId,
            cancellationToken);
        if (!isLocationAttached)
        {
            return DepartmentError.LocationNotAttached(command.DepartmentId).ToErrors();
        }

        department.Value.DetachLocation(locations.Value.Id);

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        return command.LocationId;
    }
}