using DirectoryService.Application.Validations;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Departments.Queries.GetDepartments;

public class GetDepartmentsValidation : AbstractValidator<GetDepartmentsQuery>
{
    public GetDepartmentsValidation()
    {
        When(a => a.Pagination is not null, () =>
        {
            RuleFor(a => a.Pagination.Page).Must(a => a > 0)
                .WithError(Error.Validation("page.zero", "Page должно быть не меньше 1", "Page"));

            RuleFor(a => a.Pagination.PageSize).Must(a => a is > 0 || a <= 100)
                .WithError(Error.Validation("pageSize.out.of.range", "PageSize должно быть не больше 100", "PageSize"));
        });
        RuleFor(a => a.Search).Must(a => a is null || a.Length < 150)
            .WithError(Error.Validation("search.to.longe", "Search должно содержать не больше 150 символов",
                "Search"));
    }
}