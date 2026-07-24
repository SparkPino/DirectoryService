using DirectoryService.Application.Validations;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Departments.Queries.GetDepartmentAncestors;

public class GetDepartmentAncestorsValidator : AbstractValidator<GetDepartmentAncestorsQuery>
{
    public GetDepartmentAncestorsValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .WithError(Error.Validation("department_id.empty", "Идентификатор департамента не может быть пустым"));
    }
}
