using DirectoryService.Application.Validations;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Departments.Queries.GetDirectChildrenDepartment;

public class GetDirectChildrenDepartmentValidator : AbstractValidator<GetDirectChildrenDepartmentQuery>
{
    public GetDirectChildrenDepartmentValidator()
    {
        RuleFor(a => a.ParentId).NotEmpty()
            .WithError(Error.Validation(null, message: "Не может быть пустым"));
    }
}