using Core.Validations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Shared.ValueObjects;
using FluentValidation;

namespace DirectoryService.Application.Locations.Commands.AddLocation;

public class AddLocationValidator : AbstractValidator<AddLocationCommand>
{
    public AddLocationValidator()
    {
        RuleFor(l => l.AddLocationDto.Name)
            .MustBeValueObject(LocationName.Create);
        RuleFor(l => l.AddLocationDto.TimeZone)
            .MustBeValueObject(LocationTimeZone.Create);
        RuleFor(l => l.AddLocationDto.Address)
            .MustBeValueObject(dto => Address.Create(
                dto.Country, dto.City, dto.Street,
                dto.PostalCode, dto.BuildingNumber, dto.Apartment))
            .When(a => a.AddLocationDto.Address != null);
    }
}