using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Departments;

public class AddDepartmentHendler : ICommandHandler<AddDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<AddDepartmentHendler> _logger;

    public AddDepartmentHendler(
        ILogger<AddDepartmentHendler> logger,
        IDepartmentRepository departmentRepository,
        ILocationRepository locationRepository)
    {
        _logger = logger;
        _departmentRepository = departmentRepository;
        _locationRepository = locationRepository;
    }

    public async Task<Result<Guid, Errors>> Handle(AddDepartmentCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Обработка AddDepartmentCommand: name:{name}, locationId: {LocationId}",
            command.DepartmentDto.Name, command.DepartmentDto.LocationId);

        var nameResult = DepartmentName.Create(command.DepartmentDto.Name);
        if (nameResult.IsFailure) return nameResult.Error;

        var identifierResult = DepartmentIdentifier.Create(command.DepartmentDto.Identifier);
        if (identifierResult.IsFailure) return identifierResult.Error;

        var departmentId = new DepartmentId(Guid.NewGuid());

        var locationResult = await _locationRepository
            .GetByIdAsync(command.DepartmentDto.LocationId, cancellationToken);

        if (locationResult.IsFailure)
        {
            return locationResult.Error.ToErrors();
        }

        var departmentlocation = new DepartmentLocation(departmentId, locationResult.Value.Id);

        var departmentResult = Department.CreateRoot(
            [],
            [departmentlocation],
            nameResult.Value,
            identifierResult.Value,
            departmentId);

        if (departmentResult.IsFailure)
        {
            _logger.LogError("Возникла ошибка при создании департамента с Id {departmentId}: {Error}", departmentId,
                departmentResult.Error);
            return departmentResult.Error;
        }

        var addResult = await _departmentRepository.AddAsync(departmentResult.Value, cancellationToken);

        if (addResult.IsFailure)
        {
            _logger.LogError("Не получилось сохранить департамент id:{departmentId}: {Error}", departmentId,
                addResult.Error);
            return addResult.Error.ToErrors();
        }

        _logger.LogInformation("Департамент с id:{departmentId} успешно создан", departmentResult.Value.Id);

        return departmentResult.Value.Id.Id;
    }
}