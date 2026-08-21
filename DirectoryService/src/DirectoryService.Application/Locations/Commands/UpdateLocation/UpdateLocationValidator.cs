using Core.Validations;
using DirectoryService.Application.Locations.Failures;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;

namespace DirectoryService.Application.Locations.Commands.UpdateLocation;

public class UpdateLocationValidator : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationValidator()
    {
        RuleFor(a => a.UpdateLocationDto.TimeZone)
            .MustBeValueObject(LocationTimeZone.Create!)
            .When(a => a.UpdateLocationDto.TimeZone != null);

        RuleFor(a => a.UpdateLocationDto.LocationName)
            .MustBeValueObject(LocationName.Create!)
            .When(a => a.UpdateLocationDto.LocationName != null);

        When(a => a.UpdateLocationDto.AdressDto != null, () =>
        {
            RuleFor(a => a.UpdateLocationDto.AdressDto!.Country)
                .NotEmpty()
                .When(a => a.UpdateLocationDto.AdressDto!.Country != null)
                .WithError(LocationErrors.EmptyField(nameof(LocationAdressDto.Country)));

            RuleFor(a => a.UpdateLocationDto.AdressDto!.City)
                .NotEmpty()
                .When(a => a.UpdateLocationDto.AdressDto!.City != null)
                .WithError(LocationErrors.EmptyField(nameof(LocationAdressDto.City)));

            RuleFor(a => a.UpdateLocationDto.AdressDto!.Street)
                .NotEmpty()
                .When(a => a.UpdateLocationDto.AdressDto!.Street != null)
                .WithError(LocationErrors.EmptyField(nameof(LocationAdressDto.Street)));

            RuleFor(a => a.UpdateLocationDto.AdressDto!.PostalCode)
                .NotEmpty()
                .When(a => a.UpdateLocationDto.AdressDto!.PostalCode != null)
                .WithError(LocationErrors.EmptyField(nameof(LocationAdressDto.PostalCode)));

            RuleFor(a => a.UpdateLocationDto.AdressDto!.BuildingNumber)
                .NotEmpty()
                .When(a => a.UpdateLocationDto.AdressDto!.BuildingNumber != null)
                .WithError(LocationErrors.EmptyField(nameof(LocationAdressDto.BuildingNumber)));
        });
    }
}