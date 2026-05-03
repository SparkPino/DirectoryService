using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Shared;
using DirectoryService.Domain.Shared.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Locations.AddLocation;

public class AddLocationHandler : ICommandHandler<AddLocationCommand>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<AddLocationHandler> _logger;


    public AddLocationHandler(
        ILocationRepository locationRepository,
        ILogger<AddLocationHandler> logger,
        IValidator<LocationDto> locationDtoValidator)
    {
        _locationRepository = locationRepository;
        _logger = logger;
    }


    public async Task<Result<Guid, Errors>> Handle(AddLocationCommand command, CancellationToken cancellationToken)
    {
        // 1.Validation входных даных и бизнес логики
        var nameResult = LocationName.Create(command.LocationDto.Name);
        if (nameResult.IsFailure) return nameResult.Error;

        var addressResult = Address.Create(
            command.LocationDto.Adress.Country,
            command.LocationDto.Adress.City,
            command.LocationDto.Adress.Street,
            command.LocationDto.Adress.PostalCode,
            command.LocationDto.Adress.BuildingNumber,
            command.LocationDto.Adress.Apartment);

        if (addressResult.IsFailure) return addressResult.Error.ToErrors();

        var timeZoneResult = LocationTimeZone.Create(command.LocationDto.TimeZone);
        if (timeZoneResult.IsFailure) return timeZoneResult.Error.ToErrors();

        // создание сущности
        var location = Location.Create(nameResult.Value, addressResult.Value, timeZoneResult.Value);

        // создание сущности в базе даных
        var addLocationResult = await _locationRepository.AddAsync(location, cancellationToken);
        if (addLocationResult.IsFailure)
        {
            return addLocationResult.Error.ToErrors();
        }

        // логирование об успешном или не успешном добавлении
        _logger.LogInformation("Location создана с id: {locationId}", location.Id);

        return location.Id.Id;
    }
}