using Core.Validations;
using FluentValidation;
using SharedLibrary.SharedKernel;

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
