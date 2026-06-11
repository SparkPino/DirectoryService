using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Validations;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Shared.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Locations.UpdateLocation;

public class UpdateLocationHandler : ICommandHandler<UpdateLocationCommand, Guid>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<UpdateLocationHandler> _logger;
    private readonly IValidator<UpdateLocationCommand> _validator;

    public UpdateLocationHandler(
        ILocationRepository locationRepository,
        ILogger<UpdateLocationHandler> logger,
        IValidator<UpdateLocationCommand> validator)
    {
        _locationRepository = locationRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Guid, Errors>> Handle(UpdateLocationCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

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
            var tzResult = LocationTimeZone.Create(command.UpdateLocationDto.TimeZone);
            if (tzResult.IsFailure) return tzResult.Error.ToErrors();
            newTimeZone = tzResult.Value;
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