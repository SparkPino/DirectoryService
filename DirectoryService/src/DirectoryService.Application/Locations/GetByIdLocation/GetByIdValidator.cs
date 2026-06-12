using DirectoryService.Application.Locations.Failures;
using DirectoryService.Application.Validations;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Locations.GetByIdLocation;

public class GetByIdValidator : AbstractValidator<GetByIdLocationQuery>
{
    public GetByIdValidator()
    {
        RuleFor(a => a.LocationId.Id)
            .NotEmpty()
            .WithError(LocationErrors.InvalidId);
    }
}