using DirectoryService.Application.Validations;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Departments.Queries.GetDepartmentTree;

public class GetDepartmentTreeValidator : AbstractValidator<GetDepartmentTreeQuery>
{
    public GetDepartmentTreeValidator()
    {
        RuleFor(a => a.Limit)
            .GreaterThan(0)
            .When(a => a.Limit.HasValue)
            .WithError(Error.Validation("limit.invalid", "Limit must be > 0"));

        RuleFor(a => a.Offset)
            .GreaterThanOrEqualTo(0)
            .When(a => a.Limit.HasValue)
            .WithError(Error.Validation("offset.invalid", "Offset must be >= 0"));
    }
}