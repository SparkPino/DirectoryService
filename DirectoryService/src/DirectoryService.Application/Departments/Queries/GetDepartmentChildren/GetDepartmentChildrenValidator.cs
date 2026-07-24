using DirectoryService.Application.Validations;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Departments.Queries.GetDepartmentChildren;

public class GetDepartmentChildrenValidator : AbstractValidator<GetDepartmentChildrenQuery>
{
    public GetDepartmentChildrenValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .WithError(Error.Validation("department_id.empty", "Идентификатор департамента не может быть пустым"));
    }
}