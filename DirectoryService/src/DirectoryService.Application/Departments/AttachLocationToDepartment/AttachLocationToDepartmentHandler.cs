using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Validations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Departments.AttachLocationToDepartment;

public class AttachLocationToDepartmentHandler : ICommandHandler<AttachLocationToDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<AttachLocationToDepartmentHandler> _logger;
    private readonly IValidator<AttachLocationToDepartmentCommand> _validator;

    public AttachLocationToDepartmentHandler(
        IDepartmentRepository departmentRepository,
        ILocationRepository locationRepository,
        ILogger<AttachLocationToDepartmentHandler> logger,
        IValidator<AttachLocationToDepartmentCommand> validator)
    {
        _departmentRepository = departmentRepository;
        _locationRepository = locationRepository;
        _logger = logger;
        _validator = validator;
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

        bool isLocationAttached = await _departmentRepository.IsLocationAttachedAsync(
            command.DepartmentId,
            command.LocationId,
            cancellationToken);

        if (isLocationAttached)
        {
            return Error.Conflict("attach.location.to.department", "Locations уже привязан к департаменту").ToErrors();
        }

        var addLocationResult = department.Value.AddLocation(locations.Value.Id);
        if (addLocationResult.IsFailure)
        {
            return addLocationResult.Error.ToErrors();
        }

        await _departmentRepository.SaveAsync(cancellationToken);
        return command.LocationId;
    }
}