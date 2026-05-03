using DirectoryService.Contracts;
using DirectoryService.Contracts.Locations;
using FluentValidation;

namespace DirectoryService.Application.Locations.AddLocation;

public class AddLocationValidator : AbstractValidator<LocationDto>
{
    public AddLocationValidator()
    {
        RuleFor(l => l.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(120)
            .WithMessage("Name must not exceed 500 characters");
    }
}