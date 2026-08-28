using Core.Validations;
using FluentValidation;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Locations.Queries.GetAllLocations;

public class GetAllLocationValidator : AbstractValidator<GetAllLocationQuery>
{
    public GetAllLocationValidator()
    {
        RuleFor(a => a.PageSize)
            .InclusiveBetween(1, 10)
            .WithError(Error.Validation("page-sizes.is.invalid", "Page out of range 1 - 10"));

        RuleFor(a => a.Page)
            .GreaterThanOrEqualTo(1)
            .WithError(Error.Validation("page.is.invalid", "Page must be >= 1"));
    }
}