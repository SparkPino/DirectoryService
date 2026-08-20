using Core.Validations;
using DirectoryService.Application.Locations.Failures;
using FluentValidation;

namespace DirectoryService.Application.Locations.Queries.GetByIdLocation;

public class GetByIdValidator : AbstractValidator<GetByIdLocationQuery>
{
    public GetByIdValidator()
    {
        RuleFor(a => a.LocationId.Id)
            .NotEmpty()
            .WithError(LocationErrors.InvalidId);
    }
}