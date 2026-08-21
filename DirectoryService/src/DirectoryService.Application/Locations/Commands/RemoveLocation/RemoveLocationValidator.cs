using Core.Validations;
using DirectoryService.Application.Locations.Failures;
using FluentValidation;

namespace DirectoryService.Application.Locations.Commands.RemoveLocation;

public class RemoveLocationValidator : AbstractValidator<RemoveLocationCommand>
{
    public RemoveLocationValidator()
    {
        RuleFor(l => l.LocationId)
            .NotEmpty().WithError(LocationErrors.InvalidId);
    }
}