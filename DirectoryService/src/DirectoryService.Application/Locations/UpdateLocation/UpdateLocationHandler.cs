using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Shared.ValueObjects;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Locations.UpdateLocation;

public class UpdateLocationHandler : ICommandHandler<UpdateLocationCommand, Guid>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<UpdateLocationHandler> _logger;

    public UpdateLocationHandler(ILocationRepository locationRepository, ILogger<UpdateLocationHandler> logger)
    {
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Handle(UpdateLocationCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Обработка UpdateLocationCommand id:{id}", command.Id);

        var locationResult = await _locationRepository.GetByIdAsync(command.Id, cancellationToken);
        if (locationResult.IsFailure)
            return locationResult.Error.ToErrors();

        Address? newAddress = null;
        if (command.UpdateLocationDto.AdressDto != null)
        {
            var adressResult = locationResult.Value.Address.UpdateAdress(
                command.UpdateLocationDto.AdressDto.Country,
                command.UpdateLocationDto.AdressDto.City,
                command.UpdateLocationDto.AdressDto.Street,
                command.UpdateLocationDto.AdressDto.PostalCode,
                command.UpdateLocationDto.AdressDto.BuildingNumber,
                command.UpdateLocationDto.AdressDto.Apartment);
            if (adressResult.IsFailure) return adressResult.Error.ToErrors();
            newAddress = adressResult.Value;
        }

        LocationTimeZone? newTimeZone = null;
        if (command.UpdateLocationDto.TimeZone != null)
        {
            var timeZoneResult = LocationTimeZone.Create(command.UpdateLocationDto.TimeZone);
            if (timeZoneResult.IsFailure) return timeZoneResult.Error.ToErrors();
            newTimeZone = timeZoneResult.Value;
        }

        LocationName? newName = null;
        if (command.UpdateLocationDto.LocationName != null)
        {
            var nameResult = LocationName.Create(command.UpdateLocationDto.LocationName);
            if (nameResult.IsFailure) return nameResult.Error;
            newName = nameResult.Value;
        }

        locationResult.Value.UpdateLocation(newAddress, newTimeZone, newName);

        await _locationRepository.SaveAsync(cancellationToken);

        _logger.LogInformation("Location с id:{id} успешно обновлена", command.Id);
        return command.Id;
    }
}