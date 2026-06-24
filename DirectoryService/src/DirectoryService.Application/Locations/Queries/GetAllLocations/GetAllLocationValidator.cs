using DirectoryService.Application.Validations;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Locations.Queries.GetAllLocations;

public class GetAllLocationValidator : AbstractValidator<GetAllLocationQuery>
{
    public GetAllLocationValidator()
    {
        RuleFor(a => a.PageSize)
            .InclusiveBetween(1, 10)
            .WithError(Error.Validation("page.out.of.range", "Page out of range 1 - 10"));

        RuleFor(a => a.Page)
            .GreaterThanOrEqualTo(1)
            .WithError(Error.Validation("page.invalid", "Page must be >= 1"));
    }
}