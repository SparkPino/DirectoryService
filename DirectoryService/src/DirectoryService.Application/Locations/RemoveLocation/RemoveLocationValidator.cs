using DirectoryService.Application.Locations.Failures;
using DirectoryService.Application.Validations;
using FluentValidation;

namespace DirectoryService.Application.Locations.RemoveLocation;

public class RemoveLocationValidator : AbstractValidator<RemoveLocationCommand>
{
    public RemoveLocationValidator()
    {
        RuleFor(l => l.LocationId)
            .NotEmpty().WithError(LocationErrors.InvalidId);
    }
}