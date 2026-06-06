using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Departments.Failures;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Departments.DetachLocationFromDepartment;

public class DetachLocationFromDepartmentHandler : ICommandHandler<DetachLocationFromDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<DetachLocationFromDepartmentHandler> _logger;

    public DetachLocationFromDepartmentHandler(
        IDepartmentRepository departmentRepository,
        ILocationRepository locationRepository,
        ILogger<DetachLocationFromDepartmentHandler> logger)
    {
        _departmentRepository = departmentRepository;
        _locationRepository = locationRepository;
        _logger = logger;
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
            return DepartmentError.NotFound(command.DepartmentId).ToErrors();
        }

        department.Value.DetachLocation(locations.Value.Id);

        await _departmentRepository.SaveAsync(cancellationToken);

        return command.LocationId;
    }
}