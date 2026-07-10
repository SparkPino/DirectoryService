using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Abstraction.Database;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Application.Validations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Departments.Commands.AddDepartment;

public class AddDepartmentHandler : ICommandHandler<AddDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILocationRepository _locationRepository;
    private readonly IValidator<AddDepartmentCommand> _validator;
    private readonly ILogger<AddDepartmentHandler> _logger;

    public AddDepartmentHandler(
        ILogger<AddDepartmentHandler> logger,
        IDepartmentRepository departmentRepository,
        ITransactionManager transactionManager,
        ILocationRepository locationRepository,
        IValidator<AddDepartmentCommand> validator)
    {
        _logger = logger;
        _departmentRepository = departmentRepository;
        _transactionManager = transactionManager;
        _locationRepository = locationRepository;
        _validator = validator;
    }

    public async Task<Result<Guid, Errors>> Handle(AddDepartmentCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        _logger.LogInformation("Обработка AddDepartmentCommand: name:{name}", command.DepartmentDto.Name);

        var nameResult = DepartmentName.Create(command.DepartmentDto.Name);
        var identifierResult = DepartmentIdentifier.Create(command.DepartmentDto.Identifier);

        Department? parentDepartment = null;

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
            .GetExistingIdsAsync(command.DepartmentDto.LocationIds, cancellationToken);

        if (locationResult.IsFailure)
        {
            return locationResult.Error.ToErrors();
        }

        var normalizeLocations = locationResult.Value.Select(a => new LocationId(a));

        var addLocationResult = departmentResult.Value.AddLocations(normalizeLocations);
        if (addLocationResult.IsFailure)
        {
            return addLocationResult.Error.ToErrors();
        }

        await _departmentRepository.AddAsync(departmentResult.Value, cancellationToken);

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            _logger.LogError("Не получилось сохранить департамент:{departmentId}: {Error}", departmentResult.Value.Id,
                saveResult.Error);
            return saveResult.Error.ToErrors();
        }

        _logger.LogInformation("Департамент с id:{departmentId} успешно создан", departmentResult.Value.Id);

        return departmentResult.Value.Id.Id;
    }
}