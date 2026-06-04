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
            "Обработка AddDepartmentCommand: name:{name}",
            command.DepartmentDto.Name);

        var nameResult = DepartmentName.Create(command.DepartmentDto.Name);
        if (nameResult.IsFailure) return nameResult.Error;

        var identifierResult = DepartmentIdentifier.Create(command.DepartmentDto.Identifier);
        if (identifierResult.IsFailure) return identifierResult.Error;

        Department parentDepartment = null;

        if (command.DepartmentDto.ParentId.HasValue)
        {
            var parentDepartmentResult =
                await _departmentRepository.GetByIdAsync(command.DepartmentDto.ParentId.Value, cancellationToken);

            if (parentDepartmentResult.IsFailure)
            {
                return parentDepartmentResult.Error.ToErrors();
            }

            parentDepartment = parentDepartmentResult.Value;
        }

        var departmentResult = Department.CreateDepartment(
            [],
            nameResult.Value,
            identifierResult.Value,
            parentDepartment);


        if (departmentResult.IsFailure)
        {
            _logger.LogError(
                "Возникла ошибка при создании департамента: {Error}", departmentResult.Error);
            return departmentResult.Error;
        }

        var locationResult = await _locationRepository
            .GetByIdsAsync(command.DepartmentDto.LocationIds, cancellationToken);

        if (locationResult.IsFailure)
        {
            return locationResult.Error.ToErrors();
        }

        departmentResult.Value.AddLocations(locationResult.Value.Select(l => l.Id));
        if (departmentResult.IsFailure)
        {
            return departmentResult.Error;
        }

        var addResult = await _departmentRepository.AddAsync(departmentResult.Value, cancellationToken);

        if (addResult.IsFailure)
        {
            _logger.LogError("Не получилось сохранить департамент:{departmentId}: {Error}", departmentResult.Value.Id,
                addResult.Error);
            return addResult.Error.ToErrors();
        }

        _logger.LogInformation("Департамент с id:{departmentId} успешно создан", departmentResult.Value.Id);

        return departmentResult.Value.Id.Id;
    }
}