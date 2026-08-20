using Core;
using Core.Database;
using Core.Validations;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Shared.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Locations.Commands.AddLocation;

public class AddLocationHandler : ICommandHandler<AddLocationCommand, Guid>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<AddLocationHandler> _logger;
    private readonly IValidator<AddLocationCommand> _validator;


    public AddLocationHandler(
        ILocationRepository locationRepository,
        ITransactionManager transactionManager,
        ILogger<AddLocationHandler> logger,
        IValidator<AddLocationCommand> validator)
    {
        _locationRepository = locationRepository;
        _transactionManager = transactionManager;
        _logger = logger;
        _validator = validator;
    }


    public async Task<Result<Guid, Errors>> Handle(AddLocationCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        _logger.LogInformation("Обработка AddLocationCommand name:{Name}", command.AddLocationDto.Name);

        var errors = new List<Error>();

        var nameResult = LocationName.Create(command.AddLocationDto.Name);
        if (nameResult.IsFailure) errors.AddRange(nameResult.Error);

        var addressResult = Address.Create(
            command.AddLocationDto.Address.Country,
            command.AddLocationDto.Address.City,
            command.AddLocationDto.Address.Street,
            command.AddLocationDto.Address.PostalCode,
            command.AddLocationDto.Address.BuildingNumber,
            command.AddLocationDto.Address.Apartment);

        if (addressResult.IsFailure) errors.AddRange(addressResult.Error);

        var timeZoneResult = LocationTimeZone.Create(command.AddLocationDto.TimeZone);
        if (timeZoneResult.IsFailure) errors.AddRange(timeZoneResult.Error);

        if (errors.Any())
        {
            _logger.LogWarning("Ошибка валидации локации: {Errors}", errors);
            return new Errors(errors);
        }

        var location = Location.Create(nameResult.Value, addressResult.Value, timeZoneResult.Value);

        await _locationRepository.AddAsync(location, cancellationToken);

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
        {
            _logger.LogError("Не удалось добавить локацию: {Error}", saveResult.Error);
            return saveResult.Error.ToErrors();
        }

        _logger.LogInformation("Локация создана успешно с id: {LocationId}", location.Id);

        return location.Id.Id;
    }
}