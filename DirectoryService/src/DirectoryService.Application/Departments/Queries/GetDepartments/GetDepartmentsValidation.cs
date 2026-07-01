using FluentValidation;

namespace DirectoryService.Application.Departments.Queries.GetDepartments;

public class GetDepartmentsValidation : AbstractValidator<GetDepartmentsQuery>
{
    public GetDepartmentsValidation()
    {
        RuleFor(a => a.Pagination)
            .NotNull().NotEmpty().WithMessage("The pagination parameter is required.");
    }
}