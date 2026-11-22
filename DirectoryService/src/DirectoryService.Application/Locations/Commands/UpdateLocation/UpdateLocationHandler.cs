using Core;
using Core.Database;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Shared.ValueObjects;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Locations.Commands.UpdateLocation;

public class UpdateLocationHandler : ICommandHandler<UpdateLocationCommand, Guid>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ITransactionManager _transactionManager;

    public UpdateLocationHandler(
        ILocationRepository locationRepository,
        ITransactionManager transactionManager)
    {
        _locationRepository = locationRepository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Errors>> Handle(UpdateLocationCommand command, CancellationToken cancellationToken)
    {
        (_, bool isFailure, Location? location, Error? error) = await _locationRepository.GetByIdAsync(command.Id, cancellationToken);
        if (isFailure) return error.ToErrors();

        Address? newAddress = null;
        if (command.UpdateLocationDto.AdressDto != null)
        {
            var adressResult = location.Address.UpdateAdress(
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

        location.UpdateLocation(newAddress, newTimeZone, newName);

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure) return saveResult.Error.ToErrors();

        return command.Id;
    }
}