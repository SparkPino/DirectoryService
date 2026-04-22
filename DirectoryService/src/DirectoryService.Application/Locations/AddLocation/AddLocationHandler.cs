using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Domain;
using DirectoryService.Domain.Locations;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Locations.AddLocation;

public class AddLocationHandler : ICommandHandler<AddLocationCommand>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<AddLocationHandler> _logger;


    public AddLocationHandler(
        ILocationRepository locationRepository,
        ILogger<AddLocationHandler> logger,
        IValidator<Contracts.LocationDto> locationDtoValidator)
    {
        _locationRepository = locationRepository;
        _logger = logger;
    }


    public async Task<UnitResult<string>> Handle(AddLocationCommand command, CancellationToken cancellationToken)
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

        if (addressResult.IsFailure) return addressResult.Error;

        var timeZoneResult = LocationTimeZone.Create(command.LocationDto.TimeZone);
        if (timeZoneResult.IsFailure) return timeZoneResult.Error;


        // создание сущности
        var locationResult = Location.Create(nameResult.Value, addressResult.Value, timeZoneResult.Value);
        if (locationResult.IsFailure) return locationResult.Error;


        // создание сущности в базе даных
        var result = await _locationRepository.AddAsync(locationResult.Value, cancellationToken);
        if (result.IsFailure)
        {
            return result.Error.Message;
        }

        // логирование об успешном или не успешном добавлении
        _logger.LogInformation("Location создана с id: {locationId}", locationResult.Value.Id);
        // логи лучше записывать именно так, когда параметры указаны через запятую.

        return UnitResult.Success<string>();
    }
}